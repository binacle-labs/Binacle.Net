# frozen_string_literal: true

module Jekyll
  module PageMeta
    class MetaGenerator < Jekyll::Generator
      safe true
      # title_suffix is the one key this reads off another plugin, so whatever stamps it has to run higher.
      priority :low

      def generate(site)
        resolver = Resolver.new(site, Config.from_site(site))
        (site.pages + site.documents).each { |doc| resolver.write(doc) }
      end
    end
  end
end
