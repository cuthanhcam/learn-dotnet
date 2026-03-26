# Git Usage Guidelines

This document provides a comprehensive guide on how to use Git for source code management within the project.
The goal is to ensure that all team members follow a **consistent workflow**, minimize errors, and collaborate efficiently.

The guide covers:

* Git installation and configuration
* Common Git commands and options
* Branching strategy and workflows
* Best practices for collaboration and conflict resolution

---

## 1. Git Installation

### Install Git

Download and install Git from the official website:
[https://git-scm.com/](https://git-scm.com/)

### Configure User Information

```bash
git config --global user.name "Your Name"
git config --global user.email "email@example.com"
```

#### Common Options

* `--global`: Apply configuration to all repositories for the current user
* `--local`: Apply configuration only to the current repository (default if `--global` is not specified)

Example:

```bash
git config --local user.name "Different Name"
```

This sets a repository-specific username.

---

## 2. Repository Initialization

### Initialize a New Repository

```bash
git init
```

#### Options

* `--bare`: Create a bare repository (no working directory), commonly used on servers

Example:

```bash
git init --bare
```

### Clone an Existing Repository

```bash
git clone https://github.com/username/repository.git
```

#### Options

* `--branch <branch-name>`: Clone a specific branch
* `--depth <number>`: Perform a shallow clone with limited commit history

Example:

```bash
git clone --branch develop --depth 1 https://github.com/username/repository.git
```

---

## 3. Branching Workflow

This project uses a **simplified Git Flow** model.

### Branch Types

* **main**: Stable, production-ready code
* **develop**: Integration branch for ongoing development
* **feature/***: Feature development (e.g. `feature/login-page`)
* **bugfix/***: Bug fixes (e.g. `bugfix/fix-login-error`)

---

### Creating and Working with Branches

#### Create a New Branch

```bash
git checkout -b feature/feature-name
```

Options:

* `-b`: Create and switch to the new branch immediately

Example:

```bash
git checkout -b feature/add-user-auth
```

#### Push a Branch to Remote

```bash
git push origin feature/add-user-auth
```

Options:

* `--set-upstream` (or `-u`): Link the local branch with the remote branch

Example:

```bash
git push --set-upstream origin feature/add-user-auth
```

---

### Merging Branches

1. Switch to the `develop` branch:

   ```bash
   git checkout develop
   ```

2. Pull the latest changes:

   ```bash
   git pull origin develop
   ```

   Options:

   * `--rebase`: Reapply local commits on top of remote history

   Example:

   ```bash
   git pull --rebase origin develop
   ```

3. Merge the feature branch:

   ```bash
   git merge feature/add-user-auth
   ```

   Options:

   * `--no-ff`: Always create a merge commit (recommended for clarity)
   * `--squash`: Combine all commits into a single commit

   Example:

   ```bash
   git merge --no-ff feature/add-user-auth
   ```

---

## 4. Common Git Commands and Options

### Check Repository Status

```bash
git status
```

Options:

* `-s`, `--short`: Short status output
* `-b`, `--branch`: Show current branch information

Example:

```bash
git status -s
```

---

### Stage Files

```bash
git add file_name
```

Options:

* `.`: Stage all changes
* `-u`: Stage modified and deleted files only
* `-A`, `--all`: Stage all changes (including new files)

Example:

```bash
git add -A
```

---

### Create a Commit

```bash
git commit -m "commit message following COMMIT_CONVENTION.md"
```

Options:

* `-m`: Specify commit message
* `-a`: Automatically stage tracked files
* `--amend`: Modify the previous commit

Example:

```bash
git commit -a -m "fix(auth): update login validation"
```

---

### Push Changes to Remote

```bash
git push origin branch-name
```

Options:

* `-f`, `--force`: Force push (use with extreme caution)
* `--force-with-lease`: Safer force push with remote checks

Example:

```bash
git push --force-with-lease origin feature/add-user-auth
```

---

### Pull Changes from Remote

```bash
git pull origin branch-name
```

Options:

* `--rebase`: Rebase local commits on top of remote changes
* `--no-rebase`: Use merge instead of rebase

Example:

```bash
git pull --rebase origin develop
```

---

## 5. Handling Merge Conflicts

When a merge conflict occurs, Git pauses the process and marks conflicting sections:

```text
<<<<<<< HEAD
Your changes
=======
Changes from the other branch
>>>>>>> branch-name
```

### Steps to Resolve

1. Open the conflicted file and decide which code to keep
2. Remove all conflict markers (`<<<<<<<`, `=======`, `>>>>>>>`)
3. Mark the conflict as resolved:

   ```bash
   git add file_name
   ```
4. Complete the merge:

   ```bash
   git commit
   ```

Option:

* `--no-edit`: Use the default merge commit message

Example:

```bash
git commit --no-edit
```

---

## 6. Additional Useful Commands

### View Commit History

```bash
git log --oneline --graph
```

Options:

* `--author=<name>`: Filter by author
* `--since=<date>`: Filter by date

Example:

```bash
git log --oneline --author="Your Name" --since="2025-04-01"
```

---

### Discard Uncommitted Changes

```bash
git restore file_name
```

Option:

* `--staged`: Remove file from staging but keep changes

Example:

```bash
git restore --staged file_name
```

---

### Revert a Commit

```bash
git revert commit_hash
```

Option:

* `-n`, `--no-commit`: Apply changes without committing immediately

Example:

```bash
git revert -n abc123
```

---

### Delete Branches

```bash
git branch -d branch-name
git push origin --delete branch-name
```

Options:

* `-d`: Delete only if already merged
* `-D`: Force delete (use with caution)
* `--delete`: Delete remote branch

Example:

```bash
git branch -D feature/old-branch
```

---

## 7. Best Practices

* Always pull the latest changes before starting work
* Create a dedicated branch for each feature or bug fix
* Write clear commit messages following `COMMIT_CONVENTION.md`
* Push changes frequently to avoid data loss
* Review code carefully before merging into `develop` or `main`
* Use Git options wisely to avoid unintended history changes

---

## 8. References

* **Pro Git Book**: [https://git-scm.com/book/en/v2](https://git-scm.com/book/en/v2)
* **Official Git Documentation**: [https://git-scm.com/docs](https://git-scm.com/docs)
* **Atlassian Git Tutorials**: [https://www.atlassian.com/git](https://www.atlassian.com/git)

---

If you need clarification on any Git command or workflow, please contact the repository maintainers.