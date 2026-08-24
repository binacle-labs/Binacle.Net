# frozen_string_literal: true

require 'tmpdir'
require 'fileutils'
require 'json'
require 'jekyll'
require 'jekyll-structured-data'

Jekyll.logger.log_level = :error

FIXTURE_SITE = File.expand_path('fixtures/site', __dir__).freeze
TMP_ROOT = Dir.mktmpdir('jekyll-structured-data').freeze

at_exit { FileUtils.rm_rf(TMP_ROOT) }

ORGANIZATION = {
  '@id' => 'https://www.example.com/#organization',
  'name' => 'Example Ltd',
  'url' => 'https://www.example.com',
  'logo' => '/media/logo.png',
  'same_as' => ['https://github.com/example']
}.freeze

TYPE_DEFAULTS = {
  'WebApplication' => { 'applicationCategory' => 'DeveloperApplication', 'operatingSystem' => 'Any' }
}.freeze

module SiteBuilder
  # The contract keys sit in front matter, which is where a page carries them whichever plugin wrote them.
  # This gem requires none of those plugins, so the fixture loads none of them.
  def build_site(overrides = {})
    destination = Dir.mktmpdir('site', TMP_ROOT)
    config = Jekyll.configuration(
      { 'source' => FIXTURE_SITE, 'destination' => destination, 'disable_disk_cache' => true }.merge(overrides)
    )
    site = Jekyll::Site.new(config)
    site.process
    site
  end

  # Jekyll deep-merges an override onto the config file, so a block written there could never be taken away.
  def configured(extra = {})
    { 'structured_data' => { 'organization' => ORGANIZATION, 'defaults' => TYPE_DEFAULTS }.merge(extra) }
  end

  def output(site, name)
    page = site.pages.find { |candidate| candidate.name == name }
    raise "the fixture site has no #{name}" if page.nil?

    page.output.to_s
  end

  def block(site, name)
    output(site, name)[%r{<script type="application/ld\+json">\n(.*)\n</script>}m, 1]
  end

  def graph(site, name)
    JSON.parse(block(site, name)).fetch('@graph')
  end

  def node(site, name, type)
    graph(site, name).find { |entry| entry['@type'] == type }
  end
end

RSpec.configure do |config|
  config.include SiteBuilder
end
