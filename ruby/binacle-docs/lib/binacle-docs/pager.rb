# frozen_string_literal: true

module Binacle
  module Docs
    # The page before and after each one, walking a version's menu top to bottom: its index first, then each
    # entry followed by its children.
    class Pager
      HOME_TITLE = 'Home'

      def initialize(docs, index)
        @index = index
        menu = docs.reject { |doc| doc.data.dig('nav', 'exclude') }
        @children = menu.group_by { |doc| doc.data.dig('nav', 'parent') }
      end

      def stamp
        entries = [home, *walk(nil)].compact
        entries.each_with_index do |entry, position|
          entry[:doc].data['pager'] = {
            'previous' => link(entries, position - 1, position.positive?),
            'next' => link(entries, position + 1, position < entries.size - 1)
          }.compact
        end
      end

      private

      def home
        { doc: @index, title: HOME_TITLE } if @index
      end

      def walk(parent)
        # A page with no title can have no children: read as nil, it would walk the top of the menu again.
        return [] if parent && !parent.data['title']

        ordered(@children.fetch(parent && parent.data['title'], [])).flat_map do |doc|
          [{ doc: doc, title: menu_title(doc), parent: parent }, *walk(doc)]
        end
      end

      # By nav.order, as the menu sorts; a page with none goes last, and a tie keeps the order it came in.
      def ordered(docs)
        docs.each_with_index.sort_by { |doc, position| [doc.data.dig('nav', 'order') || Float::INFINITY, position] }.map(&:first)
      end

      def menu_title(doc)
        doc.data['menu_title'] || doc.data['title']
      end

      def link(entries, position, exists)
        return unless exists

        entry = entries[position]
        { 'title' => named(entries, entry), 'url' => entry[:doc].url }
      end

      # Two pages can share a menu title, so the parent tells them apart.
      def named(entries, entry)
        twins = entries.count { |other| other[:title] == entry[:title] }
        twins > 1 && entry[:parent] ? "#{menu_title(entry[:parent])} / #{entry[:title]}" : entry[:title]
      end
    end
  end
end
