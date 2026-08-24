# frozen_string_literal: true

module Jekyll
  module BreadcrumbTrail
    class TrailGenerator < Jekyll::Generator
      safe true
      # This resolves a title, so anything stamping one must have run already. Nothing has to run after
      # it: the key it writes is read at render time.
      priority :low

      def generate(site)
        trail = Trail.new(site, Config.from_site(site))
        (site.pages + site.documents).each { |doc| trail.write(doc) }
      end
    end
  end
end
