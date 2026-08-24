# frozen_string_literal: true

module Jekyll
  module StructuredData
    class StructuredDataTag < Liquid::Tag
      def render(context)
        site = context.registers[:site]
        # Jekyll hands a tag its page as a Drop, not a Hash. Both read with [].
        page = context['page']
        return '' if site.nil? || page.nil?

        Json.block(Graph.new(site, Config.from(site), page).to_a)
      end
    end
  end
end
