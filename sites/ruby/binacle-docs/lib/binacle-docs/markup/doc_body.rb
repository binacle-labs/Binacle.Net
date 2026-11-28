# frozen_string_literal: true

require 'binacle-sites-core'
require 'nokogiri'

module Binacle
  module Docs
    module Markup
      module DocBody
        def doc_body(input)
          Converter.new(input.to_s).to_html
        end

        # For a page that keeps its emoji and draws its own blocks.
        def doc_doors(input)
          Converter.new(input.to_s).doors_to_html
        end

        # Turns what kramdown and Rouge write into the design's markup. It runs on the rendered page, so the
        # heading ids are already made and the markdown stays as it is.
        class Converter
          include Binacle::SitesCore::Markup

          # A keycap or a picture, with whatever joins to it.
          OPENING_EMOJI = /\A\s*(?:[0-9#*]\u{FE0F}?\u{20E3}|\p{Extended_Pictographic})(?:\u{FE0F}|\u{200D}\p{Extended_Pictographic})*\s*/
          EMOJI_PLACES = 'h2, h3, h4, h5, h6, p, li, li > a:first-child'

          BLOCK_CLASSES = {
            'p' => 'doc__text',
            'ul' => 'doc__list',
            'ol' => 'doc__list',
            'h2' => 'doc__heading doc__heading--2',
            'h3' => 'doc__heading doc__heading--3',
            'h4' => 'doc__heading doc__heading--4',
            'h5' => 'doc__heading doc__heading--4',
            'h6' => 'doc__heading doc__heading--4',
            'hr' => 'doc__rule',
            'blockquote' => 'doc__quote'
          }.freeze

          # Rouge's token classes. Only JSON shows them: everything else is plain ink.
          JSON_PARTS = {
            'nl' => 'code-block__property',
            'mi' => 'code-block__number',
            'mf' => 'code-block__number',
            'p' => 'code-block__punctuation'
          }.freeze
          LANGUAGE_LABELS = { 'yml' => 'yaml' }.freeze
          CALL = /\A(GET|POST|PUT|DELETE) (\S+)\z/
          FILL = /\A\d+(\.\d+)?\z/
          WIDTH = /\A[1-9]\d*\z/
          FIELD_KINDS = %w[reserved more].freeze
          ALERT = /\A\s*\[!(\w+)\]\s*/

          def initialize(html)
            @fragment = Nokogiri::HTML5.fragment(html)
          end

          def to_html
            drop_opening_emoji
            drop_rules_before_sections
            name_callouts
            name_tabs
            name_folds
            name_docs_code_blocks
            name_example_heads
            name_http_lines
            name_result_lists
            name_bit_diagrams
            name_docs_tables
            name_nested_lists
            name_door_layout
            name_labelled_lists(@fragment)
            name_steps
            name_blocks
            @fragment.to_html
          end

          def doors_to_html
            name_door_layout
            @fragment.to_html
          end

          private

          def drop_opening_emoji
            @fragment.css(EMOJI_PLACES).each do |element|
              text = element.children.first
              text.content = text.content.sub(OPENING_EMOJI, '') if text&.text?
            end
          end

          # A section draws its own line.
          def drop_rules_before_sections
            @fragment.css('hr').each do |rule|
              following = rule.next_element
              rule.remove if following.nil? || following.name == 'h2'
            end
          end

          def name_callouts
            @fragment.css('blockquote').each do |quote|
              kind = quote['class'].to_s[/block-(\w+)/, 1] || github_alert(quote)
              next unless kind

              quote.name = 'div'
              quote['class'] = "callout callout--#{kind.downcase}"
              quote.prepend_child(%(<p class="label">#{kind.capitalize}</p>))
            end
          end

          # Takes the [!NOTE] off the first line and gives back its kind.
          def github_alert(quote)
            first = quote.at_css('p')
            marker = first&.inner_html&.match(ALERT)
            return unless marker

            first.inner_html = marker.post_match
            marker[1]
          end

          # Code blocks in a row, each named in data-tab, become tabs. The first says what they choose between, in
          # data-tabs.
          def name_tabs
            @fragment.css('[data-tabs]').each do |first|
              blocks = [first]
              while (following = blocks.last.next_element)&.key?('data-tab') && !following.key?('data-tabs')
                blocks << following
              end
              first.replace(tabs(first['data-tabs'], blocks))
              blocks.drop(1).each(&:remove)
            end

            stray = @fragment.at_css('[data-tab]')
            raise ArgumentError, "no data-tabs before the tab #{stray['data-tab']}" if stray
          end

          def tabs(label, blocks)
            names = blocks.map { |block| block['data-tab'] }
            raise ArgumentError, "a tab with no data-tab, in #{label}" unless names.all?

            box = Nokogiri::HTML5.fragment('<div class="tabs" data-tabs=""><div class="tabs__list" role="tablist"></div></div>').first_element_child
            list = box.first_element_child
            list['aria-label'] = label
            blocks.each_with_index do |block, index|
              list.add_child(%(<button type="button" class="tabs__tab" role="tab" aria-selected="#{index.zero?}"></button>))
              list.last_element_child.content = names[index]
              box.add_child(%(<div class="tabs__panel" role="tabpanel"#{' hidden=""' unless index.zero?}></div>))
              copy = block.dup
              copy.remove_attribute('data-tab')
              copy.remove_attribute('data-tabs')
              box.last_element_child.add_child(copy)
            end
            box
          end

          # A code block with data-fold starts closed, under that text.
          def name_folds
            @fragment.css('[data-fold]').each do |block|
              fold = block.wrap('<details class="disclosure"></details>').parent
              fold.prepend_child('<summary class="disclosure__summary label"></summary>')
              fold.first_element_child.content = block.remove_attribute('data-fold').value
            end
          end

          # The shared block, with the docs' language, JSON colours and wrapping on top.
          def name_docs_code_blocks
            name_code_blocks(@fragment, selector: 'pre') do |pre, code, outer|
              # A language Rouge has no lexer for stays on the code element.
              language = [outer, code].filter_map { |element| element['class'].to_s[/language-(\S+)/, 1] }.first
              mark_parts(code, language)
              code['data-lang'] = language if language
              wrap(pre) if outer.key?('data-wrap')
              head(language, outer)
            end
          end

          def mark_parts(code, language)
            code.css('span').each do |span|
              part = JSON_PARTS[span['class']] if language == 'json'
              if part
                span['class'] = part
              else
                span.replace(span.children)
              end
            end
          end

          # One long string has no lines to scroll: it wraps at every width.
          def wrap(pre)
            add_class(pre, 'code-block__pre--wrap')
            pre['data-wrap'] = ''
          end

          # A block with no heading above it to read names its call in the markdown, in data-call. A file or a
          # field is named in data-name, in place of the language.
          def head(language, block)
            call = block['data-call']
            label = block['data-name'] || LANGUAGE_LABELS.fetch(language, language) || 'text'
            code_head(call ? http_line(badges(call)) : code_label(label))
          end

          def badges(call)
            method, path = call.split(' ', 2)
            badge = %(<span class="http-line__badge http-line__badge--plain">#{method}</span>)
            path ? %(#{badge}<span>#{path}</span>) : badge
          end

          # An example's head names the call: the method and path over a request, the status over a response.
          # JSON after a Response Example, up to the next heading, is another response. Every example in the docs
          # is a success, so the status is 200.
          def name_example_heads
            call = nil
            answering = false
            @fragment.css('h2, h3, p, div[data-code-block]').each do |element|
              case element.name
              when 'h2', 'h3' then answering = false
              when 'p' then call = call_in(element) || call
              else answering = name_example_head(element, call, answering)
              end
            end
          end

          def call_in(paragraph)
            paragraph.css('code').filter_map { |code| code.text.match(CALL) }.first
          end

          def name_example_head(block, call, answering)
            before = block.previous_element
            text = before&.name == 'p' ? before.text.strip : ''
            label = block.at_css('[data-code-lang]')
            return answering unless label

            if text.start_with?('Request Example') && call
              label.replace(http_line(badges(call[0])))
            elsif text.start_with?('Response Example') || (answering && block.at_css('code[data-lang=json]'))
              label.replace(http_line(badges('200')))
              return true
            end
            answering
          end

          def http_line(inner)
            %(<p class="http-line code-block__http-line">#{inner}</p>)
          end

          # After the example heads, which read the call from its code.
          def name_http_lines
            @fragment.css('p.http-line:not(.code-block__http-line)').each do |line|
              call = line.text.strip.match(CALL)
              raise ArgumentError, "an http-line is `METHOD /path`, not: #{line.text.strip}" unless call

              add_class(line, 'http-line--small')
              line.inner_html = '<span class="http-line__badge"></span><span></span>'
              line.element_children.zip(call.captures) { |part, text| part.content = text }
            end
          end

          # A table marked result-list has five columns: the bin, its size, how full it is, the result and the items.
          def name_result_lists
            @fragment.css('table.result-list').each do |table|
              rows = table.css('tbody tr').map { |row| result_row(row.css('td').map { |cell| cell.inner_html.strip }) }
              list = Nokogiri::HTML5.fragment('<ul class="result-list result-list--full"></ul>').first_element_child
              list['aria-label'] = table['aria-label'] if table['aria-label']
              list.inner_html = rows.join("\n")
              table.replace(list)
            end
          end

          def result_row(cells)
            unless cells.size == 5 && cells[2].match?(FILL)
              raise ArgumentError, "a result-list row is bin, size, fill, result, items, not: #{cells.join(' | ')}"
            end

            bin, size, fill, result, items = cells
            <<~HTML.chomp
              <li class="result-list__item"><div class="result-row result-row--static">
                <span class="result-row__title">#{bin} &middot; <span class="mono">#{size}</span></span>
                <span class="result-row__fill"><span class="result-row__fill-track"><span class="result-row__fill-line" style="width:#{fill}%"></span></span><span class="result-row__fill-number">#{fill}%</span></span>
                <span class="result-row__meta"><span>#{result}</span><span>#{items}</span></span>
              </div></li>
            HTML
          end

          # A table marked bit-diagram has four columns: the size, the name in code, the note and the width. A name
          # marked reserved or more is drawn faint.
          def name_bit_diagrams
            @fragment.css('table.bit-diagram').each do |table|
              raise ArgumentError, 'a bit-diagram needs an aria-label: a screen reader hears only that' unless table['aria-label']

              rows = table.css('tbody tr').map { |row| row.css('td') }
              fields = rows.map { |cells| bit_field(cells) }
              box = Nokogiri::HTML5.fragment('<div class="bit-diagram" role="img"></div>').first_element_child
              box['style'] = "--cols:#{rows.sum { |cells| cells[3].text.to_i }}"
              box['aria-label'] = table['aria-label']
              box.inner_html = fields.join("\n")
              table.replace(box)
            end
          end

          def bit_field(cells)
            name = cells[1]&.at_css('> code')
            unless cells.size == 4 && name && cells[3].text.strip.match?(WIDTH)
              raise ArgumentError, "a bit-diagram row is size, `name`, note, width, not: #{cells.map(&:text).join(' | ')}"
            end

            kind = (name.classes & FIELD_KINDS).map { |mark| " bit-diagram__field--#{mark}" }.join
            size, note, width = [cells[0], cells[2], cells[3]].map { |cell| cell.inner_html.strip }
            %(<div class="bit-diagram__field#{kind}" style="--span:#{width}">) +
              '<span class="dimension-line dimension-line--small dimension-line--cell"><span class="dimension-line__tick"></span>' +
              %(<span class="dimension-line__number">#{size}</span><span class="dimension-line__tick"></span></span>) +
              %(<code class="bit-diagram__name">#{name.inner_html}</code><span class="muted">#{note}</span></div>)
          end

          # The shared table, in the docs' class and a box that scrolls.
          def name_docs_tables
            name_tables(@fragment, selector: 'table', cell: ->(cell) { add_class(cell, 'mono') if all_mono?(cell) })
              .each do |table|
                add_class(table, 'doc__table')
                table.wrap('<div class="doc__table-scroll"></div>')
              end
          end

          # Such a cell takes the code's size too, or its row stands taller than the code in it.
          def all_mono?(cell)
            code = cell.css('code.mono')
            code.any? && code.map(&:text).join == cell.text.strip
          end

          def name_nested_lists
            @fragment.css('li > ul, li > ol').each { |list| add_class(list, 'doc__list--nested') }
          end

          # The shared tiles, laid out as a grid when asked and full width otherwise.
          def name_door_layout
            name_doors(@fragment).each do |nav|
              nav['class'] = nav.classes.include?('doors--grid') ? 'doors doors--grid' : 'doors doors--full'
            end
          end

          def name_steps
            @fragment.css('ol.steps').each { |list| add_class(list, 'doc__steps') }
          end

          def name_blocks
            @fragment.element_children.each do |element|
              name = BLOCK_CLASSES[element.name]
              add_class(element, name) if name
            end
          end
        end
      end
    end
  end
end
