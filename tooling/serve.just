# Runs something locally, from source.
#
#   just serve api [profile]     the API, on one of its launch profiles
#   just serve docs              the documentation site
#   just serve demo              the demo site
#   just serve www               the marketing site
#   just serve services-up [-d]  what the API talks to: aspire dashboard, azurite, postgres
#   just serve services-down     stop them, only needed after -d
#
# `services-up` runs no binacle-net at all. It brings up what `just serve api` talks to, and what the
# Postgres and AzureStorage tests need.
#
# The `full` image stack includes tooling/serve.services.yml, so `services-down -v` wipes a database that
# stack shares.
#
# Running the built image is the `image` module. Nothing here is called by CI.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# dotnet, jekyll and webpack all print why they failed, so just's own "Recipe failed" line adds nothing.
set no-exit-message := true

# List the recipes
default:
    @just --list serve

# One recipe with the profile as an argument, not four recipes - it is one run with the modules switched on
# and off. The profiles live in api/src/Binacle.Net/Properties/launchSettings.json.

# The API [Normal|WithServiceModuleOnly|WithUiModuleOnly|WithAllModules] (N|S|U|All)
api profile="Normal":
    #!/usr/bin/env bash
    set -euo pipefail

    # A typo would otherwise reach `dotnet run`, which starts the app on its default profile - a working run
    # of something nobody asked for.
    case "{{ profile }}" in
        Normal | N)                launch_profile=Normal ;;
        WithServiceModuleOnly | S) launch_profile=WithServiceModuleOnly ;;
        WithUiModuleOnly | U)      launch_profile=WithUiModuleOnly ;;
        WithAllModules | All | A)  launch_profile=WithAllModules ;;
        *)
            echo "Unknown profile '{{ profile }}'." >&2
            echo "Use Normal (N), WithServiceModuleOnly (S), WithUiModuleOnly (U) or WithAllModules (All)." >&2
            exit 1
            ;;
    esac

    echo "Running Binacle.Net with launch profile: $launch_profile"

    # Only the two UI profiles get the watches. On the others the module is off, so they would rebuild output
    # no page can ask for, into a log you are reading for something else.
    #
    # The two commands are written out here rather than calling the module's own `npm run watch`, which nests
    # one concurrently inside another and buries the per-process labels.
    case "$launch_profile" in
        WithUiModuleOnly | WithAllModules)
            npx concurrently \
                --kill-others \
                --names 'api,css,js' \
                --prefix-colors 'magenta,yellow,cyan' \
                "dotnet run --project api/src/Binacle.Net -lp $launch_profile" \
                'cd api/src/Binacle.Net.UIModule && npm run watch:css' \
                'cd api/src/Binacle.Net.UIModule && npm run watch:js'
            ;;
        *)
            dotnet run --project api/src/Binacle.Net -lp "$launch_profile"
            ;;
    esac

# Extra arguments go straight to `docker compose`, so `-d` detaches. In the foreground Ctrl-C stops them,
# which is why `services-down` is only needed after a `-d`.

# What the API talks to: aspire dashboard, azurite, postgres [-d]
services-up *args:
    #!/usr/bin/env bash
    set -euo pipefail

    # Azurite bind-mounts tooling/azurite. Left to itself docker creates a missing bind source as root, and
    # the emulator state then sits in the repo and you cannot open it - so make the folder first.
    #
    # The directory only, never `-R`: azurite writes its own files as root and they stay root's, so a
    # recursive chmod fails on exactly those while making nothing more writable.
    mkdir -p tooling/azurite
    if [ "$(stat -c '%a' tooling/azurite)" != "777" ]; then
        # sudo only for a folder docker made. A plain chmod covers the one we just made.
        chmod 777 tooling/azurite 2>/dev/null || sudo chmod 777 tooling/azurite
    fi

    docker compose -f tooling/serve.services.yml up {{ args }}

# Stop the backing services [-v drops binacle-net-postgres, shared with the full image stack]
services-down *args:
    docker compose -f tooling/serve.services.yml down {{ args }}

# Docs site: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
docs: (_serve-n-watch 'sites/docs')

# Demo site: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
demo: (_serve-n-watch 'sites/demo')

# Three processes for www, not two: its `npm run watch` is itself a concurrently pair, because the sass CLI
# compiles the stylesheet here and Jekyll does not.

# Marketing site: jekyll serve + sass and webpack watches, one terminal (Ctrl-C stops all three)
www: (_serve-n-watch 'sites/www')

# Serve and watch together, each line prefixed with its source. --kill-others makes one Ctrl-C stop both.
[private]
_serve-n-watch dir:
    cd "{{ dir }}" && npx concurrently \
        --kill-others \
        --names 'serve,watch' \
        --prefix-colors 'magenta,cyan' \
        'npm run serve' 'npm run watch'
