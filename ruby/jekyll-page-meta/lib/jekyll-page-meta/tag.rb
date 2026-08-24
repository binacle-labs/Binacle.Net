# frozen_string_literal: true

module Jekyll
  module PageMeta
    class MetaTag < Liquid::Tag
      def render(context)
        site = context.registers[:site]
        # Jekyll hands a tag its page as a Drop, not a Hash. Both read with [].
        page = context['page']
        return '' if site.nil? || page.nil?

        Head.new(site, Config.from_site(site), page).to_s
      end
    end
  end
end
