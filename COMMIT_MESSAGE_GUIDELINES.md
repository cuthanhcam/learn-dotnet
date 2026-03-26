# Commit Message Guidelines

This document defines the rules and best practices for writing commit messages to ensure **consistency, clarity, and effective change tracking** across the project.
Well-written commit messages help the team understand *why* a change was made and improve long-term maintainability of the codebase.

---

## 1. Commit Message Structure

Each commit message consists of **three main parts**:

1. **Type** – the category of change
2. **Scope** – the affected module, feature, or area (optional but recommended)
3. **Description** – a concise summary of the change

### Syntax

```
type(scope): description
```

### 1.1 Type

The `type` must be one of the following:

* **feat**: Introduces a new feature
* **fix**: Fixes a bug
* **docs**: Documentation-only changes (README, guides, comments)
* **style**: Code style changes (formatting, spacing, linting; no logic changes)
* **refactor**: Code restructuring without changing behavior or fixing bugs
* **test**: Adding or modifying tests
* **chore**: Maintenance tasks (dependencies, configs, tooling, CI)

### 1.2 Scope

The `scope` describes **where** the change occurs.

Examples:

* `auth`
* `ui`
* `api`
* `database`
* `config`
* `deps`

> Use lowercase and keep it short and meaningful.

### 1.3 Description

* Use the **imperative present tense** (e.g., “add”, “fix”, “update”)
* Keep it **concise and specific**
* Prefer **under 50 characters**
* Do **not** end with a period

---

## 2. Commit Message Examples

### New feature

```
feat(auth): add user login functionality
```

### Bug fix

```
fix(ui): correct button alignment on mobile
```

### Documentation update

```
docs(readme): update installation instructions
```

### Code refactoring

```
refactor(api): simplify user validation logic
```

### Tests

```
test(auth): add unit tests for login endpoint
```

### Dependency update

```
chore(deps): update express to version 4.18.2
```

---

## 3. Additional Rules & Best Practices

### 3.1 Language

* All commit messages **must be written in English** for professionalism and global collaboration.

### 3.2 Be Explicit

Avoid vague messages such as:

* `fix bug`
* `update code`

Prefer:

* `fix(api): handle null user input`
* `refactor(ui): remove duplicated button styles`

### 3.3 Reference Issues or Tickets

If a commit is related to an issue, include the issue ID:

```
fix(ui): resolve button overlap issue #123
```

### 3.4 Formatting Rules

* Use a colon `:` after `type(scope)`
* Add exactly **one space** after the colon
* Use lowercase for `type` and `scope`

### 3.5 Commit Body (Optional but Recommended)

For complex changes, add a **commit body** to explain *what* and *why*:

```bash
git commit -m "feat(auth): add user login functionality

- Implement JWT-based authentication
- Add input validation for login form
- Update user model with authentication fields"
```

Guidelines for the body:

* Separate the subject and body with a blank line
* Wrap lines at ~72 characters
* Use bullet points for clarity

---

## 4. Why Follow These Guidelines?

* **Consistency**: Makes commit history easy to read and understand
* **Traceability**: Simplifies debugging and change tracking
* **Automation**: Enables tools like changelog generators and semantic releases
* **Collaboration**: Improves communication within and across teams

---

## 5. Recommended Tools

### Commitizen

Helps create standardized commit messages interactively:

```bash
npm install -g commitizen
cz
```

### Conventional Commits

Use Conventional Commits to enforce commit message standards and automate releases:

* [https://www.conventionalcommits.org/](https://www.conventionalcommits.org/)
* [https://commitizen-tools.github.io/commitizen/](https://commitizen-tools.github.io/commitizen/)

---

By following these commit message guidelines, you help maintain a **clean, readable, and scalable** project history.
Consistent commits today save time and confusion tomorrow 
