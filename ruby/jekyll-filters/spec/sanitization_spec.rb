# frozen_string_literal: true

require 'spec_helper'

PLAIN_TEXT = 'hello world'

RSpec.describe Jekyll::SiteFilters::Sanitization do
  let(:filter) { Class.new { include Jekyll::SiteFilters::Sanitization }.new }

  describe '#clean_content' do
    it 'strips HTML tags' do
      expect(filter.clean_content('<p>hello <strong>world</strong></p>')).to eq(PLAIN_TEXT)
    end

    it 'collapses newlines into spaces' do
      expect(filter.clean_content("hello\nworld")).to eq(PLAIN_TEXT)
    end

    it 'collapses multiple spaces into one' do
      expect(filter.clean_content('hello  world')).to eq(PLAIN_TEXT)
    end

    it 'strips leading and trailing whitespace' do
      expect(filter.clean_content('  hello  ')).to eq('hello')
    end

    it 'truncates to 160 characters by default' do
      long = 'a ' * 100
      expect(filter.clean_content(long).length).to be <= 160
    end

    it 'does not leave a trailing space where the cut lands on one' do
      expect(filter.clean_content(PLAIN_TEXT, 6)).to eq('hello')
    end

    it 'truncates to a custom length' do
      expect(filter.clean_content(PLAIN_TEXT, 5)).to eq('hello')
    end

    it 'handles nil input gracefully' do
      expect { filter.clean_content(nil) }.not_to raise_error
    end

    it 'leaves the carriage return of a CRLF behind' do
      expect(filter.clean_content("hello\r\nworld")).to eq("hello\r world")
    end

    it 'never collapses a tab' do
      expect(filter.clean_content("hello\t\tworld")).to eq("hello\t\tworld")
    end

    it 'eats prose between a < and the next >, because the strip is a regexp' do
      expect(filter.clean_content('a < b > c')).to eq('a c')
    end

    it 'leaves an entity encoded, so it counts as five characters and reads as one' do
      expect(filter.clean_content('AT&amp;T')).to eq('AT&amp;T')
    end

    it 'does not let a tag re-form when one is nested inside another' do
      expect(filter.clean_content('<<script>script>alert(1)</script>')).not_to include('<')
    end

    it 'drops an unclosed angle bracket, which the tag regexp on its own would keep' do
      expect(filter.clean_content('5 < 10')).to eq('5 10')
    end
  end
end
