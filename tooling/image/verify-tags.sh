#!/usr/bin/env bash
# Which tags in the repository are this same image. Rows sharing a digest are one image under several names.
#   verify-tags.sh <version> <repo>
#
# Two registries, two APIs. Docker Hub's web API lists every tag with its digest in one call. GHCR is a plain
# OCI registry: an anonymous pull token, the tag list, then one HEAD per tag for its digest. Both are public,
# so neither needs a login.
set -uo pipefail

version="$1"
repo="$2"

tags_hub() {
    echo "== docker hub tags =="
    json=$(curl --proto '=https' --proto-redir '=https' -fsSL \
        "https://hub.docker.com/v2/repositories/${repo}/tags?page_size=100") || {
        echo "  could not read the tag list" >&2
        return 1
    }
    digest=$(echo "$json" | jq -r --arg v "$version" '.results[] | select(.name==$v) | .digest')
    if [ -z "$digest" ] || [ "$digest" = "null" ]; then
        echo "  no '${version}' tag on docker hub" >&2
        return 1
    fi

    # The date is printed but never compared - it moves for reasons that are not a retag.
    echo "$json" | jq -r --arg d "$digest" '
        .results[] | select(.digest==$d)
        | "  \(.name)   \(.digest[7:19])   \(.full_size)   \(.last_updated[0:10])"'
    echo "  every row above is the same image under another name"
}

tags_ghcr() {
    echo "== ghcr tags =="
    local name="${repo#ghcr.io/}" token tags digest tag
    token=$(curl --proto '=https' --proto-redir '=https' -fsSL \
        "https://ghcr.io/token?scope=repository:${name}:pull" | jq -r .token) || {
        echo "  could not get a pull token" >&2
        return 1
    }
    tags=$(curl --proto '=https' --proto-redir '=https' -fsSL -H "Authorization: Bearer $token" \
        "https://ghcr.io/v2/${name}/tags/list" | jq -r '.tags[]') || {
        echo "  could not read the tag list" >&2
        return 1
    }

    # The digest comes back in a header, so a HEAD per tag. Every media type is accepted so the registry
    # answers with the index's own digest, not a manifest it converted for the client.
    digest_of() {
        curl --proto '=https' --proto-redir '=https' -fsI -H "Authorization: Bearer $token" \
            -H 'Accept: application/vnd.oci.image.index.v1+json, application/vnd.docker.distribution.manifest.list.v2+json, application/vnd.oci.image.manifest.v1+json' \
            "https://ghcr.io/v2/${name}/manifests/$1" | tr -d '\r' | awk 'tolower($1)=="docker-content-digest:" {print $2}'
    }

    digest=$(digest_of "$version")
    if [ -z "$digest" ]; then
        echo "  no '${version}' tag on ghcr" >&2
        return 1
    fi

    # A `sha256-<digest>` tag is a cosign signature, not an image, and is skipped rather than listed as a
    # name for one.
    for tag in $tags; do
        case "$tag" in sha256-*) continue ;; esac
        [ "$(digest_of "$tag")" = "$digest" ] || continue
        echo "  ${tag}   ${digest:7:12}"
    done
    echo "  every row above is the same image under another name"
}

case "$repo" in
    ghcr.io/*) tags_ghcr ;;
    *)         tags_hub ;;
esac
