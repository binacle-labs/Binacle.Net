# frozen_string_literal: true

module Jekyll
  module Webmanifest
    class WebmanifestGenerator < Jekyll::Generator
      safe true
      # Nothing here is read from another generator. It runs last only so the collision check sees every page.
      priority :lowest

      def generate(site)
        config = Config.from(site)
        return if config.nil?

        guard_collision(site, config.path)
        site.pages << build_page(site, config)
        config.publish_url(Urls.relative(site, config.path))
      end

      private

      def build_page(site, config)
        page = Jekyll::PageWithoutAFile.new(site, site.source, config.dir, config.filename)
        page.content = Json.document(Manifest.new(site, config).to_h)
        page.data['layout'] = nil
        page.data['render_with_liquid'] = false
        page.data['sitemap'] = { 'exclude' => true }
        page
      end

      def guard_collision(site, path)
        taken = site.pages.any? { |page| page.url == path } ||
                site.static_files.any? { |file| file.url == path }
        raise Error, "the webmanifest path #{path} is already written by the site" if taken
      end
    end
  end
end
