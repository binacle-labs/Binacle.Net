# frozen_string_literal: true

Gem::Specification.new do |spec|
  spec.name = 'binacle-docs-versions'
  spec.version = '1.0.0'
  spec.authors = ['Chris Mavrommatis']
  spec.licenses = ['MIT']
  spec.summary = "The docs site's version scheme - the per-version stamps and the versioned link tag"
  spec.files = Dir['lib/**/*.rb'] + ['LICENSE']
  spec.require_paths = ['lib']
  spec.required_ruby_version = Gem::Requirement.new('>= 3.4')
  spec.add_dependency 'jekyll', '>= 4.2'
  spec.add_development_dependency 'rspec', '~> 3.0'
end
