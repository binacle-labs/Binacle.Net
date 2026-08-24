# frozen_string_literal: true

module Jekyll
  module GTM
    class BodyTag < Tag
      private

      def snippet(id)
        <<~HTML
          <!-- Google Tag Manager (noscript) -->
          <noscript><iframe src="https://www.googletagmanager.com/ns.html?id=#{id}"
          height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>
          <!-- End Google Tag Manager (noscript) -->
        HTML
      end
    end
  end
end
