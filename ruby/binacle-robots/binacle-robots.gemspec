# frozen_string_literal: true

Gem::Specification.new do |spec|
  spec.name = 'binacle-robots'
  spec.version = '1.0.0'
  spec.authors = ['Chris Mavrommatis']
  spec.licenses = ['MIT']
  spec.summary = "One organisation's robots.txt body, so its sites cannot keep different copies of it"
  spec.files = Dir['lib/**/*']
  spec.require_paths = ['lib']
  spec.required_ruby_version = Gem::Requirement.new('>= 3.1')
  spec.add_dependency 'jekyll', '>= 4.2'
  spec.add_development_dependency 'rspec', '~> 3.0'
end
