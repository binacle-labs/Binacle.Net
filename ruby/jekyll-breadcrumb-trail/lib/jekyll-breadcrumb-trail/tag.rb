# frozen_string_literal: true

module Jekyll
  module BreadcrumbTrail
    class BreadcrumbsTag < Liquid::Tag
      def render(context)
        site = context.registers[:site]
        # Jekyll hands a tag its page as a Drop, not a Hash. Both read with [].
        page = context['page']
        return '' if site.nil? || page.nil?

        Nav.new(site, Config.from_site(site), Array(page['breadcrumb_trail']).grep(Hash)).to_s
      end
    end
  end
end
