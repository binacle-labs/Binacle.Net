# frozen_string_literal: true

# The only way to hand the gem a page with no url. A normal build never produces one.
module Fixtures
  class UrllessPage < Jekyll::PageWithoutAFile
    def url
      ''
    end

    # Its destination is the site root, and writing a file over a directory raises.
    def write(dest); end
  end

  class UrllessGenerator < Jekyll::Generator
    priority :highest

    def generate(site)
      page = UrllessPage.new(site, site.source, '', 'urlless.html')
      page.data['title'] = 'Urlless'
      page.data['layout'] = 'page'
      site.pages << page
    end
  end
end
