# frozen_string_literal: true

Gem::Specification.new do |spec|
  spec.name = 'jekyll-breadcrumb-trail'
  spec.version = '1.0.0'
  spec.authors = ['Chris Mavrommatis']
  spec.licenses = ['MIT']
  spec.summary = "Resolves a page's breadcrumb trail once, and renders it as standard accessible markup"
  spec.files = Dir['lib/**/*.rb'] + ['LICENSE']
  spec.require_paths = ['lib']
  spec.required_ruby_version = Gem::Requirement.new('>= 3.4')
  spec.add_dependency 'jekyll', '>= 4.2'
  spec.add_development_dependency 'rspec', '~> 3.0'
end
