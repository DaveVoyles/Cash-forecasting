# Cash Forecasting Reference Solution

This repository demonstrates an end-to-end cash-forecasting workflow across three layers:

- **Frontend (Angular 6)** for file upload and basic UI flows
- **ASP.NET Core API** for ingesting and storing uploaded files in Azure Blob Storage
- **Azure Functions + Python model service** for validation, prediction, and result persistence

The goal is to show how a forecasting model can be operationalized in Azure using a practical, modular architecture.

## Repository Structure

- `/Frontend/ui` – Angular web client
- `/CashForecasting.Api` – ASP.NET Core API for file upload and blob interactions
- `/AzureFunctions-Final` – Blob-triggered Functions pipeline
- `/Forecasting Pred Model` – Flask + scikit-learn prediction endpoint

## High-Level Flow

1. User uploads a CSV from the frontend.
2. API stores the file in `datauploaded` blob container.
3. `FunctionValidate` moves validated input to `datavalidated`.
4. `FunctionCallForecastModel` calls the Flask model endpoint.
5. Prediction output is written to `dataresult`.

## Configuration

This repository uses **sample/local development settings** by default. Provide real values through environment variables or local untracked config during deployment.

Common values:

- `Values__ConnString` (API)
- `blobconnection` (Functions)
- `blobBaseURI` (Functions)
- `ModelEndpoint` (Functions)
- `AzureWebJobsStorage` (Functions)

## Local Development

### API

```bash
cd /tmp/workspace/DaveVoyles/Cash-forecasting

dotnet build CashForecasting.Api.sln
```

### Frontend

```bash
cd /tmp/workspace/DaveVoyles/Cash-forecasting/Frontend/ui
npm install
npm run start
```

### Azure Functions

```bash
cd /tmp/workspace/DaveVoyles/Cash-forecasting/AzureFunctions-Final
dotnet build Functions.sln
```

## Notes

- The codebase targets legacy framework versions (Angular 6, .NET Core 2.1, Azure Functions v1).
- This is a reference architecture and may require modernization for production use.
