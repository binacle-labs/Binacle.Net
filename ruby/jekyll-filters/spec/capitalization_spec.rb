# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::SiteFilters::Capitalization do
  let(:filter) { Class.new { include Jekyll::SiteFilters::Capitalization }.new }

  describe '#capitalize_all' do
    it 'capitalizes the first letter of each word' do
      expect(filter.capitalize_all('hello world')).to eq('Hello World')
    end

    it 'handles a single word' do
      expect(filter.capitalize_all('hello')).to eq('Hello')
    end

    it 'leaves already-capitalized input unchanged' do
      expect(filter.capitalize_all('Hello World')).to eq('Hello World')
    end

    it 'handles all-uppercase input' do
      expect(filter.capitalize_all('HELLO WORLD')).to eq('Hello World')
    end

    it 'handles an empty string' do
      expect(filter.capitalize_all('')).to eq('')
    end

    it 'handles nil input gracefully' do
      expect(filter.capitalize_all(nil)).to eq('')
    end

    it 'capitalizes only the first letter, so a hyphenated slug keeps its lower half' do
      expect(filter.capitalize_all('getting-started')).to eq('Getting-started')
    end

    it 'lowercases an acronym in the middle of a phrase' do
      expect(filter.capitalize_all('the API reference')).to eq('The Api Reference')
    end

    it 'collapses a run of spaces to one' do
      expect(filter.capitalize_all('hello   world')).to eq('Hello World')
    end

    it 'drops leading and trailing spaces' do
      expect(filter.capitalize_all('  hello world  ')).to eq('Hello World')
    end

    it 'splits on a tab as well as a space' do
      expect(filter.capitalize_all("hello\tworld")).to eq('Hello World')
    end
  end
end
