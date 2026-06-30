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
