# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Two plain keys, written for whatever renders them. This gem does not know what either one is for.
    class VersionGenerator < Jekyll::Generator
      safe true
      # Whatever reads these resolves at a low priority. Stamp after it and the keys are silently missing.
      priority :high

      def generate(site)
        current = site.data.dig('versions', 'current')
        return if current.nil?

        site.documents.each do |doc|
          version = doc.data['version']
          next if version.nil? || version.to_s.empty?

          doc.data['title_suffix'] ||= "(#{version})"
          doc.data['robots'] ||= 'noindex, follow' unless version.to_s == current.to_s
        end
      end
    end
  end
end
