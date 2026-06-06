## Frontend Overview

This frontend allows a user to upload CSV files that are sent to the backend API and then processed by the forecasting pipeline.

## Projects

1. `ui` = Angular 6 frontend (`http://localhost:4200`)
2. `../CashForecasting.Api` = ASP.NET Core API (`http://localhost:5000`)

## Requirements

- Node.js + npm (compatible with Angular CLI 6)
- Angular CLI
- .NET SDK for the backend API

## Setup

1. Configure Azure AD values in:
   - `ui/src/environments/environment.ts`
   - `ui/src/environments/environment.prod.ts`
2. Confirm API base URLs and auth settings match your environment.

## Run

```bash
cd /tmp/workspace/DaveVoyles/Cash-forecasting/Frontend/ui
npm install
npm run start
```

If needed for local debugging, you can temporarily bypass auth guards in `app.module.ts` and navigate directly to `/files/upload`.

## Production Checklist

- Move all secrets and connection strings out of source-controlled files.
- Configure CORS and auth for deployed origins.
- Use environment-specific API endpoints.
