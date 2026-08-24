# frozen_string_literal: true

module Binacle
  module Robots
    module Body
      PATH = File.join(__dir__, 'robots.txt')

      # Read once, at load. The text is a rights reservation, not a template - nothing substitutes into it.
      TEXT = File.read(PATH, encoding: 'UTF-8').chomp.freeze

      def self.text
        TEXT
      end
    end
  end
end
