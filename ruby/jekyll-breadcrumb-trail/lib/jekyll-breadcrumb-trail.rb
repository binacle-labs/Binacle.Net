# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-breadcrumb-trail/error'
require_relative 'jekyll-breadcrumb-trail/urls'
require_relative 'jekyll-breadcrumb-trail/labels'
require_relative 'jekyll-breadcrumb-trail/config'
require_relative 'jekyll-breadcrumb-trail/trail'
require_relative 'jekyll-breadcrumb-trail/generator'
require_relative 'jekyll-breadcrumb-trail/nav'
require_relative 'jekyll-breadcrumb-trail/tag'

Liquid::Template.register_tag('breadcrumbs', Jekyll::BreadcrumbTrail::BreadcrumbsTag)
