# frozen_string_literal: true

module Jekyll
  module StructuredData
    class Config
      ORGANIZATION = 'Organization'
      # The one config word that is not already schema.org's.
      ALIASES = { 'same_as' => 'sameAs' }.freeze

      attr_reader :default_type, :organization

      def self.from(site)
        raw = site.config['structured_data']
        new(raw.is_a?(Hash) ? raw : {})
      end

      def initialize(raw)
        @default_type = raw['default_type']&.to_s
        @defaults = build_defaults(raw['defaults'])
        @organization = build_organization(raw['organization'])
      end

      def organization_id
        @organization&.dig('@id')
      end

      def defaults_for(type)
        @defaults.fetch(type.to_s, {})
      end

      private

      def build_defaults(raw)
        return {} if raw.nil?
        raise Error, 'structured_data defaults must be a map of type to keys' unless raw.is_a?(Hash)

        raw.each_with_object({}) do |(type, keys), all|
          raise Error, "structured_data defaults for #{type} must be a map" unless keys.is_a?(Hash)

          all[type.to_s] = keys
        end
      end

      def build_organization(raw)
        return nil if raw.nil?
        raise Error, 'structured_data organization must be a map' unless raw.is_a?(Hash)

        id = raw['@id'].to_s
        # Derived from site.url it would be a different organisation on every host the site runs on.
        raise Error, 'structured_data organization needs an @id, written out in full' if id.empty?

        node = { '@type' => raw['@type'].nil? ? ORGANIZATION : raw['@type'].to_s, '@id' => id }
        raw.each { |key, value| node[ALIASES.fetch(key, key)] = value unless %w[@type @id].include?(key) }
        node['logo'] = Urls.join(node['url'], node['logo']) unless node['logo'].nil?
        node.compact
      end
    end
  end
end
