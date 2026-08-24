# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-multi-sitemap/error'
require_relative 'jekyll-multi-sitemap/doc'
require_relative 'jekyll-multi-sitemap/urls'
require_relative 'jekyll-multi-sitemap/config'
require_relative 'jekyll-multi-sitemap/selection'
require_relative 'jekyll-multi-sitemap/renderer'
require_relative 'jekyll-multi-sitemap/generator'
require_relative 'jekyll-multi-sitemap/tags'
require_relative 'jekyll-multi-sitemap/url_tag'
require_relative 'jekyll-multi-sitemap/urlset_tag'
require_relative 'jekyll-multi-sitemap/links_tag'

Liquid::Template.register_tag('sitemap_url', Jekyll::MultiSitemap::UrlTag)
Liquid::Template.register_tag('sitemap_urlset', Jekyll::MultiSitemap::UrlsetTag)
Liquid::Template.register_tag('sitemap_links', Jekyll::MultiSitemap::LinksTag)
