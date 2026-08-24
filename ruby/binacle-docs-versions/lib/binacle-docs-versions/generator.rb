# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Two plain keys, written for whatever renders them. This gem does not know what either one is for.
    class VersionGenerator < Jekyll::Generator
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
      end

      private

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
