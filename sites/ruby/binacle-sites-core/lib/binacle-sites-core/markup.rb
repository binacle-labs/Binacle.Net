# frozen_string_literal: true

require 'cgi/escape'
require 'nokogiri'

module Binacle
  module SitesCore
    # Jobs on a Nokogiri fragment that give the same markup on every site. Nothing here is a Liquid filter: a
    # site gem calls it from its own.
    module Markup
      DOOR_DASH = /\A\s+-\s+/
      RIGHT_ALIGNED = /\Atext-align:\s*right;?\z/
      NEAREST_HEADING = 'preceding-sibling::*[self::h2 or self::h3 or self::h4 or self::h5 or self::h6][1]'

      module_function

      def add_class(element, name)
        element['class'] = [element['class'], name].compact.join(' ')
      end

      # In a list marked labelled-list, the item that opens in bold is the cost, and the bold is its label.
      def name_labelled_lists(fragment)
        fragment.css('ul.labelled-list > li').each do |item|
          add_class(item, 'labelled-list__item')
          label = item.children.first
          next unless label&.name == 'strong'

          add_class(item, 'labelled-list__cost')
          label.name = 'span'
          label['class'] = 'label'
          text = label.next_sibling
          text.content = text.content.lstrip if text&.text?
        end
      end

      # A cell that is only emphasis marked table__off is greyed out whole, and not italic.
      def grey_out(cell)
        mark = cell.at_css('> em.table__off')
        return unless mark && cell.text.strip == mark.text

        add_class(cell, 'table__off')
        mark.replace(mark.children)
      end

      # A list marked doors becomes a nav of links to pages. The nearest heading above it names the nav, even
      # with words between them, so a screen reader can tell one nav from another. A picture may come before a
      # link. It gives back the navs, for a site that sets their classes itself.
      def name_doors(fragment)
        fragment.css('ul.doors').map do |list|
          heading = list.xpath(NEAREST_HEADING).first
          links = list.css('> li').map { |item| door(item) }
          list.name = 'nav'
          list['aria-label'] = heading.text.strip if heading
          list.inner_html = links.join("\n")
          list
        end
      end

      def door(item)
        picture = item.at_css('> figure:first-child')&.unlink
        picture['class'] = 'doors__picture' if picture
        link = item.at_css('> a:first-child')
        text = item.inner_html.strip.delete_prefix(link.to_s)
        unless link && text.match?(DOOR_DASH)
          raise ArgumentError, "a doors item is written [Title](link) - text, not: #{item.text.strip}"
        end

        title = link.inner_html
        link['class'] = 'doors__link'
        link.inner_html =
          %(#{picture&.to_html}<h3 class="doors__title">#{title}</h3>) +
          %(<p class="doors__text">#{text.sub(DOOR_DASH, '')}</p>)
        link.to_html
      end

      # Wraps each code block that `selector` finds. The block, if given, runs on each one and may return the
      # head to use in place of the shared one. `plain` strips the highlighter's spans, leaving text.
      def name_code_blocks(fragment, selector:, plain: false)
        fragment.css(selector).each do |pre|
          code = pre.at_css('code')
          next unless code

          outer = pre.ancestors('div.highlighter-rouge').first || pre
          pre['class'] = 'code-block__pre'
          head = yield(pre, code, outer) if block_given?
          code.remove_attribute('class')
          drop_last_newline(code, plain)
          outer.replace(<<~HTML.chomp)
            <div class="code-block" data-code-block>
            #{head || code_head_for(outer['data-name'])}
            #{pre.to_html}
            </div>
          HTML
        end
      end

      # With no name there is no head, and the button floats in the corner.
      def code_head_for(name)
        return copy_button('copy-button code-block__copy') unless name

        code_head(code_label(name))
      end

      def code_label(name)
        %(<span class="label" data-code-lang>#{CGI.escapeHTML(name)}</span>)
      end

      def code_head(inner_html)
        %(<div class="code-block__head">#{inner_html}#{copy_button('copy-button')}</div>)
      end

      def copy_button(classes)
        %(<button type="button" class="#{classes}" data-copy hidden>Copy</button>)
      end

      # The newline that ends the code would show as an empty last line.
      def drop_last_newline(code, plain)
        if plain
          code.content = code.text.chomp
        else
          last = code.xpath('.//text()').last
          last.content = last.content.chomp if last
        end
      end

      # Makes each table that `selector` finds the design's. Below 640 a row stacks, and each cell shows its
      # column's name. `cell` runs first on every body cell. It gives back the tables.
      def name_tables(fragment, selector:, cell: nil)
        fragment.css(selector).map do |table|
          add_class(table, 'table')
          table['data-table'] = ''
          columns = table.css('thead th').each { |head| head['scope'] = 'col' }.map { |head| head.text.strip }
          table.css('td').each do |data|
            cell&.call(data)
            column = columns[data.parent.element_children.index(data)]
            data['data-label'] = column if column
          end
          table.css('th, td').each { |data| align_number(data) }
          table.css('td').each { |data| grey_out(data) }
          table
        end
      end

      # A class, so a right-aligned column goes left once the table stacks on a phone.
      def align_number(cell)
        return unless cell['style']&.match?(RIGHT_ALIGNED)

        cell.remove_attribute('style')
        add_class(cell, 'table__number')
      end
    end
  end
end
