# frozen_string_literal: true

module Jekyll
  module SiteFilters
    module Sanitization
      # [^<>] rather than [^>]: '<a <b>' is one tag to [^>]* and two to a browser. Measured 2x faster
      # on angle-bracket-heavy input too.
      TAG = %r{</?[^<>]*>}

      def clean_content(input, length = 160)
        input.to_s
             .gsub(TAG, '')
             # One gsub leaves '<script' behind on '<scr<x>ipt>'. Nothing here is a tag any more.
             # CodeQL still flags the gsub above and wants it looped until stable. Don't. I measured
             # the loop at 0.4s on 4k characters, quadratic. This line is what closes it.
             .delete('<>')
             .gsub(/\n+/, ' ')
             .gsub(/ {2,}/, ' ')
             .strip[0...length]
             .strip
      end
    end
  end
end
