# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'jekyll'
require 'jekyll-page-meta'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze
TMP_ROOT = Dir.mktmpdir('jekyll-page-meta').freeze

at_exit { FileUtils.rm_rf(TMP_ROOT) }

HOME_PAGE = 'index.html'

module SiteBuilder
  # Every spec builds the fixture site for real. A hash pretending to be a page tests the spec, not the gem.
  def build_site(overrides = {})
    destination = Dir.mktmpdir('site', TMP_ROOT)
    config = Jekyll.configuration(
      { 'source' => FIXTURE_SITE, 'destination' => destination, 'disable_disk_cache' => true }.merge(overrides)
    )
    site = Jekyll::Site.new(config)
    site.process
    site
  end

  def page(site, name)
    found = (site.pages + site.documents).find { |doc| doc.relative_path.to_s.end_with?(name) }
    raise "the fixture site has no #{name}" if found.nil?

    found
  end

  def meta(site, name)
    page(site, name).data['meta']
  end

  def head(site, name)
    page(site, name).output.to_s
  end

  def from(*keys)
    { 'page_meta' => { 'description' => { 'from' => keys } } }
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
