# frozen_string_literal: true

require 'spec_helper'

RSpec.describe 'attributes' do
  it 'writes them in the order the data declares them' do
    expect(built('links.html')).to start_with('<link rel="stylesheet" type="text/css"')
    expect(built('links-reordered.html')).to eq(
      '<link type="text/css" rel="stylesheet" href="/app/css/main.css">'
    )
  end

  it 'skips a key with no value' do
    expect(built('blank-value.html')).to eq('<link rel="stylesheet" href="/app/css/blank.css">')
  end

  it 'writes true as a bare attribute and false as nothing' do
    expect(built('flags.html')).to eq('<script src="/app/js/flags.js" defer></script>')
  end

  it 'leaves a path that already has a scheme alone' do
    expect(elements('whole-path.html')).to eq(
      [
        '<link rel="stylesheet" href="https://cdn.example.org/main.css">',
        '<link rel="preconnect" href="//fonts.example.org">'
      ]
    )
  end

  it 'writes no element for an item with no path' do
    expect(elements('no-path.html')).to eq(['<link rel="stylesheet" href="/app/css/kept.css">'])
  end

  it 'escapes a value so it cannot break out of the attribute' do
    expect(built('quoted.html')).to eq(
      '<link rel="stylesheet" title="He said &quot;stop&quot; &gt; here" href="/app/css/quoted.css">'
    )
  end

  it 'renders an empty string for an empty list' do
    expect(built('empty.html')).to eq('AB')
  end

  it 'adds no whitespace of its own' do
    expect(built('tight.html')).to eq(
      'A<link rel="stylesheet" type="text/css" href="/app/css/main.css">' \
      "\n<link rel=\"icon\" type=\"image/png\" href=\"/app/icon.png\">B"
    )
  end
end
