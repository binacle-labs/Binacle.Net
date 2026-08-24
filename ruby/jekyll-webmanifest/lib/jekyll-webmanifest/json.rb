# frozen_string_literal: true

require 'json'

module Jekyll
  module Webmanifest
    module Json
      def self.document(members)
        "#{JSON.pretty_generate(members)}\n"
      end
    end
  end
end
