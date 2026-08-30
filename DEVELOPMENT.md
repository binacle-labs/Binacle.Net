# Development

What you need installed to build, test and run Binacle.Net from source, and how to get it.

This file is the one place tool versions and install steps live. Everything else in the repo points here rather
than repeating it. The four pinned binaries below are the exception, and a deliberate one: their version, their
checksum and their download are in `tooling/ci/install-<tool>.sh`, which is the same script CI runs. You run it
too, so a laptop and a runner install the same bytes by construction rather than by both being edited.

## 🧰 What you need

Where a pin file exists the version manager reads it automatically, but you still have to install the manager
first. The pins are what CI uses, so matching them locally is what keeps a green machine and a green runner
meaning the same thing.

| Tool | Version | Pin file | Needed for |
|---|---|---|---|
| .NET SDK | 10.x | none (`Directory.Build.props` sets `net10.0`) | the API, `lib`, `vipaq` - everything C# |
| Node.js, via [nvm](https://github.com/nvm-sh/nvm) | 22 | `.nvmrc` | `just install`, the TS packages, `assets`, all three sites' webpack |
| Ruby, via [rbenv](https://github.com/rbenv/rbenv) | 3.4.7 | `sites/docs/.ruby-version`, `sites/demo/.ruby-version`, `sites/www/.ruby-version` | Jekyll, for **all three** sites |
| [just](https://github.com/casey/just#installation) | any recent | none | every recipe in this repo |
| Docker | 28+ | none | the image, the `image` and `smoke` modules, the Postgres and AzureStorage tests |
| jq | any recent | none | `just image verify` - every one of its four checks reads JSON |

Only Docker and jq are optional. Everything else is needed for a full `just install` and `just test all`.

Two maintenance gems are deliberately not in either Gemfile - they are one-off tools, not site dependencies.
Install them globally if you need them:

```bash
gem install bundler-audit          # adds `bundle audit`, checks the lockfile against the CVE database
gem install bundler-audit-fix      # adds `bundle audit-fix`, bumps the flagged gems and rewrites the lockfile
```

## 🛠️ Installing

### 📦 .NET, Node, Ruby, just

```bash
# .NET SDK 10 - see https://dotnet.microsoft.com/download for other distros
sudo apt install dotnet-sdk-10.0

# just
sudo apt install just

# Node, via nvm. `nvm use` in the repo root reads .nvmrc and switches to the pinned version.
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/master/install.sh | bash
nvm install 22

# Ruby, via rbenv. Both sites pin the same version, so one install covers them.
rbenv install 3.4.7
```

`nvm use` is per shell and not sticky. If `node --version` does not match `.nvmrc`, that is why - the packages
build against whatever is active, and CI uses 22.

### 🐳 Docker

Docker CE from Docker's own repository, rather than Ubuntu's `docker.io`, so local matches what the GitHub
Actions runners use:

```bash
# The repository and its key
sudo apt-get update
sudo apt-get install -y ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] \
https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "$VERSION_CODENAME") stable" \
  | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Docker itself
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Run it without sudo. Log out and back in, or `newgrp docker` for this shell only.
sudo usermod -aG docker $USER
newgrp docker

docker run --rm hello-world
```

`$VERSION_CODENAME` resolves itself, so the same block works on any supported Ubuntu.

### 🧪 The smoke tools

Only needed for `just smoke`. Both are single static binaries and go to `~/.local/bin`, which is already on
`PATH` - no sudo, and nothing to uninstall but a file.

```bash
# container-structure-test - asserts what is inside the image
tooling/ci/install-container-structure-test.sh

# hurl - runs the .hurl files against a running stack
tooling/ci/install-hurl.sh
```

Each one downloads, checks the file against its pinned SHA-256, installs it and prints the version. A bad
checksum stops it there rather than running an unknown binary.

#### hurl on Ubuntu 26.04

The script above leaves a hurl that will not start on Ubuntu 26.04. The binary links against `libxml2.so.2`;
26.04 ships only `libxml2.so.16` (package `libxml2-16`) and carries no compat package, so it dies with a
missing-library error that looks nothing like a hurl problem. The `.deb` fails the same way - it depends on
`libxml2`, which is no longer a package name there, and on `libcurl4`, now `libcurl4t64`.

Check first. If this prints nothing, you need the workaround:

```bash
ldconfig -p | grep 'libxml2\.so\.2$'
```

Keep a private copy of the 24.04 library - and `libicu74`, which it needs in turn - somewhere only hurl looks,
behind a wrapper:

```bash
mkdir -p ~/.local/lib/hurl /tmp/hurl-libs
for deb in \
  http://security.ubuntu.com/ubuntu/pool/main/libx/libxml2/libxml2_2.9.14+dfsg-1.3ubuntu3.8_amd64.deb \
  http://security.ubuntu.com/ubuntu/pool/main/i/icu/libicu74_74.2-1ubuntu3.1_amd64.deb; do
    curl -fsSL -o /tmp/lib.deb "$deb"
    dpkg-deb -x /tmp/lib.deb /tmp/hurl-libs
done
cp -P /tmp/hurl-libs/usr/lib/x86_64-linux-gnu/libxml2.so.2* ~/.local/lib/hurl/
cp -P /tmp/hurl-libs/usr/lib/x86_64-linux-gnu/libicu*.so.74* ~/.local/lib/hurl/

# Move the binary the script installed next to them, and put a wrapper on PATH in its place.
mv ~/.local/bin/hurl ~/.local/lib/hurl/hurl.bin
cat > ~/.local/bin/hurl <<'EOF'
#!/bin/sh
# hurl needs libxml2.so.2, which this system does not have. The private copy beside the binary is the only
# thing that sees this LD_LIBRARY_PATH - see DEVELOPMENT.md.
exec env LD_LIBRARY_PATH="$HOME/.local/lib/hurl${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}" \
  "$HOME/.local/lib/hurl/hurl.bin" "$@"
EOF
chmod 755 ~/.local/bin/hurl

hurl --version
```

About 42MB, almost all of it ICU data. `rm -rf ~/.local/lib/hurl ~/.local/bin/hurl` undoes it completely -
then re-run the install script for a plain binary again.
Nothing outside hurl ever sees those libraries, which is the point - an old libxml2 on the system library path
would be a real problem. Every other distro takes the plain binary and needs none of this, CI runners included
as long as they stay on noble.

### 🔍 lychee

Only needed for `just check links`, which checks the links in a built site. Same shape as the smoke tools - one
static binary in `~/.local/bin`, pinned rather than `latest`. The musl build is deliberate: it links nothing
from the system, which is the whole class of problem the hurl section above describes.

```bash
tooling/ci/install-lychee.sh
```

Upstream publishes a `.sha256` beside every asset, and the script checks the download against it before
installing. If that step ever fails, the release asset changed - do not work around it.

### 🚦 actionlint and shellcheck

Needed for `just check workflows` and, for shellcheck alone, `just check scripts`. **Both, not just the
first** - actionlint runs shellcheck over every `run:` block when it can find it, and the GitHub runners ship
shellcheck already. Installing only actionlint here means a laptop checks less than CI does and finds out on
the pull request.

```bash
# actionlint - the workflow and action files
tooling/ci/install-actionlint.sh

# shellcheck 0.11.0 - what actionlint hands the run: blocks to, and what `just check scripts` runs
curl -fsSL -o /tmp/sc.tar.xz \
  https://github.com/koalaman/shellcheck/releases/download/v0.11.0/shellcheck-v0.11.0.linux.x86_64.tar.xz
tar xJf /tmp/sc.tar.xz -C /tmp
install -m0755 /tmp/shellcheck-v0.11.0/shellcheck ~/.local/bin/shellcheck

shellcheck --version
```

**shellcheck has no install script, because CI never installs it** - the GitHub runners ship it. This is the
one tool where a laptop does the work a runner does not.

### 🔒 cosign

Only needed for `just image verify`, which checks a published image's signature. Same shape as the smoke tools
- one static binary in `~/.local/bin`, pinned rather than `latest`, nothing to uninstall but a file.

```bash
# cosign 3.1.3 - verifies the signature on a published image
curl -fsSL -o /tmp/cosign \
  https://github.com/sigstore/cosign/releases/download/v3.1.3/cosign-linux-amd64
install -m0755 /tmp/cosign ~/.local/bin/cosign

cosign version
```

**cosign has no install script either**, for the opposite reason to shellcheck: CI installs it through
sigstore's own action, so there is no shared shell to run here.

`just image verify <version>` runs without it and says so - the other three checks still work, and only the
signature one stops. Nothing needs a `docker login`, cosign included: these are the commands a user runs, and
a check that only passes with a credential is not checking a public artifact.

## 🚀 First run

```bash
just install                     # npm workspaces, all three sites' gems, then the asset copy
just test all                    # every suite that needs nothing brought up
just build image                 # publish, then tag binacle-net:local
```

`just` with no arguments lists every task.

## 🔗 Where to go next

- `tooling/README.md` - every `just` module in detail: serve, test, coverage, build, image, smoke, and the
  container data folders they use.
- `tooling/smoke/README.md` - what the image smoke suite asserts and why.
- Each top-level folder has its own `README.md`.
