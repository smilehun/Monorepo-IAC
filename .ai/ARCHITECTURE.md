# Architecture

_Status snapshot: 2026-07-01_

AccessHub is the broader IAM platform.

This repository owns the **Identity** bounded context as the first AccessHub microservice.

## Service Scope
Identity currently targets the authentication boundary:
- Register
- Login
- Logout
- JWT Access Token
- Refresh Token
- Token Rotation
- Password Hashing
- Change Password

## Current Architectural Status
### Implemented
- Cleanly separated backend projects under `src/backend/`
- API, Application, Infrastructure, and Domain boundaries
- Authentication endpoints and supporting application/infrastructure services
- EF Core persistence model and initial migration
- JWT bearer authentication, health checks, and structured logging
- Unit and integration test coverage for the current auth slice
- Initial policy-based RBAC slice protecting `/api/auth/me`
- Initial Docker, Compose, Jenkins, and gated Harbor publishing assets under `platform/`

### Partially Implemented
- Authorization data model exists (`User`, `Role`, `Permission`, `UserRole`, `RolePermission`) and one runtime permission policy is implemented; broader RBAC administration is not implemented
- Audit log model exists, but audit event emission is not implemented
- Repository structure is aligned, but frontend, Helm, Kubernetes, ingress, and monitoring areas remain placeholders

### Not Implemented
- Frontend workflow implementation
- Helm/Kubernetes/ingress deployment assets
- Prometheus/Grafana/Loki observability assets

## Repository Structure
```text
src/
    backend/
    frontend/

platform/
    docker/
    compose/
    kubernetes/
    helm/
    jenkins/
    harbor/
    ingress/
    monitoring/

scripts/

docs/
```

## Target Delivery Flow
GitHub → Jenkins → Harbor → Helm Upgrade → k3s

## Target Observability Flow
Prometheus → Grafana

Logging → Loki

## Recommended Next Slice
Move platform delivery from image publishing into deployment packaging with a deliberately narrow Helm slice:
- add a service Helm chart for the Identity API
- template deployment, service, config, secret references, and health probes
- keep Kubernetes rollout and ingress for the following slice
