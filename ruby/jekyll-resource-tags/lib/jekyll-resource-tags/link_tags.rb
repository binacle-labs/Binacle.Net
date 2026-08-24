# frozen_string_literal: true

module Jekyll
  module ResourceTags
    class LinkTags < ResourceTag
      NAME = 'link'
      PATH_KEY = 'href'
      PATH_ATTRIBUTE = 'href'
      DEFAULTS = {}.freeze
      CLOSING = false
    end
  end
end
