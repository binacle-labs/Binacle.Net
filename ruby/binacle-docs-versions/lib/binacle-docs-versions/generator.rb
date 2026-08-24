# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Plain keys, written for whatever renders them. This gem does not know what any of them is for.
    class VersionGenerator < Jekyll::Generator
      COLLECTION = '_versions'
      REDIRECT_LAYOUT = 'redirect'

      safe true
      # title_suffix is read by a generator at :low. Stamp after it and the suffix is silently missing.
      priority :high

      def generate(site)
        versioned = site.documents.select { |doc| version_of(doc) }
        return if versioned.empty?

        # Stamped before the check below: the suffix comes off the page's own version, never off current.
        versioned.each { |doc| doc.data['title_suffix'] ||= "(#{version_of(doc)})" }

        current = current_version(site, versioned)
        versioned.each do |doc|
          doc.data['robots'] ||= 'noindex, follow' unless version_of(doc) == current
        end

        stamp_redirects(site, current)
      end

      private

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
        version = doc.data['version'].to_s
        version.empty? ? nil : version
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
