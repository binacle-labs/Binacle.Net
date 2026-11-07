# frozen_string_literal: true

# A Jekyll theme gem: it ships the shared includes and nothing else.
Gem::Specification.new do |spec|
  spec.name = 'binacle-design-system'
  spec.version = '1.0.0'
  spec.authors = ['Chris Mavrommatis']
  spec.licenses = ['AGPL-3.0-only']
  spec.summary = 'The shared markup of the Binacle sites'
  spec.files = Dir['_includes/**/*']
  spec.required_ruby_version = Gem::Requirement.new('>= 3.4')
end
