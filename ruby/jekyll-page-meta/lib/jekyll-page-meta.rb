# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-page-meta/error'
require_relative 'jekyll-page-meta/urls'
require_relative 'jekyll-page-meta/text'
require_relative 'jekyll-page-meta/config'
require_relative 'jekyll-page-meta/resolver'
require_relative 'jekyll-page-meta/generator'
require_relative 'jekyll-page-meta/elements'
require_relative 'jekyll-page-meta/head'
require_relative 'jekyll-page-meta/tag'

Liquid::Template.register_tag('page_meta', Jekyll::PageMeta::MetaTag)
