# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Jekyll::SiteFilters::Dates do
  let(:filter) { Class.new { include Jekyll::SiteFilters::Dates }.new }

  describe '#expand_year' do
    it 'replaces the placeholder with the current year' do
      expect(filter.expand_year('(c) 2023-{now} Someone')).to eq("(c) 2023-#{Time.now.year} Someone")
    end

    it 'replaces every occurrence' do
      expect(filter.expand_year('{now} and {now}')).to eq("#{Time.now.year} and #{Time.now.year}")
    end

    it 'leaves a string without the placeholder alone' do
      expect(filter.expand_year('(c) 2023 Someone')).to eq('(c) 2023 Someone')
    end

    it 'accepts a placeholder of its own' do
      expect(filter.expand_year('(c) %%YEAR%%', '%%YEAR%%')).to eq("(c) #{Time.now.year}")
    end

    it 'is why the placeholder is a default: naming one with a brace fails the parse, not the filter' do
      expect { Liquid::Template.parse('{{ s | expand_year: "{now}" }}') }.to raise_error(Liquid::SyntaxError)
      expect { Liquid::Template.parse('{{ s | expand_year }}') }.not_to raise_error
    end

    it 'handles nil input gracefully' do
      expect(filter.expand_year(nil)).to eq('')
    end

    it 'uses site.time when Liquid gives it a context' do
      site = double('site', time: Time.new(1999, 6, 1))
      context = double('context', registers: { site: site })
      filter.instance_variable_set(:@context, context)

      expect(filter.expand_year('(c) {now}')).to eq('(c) 1999')
    end
  end
end
