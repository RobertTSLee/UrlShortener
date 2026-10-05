# PostgreSQL Local Setup

## Option 1: Using Named Volume (Recommended for Local Testing)

Creates a separate Docker-managed volume for database data:

```powershell
docker run -d `
  --name postgres16 `
  -e POSTGRES_DB=urlshortener `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_PASSWORD=postgres `
  -p 5432:5432 `
  -v postgres_urlshortener_data:/var/lib/postgresql/data `
  postgres:16
```

**Verify running:**
```powershell
docker ps -a | findstr postgres16
```

**Stop container:**
```powershell
docker stop postgres16
```

**Start container:**
```powershell
docker start postgres16
```

**Remove container and volume:**
```powershell
docker stop postgres16
docker rm postgres16
docker volume rm postgres_urlshortener_data
```

---

## Option 2: Using Bind Mount (If using existing folder)

To use an existing local folder (e.g., `D:\postgres-data`):

```powershell
# Create folder if it doesn't exist
New-Item -ItemType Directory -Path D:\postgres-data -Force

# Run with bind mount
docker run -d `
  --name postgres16 `
  -e POSTGRES_DB=urlshortener `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_PASSWORD=postgres `
  -p 5432:5432 `
  -v D:\postgres-data:/var/lib/postgresql/data `
  postgres:16
```

---

## Connection String for .NET Application

For User Secrets:
```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=5432;Database=urlshortener;User Id=postgres;Password=postgres;"
```

---

## Quick Test Connection

Using psql (if installed):
```powershell
psql -h localhost -U postgres -d urlshortener -c "SELECT version();"
```

Or using Docker:
```powershell
docker exec -it postgres16 psql -U postgres -d urlshortener -c "SELECT version();"
```


## Redis Local Setup

Start Redis local, auto pull latest image
```powershell
docker run --name redis-local -p 6379:6379 -d redis
```

Verify connection
```powershell
docker exec -it redis-local redis-cli ping
```