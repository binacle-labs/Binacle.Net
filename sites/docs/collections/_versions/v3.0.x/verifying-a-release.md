---
title: Verifying a Release
description: >-
  Check that a Binacle.Net image is the one the release workflow built. Two commands: cosign verify, then
  inspect the SBOM and build provenance.
nav:
  order: 8
  icon: 🔏
---

Every image published from `3.0.0` onward is signed with [Sigstore](https://www.sigstore.dev/) cosign,
and carries an SPDX software bill of materials and SLSA build provenance. Signing is keyless and happens inside
the GitHub Actions release workflow, so there is no private key anywhere - the signature is tied to the workflow
that built the image.

Two commands cover it. Replace `<version>` with the release you pulled.

```bash
cosign verify binacle/binacle-net:<version> \
  --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@refs/heads/main$' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com

docker buildx imagetools inspect binacle/binacle-net:<version>
```

> **Releases before `3.0.0` cannot be verified.** `2.1.1` and everything earlier were
> published before the signing pipeline existed, so `cosign verify` answers `no signatures found` against them.
> The check has not failed; there is nothing there to check. It applies to a moving tag like `latest` too, for
> as long as it still points at one of those releases.
{: .block-warning}

## 🛠️ Install cosign

`cosign` is a single binary. Install it from the [Sigstore docs](https://docs.sigstore.dev/) or download it
from the [cosign releases page](https://github.com/sigstore/cosign/releases). `docker buildx` already ships
with Docker.

Use **cosign 2.6.0 or later**, or 3.0.1 or later. Sigstore is moving the public transparency log that
signatures are recorded in, and older builds cannot read entries in the new one. An out-of-date binary fails
the check the same way a tampered image would.

## 🔐 Why both cosign flags matter

Drop `--certificate-identity-regexp` and you are only asking whether *anyone* signed the image. Anyone can:
Sigstore is open to every GitHub account, and a signature on its own says nothing about who made it. The two
flags together are the whole check - the issuer says the identity came from GitHub Actions, and the identity
pattern says it was this repository's release workflow.

The pattern ends `@refs/heads/main$`. That last part is the branch the release workflow is dispatched on, and
the `$` closes the pattern there. Without it the pattern matches anything after the `@`, so a signature made
from any branch in this repository would pass the check - and pushing a branch is not a release.

The signature covers the **image digest**, not the tag. So it holds for the `3.0` and `latest` tags as well as
the exact version tag: whichever one you verify, you are verifying the same artifact.

## 🧾 Reading the attestations

`docker buildx imagetools inspect` lists what is in the image index: the platform manifests, plus one
attestation manifest per platform holding the SPDX bill of materials and the SLSA provenance.

The bill of materials is the package list - every OS package and .NET assembly in the image, with versions. It
is what you feed to a scanner or check a CVE against. The provenance records how the image was built: the
workflow, the run, and the source commit it came from.

## 🔍 What a checked release looks like

This is the record for one release, `3.0.0-beta.5`, so you know the shape of a real answer before you run it
yourself.

Its index resolves to this digest on Docker Hub:

```text
sha256:17b721c77d451f9263f7de671b5a93817be66ec5ef3eb5289ab8054e30df6813
```

Its bill of materials lists **167 packages**, and its provenance names the build that produced it:

```text
https://github.com/binacle-labs/Binacle.Net/actions/runs/33127006852/attempts/1
```

The image config in the same output shows the container runs as `app (1654)` rather than root, with `/app/data`
owned `app:app 755` - the writable folder for a mounted database or key ring.

## 🚦 What a pass means, and what it does not

A passing verify proves the image came from this repository's release workflow and has not been altered since.
That is a strong claim about **origin**.

It is not a claim about **safety**. A signature says nothing about the vulnerabilities in what was signed - a
genuine image with a known CVE in it verifies perfectly. For that question, read the bill of materials and scan
it.

> Contributors with a clone can run `just image verify <version>`, which runs four checks against a published
> image in one go.
{: .block-tip}
