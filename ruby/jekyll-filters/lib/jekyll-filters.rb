# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-filters/sanitization'
require_relative 'jekyll-filters/capitalization'
require_relative 'jekyll-filters/dates'

Liquid::Template.register_filter(Jekyll::SiteFilters::Sanitization)
Liquid::Template.register_filter(Jekyll::SiteFilters::Capitalization)
Liquid::Template.register_filter(Jekyll::SiteFilters::Dates)
