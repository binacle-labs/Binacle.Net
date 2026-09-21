#!/usr/bin/env bash
# Runs one benchmark project at one BenchmarkDotNet job. The door is tooling/bench.just.
#
#   tooling/bench.run.sh <project> <job> [words...]
#
# The words ffd, bfd, wfd, packing, fitting become one --filter glob - two globs would be OR. The glob is
# always *<Op>*<Alg>*, whatever order the words come in: the operation is in the class name and the algorithm
# is in the method name or a param, so that order matches every class. Any other word passes through to
# BenchmarkDotNet as is.

set -euo pipefail

project="$1"
job="$2"
shift 2

alg=''
op=''
passthrough=()
for word in "$@"; do
    case "$word" in
        ffd|bfd|wfd)
            if [[ -n "$alg" ]]; then echo "One algorithm word at most: got $alg and $word." >&2; exit 1; fi
            alg="${word^^}" ;;
        packing|fitting)
            if [[ -n "$op" ]]; then echo "One operation word at most: got $op and $word." >&2; exit 1; fi
            op="${word^}" ;;
        *) passthrough+=("$word") ;;
    esac
done

glob='*'
[[ -n "$op" ]] && glob="${glob}${op}*"
[[ -n "$alg" ]] && glob="${glob}${alg}*"

exec dotnet run -c Release --project "$project" -- --job "$job" --filter "$glob" "${passthrough[@]}"
