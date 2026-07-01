# Roadmap

_Status snapshot: 2026-07-01_

## Phase 1 — Repository Alignment
**Status:** In Progress
- Top-level repository structure is in place.
- Backend solution structure is established.
- Frontend and docs areas still need to move from placeholders to working assets.
- Platform has initial Docker, Compose, Jenkins, and Harbor publishing assets, while Helm/Kubernetes/ingress/monitoring remain placeholders.

## Phase 2 — Identity Backend Foundation
**Status:** In Progress
- Authentication foundation is implemented: register, login, refresh, logout, current-user, and change-password.
- Persistence, JWT auth, logging, health checks, and automated tests are in place.
- A narrow RBAC runtime slice is implemented; broader RBAC administration, user management, and audit emission remain outstanding.

## Phase 3 — Identity Frontend Foundation
**Status:** Not Started

## Phase 4 — Docker
**Status:** In Progress
- Initial Identity API Dockerfile is in place.
- Docker image build has been verified locally.

## Phase 5 — Docker Compose
**Status:** In Progress
- Initial API + PostgreSQL local Compose stack is in place.
- Compose syntax has been validated.
- Runtime verification with PostgreSQL, EF migrations, health checks, and register flow has passed locally.

## Phase 6 — CI/CD
**Status:** In Progress
- Initial Jenkins pipeline as code is in place for restore, build, test, and Docker image build.
- Optional Harbor publishing support is in place behind an explicit Jenkins parameter.
- Deployment and smoke testing remain pending.
- Runtime verification still requires an available Jenkins agent with .NET 9 and Docker.

## Phase 7 — Harbor
**Status:** In Progress
- Initial Jenkins publishing slice and image naming conventions are in place.
- Harbor lifecycle policies such as retention and scanning remain pending.
- Runtime verification requires a Harbor project and Jenkins robot credential.

## Phase 8 — Helm
**Status:** Not Started

## Phase 9 — Kubernetes and Ingress
**Status:** Not Started

## Phase 10 — Monitoring
**Status:** In Progress
- Application health endpoint, DB health check, and structured logging are present.
- Prometheus, Grafana, Loki, and metrics work are still pending.

## Phase 11 — Documentation
**Status:** In Progress
- Core repository and intent documentation exists.
- Initial platform runbooks exist for Docker/Compose, Jenkins, and Harbor publishing.
- Full operational and deployment documentation is still pending.
