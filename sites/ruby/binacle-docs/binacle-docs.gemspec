# frozen_string_literal: true

Gem::Specification.new do |spec|
  spec.name = 'binacle-docs'
  spec.version = '1.0.0'
  spec.authors = ['Chris Mavrommatis']
  spec.licenses = ['MIT']
  spec.summary = "The docs site's own plugins - its version scheme and the filters that turn a page into its markup"
  spec.files = Dir['lib/**/*.rb'] + ['LICENSE']
  spec.require_paths = ['lib']
  spec.required_ruby_version = Gem::Requirement.new('>= 3.4')
  spec.add_dependency 'jekyll', '>= 4.2'
  spec.add_dependency 'nokogiri', '~> 1.12'
  spec.add_development_dependency 'rspec', '~> 3.0'
end
