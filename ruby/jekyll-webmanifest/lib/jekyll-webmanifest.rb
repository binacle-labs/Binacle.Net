# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-webmanifest/error'
require_relative 'jekyll-webmanifest/urls'
require_relative 'jekyll-webmanifest/config'
require_relative 'jekyll-webmanifest/manifest'
require_relative 'jekyll-webmanifest/json'
require_relative 'jekyll-webmanifest/generator'
require_relative 'jekyll-webmanifest/link_tag'

Liquid::Template.register_tag('webmanifest_link', Jekyll::Webmanifest::LinkTag)
