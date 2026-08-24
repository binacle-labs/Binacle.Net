# frozen_string_literal: true

module Jekyll
  module ResourceTags
    class PrefetchTags < ResourceTag
      NAME = 'link'
      PATH_KEY = 'src'
      PATH_ATTRIBUTE = 'href'
      DEFAULTS = { 'rel' => 'prefetch', 'as' => 'script' }.freeze
      # The only content attributes that exist on <script> and not on <link>.
      SKIP = %w[async defer nomodule].freeze
      CLOSING = false
    end
  end
end
