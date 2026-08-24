# frozen_string_literal: true

require 'cgi'

module Jekyll
  module BreadcrumbTrail
    # The markup the ARIA authoring practices specify, with the class names Bootstrap made standard.
    # A site that wants other markup renders page.breadcrumb_trail in its own Liquid.
    class Nav
      ITEM = 'breadcrumb-item'
      CURRENT = 'breadcrumb-item active'

      def initialize(site, config, trail)
        @site = site
        @config = config
        @trail = trail
      end

      def to_s
        return '' if @trail.empty?

        [open, *items, '</ol>', '</nav>'].join("\n")
      end

      private

      def open
        classes = @config.extra_class? ? %( class="#{escape(@config.extra_class)}") : ''
        %(<nav aria-label="#{escape(@config.label)}"#{classes}>\n<ol class="breadcrumb">)
      end

      def items
        last = @trail.length - 1
        @trail.each_with_index.map { |crumb, index| item(crumb, index.zero?, index == last) }
      end

      def item(crumb, home, current)
        text = home && @config.home_html? ? @config.home_html : escape(crumb['name'])
        return %(<li class="#{CURRENT}" aria-current="page">#{text}</li>) if current && !@config.link_last?

        %(<li class="#{current ? CURRENT : ITEM}">#{link(crumb, text, current)}</li>)
      end

      def link(crumb, text, current)
        href = escape(Urls.relative(@site, crumb['url']))
        marker = current ? ' aria-current="page"' : nil
        %(<a href="#{href}"#{marker}>#{text}</a>)
      end

      def escape(value)
        CGI.escapeHTML(value.to_s)
      end
    end
  end
end
