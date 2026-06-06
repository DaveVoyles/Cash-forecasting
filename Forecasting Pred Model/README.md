# Flask Prediction Service (Scikit-Learn)

This folder contains a lightweight Flask service that loads a trained model (`model.joblib`) and exposes a forecast endpoint.

## Endpoint

- `POST /forecast`
- Form field: `filename` (path/URL to the CSV file)

## Local Docker Run

From repo root:

```bash
cd "Forecasting Pred Model"
docker build -t cash-forecasting-model:local .
docker run --name cash-forecasting-model -p 5000:5000 cash-forecasting-model:local
```

## Verify

```bash
docker ps
docker logs cash-forecasting-model
```

## Cleanup

```bash
docker rm -f cash-forecasting-model
```
