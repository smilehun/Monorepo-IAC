# AccessHub Identity Service

AccessHub Identity Service is a production-oriented IAM microservice repository.

It is intended to demonstrate professional software engineering, DevOps, Infrastructure as Code, CI/CD, Kubernetes, security, and cloud-native delivery in the context of a real service rather than a tutorial project.

## Repository role

AccessHub is the broader IAM platform.

This repository owns the **Identity** bounded context as the **first AccessHub microservice**.

The service is responsible for authentication-oriented capabilities such as:

- register
- login
- logout
- JWT access tokens
- refresh tokens
- token rotation
- password hashing
- change password

This repository also includes the service-owned frontend UI for Identity workflows.

## Source of truth

The authoritative project requirements and constraints currently live in `.ai/`.

`D:\Work\Monorepo-IAC\.ai\REQUIREMENTS.md` is the primary source of truth.

Supporting project documents:

- `.ai/ARCHITECTURE.md`
- `.ai/CODING_STANDARDS.md`
- `.ai/DEVOPS.md`
- `.ai/ROADMAP.md`
- `.ai/TASKS.md`

Implementation must follow these documents and proceed incrementally with approval between phases.

## Repository structure

```text
.ai/
docs/
scripts/
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
```

## Structure intent

- `src/backend/` - Identity service backend implementation
- `src/frontend/` - Identity service frontend UI
- `platform/` - service-scoped delivery and operations assets
- `scripts/` - repeatable automation for this service
- `docs/` - service architecture and operational documentation

## Delivery approach

Work should remain incremental and independently reviewable.

Near-term phases are:

1. Repository alignment
2. Identity backend foundation
3. Identity frontend foundation
4. Local Docker / Compose development environment
5. Service CI/CD
6. Service deployment with Helm / Kubernetes / ingress
7. Service observability
8. Service documentation hardening
