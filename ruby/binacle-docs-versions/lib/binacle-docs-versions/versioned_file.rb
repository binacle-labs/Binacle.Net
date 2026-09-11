# frozen_string_literal: true

module Binacle
  module DocsVersions
    # A static file inside a version folder whose url the generator sets. Jekyll's StaticFile takes its url
    # from the collection template and never from data, so a `permalink` does nothing to it; this one takes
    # the url it is given.
    class VersionedFile < Jekyll::StaticFile
      attr_writer :url

      # Jekyll exposes none of the constructor arguments as readers, so they are read off the instance.
      def self.from(file)
        new(
          file.instance_variable_get(:@site),
          file.instance_variable_get(:@base),
          file.instance_variable_get(:@dir),
          file.name,
          file.instance_variable_get(:@collection)
        )
      end
    end
  end
end
