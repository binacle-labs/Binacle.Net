# Coverage. Runs the tests with the collector attached, then prints a table.
#
#   just coverage all              every test with coverage, then the table
#   just coverage all sonar        the same, in the formats Sonar reads
#   just coverage report           merge the last cobertura run into one HTML report
#
# The format name says who reads the files, not what the files are. `cobertura` is what the table and the
# HTML report read. `sonar` is Visual Studio xml for C# and lcov for TypeScript. Each one writes one flat
# file per suite into artifacts/coverage/<format>/, named after the project or package.
#
# Nothing here knows what test projects exist, and nothing here runs a test. Each recipe sets
# COVERAGE_FORMAT and calls `just test all`, and every test writes its own files. What is left here is the
# merge and the table.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The test runner already said what broke and `table` prints the verdict, so just's own line adds nothing.
set no-exit-message := true

# Run every test with coverage, then print the table [cobertura|sonar]
all format='cobertura': (run format) (table format)

# Run every test with the coverage collector attached [cobertura|sonar]
run format:
    #!/usr/bin/env bash
    set -euo pipefail

    case '{{ format }}' in
        cobertura | sonar) ;;
        *) echo "Unknown format '{{ format }}'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    # The whole folder, not only this format's. A leftover cobertura run would still be sitting there after a
    # sonar run, and `report` would merge it and look normal. artifacts/coverage holds exactly one run.
    rm -rf artifacts/tests artifacts/coverage

    # `|| true` because a failing suite still wrote its report, and `table` is what says pass or fail.
    # Without it a failed run prints no table at all - the one time you most want to see it.
    COVERAGE_FORMAT='{{ format }}' just test all || true

# Merge the last cobertura run into one HTML report
report:
    #!/usr/bin/env bash
    set -euo pipefail

    # Only cobertura carries the per-line data the report needs. A sonar run leaves nothing to merge.
    if [ ! -d artifacts/coverage/cobertura ]; then
        echo "No cobertura coverage in artifacts/coverage. Run 'just coverage all' first." >&2
        exit 1
    fi

    dotnet tool restore > /dev/null

    # Inputs and filters come from .netconfig at the repo root, which is reportgenerator's own config file.
    dotnet reportgenerator \
        -targetdir:'artifacts/coverage/html-report' \
        -reporttypes:'Html;TextSummary;MarkdownSummaryGithub' \
        > /dev/null

    echo "HTML report: artifacts/coverage/html-report/index.html"

# One row per suite, then coverage per project, then the run's exit code (coverage needs cobertura)
table format='cobertura':
    tooling/coverage.table.sh '{{ format }}'
