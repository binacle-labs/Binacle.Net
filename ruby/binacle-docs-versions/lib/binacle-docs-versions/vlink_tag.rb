# frozen_string_literal: true

module Binacle
  module DocsVersions
    # A link to a file in the version the current page belongs to, whatever version that is.
    class VLinkTag < Liquid::Tag
      include Jekyll::Filters::URLFilters

      NAME = 'vlink'
      COLLECTION = '_versions'

      def initialize(tag_name, relative_path, tokens)
        super
        @relative_path = relative_path.strip
      end

      def render(context)
        # relative_url reads the site off @context, which Liquid does not set.
        @context = context
        site = context.registers[:site]
        version = context.registers[:page]['version']

        relative_path = Liquid::Template.parse(@relative_path).render(context)
        versioned_path = Jekyll::PathManager.join(COLLECTION, Jekyll::PathManager.join(version, relative_path))
        # A static file carries a leading slash on its relative path and a document does not.
        with_leading_slash = Jekyll::PathManager.join('', versioned_path)

        site.each_site_file do |item|
          return relative_url(item) if item.relative_path == versioned_path
          return relative_url(item) if item.relative_path == with_leading_slash
        end

        raise ArgumentError, <<~MSG
          Could not find document '#{relative_path}' in tag '#{NAME}'.

          Make sure the document exists and the path is correct.
        MSG
      end
    end
  end
end
