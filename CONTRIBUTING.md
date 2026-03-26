# Contributing

Thank you for your interest in contributing.

## How To Contribute

1. Fork the repository and create a branch from `develop`.
2. Follow the commit format in `COMMIT_MESSAGE_GUIDELINES.md`.
3. Keep changes focused and easy to review.
4. Add or update tests when behavior changes.
5. Open a pull request using the provided template.

## Branch Naming

Use one of the following prefixes:

- `feature/<short-description>`
- `bugfix/<short-description>`
- `chore/<short-description>`
- `docs/<short-description>`

## Local Validation

Before opening a PR, run:

```bash
dotnet --version
dotnet build
```

If your change includes tests, also run:

```bash
dotnet test
```

## Pull Request Expectations

- Clear title and summary
- Linked issue (if available)
- Notes about testing performed
- No unrelated file changes
