# frozen_string_literal: true

module Jekyll
  module PageMeta
    class MetaGenerator < Jekyll::Generator
      safe true
      # Anything stamping a key this reads - title_suffix, robots - runs high and must run first.
      priority :low

      def generate(site)
        resolver = Resolver.new(site, Config.from_site(site))
        (site.pages + site.documents).each { |doc| resolver.write(doc) }
      end
    end
  end
end
