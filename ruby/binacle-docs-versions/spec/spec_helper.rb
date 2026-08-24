# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'jekyll'
require 'binacle-docs-versions'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze
BROKEN_SITE = File.expand_path('fixtures/broken', __dir__).freeze
NO_INDEX_SITE = File.expand_path('fixtures/no-index', __dir__).freeze
TMP_ROOT = Dir.mktmpdir('binacle-docs-versions').freeze

at_exit { FileUtils.rm_rf(TMP_ROOT) }

module SiteBuilder
  def build_site(overrides = {}, source = FIXTURE_SITE)
    destination = Dir.mktmpdir('site', TMP_ROOT)
    config = Jekyll.configuration(
      { 'source' => source, 'destination' => destination, 'disable_disk_cache' => true }.merge(overrides)
    )
    site = Jekyll::Site.new(config)
    site.process
    site
  end

  def doc(site, path)
    found = (site.documents + site.pages).find { |candidate| candidate.relative_path.to_s.end_with?(path) }
    raise "the fixture site has no #{path}" if found.nil?

    found
  end

  # The one knob, moved. Jekyll reads _data before any generator runs, so a spec sets it after the read.
  def build_with_current(version)
    site = build_site
    site.data['versions']['current'] = version
    site.reset
    site.read
    site.data['versions']['current'] = version
    site.generate
    site
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
