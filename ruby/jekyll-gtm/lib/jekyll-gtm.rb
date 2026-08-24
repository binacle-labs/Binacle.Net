# frozen_string_literal: true

require 'jekyll'

require_relative 'jekyll-gtm/tag'
require_relative 'jekyll-gtm/head_tag'
require_relative 'jekyll-gtm/body_tag'

Liquid::Template.register_tag('gtm_head', Jekyll::GTM::HeadTag)
Liquid::Template.register_tag('gtm_body', Jekyll::GTM::BodyTag)
