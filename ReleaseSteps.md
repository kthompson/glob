# Release steps

This project uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) for automatic versioning and publishes to NuGet via CI on every push to `main`. There is no manual tagging or version-bumping required for a normal release.

## Normal release (patch/build increment)

1. **Update `CHANGELOG.md`** — move items from `[Unreleased]` into a new versioned section. You can check the upcoming version with:

   ```bash
   dotnet tool install -g nbgv
   nbgv get-version -v NuGetPackageVersion
   ```

2. **Merge to `main`** — a fast-forward merge is preferred so the version that was tested on the source branch is identical to what gets published:

   ```bash
   git checkout main
   git merge --ff-only develop
   git push
   ```

   If a fast-forward is not possible, a regular merge commit is fine — it will just increment the version height by one.

3. **CI does the rest automatically** — the `continuous` workflow runs on push to `main` and:
   - Builds and runs tests
   - Publishes the NuGet package to nuget.org
   - Creates a GitHub release tagged with the NBGV version (e.g., `v2.0.42`), using the first section of `CHANGELOG.md` as the release notes
   - Attaches the `.nupkg` files as release assets

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

| Branch         | Version format                              | Example                  |
|----------------|---------------------------------------------|--------------------------|
| `main`         | `{major}.{minor}.{height}`                  | `2.0.42`                 |
| `develop`      | `{major}.{minor}.{height}-beta`             | `2.0.42-beta`            |
| Feature branch | `{major}.{minor}.{height}-beta+{commitId}`  | `2.0.42-beta+abc1234`    |

The **height** is the number of commits since `version.json` was last changed on that branch. Only `main` (and `v*` branches) produce public release versions — all other branches produce prerelease packages.

