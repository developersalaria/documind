# Contributing to DocuMind

Thanks for your interest! Bug reports, ideas and pull requests are all welcome.

## Getting set up

Follow **Getting started** in the [README](README.md). Before you open a pull request, make sure these pass:

```bash
dotnet build
dotnet test
cd web && npm run lint && npm run build
```

## Pull requests

- For larger changes, open an issue first so we can agree on the approach.
- Keep each pull request focused on one change.
- Add or update tests when behavior changes.
- Write clear commit messages, ideally using [Conventional Commits](https://www.conventionalcommits.org/), for example `feat: add PDF extractor` or `fix: handle empty files`.

## Code style

- The repository has an `.editorconfig`; most IDEs apply it automatically.
- The build treats warnings as errors, so fix warnings before you push.
