# frozen_string_literal: true

require 'json'

module Jekyll
  module StructuredData
    module Json
      CONTEXT = 'https://schema.org'

      def self.block(graph)
        return '' if graph.empty?

        json = JSON.pretty_generate('@context' => CONTEXT, '@graph' => graph)
        # A </ anywhere in a value closes the element early and the rest of the graph renders as markup.
        %(<script type="application/ld+json">\n#{json.gsub('</') { '<\\/' }}\n</script>)
      end
    end
  end
end
