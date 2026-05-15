# k6 Limit Test Suite

These scripts help find and validate throughput limits for `POST /api/evaluation`.

## 1) Progressive Limit Ramp

File: `tests/perf/limit-ramp.js`

Increases arrival rate in steps to find where latency or failures start to degrade.

Run:

```powershell
k6 run tests/perf/limit-ramp.js
```

Useful overrides:

```powershell
$env:START_RATE = "200"
$env:RATE_STEP = "200"
$env:STEPS = "12"
$env:STEP_DURATION = "30s"
$env:MAX_VUS = "2500"
k6 run tests/perf/limit-ramp.js
```

## 2) Spike Test

File: `tests/perf/spike.js`

Validates resilience during sudden bursts and recovery after spike.

Run:

```powershell
k6 run tests/perf/spike.js
```

Useful overrides:

```powershell
$env:BASELINE_RATE = "300"
$env:SPIKE_RATE = "3500"
$env:WARMUP_DURATION = "30s"
$env:SPIKE_DURATION = "20s"
$env:RECOVERY_DURATION = "30s"
k6 run tests/perf/spike.js
```

## 3) Soak Test

File: `tests/perf/soak.js`

Checks sustained stability (latency drift, error growth) over longer duration.

Run:

```powershell
$env:RATE = "1400"
$env:DURATION = "15m"
k6 run tests/perf/soak.js
```

## Shared Environment Variables

All scripts support:

- `BASE_URL` (default `http://localhost:5017`)
- `FLAG_KEY` (default `test-flag`)
- `ENVIRONMENT` (default `Production`)
- `REQ_TIMEOUT` (default `5s`)
- `PRE_ALLOCATED_VUS`
- `MAX_VUS`

## Interpreting Results

- `dropped_iterations > 0` means k6 could not keep the target arrival rate with configured VUs.
- Rising `http_req_duration` p95/p99 indicates capacity pressure.
- Rising `http_req_failed` indicates behavior has moved beyond acceptable limits.
- Use `limit-ramp` first to find a knee point, then validate with `soak` at and below that rate.
