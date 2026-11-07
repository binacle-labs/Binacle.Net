# frozen_string_literal: true

require 'spec_helper'

RSpec.describe Binacle::Docs::Pager do
  page = Struct.new(:data, :url)

  define_method(:entry) do |title, url, nav = {}, extra = {}|
    page.new({ 'title' => title, 'nav' => nav }.merge(extra), url)
  end

  let(:index) { entry('Welcome', '/', { 'exclude' => true }) }
  let(:docker) { entry('Docker', '/docker/', { 'order' => 2 }) }
  let(:kubernetes) { entry('Kubernetes', '/k8s/', { 'order' => 3 }) }
  let(:docker_minimal) { entry('Minimal', '/docker/minimal/', { 'parent' => 'Docker', 'order' => 1 }) }
  let(:k8s_minimal) { entry('Minimal', '/k8s/minimal/', { 'parent' => 'Kubernetes', 'order' => 1 }) }
  let(:start) { entry('Quick Start Guide', '/start/', { 'order' => 1 }, { 'menu_title' => 'Start' }) }
  let(:hidden) { entry('Hidden', '/hidden/', { 'exclude' => true }) }
  let(:docs) { [kubernetes, k8s_minimal, docker, hidden, docker_minimal, start, index] }

  before { described_class.new(docs, index).stamp }

  it 'starts at the index, with no previous' do
    expect(index.data['pager']).to eq('next' => { 'title' => 'Start', 'url' => '/start/' })
  end

  it 'walks the menu in order, children after their parent' do
    expect(docker.data['pager']['next']['url']).to eq('/docker/minimal/')
    expect(docker_minimal.data['pager']['next']['url']).to eq('/k8s/')
  end

  it 'calls the index Home' do
    expect(start.data['pager']['previous']).to eq('title' => 'Home', 'url' => '/')
  end

  it 'uses the menu title' do
    expect(docker.data['pager']['previous']['title']).to eq('Start')
  end

  it 'names the parent where two pages share a title' do
    expect(kubernetes.data['pager']['previous']['title']).to eq('Docker / Minimal')
    expect(kubernetes.data['pager']['next']['title']).to eq('Kubernetes / Minimal')
  end

  it 'ends at the last page, with no next' do
    expect(k8s_minimal.data['pager'].keys).to eq(['previous'])
  end

  it 'gives a page out of the menu no pager' do
    expect(hidden.data['pager']).to be_nil
  end

  it 'walks past a page with no title' do
    nameless = page.new({ 'nav' => { 'order' => 1 } }, '/nameless/')
    other = entry('Other', '/other/', { 'order' => 2 })
    described_class.new([nameless, other], nil).stamp

    expect(nameless.data['pager']['next']['url']).to eq('/other/')
  end

  it 'works with no index' do
    lone = entry('Lone', '/lone/')
    described_class.new([lone], nil).stamp

    expect(lone.data['pager']).to eq({})
  end
end
