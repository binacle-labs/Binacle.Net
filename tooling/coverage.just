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

    # The test-support assemblies are dropped - a test helper scoring itself says nothing about shipped code.
    # `*.TestsKernel` covers every kernel, so adding one needs no edit here.
    dotnet reportgenerator \
        -reports:'artifacts/coverage/cobertura/*.xml' \
        -targetdir:'artifacts/coverage/html-report' \
        -assemblyfilters:'-*.TestsKernel;-Binacle.TestReporting;-*.UnitTests;-*.IntegrationTests' \
        -reporttypes:'Html;TextSummary;MarkdownSummaryGithub' \
        > /dev/null

    echo "HTML report: artifacts/coverage/html-report/index.html"

# One row per suite, and the run's exit code (the Coverage column needs cobertura)
table format='cobertura':
    #!/usr/bin/env bash
    # No -e. A grep that finds nothing must leave a blank cell, not kill the table.
    set -uo pipefail
    shopt -s nullglob

    total_failed=0
    seen=""

    # The numbers sit in a small summary object, so one grep each is enough - no json parser needed.
    count() { grep -o "\"$2\":[0-9]*" "$1" | head -1 | cut -d: -f2; }

    # Cobertura puts the whole-file rate on its root element, which is all a row needs.
    rate() {
        local file="artifacts/coverage/{{ format }}/$1.xml"
        [ -f "$file" ] || { echo "-"; return; }
        local value
        value=$(grep -o 'line-rate="[0-9.]*"' "$file" | head -1 | cut -d'"' -f2)
        [ -n "$value" ] && awk "BEGIN { printf \"%.1f%%\", $value * 100 }" || echo "-"
    }

    row() {
        printf '%-44s Passed:%-6s Failed:%-4s Skipped:%-4s Coverage:%s\n' "$1" "$2" "$3" "$4" "$(rate "$1")"
        total_failed=$((total_failed + $3))
        seen="$seen $1"
    }

    echo ""

    # C# writes CTRF, jest writes its own json and rspec writes a third. The same three numbers under three
    # sets of field names.
    for report in artifacts/tests/*.ctrf.json; do
        name=$(basename "$report" .ctrf.json)
        row "$name" "$(count "$report" passed)" "$(count "$report" failed)" "$(count "$report" skipped)"
    done

    for report in artifacts/tests/*.jest.json; do
        name=$(basename "$report" .jest.json)
        row "$name" "$(count "$report" numPassedTests)" "$(count "$report" numFailedTests)" \
            "$(count "$report" numPendingTests)"
    done

    for report in artifacts/tests/*.rspec.json; do
        name=$(basename "$report" .rspec.json)
        row "$name" "$(count "$report" example_count)" "$(count "$report" failure_count)" \
            "$(count "$report" pending_count)"
    done

    # A suite that ran and wrote nothing would be missing from the table, which reads as "it does not exist".
    # Every test writes its name into expected.txt before it runs, so a missing report is a red row instead.
    while IFS= read -r name; do
        [ -n "$name" ] || continue
        case " $seen " in *" $name "*) continue ;; esac
        printf '%-44s no report\n' "$name"
        total_failed=$((total_failed + 1))
    done < <(sort -u artifacts/tests/expected.txt 2>/dev/null)

    echo ""

    if [ "$total_failed" -gt 0 ]; then
        echo "$total_failed failed - scroll up for the failures, each suite printed its own."
        exit 1
    fi

    echo "All suites passed."
