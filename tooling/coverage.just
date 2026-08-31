# Coverage. Runs the tests with the collector attached, then prints a table.
#
#   just coverage all                             every test with coverage, then the table
#   just coverage all sonar                       the same, in the formats Sonar reads
#   just coverage all sonar all-with-services     and including the backends that need services up
#   just coverage report                          merge the last cobertura run into one HTML report
#
# The format name says who reads the files, not what the files are. `cobertura` is what the table and the
# HTML report read. `sonar` is Visual Studio xml for C# and lcov for TypeScript. Each one writes one flat
# file per suite into artifacts/coverage/<format>/, named after the project or package - plus the backend,
# where one project runs against several.
#
# Nothing here knows what test projects exist, and nothing here runs a test. Each recipe sets
# COVERAGE_FORMAT and calls a `just test` list, and every test writes its own files. What is left here is the
# merge and the table.
#
# The list defaults to `all`, which starts nothing. `all-with-services` is the one CI passes.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The test runner already said what broke and `table` prints the verdict, so just's own line adds nothing.
set no-exit-message := true

# Run a test list with coverage, then print the table [cobertura|sonar] [test list]
all format='cobertura' list='all': (run format list) (table format)

# Run a test list with the coverage collector attached [cobertura|sonar] [test list]
run format list='all':
    tooling/coverage.run.sh '{{ format }}' '{{ list }}'

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
