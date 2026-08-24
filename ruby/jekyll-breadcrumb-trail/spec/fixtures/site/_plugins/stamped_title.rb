# frozen_string_literal: true

# Proves the trail generator runs late enough to see a stamped key. Swap the two priorities and the
# crumb quietly falls back to the front matter title.
class StampedTitle < Jekyll::Generator
  safe true
  priority :high

  def generate(site)
    page = site.pages.find { |candidate| candidate.url == '/section/v1.0.x/notes/three.html' }
    page.data['crumbtitle'] = 'Stamped crumb' unless page.nil?
  end
end
