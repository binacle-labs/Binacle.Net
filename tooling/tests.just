# Runs the tests. One recipe per test project, so one failure names one project.
#
#   just test                            the list
#   just test all                        every project
#   just test image                      the ones the Docker image ships
#   just test sites                      the ones a site ships
#   just test cs_binacle-lib_unit        one project
#
# A name is <language>_<project>_<kind>, taken from the assembly, package or gem it runs.
#
# The single projects are [private], so the list shows the three groups instead of forty names. A private
# recipe still runs when you name it.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The test runner already said what broke, so just's own "Recipe failed" line adds nothing. The exit code
# still gets through, so CI still fails.
set no-exit-message := true

# Extra flags for `dotnet test`. CI builds the solution once and passes --no-build; on a laptop you want
# none. An environment variable and not a just variable, because just cannot set a module variable from the
# command line and would ignore it without saying so.
#
#   DOTNET_TEST_ARGS="--configuration Release --no-build" just test all
dotnet_test_args := env('DOTNET_TEST_ARGS', '')

# List the recipes
default:
    @just --list test

# The three lists below are written out by hand, and nothing checks one against another. That is on purpose.
#
# A list stops at the first failure, so the quick ones go first.

# Every test the Docker image ships
image:
    just test cs_binacle-lib_unit
    just test cs_binacle-compact-notation_unit
    just test ts_binacle-compact-notation_unit
    just test cs_binacle-flux-results_unit
    just test cs_binacle-vipaq_unit
    just test ts_binacle-vipaq_unit
    just test ts_binacle-net-ui_unit
    just test ts_cookies_unit
    just test ts_theme-switcher_unit
    just test cs_binacle-net_unit
    just test cs_binacle-net-kernel_unit
    just test cs_binacle-net-diagnostics-module_unit
    just test cs_binacle-net-ui-module_unit
    just test cs_binacle-net-service-module_unit
    just test cs_binacle-net-ui-module_integration
    just test cs_binacle-net_integration
    just test cs_binacle-net-service-module_integration Sqlite

# Every test a site ships - the gems, and the javascript a site bundles
sites:
    just test ts_binacle-compact-notation_unit
    just test ts_binacle-vipaq_unit
    just test ts_binacle-net-ui_unit
    just test ts_cookies_unit
    just test ts_theme-switcher_unit
    just test rb_binacle-docs-versions_unit
    just test rb_binacle-robots_unit
    just test rb_jekyll-breadcrumb-trail_unit
    just test rb_jekyll-filters_unit
    just test rb_jekyll-gtm_unit
    just test rb_jekyll-multi-sitemap_unit
    just test rb_jekyll-page-meta_unit
    just test rb_jekyll-resource-tags_unit
    just test rb_jekyll-structured-data_unit
    just test rb_jekyll-webmanifest_unit

# Every test that needs nothing started first
all:
    just test cs_binacle-lib_unit
    just test cs_binacle-compact-notation_unit
    just test ts_binacle-compact-notation_unit
    just test cs_binacle-flux-results_unit
    just test cs_binacle-vipaq_unit
    just test ts_binacle-vipaq_unit
    just test ts_binacle-net-ui_unit
    just test ts_cookies_unit
    just test ts_theme-switcher_unit
    just test cs_binacle-net_unit
    just test cs_binacle-net-kernel_unit
    just test cs_binacle-net-diagnostics-module_unit
    just test cs_binacle-net-ui-module_unit
    just test cs_binacle-net-service-module_unit
    just test cs_binacle-net-ui-module_integration
    just test cs_binacle-net_integration
    just test cs_binacle-net-service-module_integration Sqlite
    just test rb_binacle-docs-versions_unit
    just test rb_binacle-robots_unit
    just test rb_jekyll-breadcrumb-trail_unit
    just test rb_jekyll-filters_unit
    just test rb_jekyll-gtm_unit
    just test rb_jekyll-multi-sitemap_unit
    just test rb_jekyll-page-meta_unit
    just test rb_jekyll-resource-tags_unit
    just test rb_jekyll-structured-data_unit
    just test rb_jekyll-webmanifest_unit

# The C# tests. The argument is the test project's folder.

[private]
cs_binacle-lib_unit: (_dotnet_test 'lib/test/Binacle.Lib.UnitTests')

[private]
cs_binacle-compact-notation_unit: (_dotnet_test 'shared/test/Binacle.CompactNotation.UnitTests')

[private]
cs_binacle-flux-results_unit: (_dotnet_test 'shared/test/Binacle.FluxResults.UnitTests')

[private]
cs_binacle-vipaq_unit: (_dotnet_test 'vipaq/test/Binacle.ViPaq.UnitTests')

