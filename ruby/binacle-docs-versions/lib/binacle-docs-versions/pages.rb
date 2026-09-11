# frozen_string_literal: true

module Binacle
  module DocsVersions
    # The versioned documents, keyed by version and by the path inside the version folder, so the same page
    # can be found in another version.
    class Pages
      INDEX = /\Aindex\.\w+\z/

      def initialize(versioned)
        @by_version = Hash.new { |hash, version| hash[version] = {} }
        versioned.each do |doc|
          @by_version[doc.data['version'].to_s][path_of(doc)] = doc
        end
      end

      def versions
        @by_version.keys.sort
      end

      # The path of the page inside its version folder: _versions/v1.x/a/b.md is a/b.md.
      def path_of(doc)
        doc.relative_path.to_s.sub(VersionGenerator::FOLDER, '')
      end

      # The same page in another version, or that version's index when the page is not there.
      def counterpart(doc, version)
        pages = @by_version[version]
        pages[path_of(doc)] || pages.values.find { |candidate| path_of(candidate).match?(INDEX) }
      end

      # Every page in one version with no page at the same path in another. The redirect list for a major.
      def missing_from(version, other)
        @by_version[version].keys.reject { |path| @by_version[other].key?(path) }.sort
      end
    end
  end
end
