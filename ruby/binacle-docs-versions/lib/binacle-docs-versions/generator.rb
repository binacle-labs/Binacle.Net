# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Plain keys, written for whatever renders them. This gem does not know what any of them is for.
    class VersionGenerator < Jekyll::Generator
      COLLECTION = '_versions'
      REDIRECT_LAYOUT = 'redirect'
      FOLDER = %r{\A#{Regexp.escape(COLLECTION)}/([^/]+)/}

      safe true
      # title_suffix is read by a generator at :low. Stamp after it and the suffix is silently missing.
      priority :high

      def generate(site)
        versioned = site.documents.select { |doc| stamp_version(doc) }
        return if versioned.empty?

        current = current_version(site, versioned)
        tags = version_tags(site)
        # The suffix and the tag come off the page's own version, never off current.
        versioned.each do |doc|
          version = version_of(doc)
          doc.data['title_suffix'] ||= "(#{version})"
          doc.data['version_tag'] ||= tags.fetch(version) { raise Error, missing_tag(version, tags) }
          doc.data['robots'] ||= 'noindex, follow' unless version == current
        end

        stamp_redirects(site, current)
      end

      private

      # The folder under _versions/ is the version. Nothing in the folder, and no config block, has to say so.
      def stamp_version(doc)
        folder = doc.relative_path.to_s[FOLDER, 1]
        return false if folder.nil?

        doc.data['version'] ||= folder
        true
      end

      # Raising rather than skipping: a page would print a pull command with no tag on it, and nothing would say so.
      def missing_tag(version, tags)
        known = tags.keys.sort
        "versions list has no version_tag for #{version}; it has one for #{known.empty? ? 'nothing' : known.join(', ')}"
      end

      def version_tags(site)
        list = site.data.dig('versions', 'list')
        return {} unless list.is_a?(Array)

        list.each_with_object({}) do |entry, tags|
          next unless entry.is_a?(Hash) && entry['id'] && entry['version_tag']

          tags[entry['id'].to_s] = entry['version_tag'].to_s
        end
      end

      # The redirect page's canonical is the page it points at, which is what says the two are one
      # destination. Both keys carry the same url so the head and the redirect cannot disagree.
      def stamp_redirects(site, current)
        pages = (site.pages + site.documents).select { |doc| doc.data['layout'].to_s == REDIRECT_LAYOUT }
        return if pages.empty?

        url = current_root(site, current)
        pages.each do |doc|
          doc.data['redirect_to'] ||= url
          doc.data['canonical'] ||= doc.data['redirect_to']
          doc.data['robots'] ||= 'noindex'
        end
      end

      def current_root(site, current)
        index = %r{\A#{Regexp.escape(COLLECTION)}/#{Regexp.escape(current)}/index\.\w+\z}
        found = site.documents.find { |doc| doc.relative_path.to_s.match?(index) }
        raise Error, "#{COLLECTION}/#{current}/index has no document, so a redirect has nowhere to point" if found.nil?

        found.url
      end

      def version_of(doc)
        doc.data['version'].to_s
      end

      # Raising rather than skipping: a current that matches nothing would noindex the whole site, silently.
      def current_version(site, versioned)
        known = versioned.map { |doc| version_of(doc) }.uniq.sort
        current = site.data.dig('versions', 'current').to_s
        raise Error, "versions current is not set, and the site has #{known.join(', ')}" if current.empty?

        return current if known.include?(current)

        raise Error, "versions current is #{current.inspect}, which is not one of #{known.join(', ')}"
      end
    end
  end
end
