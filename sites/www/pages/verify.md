---
permalink: /verify/
title: Verify a release
heading: Verify a release.
description: >-
  Check that a Binacle.Net image is the one the release workflow built. Every image from 3.0.0 onward is signed
  with Sigstore cosign, and carries a software bill of materials and build provenance.
---

# {{ page.heading }}
{: .band--split }

Check that a Binacle.Net image is the one the release workflow built. Every image from 3.0.0 onward is signed with
Sigstore cosign, and carries a software bill of materials and build provenance.

Signing is keyless and happens inside the release workflow, so there is no private key anywhere.
{: .muted }

## Two commands
{: .band--wide .band--tint data-label="The check" }

The first checks the signature. The second lists the bill of materials and the provenance.

They name `3`, the major tag. Any tag or digest pointing at a signed image works the same way.

```bash
cosign verify binacle/binacle-net:3 \
  --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@refs/heads/main$' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com
```
{: data-name="Signature &middot; bash" }

```bash
docker buildx imagetools inspect binacle/binacle-net:3
```
{: data-name="Bill of materials &middot; bash" }

Use cosign 2.6.0 or later, or 3.0.1 or later. An older binary fails the check the same way a tampered image would.
{: .verdict }

## Why both cosign flags matter
{: .band--tint data-label="The two flags" }

Drop `--certificate-identity-regexp` and you are only asking whether anyone signed the image. Anyone can. The issuer
says the identity came from GitHub Actions; the pattern says it was this repository's release workflow.

## A pass proves where it came from. Not that it is safe.
{: .statement }

A genuine image with a known CVE in it verifies perfectly. For that question, read the bill of materials and scan it.
{: .muted }

## Releases before 3.0.0 cannot be verified
{: .band--compact data-label="Older releases" }

They were published before the signing pipeline existed. `cosign verify` answers `no signatures found`: there is
nothing there to check.
