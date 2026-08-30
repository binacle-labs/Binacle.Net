#!/usr/bin/env bash
# Copy one image to one or more tags, by digest, then prove every tag reads back as that digest.
#   copy-tags.sh <source ref with digest> '<tags, one per line or space separated>'
set -euo pipefail

source_ref="$1"

# Both callers hand this over differently - the workflow's tag list arrives one per line, the moving tags
# come back from moving-tags.sh on one line. Tags never contain a space, so flattening is safe.
tags=()
while read -r tag; do
    [[ -n "$tag" ]] || continue
    tags+=("$tag")
done < <(tr ' ' '\n' <<<"$2")

if [[ "${#tags[@]}" -eq 0 ]]; then
    echo "no tags to copy" >&2
    exit 1
fi

args=()
for tag in "${tags[@]}"; do
    args+=(-t "$tag")
done

echo "Copying ${source_ref} to: ${tags[*]}"
docker buildx imagetools create "${args[@]}" "$source_ref"

# A copy is by digest, so every tag must read back as the same one. This is the only place the log shows it.
for tag in "${tags[@]}"; do
    echo -n "${tag} -> "
    docker buildx imagetools inspect "$tag" --format '{{ .Manifest.Digest }}'
done
