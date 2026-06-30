# Architecture

_Status snapshot: 2026-06-30_

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

### Partially Implemented
- Authorization data model exists (`User`, `Role`, `Permission`, `UserRole`, `RolePermission`), but runtime RBAC policies are not implemented
- Audit log model exists, but audit event emission is not implemented
- Repository structure is aligned, but most frontend and platform areas remain placeholders

### Not Implemented
- Frontend workflow implementation
- Delivery, deployment, and observability platform assets

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
Move from authentication into authorization with a deliberately narrow RBAC slice:
- add policy-based authorization
- protect one endpoint with a real policy
- verify 401 versus 403 behavior with tests
