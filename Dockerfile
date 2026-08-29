FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Constant OCI labels only. version, revision and created are per-build and are applied by the build command
# (--label in build.just, metadata-action in CI), so they add no layer and never bust this cache. In CI
# metadata-action overrides the keys it also emits (title, description, source, url, licenses); documentation,
# vendor and base.name are set nowhere else, so they survive from here. description is pinned to this same
# string in two other places - release-docker-image.yml, and api/src/Binacle.Net.Kernel/Metadata.cs, which is
# what Swagger UI, Scalar and both published OpenAPI documents read. Change all three, or the image label and
# the API documents disagree with no error anywhere.
LABEL org.opencontainers.image.title="Binacle.Net" \
      org.opencontainers.image.description="Binacle.Net answers which box an order goes in, and whether it fits, in milliseconds." \
      org.opencontainers.image.source="https://github.com/binacle-labs/Binacle.Net" \
      org.opencontainers.image.url="https://www.binacle.net" \
      org.opencontainers.image.documentation="https://docs.binacle.net" \
      org.opencontainers.image.vendor="Binacle Labs" \
      org.opencontainers.image.licenses="GPL-3.0-only AND CC-BY-4.0 AND Apache-2.0 AND MIT" \
      org.opencontainers.image.base.name="mcr.microsoft.com/dotnet/aspnet:10.0"

ARG VERSION
ENV BINACLE_VERSION=$VERSION

# Npgsql probes for GSSAPI whenever it opens a connection. The app works without it — we authenticate with a
# password, not Kerberos — but it prints "Cannot load library libgssapi_krb5.so.2" on every start, which reads
# like a fatal error in the logs of anyone running the Postgres backend. Cheaper to ship the library than to
# explain the message. Kept above the COPY so it caches across builds.
RUN apt-get update \
 && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
 && rm -rf /var/lib/apt/lists/*

# There is no build stage - `just build publish` writes this folder before docker runs. The path is hardcoded
# here and is the one allowlisted entry in .dockerignore, so publishing anywhere else builds an empty image.
COPY ["artifacts/binacle-net", "."]

# Logs, pack-logs, and the SQLite database are written here. It has to exist in the image and be owned by the
# app user: docker creates a mount point that the image does not have as root, and the app does not run as
# root, so a volume mounted over a missing /app/data is unwritable. A fresh named volume inherits this
# ownership from the image.
RUN mkdir -p /app/data && chown $APP_UID:$APP_UID /app/data

USER $APP_UID

ENTRYPOINT ["dotnet", "Binacle.Net.dll"]
