# frozen_string_literal: true

require 'jekyll'

require_relative 'binacle-www/sections'

Liquid::Template.register_filter(Binacle::Www::Sections)
