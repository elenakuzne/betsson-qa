[![Tests](https://github.com/elenakuzne/betsson-qa/actions/workflows/tests.yml/badge.svg)](https://github.com/elenakuzne/betsson-qa/actions/workflows/tests.yml)

# qa-backend-code-challenge

Code challenge for QA Backend Engineer candidates.

### Build Docker image

Run this command from the directory where there is the solution file.

```
docker build -f src/Betsson.OnlineWallets.Web/Dockerfile .
```

### Run Docker container

```
docker run -p <port>:8080 <image id>
```

### Open Swagger

```
http://localhost:<port>/swagger/index.html
```
