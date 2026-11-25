# frozen_string_literal: true

require 'cgi/escape'
require 'nokogiri'

module Binacle
  module Www
    module Sections
      def www_sections(input)
        Converter.new(input.to_s).to_html
      end

      # Turns what kramdown writes into the design's sections. Every h1 and h2 starts one, and the attributes on
      # the heading say how the section looks.
      class Converter
        HEADINGS = %w[h1 h2].freeze
        WORDS = %w[h1 h2 h3 h4 p ul ol].freeze
        # What a heading may be marked. Anything else is a typo, which would show as a section with no look.
        BANDS = %w[
          band--compact band--deep band--mirrored band--numbered band--number-in-column band--number-in-row
          band--split band--tint band--wide
        ].freeze
        SPLITS = %w[split--centred split--text-left split--text-right].freeze
        BODIES = %w[band__body--code-left].freeze
        ONE_LINE = 'prose--one-line'
        COLUMNS = 'columns'
        STATEMENT = 'statement'
        # With no label column, any other band would put its body in the label's place.
        ONE_COLUMN = %w[band--mirrored band--split band--wide].freeze
        DOOR_DASH = /\A\s+-\s+/

        def initialize(html)
          @html = html
          @fragment = Nokogiri::HTML5.fragment(html)
        end

        def to_html
          # A page written in HTML has its sections already.
          return @html if @fragment.element_children.any? { |block| block.name == 'section' }

          name_inline_code
          name_labelled_lists
          name_code_blocks
          name_tables
          name_doors
          place_verdicts
          blocks = @fragment.element_children.to_a
          return @html if blocks.none? { |block| heading?(block) }

          unless heading?(blocks.first)
            raise ArgumentError, "a page starts with its heading, not: #{blocks.first.text.strip}"
          end

          blocks.slice_before { |block| heading?(block) }.map { |group| section(group) }.join("\n")
        end

        private

        def heading?(block)
          HEADINGS.include?(block.name)
        end

        # kramdown names a highlighter on code in a line, which the design does not use.
        def name_inline_code
          @fragment.css('code.highlighter-rouge').each { |code| code.remove_attribute('class') }
        end

        # In a list marked labelled-list, the item that opens in bold is the cost, and the bold is its label.
        def name_labelled_lists
          @fragment.css('ul.labelled-list > li').each do |item|
            add_class(item, 'labelled-list__item')
            label = item.children.first
            next unless label&.name == 'strong'

            add_class(item, 'labelled-list__cost')
            label.name = 'span'
            label['class'] = 'label'
            text = label.next_sibling
            text.content = text.content.lstrip if text&.text?
          end
        end

        # kramdown's own code blocks. One in an include has its class already.
        def name_code_blocks
          @fragment.css('pre.highlight, pre:not([class])').each do |pre|
            code = pre.at_css('code')
            next unless code

            outer = pre.ancestors('div.highlighter-rouge').first || pre
            code.remove_attribute('class')
            code.content = code.text.chomp
            pre['class'] = 'code-block__pre'
            outer.replace(<<~HTML.chomp)
              <div class="code-block" data-code-block>
              #{code_head(outer['data-name'])}
              #{pre.to_html}
              </div>
            HTML
          end
        end

        # With no name there is no head, and the button floats in the corner.
        def code_head(name)
          return copy_button('copy-button code-block__copy') unless name

          %(<div class="code-block__head"><span class="label" data-code-lang>#{escape(name)}</span>) +
            %(#{copy_button('copy-button')}</div>)
        end

        def copy_button(classes)
          %(<button type="button" class="#{classes}" data-copy hidden>Copy</button>)
        end

        # Below 640 a row stacks, and each cell shows its column's name.
        def name_tables
          @fragment.css('table:not([class])').each do |table|
            table['class'] = 'table'
            table['data-table'] = ''
            columns = table.css('thead th').each { |cell| cell['scope'] = 'col' }.map { |cell| cell.text.strip }
            table.css('tbody tr').each do |row|
              row.css('td').zip(columns) { |cell, column| name_cell(cell, column) }
            end
          end
        end

        # A cell that is only emphasis marked table__off is greyed out whole, and not italic.
        def name_cell(cell, column)
          cell['class'] = 'table__text'
          cell['data-label'] = column if column
          mark = cell.at_css('> em.table__off')
          return unless mark && cell.text.strip == mark.text

          add_class(cell, 'table__off')
          mark.replace(mark.children)
        end

        # A list marked doors becomes links to pages.
        def name_doors
          @fragment.css('ul.doors').each do |list|
            links = list.css('> li').map { |item| door(item) }
            list.name = 'div'
            list.inner_html = links.join("\n")
          end
        end

        def door(item)
          link = item.at_css('> a:first-child')
          text = item.inner_html.strip.delete_prefix(link.to_s)
          unless link && text.match?(DOOR_DASH)
            raise ArgumentError, "a doors item is written [Title](link) - text, not: #{item.text.strip}"
          end

          title = link.inner_html
          link['class'] = 'doors__link'
          link.inner_html =
            %(<h3 class="doors__title">#{title}</h3><p class="doors__text">#{text.sub(DOOR_DASH, '')}</p>)
          link.to_html
        end

        def place_verdicts
          @fragment.element_children.each do |verdict|
            next unless verdict.name == 'p' && verdict.classes.include?('verdict')

            before = verdict.previous_element
            next unless before && judged?(before)

            caption_fit_row(verdict) if before.classes.include?('fit-row')
            verdict.parent = before
          end
        end

        def caption_fit_row(verdict)
          verdict.name = 'figcaption'
          verdict['class'] = 'fit-row__caption verdict'
        end

        # What a verdict may be the last line of.
        def judged?(block)
          block.name == 'figure' || block.classes.include?('exchange')
        end

        def section(blocks)
          heading = blocks.first
          looks = known_looks(heading)
          label = heading.remove_attribute('data-label')
          id = heading.remove_attribute('id')
          heading.remove_attribute('class')
          return statement(blocks, looks, label, id) if looks.include?(STATEMENT)

          bands = looks & BANDS
          split = looks & SPLITS
          name_heading(heading, split.any?)
          if label && bands.include?('band--mirrored')
            blocks = [label_over(heading, label), *blocks]
            label = nil
          end

          body_classes = looks & BODIES
          body_classes += ['split', *split] if split.any?
          inner, is_prose = body(blocks, looks, split)
          body_classes << 'prose' if is_prose

          in_grid = label || bands.intersect?(ONE_COLUMN)
          <<~HTML
            #{open_section(bands, id)}
            <div class="container#{' band__grid' if in_grid}">
            #{%(<div class="band__label">#{%(<p class="label">#{escape(label.value)}</p>) unless label.value.empty?}</div>) if label}
            <div class="#{['band__body', *body_classes].join(' ')}">
            #{inner}
            </div>
            </div>
            </section>
          HTML
        end

        def open_section(classes, id)
          %(<section class="#{['band', *classes].join(' ')}"#{%( id="#{escape(id.value)}") if id}>)
        end

        # A mirrored section has no label column, so its label sits over the heading.
        def label_over(heading, label)
          heading.add_previous_sibling(%(<p class="label">#{escape(label.value)}</p>))
          heading.previous_element
        end

        # The heading at display size, each sentence in a span of its own, and the words under it.
        def statement(blocks, looks, label, id)
          heading, *rest = blocks
          if label || looks.difference(BANDS, [STATEMENT]).any?
            raise ArgumentError, "a statement takes only band looks, on: #{heading.text.strip}"
          end

          sentences = heading.inner_html.strip.split(/(?<=\.)\s+/).map { |sentence| "<span>#{sentence}</span>" }
          rest.each { |block| add_class(block, 'statement__text') if block.name == 'p' }
          <<~HTML
            #{open_section([STATEMENT, *(looks & BANDS)], id)}
            <div class="container">
            <p class="display-2">#{sentences.join(' ')}</p>
            #{join(rest)}
            </div>
            </section>
          HTML
        end

        def known_looks(heading)
          unknown = heading.classes.difference(BANDS, SPLITS, BODIES, [ONE_LINE, COLUMNS, STATEMENT])
          return heading.classes if unknown.empty?

          raise ArgumentError, "no section look called #{unknown.join(', ')}, on: #{heading.text.strip}"
        end

        def name_heading(heading, split)
          if heading.name == 'h2'
            heading['class'] = 'band__heading'
          elsif split
            heading['class'] = 'split__title'
          end
        end

        # The inside of a section, and whether the whole of it is one prose box.
        def body(blocks, looks, split)
          before, columns, after = looks.include?(COLUMNS) ? part_columns(blocks) : [blocks, [], []]
          if (before + columns + after).any? { |block| block.name == 'hr' }
            raise ArgumentError, "a --- only ends columns, in: #{blocks.first.text.strip}"
          end

          one_line = looks.include?(ONE_LINE)
          all_words = columns.empty? && before.all? { |block| words?(block) }
          return [join(before), true] if all_words && split.empty? && !one_line

          opening, rest = part_opening(before)
          drawn = columns.empty? ? [] : [columns_html(columns)]
          [[opening_html(opening, split, one_line), *beside(rest, split), *drawn, *runs(after)].join("\n"), false]
        end

        # The heading and the words straight under it.
        def part_opening(blocks)
          opening = blocks.take_while { |block| words?(block) }
          [opening, blocks.drop(opening.size)]
        end

        # A heading on its own keeps out of the box, for the space the design puts under it.
        def opening_html(blocks, split, one_line)
          return join(blocks) if blocks.size == 1 && split.empty?

          blocks.each { |block| add_class(block, 'band__text') if block.name == 'p' } if one_line
          classes = []
          classes << ONE_LINE if one_line
          classes << 'split__text' if split.include?('split--text-right')
          prose(join(blocks), classes)
        end

        # In a split all of it shares a box, or each block would take a cell of the grid.
        def beside(blocks, split)
          return runs(blocks) if split.empty? || blocks.size < 2

          [%(<div class="band__body">\n#{runs(blocks).join("\n")}\n</div>)]
        end

        # Every run of words sits in a prose box. The rest stands on its own.
        def runs(blocks)
          blocks.chunk_while { |one, next_one| words?(one) == words?(next_one) }.map do |run|
            words?(run.first) ? prose(join(run)) : join(run)
          end
        end

        # A block that brings its own prose is outside the run.
        def words?(block)
          WORDS.include?(block.name) && !block.classes.intersect?(%w[prose verdict])
        end

        # The columns start at the first h3 and end at a rule, which is not drawn.
        def part_columns(blocks)
          first = blocks.index { |block| block.name == 'h3' }
          raise ArgumentError, "columns with no h3 in: #{blocks.first.text.strip}" unless first

          rest = blocks.drop(first)
          last = rest.index { |block| block.name == 'hr' } || rest.size
          [blocks.take(first), rest.take(last), rest.drop(last + 1)]
        end

        def columns_html(blocks)
          columns = blocks.slice_before { |block| block.name == 'h3' }.map do |column|
            column.each { |block| name_in_column(block) }
            %(<div class="columns__column">\n#{join(column)}\n</div>)
          end
          %(<div class="columns">\n#{columns.join("\n")}\n</div>)
        end

        def name_in_column(block)
          add_class(block, 'columns__text') if block.name == 'p'
          add_class(block, 'labelled-list--small') if block.classes.include?('labelled-list')
        end

        def prose(html, classes = [])
          %(<div class="#{['prose', *classes].join(' ')}">\n#{html}\n</div>)
        end

        def join(blocks)
          blocks.map(&:to_html).join("\n")
        end

        def escape(text)
          CGI.escapeHTML(text)
        end

        def add_class(element, name)
          element['class'] = [element['class'], name].compact.join(' ')
        end
      end
    end
  end
end
