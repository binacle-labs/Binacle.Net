# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::DocsVersions::VLinkTag do
  it 'resolves a static file inside the page version' do
    expect(doc(build_site, 'v1.0.x/links.md').output)
      .to include('static: /versions/v1.0.x/swagger/v3.json')
  end

  it 'renders the liquid in its own argument first' do
    expect(doc(build_site, 'v1.0.x/links.md').output)
      .to include('liquid: /versions/v1.0.x/swagger/v3.json')
  end

  it 'resolves a document as well as a static file' do
    expect(doc(build_site, 'v1.0.x/links.md').output).to include('document: /versions/v1.0.x/guide.html')
  end

  it 'resolves the same path to a different file on a different version' do
    expect(doc(build_site, 'v2.0.x/links.md').output).to include('/versions/v2.0.x/guide.html')
  end

  it 'fails the build when the file is not in that version' do
    expect { build_site({}, BROKEN_SITE) }.to raise_error(ArgumentError, /Could not find document/)
  end
end
