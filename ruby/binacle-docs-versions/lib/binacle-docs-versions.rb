# frozen_string_literal: true

require 'jekyll'

require_relative 'binacle-docs-versions/generator'
require_relative 'binacle-docs-versions/vlink_tag'

Liquid::Template.register_tag(Binacle::DocsVersions::VLinkTag::NAME, Binacle::DocsVersions::VLinkTag)
