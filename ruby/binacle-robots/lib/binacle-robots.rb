# frozen_string_literal: true

require 'jekyll'

require_relative 'binacle-robots/body'
require_relative 'binacle-robots/tag'

Liquid::Template.register_tag('robots', Binacle::Robots::RobotsTag)
