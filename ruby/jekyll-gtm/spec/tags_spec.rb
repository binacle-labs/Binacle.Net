# frozen_string_literal: true

require 'spec_helper'

def render_tag(tag_class, tag_name, markup, assigns = {})
  Liquid::Template.register_tag(tag_name, tag_class)
  Liquid::Template.parse("{% #{tag_name} #{markup} %}").render(assigns)
end

RSpec.describe Jekyll::GTM::HeadTag do
  it 'outputs the GTM head script with the given ID' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head', 'GTM-XXXX')
    expect(output).to include('GTM-XXXX')
    expect(output).to include('googletagmanager.com/gtm.js')
  end

  it 'returns an empty string when the ID is blank' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_empty', '')
    expect(output.strip).to eq('')
  end
end

RSpec.describe Jekyll::GTM::BodyTag do
  it 'outputs the GTM noscript tag with the given ID' do
    output = render_tag(Jekyll::GTM::BodyTag, 'gtm_body', 'GTM-XXXX')
    expect(output).to include('GTM-XXXX')
    expect(output).to include('googletagmanager.com/ns.html')
  end

  it 'returns an empty string when the ID is blank' do
    output = render_tag(Jekyll::GTM::BodyTag, 'gtm_body_empty', '')
    expect(output.strip).to eq('')
  end
end

RSpec.describe Jekyll::GTM::Tag do
  it 'reads the ID from a variable' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_var', 'site.gtm', 'site' => { 'gtm' => 'GTM-FROMVAR' })
    expect(output).to include('GTM-FROMVAR')
  end

  it 'renders nothing when the variable resolves to nothing' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_missing', 'site.gtn')
    expect(output.strip).to eq('')
  end

  it 'renders nothing when the variable is set to an empty string' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_off', 'site.gtm', 'site' => { 'gtm' => '' })
    expect(output.strip).to eq('')
  end

  it 'accepts an ID that is not a string' do
    output = render_tag(Jekyll::GTM::BodyTag, 'gtm_body_number', 'site.gtm', 'site' => { 'gtm' => 12_345 })
    expect(output).to include('id=12345')
  end

  it 'renders nothing for a literal in the wrong case, which is not the shape of an id' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_lower', 'GTM-lowercase')
    expect(output.strip).to eq('')
  end

  it 'renders nothing for a literal with no GTM- prefix' do
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_bare', 'gtm')
    expect(output.strip).to eq('')
  end

  it 'writes a variable into the snippet raw, so the shape check never sees it' do
    assigns = { 'site' => { 'gtm' => 'gtm-not an id"' } }
    output = render_tag(Jekyll::GTM::HeadTag, 'gtm_head_raw', 'site.gtm', assigns)

    expect(output).to include(%('gtm-not an id"'))
  end
end
