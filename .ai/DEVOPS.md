# DevOps

_Status snapshot: 2026-07-01_

## Current Status
### Implemented
- Backend solution can build and test locally
- Structured logging is configured with Serilog
- Health checks are exposed by the API
- Platform directory structure exists under `platform/`
- Initial Docker build asset exists for the Identity API
- Initial Docker Compose local environment exists for API and PostgreSQL
- Docker Compose stack has been runtime-verified with the containerized API and PostgreSQL
- Initial Jenkins pipeline as code exists for restore, build, test, and Docker image build
- Initial gated Harbor publishing support exists in the Jenkins pipeline

Jenkins and Harbor publishing validation still require an available Jenkins agent with .NET 9, Docker, and Harbor credentials for publish runs.

### Not Yet Implemented
- Harbor lifecycle policies such as retention and scanning conventions
- Helm charts
- Kubernetes deployment assets
- Ingress configuration
- Prometheus / Grafana / Loki assets

## Target Capabilities
### Docker
- Multi-stage build
- Small images
- Non-root user
- Healthcheck

### Compose
- Local development environment

### CI/CD
- Jenkins pipeline as code
- Harbor image publishing

### Deployment
- Helm charts
- Kubernetes manifests
- Ingress

**Current status:** Not started.

### Monitoring
- Prometheus
- Grafana
- Loki

**Current status:** Not started beyond application health checks and structured logging.
