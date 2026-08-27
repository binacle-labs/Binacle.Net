# frozen_string_literal: true

# Coverage for the gem specs, loaded through RUBYOPT by the rspec recipe in tests.just - never by a gem.
#
# RUBYOPT, not a require in each spec_helper: a gem installed from a package index has no parent directory to
# require, so a shared helper above the gem folders would break the rule that a gem drops into an unrelated
# site. This file is the build's, not the gem's.
#
# It loads before the gem does, which is the whole requirement - SimpleCov measures nothing that was already
# required.

format = ENV.fetch('COVERAGE_FORMAT', '')

unless format.empty?
  # CI installs the bundle into ruby/vendor/bundle, so nothing below is on the load path until bundler puts
  # it there. On a laptop the gems are usually installed globally and this looks unnecessary.
  require 'bundler/setup'
  require 'simplecov'

  case format
  when 'cobertura'
    require 'simplecov-cobertura'
    SimpleCov.formatter = SimpleCov::Formatter::CoberturaFormatter
  when 'sonar'
    require 'simplecov_json_formatter'
    SimpleCov.formatter = SimpleCov::Formatter::JSONFormatter
  else
    abort("Unknown COVERAGE_FORMAT '#{format}'. Use cobertura or sonar.")
  end

  # Absolute, and set by the recipe: rspec runs from inside the gem folder.
  dir = ENV.fetch('COVERAGE_DIR', '')
  SimpleCov.coverage_dir(dir) unless dir.empty?

  SimpleCov.start do
    skip '/spec/'
  end
end
