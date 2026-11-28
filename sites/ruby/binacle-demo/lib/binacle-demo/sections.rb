# frozen_string_literal: true

require 'binacle-sites-core'

module Binacle
  module Demo
    module Sections
      def demo_sections(input)
        Converter.new(input.to_s).to_html
      end

      # The shared sections, with the one look only this site has: the hero at the top of its home page.
      class Converter < Binacle::SitesCore::Sections
        HERO = 'hero'
        # The classes the hero writes, which the design's Sass has to know.
        HERO_BODY = %w[band__body split hero split--text-left split--centred].freeze
        HERO_TITLE = %w[split__title display-1].freeze
        # What the picture is made of: the figure, and the box it opens larger.
        PICTURE = %w[figure dialog].freeze

        private

        # The tile lists, before the sections are cut, so a band of tiles is not read as words.
        def prepare
          Binacle::SitesCore::Markup.name_doors(@fragment)
        end

        def site_looks
          [HERO]
        end

        def site_section(blocks, looks, label, id)
          hero(blocks, looks, label, id) if looks.include?(HERO)
        end

        # The words, the boxes and the button on one side, the picture on the other.
        def hero(blocks, looks, label, id)
          heading, *rest = blocks
          if label || looks != [HERO]
            raise ArgumentError, "a #{HERO} takes no other look and no label, on: #{heading.text.strip}"
          end

          pictures, copy = rest.partition { |block| PICTURE.include?(block.name) }
          unless pictures.count { |block| block.name == 'figure' } == 1
            raise ArgumentError, "a #{HERO} has one picture, on: #{heading.text.strip}"
          end

          heading['class'] = HERO_TITLE.join(' ')
          <<~HTML
            #{open_section(['band--split'], id)}
            <div class="container band__grid">
            <div class="#{HERO_BODY.join(' ')}">
            <div class="hero__copy">
            #{heading.to_html}
            #{hero_copy(copy)}
            </div>
            #{join(pictures)}
            </div>
            </div>
            </section>
          HTML
        end

        def hero_copy(blocks)
          words = blocks.take_while { |block| words?(block) && !block.classes.include?('actions') }
          [prose(join(words), ['prose--marketing']), join(blocks.drop(words.size))].join("\n")
        end
      end
    end
  end
end
