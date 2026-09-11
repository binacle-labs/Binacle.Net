# frozen_string_literal: true

module Binacle
  module DocsVersions
    # Plain keys, written for whatever renders them. This gem does not know what any of them is for.
    class VersionGenerator < Jekyll::Generator
      COLLECTION = '_versions'
      REDIRECT_LAYOUT = 'redirect'
      FOLDER = %r{\A#{Regexp.escape(COLLECTION)}/([^/]+)/}
      # What every entry in the versions list carries, beside its id.
      KEYS = %w[url_segment label version_tag].freeze

      safe true
      # title_suffix is read by a generator at :low. Stamp after it and the suffix is silently missing.
      priority :high

      def generate(site)
        replace_static_files(site)
        versioned = site.documents.select { |doc| folder_of(doc) }
        return if versioned.empty?

        # The folder under _versions/ is the version. Nothing in the folder, and no config block, has to say so.
        versioned.each { |doc| doc.data['version'] ||= folder_of(doc) }

        current = current_version(site, versioned)
        entries = list_entries(site, versioned)
        stamp_urls(site, versioned)
        pages = Pages.new(versioned)
        stamp_list_urls(site, pages)
        # The label and tag come off the page's own version. The suffix keeps an old page's title apart from
        # the root page of the same name; the root page itself carries no version in its url, so none in its
        # title either.
        versioned.each do |doc|
          version = version_of(doc)
          entry = entries.fetch(version)
          doc.data['version_label'] ||= entry['label']
          doc.data['version_tag'] ||= entry['version_tag']
          next if version == current

          doc.data['title_suffix'] ||= "(#{entry['label']})"
          doc.data['robots'] ||= 'noindex, follow'
        end

        stamp_version_urls(pages, versioned)
        stamp_redirects(site, current)
        check_collisions(site)
        print_removed(pages, current, previous_version(site, current))
      end

      # Every page in the previous version with no page at the same path in the current one. Printed, not
      # written anywhere: it is the redirect list whoever opens a major has to write.
      def removed_pages(pages, current, previous)
        return [] if previous.nil?

        pages.missing_from(previous, current)
      end

      private

      # The gem is the only place a url is decided, so a permalink a page wrote is overwritten, not kept.
      # Set before anything reads url: Jekyll memoises it on the first read and a later permalink is ignored.
      def stamp_urls(site, versioned)
        urls = Urls.new(site)
        versioned.each do |item|
          if item.is_a?(VersionedFile)
            item.url = urls.for(item)
          else
            item.data['permalink'] = urls.for(item)
          end
        end
      end

      # Every static file under _versions/ becomes one whose url can be set, in both lists Jekyll keeps it in.
      def replace_static_files(site)
        replaced = {}
        site.collections.each_value do |collection|
          collection.files.map! do |file|
            next file unless folder_of(file)

            replaced[file] = VersionedFile.from(file)
          end
        end
        site.static_files.map! { |file| replaced.fetch(file, file) }
      end

      # For the selector: where this same page is in every version, or the index of a version that lacks it.
      def stamp_version_urls(pages, versioned)
        versioned.each do |doc|
          doc.data['version_urls'] ||= pages.versions.each_with_object({}) do |version, urls|
            target = pages.counterpart(doc, version)
            urls[version] = target.url unless target.nil?
          end
        end
      end

      # For the version list and the selector: each entry in versions.yml learns where its index renders, so no
      # template builds a url from an id.
      def stamp_list_urls(site, pages)
        list = site.data.dig('versions', 'list')
        return unless list.is_a?(Array)

        list.each do |entry|
          next unless entry.is_a?(Hash) && entry['id']

          index = pages.index_of(entry['id'].to_s)
          entry['url'] ||= index.url unless index.nil?
        end
      end

      # Every version folder has a list entry, and every entry carries all of KEYS - nothing is derived. A
      # folder without one would print a pull command with nothing after the colon, or a nameless selector row.
      def list_entries(site, versioned)
        list = site.data.dig('versions', 'list')
        list = [] unless list.is_a?(Array)
        entries = list.select { |entry| entry.is_a?(Hash) && entry['id'] }.to_h { |entry| [entry['id'].to_s, entry] }

        versioned.map { |doc| version_of(doc) }.uniq.each do |version|
          entry = entries[version]
          raise Error, "versions list has no entry for #{version}; it has #{entries.keys.sort.join(', ')}" if entry.nil?

          missing = KEYS.reject { |key| entry[key] }
          raise Error, "versions list entry #{version} has no #{missing.join(', ')}" unless missing.empty?
        end
        entries.transform_values { |entry| entry.transform_values(&:to_s) }
      end

      # Jekyll only warns when two files render to one destination, and a warning is how the wrong page ships.
      def check_collisions(site)
        outputs = (site.pages + site.documents).select(&:write?)
        outputs.group_by(&:url).each_value do |files|
          next if files.size < 2

          paths = files.map { |file| file.relative_path.to_s }.sort
          raise Error, "#{paths.join(' and ')} both render at #{files.first.url}"
        end
      end

      # The folder listed right after current in versions.yml. The list is newest first, so that is the line
      # current grew out of.
      def previous_version(site, current)
        list = site.data.dig('versions', 'list')
        return nil unless list.is_a?(Array)

        ids = list.filter_map { |entry| entry['id'].to_s if entry.is_a?(Hash) && entry['id'] }
        position = ids.index(current)
        position.nil? ? nil : ids[position + 1]
      end

      def print_removed(pages, current, previous)
        removed = removed_pages(pages, current, previous)
        return if removed.empty?

        Jekyll.logger.info 'Docs versions:', "#{removed.size} pages in #{previous} have no counterpart in #{current}"
        removed.each { |path| Jekyll.logger.info '', "  #{previous}/#{path}" }
      end

      def folder_of(doc)
        doc.relative_path.to_s[FOLDER, 1]
      end

      # The redirect page's canonical is the page it points at, which is what says the two are one
      # destination. Both keys carry the same url so the head and the redirect cannot disagree.
      def stamp_redirects(site, current)
        pages = (site.pages + site.documents).select { |doc| doc.data['layout'].to_s == REDIRECT_LAYOUT }
        return if pages.empty?

        url = current_root(site, current)
        pages.each do |doc|
          doc.data['redirect_to'] ||= url
          doc.data['canonical'] ||= doc.data['redirect_to']
          doc.data['robots'] ||= 'noindex'
        end
      end

      def current_root(site, current)
        index = %r{\A#{Regexp.escape(COLLECTION)}/#{Regexp.escape(current)}/index\.\w+\z}
        found = site.documents.find { |doc| doc.relative_path.to_s.match?(index) }
        raise Error, "#{COLLECTION}/#{current}/index has no document, so a redirect has nowhere to point" if found.nil?

        found.url
      end

      def version_of(doc)
        doc.data['version'].to_s
      end

      # Raising rather than skipping: a current that matches nothing would noindex the whole site, silently.
      def current_version(site, versioned)
        known = versioned.map { |doc| version_of(doc) }.uniq.sort
        current = site.data.dig('versions', 'current').to_s
        raise Error, "versions current is not set, and the site has #{known.join(', ')}" if current.empty?

        return current if known.include?(current)

        raise Error, "versions current is #{current.inspect}, which is not one of #{known.join(', ')}"
      end
    end
  end
end
