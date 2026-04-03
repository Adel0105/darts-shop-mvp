# Darts Shop MVP

A demo e-commerce web application for browsing products, managing a shopping cart, placing orders, and an admin area (JWT) for stock and order status. Intended for learning and portfolio use; not a production payment integration.

## Tech stack

| Layer    | Technologies |
| -------- | ------------ |
| Frontend | Angular 21, TypeScript, RxJS, reactive forms, HTTP client |
| Backend  | ASP.NET Core 9, Minimal APIs, JWT Bearer, EF Core 9 |
| Database | Microsoft SQL Server |
| Other    | CORS (development), EF Core migrations, seeded data, static assets in `frontend/public` |

## Repository layout

```
darts-shop-mvp/
├── backend/          # ASP.NET Core API, EF Core, SQL Server
├── frontend/         # Angular app (public assets in frontend/public)
└── README.md
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS) and npm
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, Docker, or full instance)

## Database setup

1. Ensure SQL Server is running and reachable.
2. Set `ConnectionStrings:DefaultConnection` in `backend/appsettings.json`, or override with [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables (recommended for secrets).
3. Apply migrations from the `backend` directory:

```bash
cd backend
dotnet ef database update
```

**Security:** do not commit production credentials or JWT signing keys to a public repository.

## Run the API (backend)

```bash
cd backend
dotnet restore
dotnet run
```

- Base URL (HTTP profile): `http://localhost:5231`
- Health check: `GET http://localhost:5231/api/health`

## Run the web app (frontend)

```bash
cd frontend
npm install
npm start
```

- Application URL: `http://localhost:4200`

The Angular app calls the API using the base URL configured in `frontend/src/app/core` (default `http://localhost:5231`). Update both the backend port and the frontend URL if you change either.

## Features

### Storefront

- Product catalog with filters (category, price range, search, sort)
- Product detail page
- Shopping cart with browser persistence
- Checkout form and order submission

### Admin (JWT)

- Login at `/login`; protected route `/admin`
- View orders and update status
- Update product stock

### API

- REST endpoints for products, categories, and orders
- JWT issuance at `POST /api/auth/login`
- Admin routes protected with `[Authorize(Roles = "Admin")]`

## Default admin credentials (MVP / local dev only)

Hard-coded for local development in `backend/Program.cs`. **Change or remove before any real deployment.**

## License

This project is provided as-is for portfolio and educational purposes. Add a license file if you redistribute the code.
