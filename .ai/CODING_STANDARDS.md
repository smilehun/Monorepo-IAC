# Coding Standards

_Status snapshot: 2026-06-30_

## Backend
- Use Clean Architecture.
- Prefer constructor injection.
- Avoid static services.
- No magic strings.
- No magic numbers.

**Current status:** Mostly aligned in the implemented backend slices. Layering and constructor injection are in use. Some literal strings remain in auth and claim handling.

## Frontend
- Feature-first folder structure.
- Strong typing.
- Reusable components.

**Current status:** Not started.

## Infrastructure
- Everything declarative.
- Everything version controlled.
- Everything documented.

**Current status:** Intent is documented, but working infrastructure assets are not implemented yet.
