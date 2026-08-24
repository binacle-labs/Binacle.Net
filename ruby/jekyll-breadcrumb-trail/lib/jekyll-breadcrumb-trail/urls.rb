# frozen_string_literal: true

module Jekyll
  module BreadcrumbTrail
    # A crumb url is published without the baseurl, the way Jekyll writes doc.url, so that whatever
    # resolves it to an absolute url can prepend the baseurl itself without doubling it.
    module Urls
      def self.relative(site, input)
        baseurl = site.config['baseurl'].to_s.chomp('/')
        return input.to_s if baseurl.empty?

        "#{baseurl}#{input}"
      end
    end
  end
end
