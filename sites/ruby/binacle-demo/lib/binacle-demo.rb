# frozen_string_literal: true

require 'jekyll'

require_relative 'binacle-demo/sections'

Liquid::Template.register_filter(Binacle::Demo::Sections)
