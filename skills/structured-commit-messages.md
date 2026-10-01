# Structured Commit Message Skill

## Purpose

This skill ensures that all git commit messages in the UrlShortener project follow a consistent, structured format that is:
- Easy to read and understand
- Machine-readable for automated tooling (changelog generation, version bumping)
- Professional and maintainable for team collaboration
- Clear about what changed and why

## Commit Message Format

All commits must follow the **Conventional Commits** specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type

Required. Describes the nature of the change. Must be one of:

| Type | Description | Examples |
|------|-------------|----------|
| **feat** | A new feature | `feat(auth): add user authentication` |
| **fix** | A bug fix | `fix(api): resolve null reference in endpoint` |
| **docs** | Documentation changes | `docs: update README with setup instructions` |
| **style** | Code style changes (formatting, semicolons, etc.) | `style: format code with prettier` |
| **refactor** | Code refactoring without feature/fix | `refactor(service): simplify URL generation logic` |
| **perf** | Performance improvements | `perf(db): optimize query with indexing` |
| **test** | Adding or updating tests | `test(api): add validation tests for endpoint` |
| **chore** | Build, dependencies, or tooling changes | `chore: update dependencies to latest version` |
| **ci** | CI/CD pipeline changes | `ci: add GitHub Actions workflow` |
| **revert** | Reverting a previous commit | `revert: undo auth implementation` |

### Scope

Optional. Specifies which part of the codebase is affected. Use lowercase.

**Valid Scopes for UrlShortener:**
- `api` - API endpoints and controllers
- `db` - Database and migrations
- `auth` - Authentication and authorization
- `service` - Business logic services
- `repo` - Repository/data access layer
- `model` - Entity models
- `config` - Configuration and settings
- `test` - Test files and infrastructure
- `doc` - Documentation

### Subject

Required. Concise description of the change.

**Rules:**
- Use imperative mood ("add" not "added" or "adds")
- Don't capitalize first letter
- No period (.) at the end
- Maximum 50 characters
- Clear and descriptive

**Good Examples:**
- `add user authentication service`
- `fix null reference in url shortening`
- `refactor database configuration`

**Bad Examples:**
- ❌ `Added user authentication` (not imperative)
- ❌ `Fix bug in shortening logic.` (has period)
- ❌ `implemented feature that adds url shortening and caching with optimizations` (too long)

### Body

Optional. Detailed explanation of the changes.

**Rules:**
- Wrap at 72 characters
- Explain *what* and *why*, not *how* (code shows the how)
- Separate from subject with blank line
- Use bullet points or paragraphs as appropriate

**Example:**
```
feat(service): add rate limiting for URL shortening

- Implement token bucket algorithm
- Allow 100 requests per minute per user
- Return 429 Too Many Requests when limit exceeded
- Useful for preventing abuse and DDoS attacks
```

### Footer

Optional. Reference issues, breaking changes, or other metadata.

**Breaking Changes:**
```
BREAKING CHANGE: Configuration format changed from appsettings.json to .env
Migration guide available in UPGRADE.md
```

**Close Issues:**
```
Closes #123
Fixes #456
Resolves #789
```

**References:**
```
Refs #999
Related to #888
```

## Complete Examples

### Example 1: New Feature with Body
```
feat(api): add endpoint for generating custom short codes

Add POST /api/urls/custom endpoint that allows users to specify
their own short code for a URL. Validates code uniqueness and
enforces alphanumeric characters only (3-10 chars).

Closes #45
```

### Example 2: Bug Fix
```
fix(db): resolve connection timeout in migration

Increase default timeout from 30s to 60s for migrations
when database is under load. Affects InitialCreate migration.
```

### Example 3: Simple Commit
```
docs: update database schema diagram in README
```

### Example 4: Breaking Change
```
refactor(config)!: migrate from appsettings to environment variables

BREAKING CHANGE: Configuration now uses environment variables exclusively.
See UPGRADE.md for migration steps.

- Remove appsettings.json from production deployments
- Update docker-compose to set env vars
- All connection strings must be ENV:* format
```

### Example 5: Chore with Dependencies
```
chore: update entity framework to 10.0.1

- Update Microsoft.EntityFrameworkCore to 10.0.1
- Update Npgsql.EntityFrameworkCore.PostgreSQL to 10.0.1
- No breaking changes in this update
```

## Best Practices

### Do's ✅

