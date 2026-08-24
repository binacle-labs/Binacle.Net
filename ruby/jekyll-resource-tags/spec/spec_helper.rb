# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'jekyll'
require 'jekyll-resource-tags'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze

module SiteBuilder
  # Every spec reads a page the fixture site really built. A hash pretending to be a context tests the spec,
  # not the gem. Nothing varies per example, so the site is built once.
  def self.site
    @site ||= begin
      destination = Dir.mktmpdir('jekyll-resource-tags')
      at_exit { FileUtils.rm_rf(destination) }
      config = Jekyll.configuration(
        'source' => FIXTURE_SITE, 'destination' => destination, 'disable_disk_cache' => true
      )
      Jekyll::Site.new(config).tap(&:process)
    end
  end

  def built(path)
    File.read(File.join(SiteBuilder.site.dest, path)).chomp
  end

  def elements(path)
    built(path).lines.map(&:chomp)
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
