# frozen_string_literal: true

require 'cgi/escape'
require 'nokogiri'
require_relative 'markup'

module Binacle
  module SitesCore
    # Turns what kramdown writes into the design's sections. Every h1 and h2 starts one, and the attributes on
    # the heading say how the section looks. A site subclasses it and overrides the hooks at the bottom.
    class Sections
      include Markup

      HEADINGS = %w[h1 h2].freeze
      WORDS = %w[h1 h2 h3 h4 p ul ol].freeze
      # What a heading may be marked on every site. Anything else is a typo, which would show as a section with
      # no look.
      BANDS = %w[
        band--compact band--deep band--mirrored band--numbered band--number-in-column band--number-in-row
        band--split band--tint band--wide
      ].freeze
      SPLITS = %w[split--centred split--text-left split--text-right].freeze
      BODIES = %w[band__body--code-left].freeze
      ONE_LINE = 'prose--one-line'
      # With no label column, any other band would put its body in the label's place.
      ONE_COLUMN = %w[band--mirrored band--split band--wide].freeze

      def initialize(html)
        @fragment = Nokogiri::HTML5.fragment(html)
      end

      def to_html
        prepare
        blocks = @fragment.element_children.to_a
        return @fragment.to_html if blocks.none? { |block| heading?(block) }

        unless heading?(blocks.first)
          raise ArgumentError, "a page starts with its heading, not: #{blocks.first.text.strip}"
        end

        blocks.slice_before { |block| heading?(block) }.flat_map { |group| part(group) }.join("\n")
      end

      private

      # Hooks a site overrides.

      # Runs on the whole page before it is split.
      def prepare; end

      # The looks this site adds to the shared ones.
      def site_looks
        []
      end

      # A section whose look is the site's own. Gives back its html, or nil to build it the shared way.
      def site_section(_blocks, _looks, _label, _id)
        nil
      end

      # What a section is written as: the section and anything the site puts after it.
      def part(blocks)
        [section(blocks)]
      end

      # Cuts a section's blocks into those before the site's own part, that part, and those after it.
      def divide(blocks, _looks)
        [blocks, [], []]
      end

      def draw_columns(_columns)
        raise NotImplementedError, "#{self.class} divides a section but does not draw the part"
      end

      def heading?(block)
        HEADINGS.include?(block.name)
      end

      def section(blocks)
        heading = blocks.first
        looks = known_looks(heading)
        label = heading.remove_attribute('data-label')
        id = heading.remove_attribute('id')
        heading.remove_attribute('class')
        own = site_section(blocks, looks, label, id)
        return own if own

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

      def known_looks(heading)
        unknown = heading.classes.difference(BANDS, SPLITS, BODIES, [ONE_LINE], site_looks)
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
        before, own, after = divide(blocks, looks)
        # A divide takes the rules it uses. One left over is a typo, which would draw a stray line.
        if (before + own + after).any? { |block| block.name == 'hr' }
          raise ArgumentError, "a --- only ends columns, in: #{blocks.first.text.strip}"
        end

        one_line = looks.include?(ONE_LINE)
        all_words = own.empty? && before.all? { |block| words?(block) }
        return [join(before), true] if all_words && split.empty? && !one_line

        opening, rest = part_opening(before)
        drawn = own.empty? ? [] : [draw_columns(own)]
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

      def prose(html, classes = [])
        %(<div class="#{['prose', *classes].join(' ')}">\n#{html}\n</div>)
      end

      def join(blocks)
        blocks.map(&:to_html).join("\n")
      end

      def escape(text)
        CGI.escapeHTML(text)
      end
    end
  end
end
