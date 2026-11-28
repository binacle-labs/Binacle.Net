# frozen_string_literal: true

Gem::Specification.new do |spec|
  spec.name = 'binacle-sites-core'
  spec.version = '1.0.0'
  spec.authors = ['Chris Mavrommatis']
  spec.licenses = ['AGPL-3.0-only']
  spec.summary = 'The markup jobs the Binacle site gems share - plain Ruby the site gems call, with no plugin of its own'
  spec.files = Dir['lib/**/*.rb'] + ['LICENSE']
  spec.require_paths = ['lib']
  spec.required_ruby_version = Gem::Requirement.new('>= 3.4')
  spec.add_dependency 'nokogiri', '~> 1.12'
  spec.add_development_dependency 'rspec', '~> 3.0'
end