- ✅ Use imperative mood ("add feature", "fix bug", "update docs")
- ✅ Be specific about what changed
- ✅ Include relevant issue numbers in footer
- ✅ Write clear, descriptive subjects
- ✅ Use scopes to organize changes by component
- ✅ Keep subject line to 50 characters max
- ✅ Explain the "why", not just the "what"
- ✅ Reference related commits when appropriate

### Don'ts ❌

- ❌ Don't use generic subjects ("update code", "fix stuff", "changes")
- ❌ Don't mix multiple concerns in one commit
- ❌ Don't capitalize the subject
- ❌ Don't use periods in subject line
- ❌ Don't use past tense ("added", "fixed")
- ❌ Don't commit large unrelated changes together
- ❌ Don't leave subjects ambiguous
- ❌ Don't forget to reference closing issues

## Commit Message Checklist

Before committing, verify:

- [ ] Type is one of: feat, fix, docs, style, refactor, perf, test, chore, ci, revert
- [ ] Scope (if used) is relevant and lowercase
- [ ] Subject uses imperative mood
- [ ] Subject is 50 characters or less
- [ ] Subject starts with lowercase (except proper nouns)
- [ ] Subject has no period
- [ ] Body wraps at 72 characters (if included)
- [ ] Breaking changes marked with `BREAKING CHANGE:` footer
- [ ] Related issues referenced with `Closes`, `Fixes`, or `Refs`
- [ ] Only related changes are included (no "while I was here" changes)

## Git Commands

### Commit with Message
```powershell
git commit -m "feat(api): add user authentication endpoint"
```

### Commit with Body
```powershell
git commit -m "feat(api): add user authentication endpoint" -m "Implement JWT token-based authentication with refresh tokens. Validates credentials against user table."
```

### Commit with Interactive Editor
```powershell
git commit
# Opens default editor for multi-line message
```

### Amend Last Commit
```powershell
git commit --amend -m "fix(api): resolve authentication issue"
```

## Commit Template (Optional Setup)

To enforce structured messages locally, create a `.gitmessage` template file:

**.gitmessage**
```
# <type>(<scope>): <subject>
#
# <body>
#
# <footer>

# Type must be one of the following:
# feat     - A new feature
# fix      - A bug fix
# docs     - Documentation only changes
# style    - Changes that do not affect code meaning (formatting)
# refactor - Code change that neither fixes a bug nor adds a feature
# perf     - Code change that improves performance
# test     - Adding missing tests or correcting existing tests
# chore    - Changes to build process, dependencies, or tooling
# ci       - Changes to CI configuration files and scripts
# revert   - Revert a previous commit
#
# Scope: Specify which part of the code is affected (e.g., api, db, auth, service)
#
# Subject: Use imperative mood, don't capitalize, no period at the end, max 50 chars
#
# Body: Explain what and why, not how. Wrap at 72 characters.
#
# Footer: Reference issues (Closes #123) and breaking changes (BREAKING CHANGE: ...)
```

Configure git to use this template:
```powershell
git config commit.template .gitmessage
```

## Commit Frequency Guidelines

- **Commit frequently** - Multiple small commits are better than one large commit
- **One logical change per commit** - Don't mix unrelated changes
- **Runnable state** - Ideally each commit should leave code in a working state
- **Clean history** - Use `git rebase` to organize commits before pushing (if needed)

## Changelog Generation

With structured commit messages, automated tools can generate changelogs:

### Using commitizen-cli
```powershell
npm install -g commitizen
cz init cz:conventional:commitizen
cz commit
```

### Using semantic-release
```powershell
npm install --save-dev semantic-release
# Automatically bumps version and generates changelog based on commits
```

### Using standard-changelog
```powershell
npm install --save-dev standard-changelog
npx standard-changelog --outfile CHANGELOG.md
```

## Team Guidelines

1. **Enforce this skill** - Make structured commits a team standard
2. **Code review commits** - Check message quality during PR review
3. **Use branch protection** - Require descriptive messages in PRs
4. **Document exceptions** - If a commit doesn't fit the format, discuss and update this skill
5. **Lead by example** - Use structured messages in all commits
6. **Educate new team members** - Share this skill during onboarding

## Integration with CI/CD

This structured format enables:
- ✅ **Automatic changelog generation** from commit history
- ✅ **Semantic versioning** based on commit types (feat → minor, fix → patch)
- ✅ **Automated release notes** for releases
- ✅ **Commit validation** in pre-commit hooks
- ✅ **Better code review** with clear commit intent

---

**Last Updated:** Applied to UrlShortener project
**Standard:** Conventional Commits (https://www.conventionalcommits.org/)
**Git Repository:** https://github.com/RobertTSLee/UrlShortener
