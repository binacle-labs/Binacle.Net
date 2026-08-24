# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-resource-tags/element'
require_relative 'jekyll-resource-tags/resource_tag'
require_relative 'jekyll-resource-tags/link_tags'
require_relative 'jekyll-resource-tags/script_tags'
require_relative 'jekyll-resource-tags/prefetch_tags'

Liquid::Template.register_tag('link_tags', Jekyll::ResourceTags::LinkTags)
Liquid::Template.register_tag('script_tags', Jekyll::ResourceTags::ScriptTags)
Liquid::Template.register_tag('prefetch_tags', Jekyll::ResourceTags::PrefetchTags)
