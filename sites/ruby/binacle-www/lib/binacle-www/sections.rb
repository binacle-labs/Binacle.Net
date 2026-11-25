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
        COLUMNS = 'columns'

        def initialize(html)
          @html = html
          @fragment = Nokogiri::HTML5.fragment(html)
        end

        def to_html
          # A page written in HTML has its sections already.
          return @html if @fragment.element_children.any? { |block| block.name == 'section' }

          name_inline_code
          name_labelled_lists
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

        def place_verdicts
          @fragment.element_children.each do |verdict|
            next unless verdict.name == 'p' && verdict.classes.include?('verdict')

            before = verdict.previous_element
            verdict.parent = before if before && judged?(before)
          end
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

          split = looks & SPLITS
          in_columns = looks.include?(COLUMNS)
          name_heading(heading, split.any?)

          body_classes = split.any? ? ['split', *split] : []
          inner, all_words = body(blocks, in_columns)
          if all_words && split.empty?
            body_classes << 'prose'
          elsif all_words
            inner = prose(inner)
          end

          bands = looks & BANDS
          <<~HTML
            <section class="#{['band', *bands].join(' ')}"#{%( id="#{escape(id.value)}") if id}>
            <div class="container band__grid">
            #{%(<div class="band__label">#{%(<p class="label">#{escape(label.value)}</p>) unless label.value.empty?}</div>) if label}
            <div class="#{['band__body', *body_classes].join(' ')}">
            #{inner}
            </div>
            </div>
            </section>
          HTML
        end

        def known_looks(heading)
          unknown = heading.classes - BANDS - SPLITS - [COLUMNS]
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

        # The inside of a section, and whether it is words from end to end.
        def body(blocks, in_columns)
          before, columns, after = in_columns ? part_columns(blocks) : [blocks, [], []]
          if (before + columns + after).any? { |block| block.name == 'hr' }
            raise ArgumentError, "a --- only ends columns, in: #{blocks.first.text.strip}"
          end

          return [join(before), true] if columns.empty? && before.all? { |block| words?(block) }

          drawn = columns.empty? ? [] : [columns_html(columns)]
          [(runs(before) + drawn + runs(after)).join("\n"), false]
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

        def prose(html)
          %(<div class="prose">\n#{html}\n</div>)
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
