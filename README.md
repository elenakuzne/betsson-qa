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

---

## Testing

### Run all tests

```
dotnet test
```

### Unit tests

Located in `tests/Betsson.OnlineWallets.UnitTests`.

### Integration tests

Located in `tests/Betsson.OnlineWallets.Web.IntegrationTests`.

Uses `WebApplicationFactory` with an EF Core in-memory database. The database is reset before each test to ensure full isolation.

**Covered scenarios:**

| Area | Test |
|------|------|
| Balance | Returns `200 OK` |
| Balance | Returns `0` when no transactions exist |
| Deposit | Rejects negative amounts with `400 Bad Request` |
| Deposit | Accepts zero and positive amounts, returns updated balance |
| Deposit | Accumulates multiple deposits correctly |
| Withdraw | Rejects negative amounts with `400 Bad Request` |
| Withdraw | Zero withdrawal leaves balance unchanged |
| Withdraw | Returns `400 Bad Request` when funds are insufficient |
| Withdraw | Does not change balance on a failed withdrawal |
| Withdraw | Deducts correctly across single and multiple withdrawals |

**Infrastructure:**

- `CustomWebApplicationFactory` — spins up the full ASP.NET Core pipeline with a clean in-memory database per test.
- `ControllerTestBase` — base class that resets data and creates an `HttpClient` before each test; disposes the factory after.
