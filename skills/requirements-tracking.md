# Requirements Tracking Skill

## Purpose

This skill ensures that the README.md file stays synchronized with implemented features in the codebase. When new features, services, or components are added to the application, they must be logged and documented in the README.md file.

## Current Project Requirements (URL Shortener)

### Core Functionality
- ✅ Receive requests via HTTP POST
- ✅ Accept long URL in request body
- ✅ Return shortened URL in response
- ✅ Ensure unique shortened URLs
- ✅ Redirect shortened URL to original long URL
- ✅ Store URL mappings in database
- ✅ Handle concurrent requests
- ✅ Implement async/await concurrency handling
- ✅ Bulk insert optimization (max 1000 records per insert)

### Technical Stack
- .NET 10.0
- Entity Framework Core
- SQL Server
- ASP.NET Core Web API

## When to Update README.md

Update the README.md file whenever:

1. **New Feature Implementation**
   - Example: "Added rate limiting for shortened URL creation"
   - Add to appropriate section with status indicator

2. **New Service/Component Added**
   - Example: "Implemented IUrlShorteningService for core business logic"
   - Document in Features or Architecture section

3. **New API Endpoint**
   - Example: "POST /api/urls/shorten - Creates shortened URL"
   - Add with parameters and response format

4. **New Database Entity/Model**
   - Example: "Added UrlMapping entity to track long-to-short URL associations"
   - Document schema or key properties

5. **New Dependencies Added**
   - Example: "Added Redis for caching shortened URLs"
   - Update Technical Stack section

6. **New Configuration/Settings**
   - Example: "Database connection string configuration"
   - Document in Configuration section

7. **Architecture or Design Pattern Changes**
   - Example: "Implemented Repository pattern for data access"
   - Add Architecture section if not present

## README.md Structure Template

Maintain this structure:

```markdown
# URL Shortener

## Overview
Brief description of what the application does.

## Features

### Core Features
- Feature 1 (description)
- Feature 2 (description)
- Feature 3 (description)

### API Endpoints
- `POST /api/urls/shorten` - Creates a shortened URL
- `GET /api/urls/{shortCode}` - Redirects to original URL
- Additional endpoints...

## Technical Scope
- Framework: .NET 10.0
- ORM: Entity Framework Core
- Database: SQL Server
- API: ASP.NET Core Web API
- Concurrency: async/await patterns
- Performance: Bulk insert optimization (max 1000 records)

## Architecture

### Services
- List of implemented services

### Data Models
- URL Mapping entity
- Additional entities...

### Design Patterns
- Repository pattern (if implemented)
- Dependency Injection
- Other patterns...

## Database Schema

### Tables
- UrlMappings table
  - ShortCode (PK)
  - LongUrl
  - CreatedAt
  - ExpirationDate (if applicable)
  - Additional columns...

## Installation & Setup
Instructions for running the application locally.

## Usage
Examples of how to use the API.
```

## Checklist for Code Generation

When generating new features, ensure:

- ✅ Code is properly documented with XML comments (see comment-standards.md skill)
- ✅ New feature is described in README.md
- ✅ API endpoints are listed in README.md
- ✅ New services are listed in Architecture section
- ✅ Database changes are documented in Database Schema section
- ✅ Updated README.md includes modification timestamp or feature version
- ✅ Breaking changes are highlighted in README.md

## Update Log Format

When updating README.md, add an "Updates" or "Change Log" section:

```markdown
## Recent Updates

### [Version/Date]
- Added feature: Brief description
- Modified: Component name - What changed
- Fixed: Bug description
- New Endpoint: Description

### [Previous Version/Date]
- Previous updates...
```

## Enforcement Strategy

1. **After implementing any new feature**: Review README.md and add corresponding entry
2. **Before committing code**: Verify README.md reflects all new additions
3. **During code review**: Ensure README.md was updated alongside code changes
4. **Keep single source of truth**: README.md should match implemented features

---

**Last Updated**: Applied to UrlShortener project
**Skill Integration**: Works alongside comment-standards.md to ensure complete documentation
