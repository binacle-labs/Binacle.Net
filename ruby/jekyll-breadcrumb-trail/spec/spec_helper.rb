# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'jekyll'
require 'jekyll-breadcrumb-trail'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze
TMP_ROOT = Dir.mktmpdir('jekyll-breadcrumb-trail').freeze

at_exit { FileUtils.rm_rf(TMP_ROOT) }

DEEP_PAGE = 'deep.html'

module SiteBuilder
  # Every spec builds the fixture site for real. A hash pretending to be a page tests the spec, not the gem.
  def build_site(breadcrumbs = {}, overrides = {})
    destination = Dir.mktmpdir('site', TMP_ROOT)
    config = Jekyll.configuration(
      { 'source' => FIXTURE_SITE, 'destination' => destination, 'disable_disk_cache' => true,
        'breadcrumbs' => breadcrumbs }.merge(overrides)
    )
    site = Jekyll::Site.new(config)
    site.process
    site
  end

  def page(site, name)
    found = (site.pages + site.documents).find { |doc| doc.relative_path.to_s.end_with?(name) }
    raise ArgumentError, "the fixture site has no #{name}" if found.nil?

    found
  end

  def trail(site, name)
    page(site, name).data['breadcrumb_trail']
  end

  def nav(site, name)
    page(site, name).output.to_s.strip
  end

  def versioned(overrides = {})
    { 'exclude' => ['section', '*.*'] }.merge(overrides)
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