[private]
cs_binacle-net_unit: (_dotnet_test 'api/test/Binacle.Net.UnitTests')

[private]
cs_binacle-net-kernel_unit: (_dotnet_test 'api/test/Binacle.Net.Kernel.UnitTests')

[private]
cs_binacle-net-diagnostics-module_unit: (_dotnet_test 'api/test/Binacle.Net.DiagnosticsModule.UnitTests')

[private]
cs_binacle-net-ui-module_unit: (_dotnet_test 'api/test/Binacle.Net.UIModule.UnitTests')

[private]
cs_binacle-net-service-module_unit: (_dotnet_test 'api/test/Binacle.Net.ServiceModule.UnitTests')

[private]
cs_binacle-net_integration: (_dotnet_test 'api/test/Binacle.Net.IntegrationTests')

[private]
cs_binacle-net-ui-module_integration: (_dotnet_test 'api/test/Binacle.Net.UIModule.IntegrationTests')

# The backend is an argument and not a variable, because just cannot set a module variable from the command
# line and would ignore it without saying so.
#
# No backend means SQLite. Postgres and AzureStorage need `just serve services-up -d` running first.

[private]
cs_binacle-net-service-module_integration backend="":
    #!/usr/bin/env bash
    set -euo pipefail

    # A typo would otherwise fall through to the default and report a green run for a backend nobody tested.
    case "{{ backend }}" in
        "" | Sqlite | Postgres | AzureStorage) ;;
        *) echo "Unknown backend '{{ backend }}'. Use Sqlite, Postgres or AzureStorage." >&2; exit 1 ;;
    esac

    export BINACLE_TEST_INFRA="{{ backend }}"

    just test::_dotnet_test api/test/Binacle.Net.ServiceModule.IntegrationTests

# The TypeScript tests. The argument is the package's jest displayName.

[private]
ts_binacle-compact-notation_unit: (_jest_test 'binacle-compact-notation')

[private]
ts_binacle-vipaq_unit: (_jest_test 'binacle-vipaq')

[private]
ts_binacle-net-ui_unit: (_jest_test 'binacle-net-ui')

[private]
ts_cookies_unit: (_jest_test 'cookies')

[private]
ts_theme-switcher_unit: (_jest_test 'theme-switcher')

# The Ruby tests. The argument is the gem's folder under ruby/.

[private]
rb_binacle-docs-versions_unit: (_rspec_test 'binacle-docs-versions')

[private]
rb_binacle-robots_unit: (_rspec_test 'binacle-robots')

[private]
rb_jekyll-breadcrumb-trail_unit: (_rspec_test 'jekyll-breadcrumb-trail')

[private]
rb_jekyll-filters_unit: (_rspec_test 'jekyll-filters')

[private]
rb_jekyll-gtm_unit: (_rspec_test 'jekyll-gtm')

[private]
rb_jekyll-multi-sitemap_unit: (_rspec_test 'jekyll-multi-sitemap')

[private]
rb_jekyll-page-meta_unit: (_rspec_test 'jekyll-page-meta')

[private]
rb_jekyll-resource-tags_unit: (_rspec_test 'jekyll-resource-tags')

[private]
rb_jekyll-structured-data_unit: (_rspec_test 'jekyll-structured-data')

[private]
rb_jekyll-webmanifest_unit: (_rspec_test 'jekyll-webmanifest')

# Runs one C# test project. Every C# test goes through here, so the flags sit in one place.
#
# With COVERAGE_FORMAT unset this is a plain `dotnet test`. With it set, the same run also writes a test
# report and a coverage file under artifacts/. `just coverage all` is what sets it.

[private]
_dotnet_test project:
    #!/usr/bin/env bash
    set -euo pipefail

    if [ -z "${COVERAGE_FORMAT:-}" ]; then
        dotnet test --project {{ project }} {{ dotnet_test_args }}
        exit 0
    fi

    # The format name says who reads the file, not what the file is.
    case "$COVERAGE_FORMAT" in
        cobertura) output_format=cobertura ;;
        sonar)     output_format=xml ;;  # Visual Studio coverage, the only C# format Sonar reads
        *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    name=$(basename {{ project }})
    results="$(pwd)/artifacts/tests"
    coverage="$(pwd)/artifacts/coverage/$COVERAGE_FORMAT"
    mkdir -p "$results" "$coverage"

    # Written before the run, so `just coverage table` can tell "wrote no report" from "never ran".
    echo "$name" >> "$results/expected.txt"

    # Both paths are absolute. A relative coverage path lands inside the project's own bin folder.
    #
    # No `--` in front of these options. `dotnet test` hands anything it does not recognise to the test app
    # itself, and a `--` makes it read the project path as junk.
    dotnet test --project {{ project }} {{ dotnet_test_args }} \
        --results-directory "$results" \
        --report-ctrf --report-ctrf-filename "$name.ctrf.json" \
        --coverage --coverage-output-format "$output_format" \
        --coverage-output "$coverage/$name.xml"

