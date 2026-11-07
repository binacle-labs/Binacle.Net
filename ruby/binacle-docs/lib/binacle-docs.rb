# frozen_string_literal: true

require 'jekyll'

require_relative 'binacle-docs/versions/error'
require_relative 'binacle-docs/versions/pages'
require_relative 'binacle-docs/pager'
require_relative 'binacle-docs/versions/urls'
require_relative 'binacle-docs/versions/versioned_file'
require_relative 'binacle-docs/versions/generator'
require_relative 'binacle-docs/versions/vlink_tag'
require_relative 'binacle-docs/markup/doc_body'
require_relative 'binacle-docs/markup/sections'

Liquid::Template.register_tag(Binacle::Docs::Versions::VLinkTag::NAME, Binacle::Docs::Versions::VLinkTag)
Liquid::Template.register_filter(Binacle::Docs::Markup::DocBody)
Liquid::Template.register_filter(Binacle::Docs::Markup::Sections)
