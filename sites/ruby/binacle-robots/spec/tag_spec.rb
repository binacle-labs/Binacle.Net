# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::Robots::RobotsTag do
  let(:built) { build_site }

  it 'writes the body and nothing else, so the file ends where the site says it ends' do
    expect(built).to eq("#{Binacle::Robots::Body.text}\n\nSitemap: https://example.com/sitemap.xml\n")
  end

  it 'emits no trailing newline of its own' do
    expect(Binacle::Robots::Body.text).not_to end_with("\n")
  end

  it 'substitutes nothing - the text is a rights reservation, not a template' do
    expect(Binacle::Robots::Body.text).not_to match(/\{[%{]/)
  end

  describe 'the lines that carry the legal meaning' do
    it 'reserves rights under the directive, in these words' do
      expect(built).to include(
        '# ANY RESTRICTIONS EXPRESSED VIA CONTENT SIGNALS ARE EXPRESS RESERVATIONS OF',
        '# RIGHTS UNDER ARTICLE 4 OF THE EUROPEAN UNION DIRECTIVE 2019/790 ON COPYRIGHT',
        '# AND RELATED RIGHTS IN THE DIGITAL SINGLE MARKET.'
      )
    end

    it 'defines all three signals, because a signal left undefined grants nothing and restricts nothing' do
      expect(built).to include('# search:', '# ai-input:', '# ai-train:')
    end

    it 'allows every agent' do
      expect(built).to include("\nUser-Agent: *\n")
    end

    # Whether it should be live is a separate question. This pins that it is not, so turning it on is a choice.
    it 'leaves the content signals commented out' do
      expect(built).to include('# Content-Signals: ai-train=no, search=yes, ai-input=no')
      expect(built).not_to match(/^Content-Signals:/)
    end
  end
end