# Runs one gem's tests. Bundler walks up from the gem folder and finds ruby/Gemfile, so every gem runs
# against the same Jekyll the sites are locked to.
#
# Starting from ruby/ itself does not work. rspec has to start inside the gem for its spec_helper to be
# found.

[private]
_rspec_test gem:
    #!/usr/bin/env bash
    # No -e. The exit code is kept below, so the coverage file still gets moved out of a failed run.
    set -uo pipefail

    if [ -z "${COVERAGE_FORMAT:-}" ]; then
        cd ruby/{{ gem }} && exec bundle exec rspec
    fi

    # SimpleCov names its file after the formatter, so the extension is what tells them apart.
    case "$COVERAGE_FORMAT" in
        cobertura) produced=coverage.xml ;;
        sonar)     produced=coverage.json ;;  # what Sonar reads for Ruby
        *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    results="$(pwd)/artifacts/tests"
    coverage="$(pwd)/artifacts/coverage/$COVERAGE_FORMAT"
    mkdir -p "$results" "$coverage"
    echo '{{ gem }}' >> "$results/expected.txt"

    # Both absolute, because rspec runs from inside the gem folder.
    #
    # RUBYOPT and not a require in each spec_helper, because a gem must not reach above its own folder.
    # The file requires bundler/setup itself - CI vendors the bundle, so nothing is on the load path until
    # bundler puts it there.
    export COVERAGE_DIR="$coverage/{{ gem }}"
    export RUBYOPT="-r$(pwd)/tooling/tests.ruby-coverage.rb ${RUBYOPT:-}"

    cd ruby/{{ gem }}
    bundle exec rspec --format progress --format json --out "$results/{{ gem }}.rspec.json"
    rc=$?

    # SimpleCov buries the file in a folder. Lift it out, named after the gem, so the folder holds one flat
    # file per project. Not fatal on its own - a failed run reports through rc.
    if [ -f "$COVERAGE_DIR/$produced" ]; then
        mv "$COVERAGE_DIR/$produced" "$coverage/{{ gem }}.${produced##*.}"
    fi
    rm -rf "$COVERAGE_DIR"

    exit $rc

# Runs one TypeScript package. The jest half of _dotnet_test, with the same two modes.
#
# --selectProjects and not --projects <path>, because the root jest.config.js keeps the workspace folder in
# the recorded paths, and its collectCoverageFrom is ignored when a package's own config drives the run.
#
# Paths are absolute. jest resolves a relative one against the package, not against artifacts/.

[private]
_jest_test package:
    #!/usr/bin/env bash
    set -euo pipefail

    if [ -z "${COVERAGE_FORMAT:-}" ]; then
        npx jest --selectProjects {{ package }}
        exit 0
    fi

    # The same two format names as _dotnet_test, in what jest calls them. Sonar reads javascript coverage as
    # lcov, not the Visual Studio xml it wants for C#.
    case "$COVERAGE_FORMAT" in
        cobertura) reporter=cobertura ;;
        sonar)     reporter=lcovonly ;;
        *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    results="$(pwd)/artifacts/tests"
    coverage="$(pwd)/artifacts/coverage/$COVERAGE_FORMAT"
    mkdir -p "$results" "$coverage"
    echo '{{ package }}' >> "$results/expected.txt"

    npx jest --selectProjects {{ package }} \
        --coverage --coverageReporters="$reporter" --coverageDirectory="$coverage/{{ package }}" \
        --json --outputFile="$results/{{ package }}.jest.json"

    # jest names the file after its reporter and buries it in a folder. Lift it out, named after the package,
    # so the folder holds one flat file per project.
    for produced in "$coverage/{{ package }}"/*; do
        [ -f "$produced" ] || continue
        mv "$produced" "$coverage/{{ package }}.${produced##*.}"
    done
    rmdir "$coverage/{{ package }}"

    # jest names its cobertura packages after the folders inside the package - "main", "src", "src.core" -
    # and three of ours are called "main". The merged report adds same-named packages together, so cookies,
    # theme-switcher and binacle-compact-notation came out as one row. Overwrite them all with the package
    # name, which is what the C# and ruby files already carry.
    if [ "$reporter" = cobertura ]; then
        sed -i 's|<package name="[^"]*"|<package name="{{ package }}"|g' "$coverage/{{ package }}.xml"
    fi
