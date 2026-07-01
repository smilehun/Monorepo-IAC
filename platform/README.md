# platform

Service-scoped delivery and operations assets for the AccessHub Identity microservice belong under `platform/`.

## Intended layout

- `docker/` - service container build assets
- `compose/` - local development orchestration for the service and its dependencies
- `kubernetes/` - service-level manifests when not better expressed elsewhere
- `helm/` - Helm charts and values for deploying the service
- `jenkins/` - CI/CD assets for the service pipeline
- `harbor/` - image registry conventions and related service assets
- `ingress/` - ingress configuration and conventions for exposing the service
- `monitoring/` - service observability assets such as metrics, logs, and dashboards

## Scope

Everything under `platform/` should support this single service.

Do not treat this directory as the infrastructure home for the entire AccessHub platform.

## Local Docker / Compose

Build the Identity API image from the repository root:

```bash
docker build -f platform/docker/Dockerfile -t accesshub-identity-api:local .
```

Run the local API and PostgreSQL stack:

```bash
docker compose --env-file platform/compose/.env.example -f platform/compose/compose.yaml up --build -d
```

For local development, copy the example env file and replace the placeholder values with local-only secrets:

```bash
cp platform/compose/.env.example platform/compose/.env
docker compose --env-file platform/compose/.env -f platform/compose/compose.yaml up --build -d
```

Check container status:

```bash
docker compose --env-file platform/compose/.env.example -f platform/compose/compose.yaml ps
```

Verify the API:

```bash
curl http://localhost:5080/health
curl http://localhost:5080/
```

Stop the stack:

```bash
docker compose --env-file platform/compose/.env.example -f platform/compose/compose.yaml down
```

Remove the PostgreSQL volume when a clean local database is needed:

```bash
docker compose --env-file platform/compose/.env.example -f platform/compose/compose.yaml down -v
```

## Jenkins CI

The Jenkins pipeline lives at `platform/jenkins/Jenkinsfile`.

Configure Jenkins with:

- Definition: `Pipeline script from SCM`
- Script Path: `platform/jenkins/Jenkinsfile`

The current pipeline restores, builds, tests, and builds the Identity API Docker image locally on the Jenkins agent. It can optionally publish the built image to Harbor when explicitly enabled with Jenkins parameters. It does not deploy to Kubernetes yet.

## Harbor

Harbor image naming and tagging conventions live in `platform/harbor/README.md`.

Local Docker Compose continues to build and use `accesshub-identity-api:local`; it does not require Harbor.

## Database migrations

Docker Compose starts PostgreSQL and the API, but it does not automatically apply Entity Framework Core migrations yet.

Until a dedicated migration/bootstrap slice is added, apply migrations explicitly from the backend tooling when database schema is required for endpoint testing beyond health checks.
