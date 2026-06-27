# Release steps

This project uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) for automatic versioning and publishes to NuGet via CI on every push to `main`. There is no manual tagging or version-bumping required for a normal release.

## Normal release (patch/build increment)

1. **Update `CHANGELOG.md`** — move items from `[Unreleased]` into a new versioned section. You won't know the exact version number until after the merge, but you can look it up from the CI build or use `nbgv get-version` locally beforehand.

2. **Merge to `main`** — a fast-forward merge is preferred so the version that was tested on the source branch is identical to what gets published:

   ```bash
   git checkout main
   git merge --ff-only develop
   git push
   ```

   If a fast-forward is not possible, a regular merge commit is fine — it will just increment the version height by one.

3. **CI publishes automatically** — the `continuous` workflow runs on push to `main`, builds the package, and pushes it to NuGet.org using the `PUBLIC_NUGET_API_KEY` secret. No manual action needed.

4. **Create a GitHub release** (optional but recommended) — after the CI build completes, create a GitHub release tagged with the version number for visibility.

## Bumping the major or minor version

To release a new major or minor version (e.g., `2.1` or `3.0`), update `version.json` before merging:

```json
{
  "version": "2.1-beta",
  "publicReleaseRefSpec": [
    "^refs/heads/main$",
    "^refs/heads/v\\d+(?:\\.\\d+)?$"
  ]
}
```

Commit this change on `develop` (or a feature branch), then follow the normal release steps above.

## How versioning works

| Branch | Version format | Example |
|--------|---------------|---------|
| `main` | `{major}.{minor}.{height}` | `2.0.42` |
| `develop` | `{major}.{minor}.{height}-beta` | `2.0.42-beta` |
| Feature branch | `{major}.{minor}.{height}-beta+{commitId}` | `2.0.42-beta+abc1234` |

The **height** is the number of commits since `version.json` was last changed on that branch. Only `main` (and `v*` branches) produce public release versions — all other branches produce prerelease packages.

