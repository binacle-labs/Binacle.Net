# frozen_string_literal: true

module Jekyll
  module MultiSitemap
    class SitemapGenerator < Jekyll::Generator
      safe true
      # A sitemap written before another generator has added its pages is missing them, and nothing fails.
      priority :lowest

      def generate(site)
        config = Config.from(site)
        return if config.nil? || config.files.empty?

        pages = config.files.map { |file| build_file(site, file, config.mode) }
        urls = pages.map { |page| Urls.absolute(site, page.url) }

        if config.index
          collision = pages.find { |page| page.url == config.index_path }
          raise Error, "the sitemaps index path #{config.index} is also a generated file" if collision

          index = build_index(site, config, urls)
          pages << index
          urls = [Urls.absolute(site, index.url)]
        end

        config.publish_urls(urls)
        site.pages.concat(pages)
      end

      private

      def build_file(site, file, mode)
        documents = Selection.documents(site, file, mode)
        page(site, file.dir, file.filename, Renderer.urlset(site, documents))
      end

      def build_index(site, config, urls)
        page(site, config.index_dir, config.index_name, Renderer.index(site, urls))
      end

      def page(site, dir, name, content)
        page = Jekyll::PageWithoutAFile.new(site, site.source, dir, name)
        page.content = content
        page.data['layout'] = nil
        page.data['render_with_liquid'] = false
        page.data['sitemap'] = { 'exclude' => true }
        page
      end
    end
  end
end
