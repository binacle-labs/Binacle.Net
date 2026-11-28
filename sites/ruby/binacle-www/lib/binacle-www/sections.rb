# frozen_string_literal: true

require 'binacle-sites-core'

module Binacle
  module Www
    module Sections
      def www_sections(input)
        Converter.new(input.to_s).to_html
      end

      # The shared sections, with the looks and the parts only this site has: the hero, the statement, columns,
      # the row of steps and the verdicts.
      class Converter < Binacle::SitesCore::Sections
        COLUMNS = 'columns'
        STATEMENT = 'statement'
        HERO = 'hero'
        # The parts of a hero that are not in its prose box.
        HERO_PARTS = %w[actions hero__source].freeze

        private

        def prepare
          name_inline_code
          name_labelled_lists(@fragment)
          name_www_code_blocks
          name_www_tables
          name_doors(@fragment)
          place_verdicts
        end

        def site_looks
          [COLUMNS, STATEMENT, HERO]
        end

        def site_section(blocks, looks, label, id)
          return statement(blocks, looks, label, id) if looks.include?(STATEMENT)

          hero(blocks, looks, label, id) if looks.include?(HERO)
        end

        # kramdown names a highlighter on code in a line, which the design does not use.
        def name_inline_code
          @fragment.css('code.highlighter-rouge').each { |code| code.remove_attribute('class') }
        end

        # kramdown's own code blocks. One in an include has its class already.
        def name_www_code_blocks
          name_code_blocks(@fragment, selector: 'pre.highlight, pre:not([class])', plain: true)
        end

        # Only a table with no class. One in an include has its own.
        def name_www_tables
          name_tables(@fragment, selector: 'table:not([class])', cell: method(:name_cell))
        end

        # A cell that is only code is set as the design sets a value.
        def name_cell(cell)
          cell['class'] = 'table__text'
          return unless cell.children.one? && cell.children.first.name == 'code'

          cell['class'] = 'mono'
          cell.content = cell.text
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

        # A section, and the row of steps written in it, which the design puts after it.
        def part(blocks)
          paths, blocks = blocks.partition { |block| block.classes.include?('path') }
          [section(blocks), *paths.map { |list| path(list) }]
        end

        def path(list)
          name = list.remove_attribute('aria-label')
          list['class'] = 'path__list'
          list.css('> li').each { |item| path_item(item) }
          <<~HTML
            <nav class="band path"#{%( aria-label="#{escape(name.value)}") if name}>
            <div class="container">
            #{list.to_html}
            </div>
            </nav>
          HTML
        end

        def path_item(item)
          link = item.at_css('> a')
          text = item.text.strip
          raise ArgumentError, "a path item is only a link, not: #{text}" unless link&.text == text

          item['class'] = 'path__item'
          link['class'] = 'path__link'
        end

        # The heading at display size, each sentence in a span of its own, and the words under it.
        def statement(blocks, looks, label, id)
          heading, *rest = blocks
          only_bands(STATEMENT, heading, looks, label)
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

        # The top of a home page: the words and the actions on one side, the picture on the other.
        def hero(blocks, looks, label, id)
          heading, *rest = blocks
          only_bands(HERO, heading, looks, label)
          pictures, copy = rest.partition { |block| block.name == 'figure' }
          raise ArgumentError, "a hero has one picture, on: #{heading.text.strip}" unless pictures.one?

          heading['class'] = 'display-1'
          add_class(pictures.first, 'hero__picture')
          <<~HTML
            #{open_section([*(looks & BANDS), HERO], id)}
            <div class="container hero__grid hero__grid--picture-first">
            <div class="hero__copy">
            #{heading.to_html}
            #{hero_copy(copy)}
            </div>
            #{pictures.first.to_html}
            </div>
            </section>
          HTML
        end

        def hero_copy(blocks)
          words = blocks.take_while { |block| words?(block) && !block.classes.intersect?(HERO_PARTS) }
          source, actions = blocks.drop(words.size).partition { |block| block.classes.include?('hero__source') }
          parts = [prose(join(words), ['prose--marketing'])]
          parts << %(<div class="hero__actions">\n#{join(actions)}\n</div>) if actions.any?
          [*parts, *source.map(&:to_html)].join("\n")
        end

        def only_bands(look, heading, looks, label)
          return unless label || looks.difference(BANDS, [look]).any?

          raise ArgumentError, "a #{look} takes only band looks, on: #{heading.text.strip}"
        end

        # The columns start at the first h3 and end at a rule, which is not drawn.
        def divide(blocks, looks)
          looks.include?(COLUMNS) ? part_columns(blocks) : [blocks, [], []]
        end

        def part_columns(blocks)
          first = blocks.index { |block| block.name == 'h3' }
          raise ArgumentError, "columns with no h3 in: #{blocks.first.text.strip}" unless first

          rest = blocks.drop(first)
          last = rest.index { |block| block.name == 'hr' } || rest.size
          [blocks.take(first), rest.take(last), rest.drop(last + 1)]
        end

        def draw_columns(blocks)
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
      end
    end
  end
end
