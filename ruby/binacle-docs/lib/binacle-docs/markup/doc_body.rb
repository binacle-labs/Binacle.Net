# frozen_string_literal: true

require 'nokogiri'

module Binacle
  module Docs
    module Markup
      module DocBody
        def doc_body(input)
          Converter.new(input.to_s).to_html
        end

        # Turns what kramdown and Rouge write into the design's markup. It runs on the rendered page, so the
        # heading ids are already made and the markdown stays as it is.
        class Converter
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
          ALERT = /\A\s*\[!(\w+)\]\s*/

          def initialize(html)
            @fragment = Nokogiri::HTML5.fragment(html)
          end

          def to_html
            drop_opening_emoji
            drop_rules_before_sections
            name_callouts
            name_code_blocks
            name_example_heads
            name_tables
            name_nested_lists
            name_blocks
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

          def name_code_blocks
            @fragment.css('pre').each do |pre|
              code = pre.at_css('code')
              next unless code

              outer = pre.ancestors('div.highlighter-rouge').first || pre
              # A language Rouge has no lexer for stays on the code element.
              language = [outer, code].filter_map { |element| element['class'].to_s[/language-(\S+)/, 1] }.first
              mark_parts(code, language)
              code.remove_attribute('class')
              code['data-lang'] = language if language
              pre['class'] = 'code-block__pre'
              wrap(pre) if outer.key?('data-wrap')
              outer.replace(%(<div class="code-block" data-code-block>#{head(language, outer)}#{pre.to_html}</div>))
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
            name = call ? http_line(badges(call)) : %(<span class="label" data-code-lang>#{label}</span>)
            %(<div class="code-block__head">#{name}) +
              %(<button type="button" class="copy-button" data-copy hidden>Copy</button></div>)
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

          def name_tables
            @fragment.css('table').each do |table|
              add_class(table, 'table doc__table')
              table['data-table'] = ''
              table.css('td').each { |cell| add_class(cell, 'mono') if all_mono?(cell) }
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

          def name_blocks
            @fragment.element_children.each do |element|
              name = BLOCK_CLASSES[element.name]
              add_class(element, name) if name
            end
          end

          def add_class(element, name)
            element['class'] = [element['class'], name].compact.join(' ')
          end
        end
      end
    end
  end
end
