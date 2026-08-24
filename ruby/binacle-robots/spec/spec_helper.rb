# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'jekyll'
require 'binacle-robots'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze
TMP_ROOT = Dir.mktmpdir('binacle-robots').freeze

at_exit { FileUtils.rm_rf(TMP_ROOT) }

module SiteBuilder
  # The fixture site builds for real, because the thing under test is a file on disk after a build.
  def build_site
    destination = Dir.mktmpdir('site', TMP_ROOT)
    config = Jekyll.configuration(
      'source' => FIXTURE_SITE, 'destination' => destination, 'disable_disk_cache' => true
    )
    site = Jekyll::Site.new(config)
    site.process
    File.read(File.join(destination, 'robots.txt'))
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
