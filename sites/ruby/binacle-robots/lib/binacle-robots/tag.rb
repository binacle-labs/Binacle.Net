# frozen_string_literal: true

module Binacle
  module Robots
    class RobotsTag < Liquid::Tag
      # No trailing newline: the newline after the tag in the file supplies it.
      def render(_context)
        Body.text
      end
    end
  end
end
