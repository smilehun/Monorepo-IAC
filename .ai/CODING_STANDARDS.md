# Coding Standards

_Status snapshot: 2026-07-01_

## Backend
- Use Clean Architecture.
- Prefer constructor injection.
- Avoid static services.
- No magic strings.
- No magic numbers.

**Current status:** Mostly aligned in the implemented backend slices. Layering and constructor injection are in use. Authorization constants now reduce policy/permission magic strings. Some literal strings remain in auth and claim handling.

## Frontend
- Feature-first folder structure.
- Strong typing.
- Reusable components.

**Current status:** Not started.

## Infrastructure
- Everything declarative.
- Everything version controlled.
- Everything documented.

**Current status:** Initial Docker, Compose, Jenkins, and gated Harbor publishing assets are implemented and documented. Helm, Kubernetes, ingress, and monitoring assets remain pending.
