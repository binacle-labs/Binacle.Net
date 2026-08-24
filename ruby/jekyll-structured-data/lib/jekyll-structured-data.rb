# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-structured-data/error'
require_relative 'jekyll-structured-data/urls'
require_relative 'jekyll-structured-data/config'
require_relative 'jekyll-structured-data/graph'
require_relative 'jekyll-structured-data/json'
require_relative 'jekyll-structured-data/tag'

Liquid::Template.register_tag('structured_data', Jekyll::StructuredData::StructuredDataTag)
