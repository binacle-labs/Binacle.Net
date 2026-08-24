# frozen_string_literal: true

module Jekyll
  module BreadcrumbTrail
    # The one computation. The tag renders what this writes and works nothing out of its own, so the
    # visible trail and any markup built from the same key cannot disagree.
    class Trail
      def initialize(site, config)
        @site = site
        @config = config
      end

      def write(doc)
        doc.data['breadcrumb_trail'] = compute(doc)
      end

      private

      def compute(doc)
        return [] if doc.data['breadcrumbs'] == false

        url = doc.url.to_s.sub(%r{/index\.html\z}, '/')
        segments = url.split('/').reject(&:empty?)
        return [] if segments.empty?

        crumbs(doc, url, segments)
      end

      def crumbs(doc, url, segments)
        last = segments.length - 1
        start = home_index(segments)
        trail = [crumb(@config.home_title, path(segments, start, true))]
        ((start + 1)..last).each do |index|
          directory = index < last || url.end_with?('/')
          trail << crumb(name(doc, segments, index, last), path(segments, index, directory))
        end
        trail
      end

      # The home crumb is the deepest excluded segment, not the site root, so a trail below one starts
      # there instead of offering a link out of it. The current page is never excluded, whatever it matches.
      def home_index(segments)
        (0...(segments.length - 1)).select { |index| @config.excluded?(segments[index]) }.max || -1
      end

      # An excluded segment keeps its place in every url below it. A trail a crawler follows has to be real.
      def path(segments, index, directory)
        return '/' if index.negative?

        built = "/#{segments[0..index].join('/')}"
        directory ? "#{built}/" : built
      end

      def name(doc, segments, index, last)
        return Labels.humanize(segments[index]) unless index == last

        Labels.first_of(doc.data, @config.title_from) || Labels.humanize(segments[index])
      end

      def crumb(name, url)
        { 'name' => name, 'url' => url }
      end
    end
  end
end
