# frozen_string_literal: true

module Binacle
  module DocsVersions
    # A link to a file in a version folder - the current page's version, or the one named first.
    #   {% vlink /swagger/v3.json %}                              inside the page's own version
    #   {% vlink v2.x /configuration/service-module/index.md %}   inside another version, by folder id
    # Adapted from Jekyll's own link tag, so the not-found message reads the same on purpose.
    class VLinkTag < Liquid::Tag
      include Jekyll::Filters::URLFilters

      NAME = 'vlink'
      COLLECTION = '_versions'

      def initialize(tag_name, markup, tokens)
        super
        @markup = markup.strip
      end

      def render(context)
        # relative_url reads the site off @context, which Liquid does not set.
        @context = context
        site = context.registers[:site]
        version, path = split(site, context.registers[:page]['version'])

        relative_path = Liquid::Template.parse(path).render(context)
        versioned_path = Jekyll::PathManager.join(COLLECTION, Jekyll::PathManager.join(version, relative_path))

        site.each_site_file do |item|
          return relative_url(item) if item.relative_path == versioned_path
        end

        raise ArgumentError, <<~MSG
          Could not find document '#{relative_path}' in version '#{version}' in tag '#{NAME}'.

          Make sure the document exists and the path is correct.
        MSG
      end

      private

      # The first word is a version only if the versions list knows it; a path may carry Liquid with spaces
      # in it, so the split cannot be blind.
      def split(site, own_version)
        first, rest = @markup.split(/\s+/, 2)
        return [first, rest] if rest && known_versions(site).include?(first)

        [own_version, @markup]
      end

      def known_versions(site)
        list = site.data.dig('versions', 'list')
        return [] unless list.is_a?(Array)

        list.filter_map { |entry| entry['id'].to_s if entry.is_a?(Hash) && entry['id'] }
      end
    end
  end
end
