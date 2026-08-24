# frozen_string_literal: true

module Jekyll
  module ResourceTags
    class ScriptTags < ResourceTag
      NAME = 'script'
      PATH_KEY = 'src'
      PATH_ATTRIBUTE = 'src'
      DEFAULTS = {}.freeze
      CLOSING = true
    end
  end
end
