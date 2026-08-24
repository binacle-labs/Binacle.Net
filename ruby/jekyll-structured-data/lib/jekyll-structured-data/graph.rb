# frozen_string_literal: true

module Jekyll
  module StructuredData
    # Every value here was resolved by something else. Nothing in this gem works one out.
    class Graph
      def initialize(site, config, page)
        @site = site
        @config = config
        @page = page
        @meta = hash(page['meta'])
        @page_config = hash(page['structured_data'])
        @trail = page['breadcrumb_trail']
      end

      def to_a
        page = page_node
        crumbs = breadcrumb_node
        # An organisation on its own says nothing about the page it is on.
        return [] if page.nil? && crumbs.nil?

        [@config.organization, page, crumbs].compact
      end

      private

      def page_node
        type = @page_config['type'] || @config.default_type
        return nil if type.nil? || type.to_s.empty?

        node = { '@type' => type.to_s }
        node['@id'] = "#{canonical}#page" if canonical
        node['name'] = name if name
        node['description'] = @meta['description'] if @meta['description']
        node['url'] = canonical if canonical
        node['image'] = @meta['image'] if @meta['image']
        node['publisher'] = { '@id' => @config.organization_id } if @config.organization_id
        node['breadcrumb'] = { '@id' => breadcrumb_id } if breadcrumb_node && breadcrumb_id

        @config.defaults_for(type).each { |key, value| node[key] = value unless node.key?(key) }
        @page_config.each { |key, value| node[key] = value unless key == 'type' }
        node
      end

      def breadcrumb_node
        return @breadcrumb_node if defined?(@breadcrumb_node)

        @breadcrumb_node = build_breadcrumb_node
      end

      def build_breadcrumb_node
        # A trail on a page nothing will crawl claims a hierarchy nothing will follow.
        return nil if noindex?

        crumbs = Array(@trail).grep(Hash)
        return nil if crumbs.empty?

        node = { '@type' => 'BreadcrumbList' }
        node['@id'] = breadcrumb_id if breadcrumb_id
        node['itemListElement'] = crumbs.each_with_index.map { |crumb, index| list_item(crumb, index + 1) }
        node
      end

      def list_item(crumb, position)
        item = { '@type' => 'ListItem', 'position' => position }
        item['name'] = crumb['name'].to_s unless crumb['name'].to_s.empty?
        item['item'] = Urls.absolute(@site, crumb['url']) unless crumb['url'].to_s.empty?
        item
      end

      def name
        found = @page_config['name'] || @meta['title']
        found.to_s.empty? ? nil : found
      end

      def canonical
        return @canonical if defined?(@canonical)

        url = @meta['canonical'].to_s
        @canonical = url.empty? ? nil : Urls.absolute(@site, url)
      end

      def breadcrumb_id
        canonical && "#{canonical}#breadcrumbs"
      end

      def noindex?
        @page['robots'].to_s.downcase.include?('noindex')
      end

      def hash(value)
        value.is_a?(Hash) ? value : {}
      end
    end
  end
end
