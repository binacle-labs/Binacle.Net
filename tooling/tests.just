# Runs the tests. One recipe per test project, so one failure names one project.
#
# The three runners - one per language - are scripts in tooling/tests/, because a `.just` body is invisible
# to shellcheck and cannot be run on its own. The recipes here are the names and the lists.
#
#   just test                            the list
#   just test all                        every project
#   just test all-with-services          the same, plus the backends that need services running
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

# Extra flags for `dotnet test` go in DOTNET_TEST_ARGS, read by tooling/tests/dotnet.sh. CI builds the
# solution once and passes --no-build; on a laptop you want none.
#
#   DOTNET_TEST_ARGS="--configuration Release --no-build" just test all

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
    just test ts_binacle-net-client_unit
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
    just test ts_binacle-net-client_unit
    just test ts_binacle-net-service-client_unit
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
    just test ts_binacle-net-client_unit
    just test ts_binacle-net-service-client_unit
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

# Every test, plus the backends that need `just serve services-up -d` first
all-with-services:
    just test all
    just test cs_binacle-net-service-module_integration Postgres
    just test cs_binacle-net-service-module_integration AzureStorage

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

    # No backend is the harness's SQLite fallback, so the file says what actually ran.
    label="{{ backend }}"
    label="${label:-Sqlite}"

    just test::_dotnet_test api/test/Binacle.Net.ServiceModule.IntegrationTests "$label"

# The TypeScript tests. The argument is the package's jest displayName.

[private]
ts_binacle-compact-notation_unit: (_jest_test 'binacle-compact-notation')

[private]
ts_binacle-vipaq_unit: (_jest_test 'binacle-vipaq')

[private]
ts_binacle-net-client_unit: (_jest_test 'binacle-net-client')

[private]
ts_binacle-net-service-client_unit: (_jest_test 'binacle-net-service-client')

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

# One C# test project, with a label for a project run more than once
[private]
_dotnet_test project label="":
    @tooling/tests/dotnet.sh {{ quote(project) }} {{ quote(label) }}

# One gem's specs
[private]
_rspec_test gem:
    @tooling/tests/rspec.sh {{ quote(gem) }}

# One TypeScript package, by its jest displayName
[private]
_jest_test package:
    @tooling/tests/jest.sh {{ quote(package) }}
