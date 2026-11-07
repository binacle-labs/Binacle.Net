# frozen_string_literal: true

# Stands in for the reader this gem is stamping for, which runs at :low and reads title_suffix from a
# generator. It records what it saw so a spec can prove the stamp landed first.
module Fixtures
  class SuffixReader < Jekyll::Generator
    priority :low

    def generate(site)
      site.documents.each do |doc|
        doc.data['suffix_seen_at_low'] = doc.data['title_suffix']
      end
    end
  end
end
