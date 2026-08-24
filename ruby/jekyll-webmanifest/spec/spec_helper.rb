# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'json'
require 'jekyll'
require 'jekyll-webmanifest'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze
TMP_ROOT = Dir.mktmpdir('jekyll-webmanifest').freeze

at_exit { FileUtils.rm_rf(TMP_ROOT) }

module SiteBuilder
  # Every spec builds the fixture site for real. A hash pretending to be a site tests the spec, not the gem.
  def build_site(overrides = {})
    destination = Dir.mktmpdir('site', TMP_ROOT)
    config = Jekyll.configuration(
      { 'source' => FIXTURE_SITE, 'destination' => destination, 'disable_disk_cache' => true }.merge(overrides)
    )
    site = Jekyll::Site.new(config)
    site.process
    site
  end

  def built(site, path)
    File.read(File.join(site.dest, path))
  end

  def written?(site, path)
    File.exist?(File.join(site.dest, path))
  end

  def manifest(site, path = 'site.webmanifest')
    JSON.parse(built(site, path))
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
