#!/usr/bin/env bash
# Runs one gem's tests. Bundler walks up from the gem folder and finds ruby/Gemfile, so every gem runs
# against the same Jekyll the sites are locked to.
#   rspec.sh <gem folder under ruby/>
#
# Starting from ruby/ itself does not work. rspec has to start inside the gem for its spec_helper to be found.
#
# No -e. The exit code is kept below, so the coverage file still gets moved out of a failed run.
set -uo pipefail

gem="$1"

if [ -z "${COVERAGE_FORMAT:-}" ]; then
    cd "ruby/$gem" && exec bundle exec rspec
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
echo "$gem" >> "$results/expected.txt"

# Both absolute, because rspec runs from inside the gem folder.
#
# RUBYOPT and not a require in each spec_helper, because a gem must not reach above its own folder.
root="$(pwd)"
export COVERAGE_DIR="$coverage/$gem"
export RUBYOPT="-r$root/tooling/tests/ruby-coverage.rb ${RUBYOPT:-}"

cd "ruby/$gem" || exit 1
bundle exec rspec --format progress --format json --out "$results/$gem.rspec.json"
rc=$?

# SimpleCov buries the file in a folder. Lift it out, named after the gem, so the folder holds one flat file
# per project. Not fatal on its own - a failed run reports through rc.
if [ -f "$COVERAGE_DIR/$produced" ]; then
    mv "$COVERAGE_DIR/$produced" "$coverage/$gem.${produced##*.}"
fi
rm -rf "$COVERAGE_DIR"

exit $rc
