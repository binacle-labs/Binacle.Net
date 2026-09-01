#!/usr/bin/env python3
"""Rewrites the _index.md manifests under .agents/.

    tooling/agents/generate-index.py              rewrite all of them
    tooling/agents/generate-index.py plans        rewrite one

What to write is in indexes.toml beside this file: the headings, the blurbs, the group names that
cannot be guessed, and which front matter keys are copied into an entry.

An index lists every .md under its directory, grouped by area - the first path segment, with root-level
files under "General".
"""

import sys
import tomllib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
CONFIG = Path(__file__).resolve().parent / "indexes.toml"


def front_matter(path):
    """The lines between the opening and closing --- , or nothing if line 1 is not ---."""
    lines = path.read_text(encoding="utf-8").splitlines()
    if not lines or lines[0] != "---":
        return []
    out = []
    for line in lines[1:]:
        if line == "---":
            break
        out.append(line)
    return out


def unquote(value):
    """Drop one leading and one trailing double quote."""
    if value.startswith('"'):
        value = value[1:]
    if value.endswith('"'):
        value = value[:-1]
    return value


def field(path, key):
    """One front matter value, empty if it is not there."""
    for line in front_matter(path):
        if line.startswith(key + ":"):
            return unquote(line[len(key) + 1:].lstrip())
    return ""


def yaml_list(path, key):
    """A front matter yaml list, flattened to '"a", "b"'."""
    items, collecting = [], False
    for line in front_matter(path):
        if line.startswith(key + ":"):
            collecting = True
            continue
        if not collecting:
            continue
        stripped = line.lstrip()
        if line[:1].strip() and not line.startswith(" "):
            break
        if stripped.startswith("- "):
            items.append(stripped[2:].replace('"', ""))
    return ", ".join(f'"{item}"' for item in items)


def describe(path):
    """One line for the entry: the description, then the first heading, then the filename."""
    desc = field(path, "description")
    if not desc:
        for line in path.read_text(encoding="utf-8").splitlines():
            if line.startswith("# "):
                desc = line[1:].lstrip()
                break
    if not desc:
        desc = path.name
    return desc.replace("|", r"\|")


def markdown_files(directory):
    """Every .md under the directory, minus the index itself and this directory's own guide README.

    A nested README - api/README.md, vipaq/README.md - is real content and stays.
    """
    files = [
        p.relative_to(directory).as_posix()
        for p in directory.rglob("*.md")
        if p.name != "_index.md"
    ]
    return sorted(f for f in files if f != "README.md")


def group_of(relative_path):
    return relative_path.split("/", 1)[0] if "/" in relative_path else "General"


def groups_in_order(files):
    """General first, every other group sorted."""
    found = {group_of(f) for f in files}
    ordered = ["General"] if "General" in found else []
    return ordered + sorted(found - {"General"})


def entry(directory, relative_path, index_name, config):
    path = directory / relative_path
    lines = [f"- file: {relative_path}"]
    lines.append('  description: "%s"' % describe(path).replace('"', r"\""))

    keys = list(config["fields"]["always"])
    if index_name == "plans":
        keys += config["fields"]["plans_only"]

    for key in keys:
        value = field(path, key)
        if not value:
            continue
        if key in ("load", "state", "horizon"):
            lines.append(f"  {key}: {value}")
        else:
            lines.append('  %s: "%s"' % (key, value.replace('"', r"\"")))

    for key in config["fields"]["lists"]:
        flattened = yaml_list(path, key)
        if flattened:
            lines.append(f"  {key}: [{flattened}]")

    return lines


def write_index(index_name, config):
    settings = config["indexes"].get(index_name)
    if settings is None:
        known = ", ".join(config["indexes"])
        sys.exit(f"Unknown index '{index_name}'. Use {known}.")

    directory = ROOT / ".agents" / index_name
    if not directory.is_dir():
        sys.exit(f".agents/{index_name} does not exist.")

    files = markdown_files(directory)
    out = [
        "---",
        f"description: Manifest of every file under .agents/{index_name}, "
        "grouped by area. Regenerate with just agents all.",
        "---",
        "",
        f"# {settings['heading']}",
        "",
        settings["blurb"],
    ]

    for group in groups_in_order(files):
        out += ["", f"## {config['group_headings'].get(group, group[:1].upper() + group[1:])}", "", "```yaml"]
        for relative_path in files:
            if group_of(relative_path) == group:
                out += entry(directory, relative_path, index_name, config)
        out.append("```")

    index_file = directory / "_index.md"
    index_file.write_text("\n".join(out) + "\n", encoding="utf-8")
    print(f"Regenerated .agents/{index_name}/_index.md")


def main():
    config = tomllib.loads(CONFIG.read_text(encoding="utf-8"))
    names = sys.argv[1:] or list(config["indexes"])
    for name in names:
        write_index(name, config)


if __name__ == "__main__":
    main()
