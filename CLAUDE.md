# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A .NET 10 ASP.NET Core demo app designed to work with [Uptime Kuma](https://github.com/louislam/uptime-kuma) for uptime monitoring. Provides custom health check endpoints with failure/recovery simulation for testing monitoring workflows.

## Build & Run Commands

```bash
# Restore dependencies
dotnet restore DotnetKumaUptimeDemo.slnx

# Build solution
dotnet build DotnetKumaUptimeDemo.slnx

# Run main API (http on port 5192, https on 7185)
dotnet run --project DotnetKumaUptimeDemo

# Run with Aspire orchestration
dotnet run --project DotnetKumaUptimeDemo.AppHost

# Docker Compose (API + Uptime Kuma)
docker-compose up --build
```

There are no tests in this project.

## Architecture

Three-project .NET Aspire solution (`DotnetKumaUptimeDemo.slnx`):

- **DotnetKumaUptimeDemo** — Main web API. All health checks, endpoints, and simulation logic live in `Program.cs` (minimal API style).
- **DotnetKumaUptimeDemo.AppHost** — Aspire orchestrator (`AppHost.cs`). References the main project.
- **DotnetKumaUptimeDemo.ServiceDefaults** — Shared Aspire service defaults: OpenTelemetry (metrics/tracing/logging), service discovery, HTTP resilience, health check base setup (`Extensions.cs`).

### Key Endpoints

| Endpoint | Purpose |
|---|---|
| `GET /health` | Full health report (JSON) with all checks |
| `GET /health/database` | Database-tagged checks only |
| `GET /health/cache` | Cache-tagged checks only |
| `POST /api/simulate/failure/{component}` | Simulate failure (postgres/redis/api) |
| `POST /api/simulate/recovery/{component}` | Recover from simulated failure |
| `GET /api/users` | Sample API endpoint (respects api health state) |
| `GET /api/status` | Service info endpoint |
| `GET /scalar/v1` | Scalar API docs (dev only) |

### Health Checks

Three custom `IHealthCheck` implementations defined inline in `Program.cs`: `PostgresHealthCheck`, `RedisHealthCheck`, `ApiHealthCheck`. Each supports static `SimulateFailure(bool)` for testing. Redis is optional — controlled by `Redis:Enabled` config flag.

### Docker

`docker-compose.yml` runs the API alongside Uptime Kuma 2 on a shared bridge network. The API Dockerfile is in `DotnetKumaUptimeDemo/Dockerfile` (multi-stage, .NET 10).

## Conventions

- Minimal API style (no controllers except the scaffolded `WeatherForecastController`)
- Health check classes are defined in `Program.cs`, not separate files
- Scalar is used for API documentation instead of Swagger
- NuGet packages: Npgsql (PostgreSQL), StackExchange.Redis, Scalar.AspNetCore, Microsoft.AspNetCore.OpenApi
