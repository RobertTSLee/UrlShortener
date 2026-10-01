## User Secrets Configuration

This document describes how to manage sensitive configuration data for the UrlShortener application using .NET User Secrets.

### What are User Secrets?

User Secrets is a secure way to store sensitive configuration data (passwords, API keys, connection strings) outside of your source code. Secrets are stored locally on your machine and are not committed to version control.

### Current Configuration

The following sensitive data is stored in User Secrets:

- **ConnectionStrings:DefaultConnection** - PostgreSQL connection string with username and password

### Project UserSecretsId

```
2652fcb6-abcd-44a8-b43e-4ac4d25f3ec8
```

### How to Update User Secrets

#### 1. Set or Update a Secret

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=UrlShortener;User Id=postgres;Password=your_password;"
```

#### 2. List All Secrets

```powershell
dotnet user-secrets list
```

#### 3. Remove a Secret

```powershell
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

#### 4. Clear All Secrets

```powershell
dotnet user-secrets clear
```

### PostgreSQL Connection String Format

```
Server=<host>;Port=<port>;Database=<database_name>;User Id=<username>;Password=<password>;
```

**Example:**
```
Server=localhost;Port=5432;Database=UrlShortener;User Id=postgres;Password=your_password;
```

### Using User Secrets with appsettings.json

The `appsettings.json` file contains a template connection string with placeholders:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=UrlShortener;User Id={userid};Password={password};"
  }
}
```

**Note:** When you set a user secret with the same key as `appsettings.json`, the secret value will override the template during runtime.

### Environment-Specific Configuration

For different environments, you can use environment-specific appsettings files:

- `appsettings.Development.json` - Development environment
- `appsettings.Production.json` - Production environment
- `appsettings.Staging.json` - Staging environment

User secrets are automatically loaded and will override values from these files.

### Security Best Practices

✅ **Do:**
- Store sensitive credentials in User Secrets
- Use environment variables or User Secrets in production
- Rotate passwords regularly
- Use different credentials for different environments

❌ **Don't:**
- Commit secrets to version control
- Hard-code passwords in configuration files
- Share secrets across team members
- Use the same credentials for all environments

### Team Collaboration

When sharing the project with team members:

1. **Don't commit User Secrets** - They are stored locally on each developer's machine
2. **Document the required secrets** - Provide a list of required secret keys
3. **Each team member must set their own secrets** using the commands listed above
4. **Use environment variables in production** - Never rely on User Secrets in production deployments

### Production Deployment

For production environments, use one of these approaches:

1. **Environment Variables:**
   ```powershell
   # Set environment variable
   $env:ConnectionStrings__DefaultConnection = "Server=prod-host;..."
   ```

2. **Azure Key Vault** - Recommended for cloud deployments
3. **Configuration Management Tools** - HashiCorp Vault, AWS Secrets Manager, etc.

---

**Last Updated:** Applied to UrlShortener project
**UserSecretsId:** 2652fcb6-abcd-44a8-b43e-4ac4d25f3ec8
