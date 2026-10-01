## How User Secrets are Added to Connection String at Boot

### Automatic Configuration Loading Process

When the application boots using `WebApplication.CreateBuilder(args)`, the configuration system automatically loads configuration from multiple sources in a specific order:

```
1. appsettings.json (base configuration)
   ↓
2. appsettings.{Environment}.json (environment-specific overrides)
   ↓
3. User Secrets (development environment only)
   ↓
4. Environment Variables
   ↓
5. Command-line arguments
```

**Key Point:** Values loaded later override values from earlier sources. This is called the **Configuration Hierarchy**.

### Step-by-Step: How It Works in Your Application

#### 1. **appsettings.json (Initial Load)**
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=UrlShortener;User Id={userid};Password={password};"
  }
}
```
Status: Contains placeholder values, **not yet usable**

#### 2. **User Secrets Override (Development Only)**
```
Secret Key: ConnectionStrings:DefaultConnection
Secret Value: Server=localhost;Database=UrlShortener;User Id=postgres;Password=your_password;
```
This replaces the entire placeholder string with the actual connection string.

#### 3. **Program.cs Retrieves the Merged Value**
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Result: "Server=localhost;Database=UrlShortener;User Id=postgres;Password=your_password;"
```

### Configuration Loading Code in WebApplicationBuilder

When you call `WebApplication.CreateBuilder(args)`, it internally calls this configuration setup:

```csharp
var builder = WebApplication.CreateBuilder(args);

// The builder.Configuration property is automatically configured with:
builder.Configuration
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
	.AddUserSecrets<Program>()  // ← Automatically added in Development
	.AddEnvironmentVariables()
	.AddCommandLine(args);
```

**Note:** `.AddUserSecrets<Program>()` is automatically called when:
- Environment is **Development** (default in debug mode)
- The project has a UserSecretsId in the .csproj file

### Your Current Implementation

Your `Program.cs` already handles this correctly:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<UrlShortenerContext>(options =>
	options.UseNpgsql(connectionString));
```

**What happens:**
1. `builder.Configuration` automatically has User Secrets merged in
2. `.GetConnectionString("DefaultConnection")` retrieves the merged value from User Secrets (not the placeholder)
3. This connection string is passed to the DbContext

### Environment-Based Behavior

| Environment | User Secrets Loaded? | Configuration Source |
|---|---|---|
| **Development** | ✅ Yes | appsettings.json + appsettings.Development.json + User Secrets + Env Vars |
| **Production** | ❌ No | appsettings.json + appsettings.Production.json + Env Vars + Azure Key Vault |
| **Testing** | ❌ No | appsettings.json + appsettings.Testing.json + Env Vars |

### How to Verify Secrets Are Loaded

Add this temporary code to see what's being loaded:

```csharp
// Debug: Print the merged configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Loaded Connection String: {connectionString}");
// Output: Loaded Connection String: Server=localhost;Database=UrlShortener;User Id=postgres;Password=your_password;
```

### Checking Your UserSecretsId

Your `.csproj` file has been updated with:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <UserSecretsId>2652fcb6-abcd-44a8-b43e-4ac4d25f3ec8</UserSecretsId>
</PropertyGroup>
```

This ID connects the project to your local User Secrets store.

### Secrets File Location

User Secrets are stored in:

**Windows:**
```
%APPDATA%\Microsoft\UserSecrets\2652fcb6-abcd-44a8-b43e-4ac4d25f3ec8\secrets.json
```

**Linux/Mac:**
```
~/.microsoft/usersecrets/2652fcb6-abcd-44a8-b43e-4ac4d25f3ec8/secrets.json
```

You can view the secrets file directly:
```powershell
cat "$env:APPDATA\Microsoft\UserSecrets\2652fcb6-abcd-44a8-b43e-4ac4d25f3ec8\secrets.json"
```

### Complete Flow Diagram

```
┌─────────────────────────────────────┐
│     WebApplication.CreateBuilder    │
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│   Load appsettings.json             │
│   (Placeholders: {userid}, {password})|
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│   Load appsettings.Development.json │
│   (Overrides base config)           │
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│   Load User Secrets (Dev only)      │
│   ConnectionStrings:DefaultConnection│
│   ✅ REPLACES the placeholder!      │
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│   Load Environment Variables        │
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│  builder.Configuration              │
│  (Merged configuration)             │
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│ GetConnectionString("DefaultConnection")
│ Returns the merged value with       │
│ actual credentials                  │
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│  Pass to DbContext options          │
│  options.UseNpgsql(connectionString)│
└──────────────┬──────────────────────┘
			   │
			   ▼
┌─────────────────────────────────────┐
│  ✅ Connected to PostgreSQL!        │
└─────────────────────────────────────┘
```

### What Happens in Production?

In production, User Secrets are **NOT** loaded. Instead, use one of these approaches:

**1. Environment Variables:**
```powershell
# Set environment variable
$env:ConnectionStrings__DefaultConnection = "Server=prod-server;Database=UrlShortener;User Id=produser;Password=prodpass;"
```

**2. Azure Key Vault (Recommended):**
```csharp
var builder = WebApplication.CreateBuilder(args);
var keyVaultEndpoint = new Uri("https://your-keyvault.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(
	keyVaultEndpoint,
	new DefaultAzureCredential());
```

**3. appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=prod-server;..."
  }
}
```
(Note: Still not recommended for storing actual passwords)

### Summary

✅ User Secrets are **automatically** merged into `builder.Configuration`
✅ They override placeholder values from `appsettings.json`
✅ Your current code in `Program.cs` is correct and needs no changes
✅ Secrets are only loaded in **Development** environment
✅ In **Production**, use environment variables or Azure Key Vault

The magic happens in the Configuration hierarchy - you don't need to do anything special! 🎯

---

**Last Updated:** Explains configuration loading and User Secrets integration
