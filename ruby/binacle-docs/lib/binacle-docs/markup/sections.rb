# frozen_string_literal: true

module Binacle
  module Docs
    module Markup
      module Sections
        # kramdown writes every h2 on one pattern, so this needs no HTML parser.
        SECTION = %r{<h2\b[^>]*\bid="([^"]+)"[^>]*>(.*?)</h2>}m
        TAG = %r{</?[^<>]*>}

        def doc_sections(input)
          input.to_s.scan(SECTION).map do |id, title|
            { 'id' => id, 'title' => title.gsub(TAG, '').strip }
          end
        end
      end
    end
  end
end
