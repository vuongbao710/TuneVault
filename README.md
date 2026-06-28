# TuneVault

Media streaming web app (Spotify-like UI) built with **ASP.NET Core 8** (Clean Architecture) and **React + TypeScript + Vite + Tailwind**.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- **SQL Server LocalDB** (installed with Visual Studio or SQL Server Express)

## Project structure

```
TuneVault/
├── backend/                 # ASP.NET Core Web API (4 layers)
├── frontend/tunevault-web/  # React SPA
├── docs/                    # ERD + pipeline diagrams
└── TuneVault.http           # REST Client demo requests
```

## Quick start

### 1. Backend

```powershell
cd C:\Users\truon\Desktop\TuneVault
dotnet restore TuneVault.sln
dotnet run --project backend\TuneVault.API
```

- API: http://localhost:5080
- Swagger: http://localhost:5080/swagger
- Database is created automatically on first run (`EnsureCreated` + seed)

### 2. Frontend

```powershell
cd frontend\tunevault-web
npm install
npm run dev
```

- UI: http://localhost:5173

Optional `.env`:

```
VITE_API_URL=http://localhost:5080
```

## Seed accounts

| Email | Password | Display name |
|-------|----------|--------------|
| alice@demo.com | Demo@123 | Alice Nguyen |
| bob@demo.com | Demo@123 | Bob Tran |

Seed includes **10 media items** (8 audio + 2 video) and **2 playlists**.

Copy **User id** from Profile page to share media between accounts.

## Demo flow (Auth → Upload → Share → Notify)

1. Login as `alice@demo.com`
2. Upload a track in **Library**
3. Open **Profile** on Bob's account, copy his user id
4. Login as Alice → **Shares** → select track → paste Bob's user id → Send
5. Login as Bob → **Notifications** (real-time badge via SignalR)

## EF Core migrations (for submission)

The app uses `EnsureCreated` for easy local setup. For assignment submission, generate 2 migrations:

```powershell
dotnet ef migrations add InitialCreate --project backend\TuneVault.Infrastructure --startup-project backend\TuneVault.API --output-dir Persistence\Migrations
dotnet ef migrations add AddIndexes --project backend\TuneVault.Infrastructure --startup-project backend\TuneVault.API --output-dir Persistence\Migrations
dotnet ef database update --project backend\TuneVault.Infrastructure --startup-project backend\TuneVault.API
```

Then replace `EnsureCreatedAsync()` with `MigrateAsync()` in `DbSeeder.cs`.

## API overview (22 endpoints)

- Auth: register, login
- Profile: get, update
- Media: list, get, upload, delete, stream (Range supported)
- Playlists: CRUD + add/remove tracks
- Search, Shares, Notifications, Favorites, History

## Security note

JWT is stored in `localStorage` for simplicity in this demo. For production, prefer httpOnly cookies.

## Architecture

- **Domain**: entities, enums
- **Application**: MediatR handlers, FluentValidation, DTOs
- **Infrastructure**: EF Core, Identity, JWT, file storage, SignalR
- **API**: controllers (MediatR only), Swagger, CORS

See [docs/ERD.md](docs/ERD.md) and [docs/PIPELINE.md](docs/PIPELINE.md).
