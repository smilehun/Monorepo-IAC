# Tasks

_Status snapshot: 2026-07-01_

## Done
- [x] Establish repository layout for backend, frontend, platform, scripts, and docs
- [x] Create backend solution with API, Application, Infrastructure, and Domain projects
- [x] Add core persistence model for users, roles, permissions, refresh tokens, and audit logs
- [x] Add EF Core configuration and initial migration
- [x] Configure PostgreSQL provider integration
- [x] Configure OpenAPI, Serilog, JWT bearer auth, and health checks
- [x] Implement register, login, refresh, logout, current-user, and change-password flows
- [x] Implement password hashing and refresh-token rotation
- [x] Add unit and integration tests for the current authentication slice
- [x] Add initial Dockerfile for the Identity API
- [x] Add Docker Compose local environment for API and PostgreSQL
- [x] Add Compose environment example
- [x] Add initial Jenkins pipeline for restore, build, test, and Docker image build
- [x] Add Jenkins pipeline runbook
- [x] Add gated Jenkins Harbor image publishing
- [x] Add Harbor image naming and tagging conventions
- [x] Add narrow policy-based RBAC slice for `/api/auth/me`
- [x] Verify 401 versus 403 behavior for the initial authorization policy
- [x] Verify Docker Compose stack with real PostgreSQL and containerized API register flow

## In Progress
- [ ] Expand policy-based RBAC beyond the initial current-user policy
- [ ] Expand documentation beyond placeholders and intent notes
- [ ] Replace remaining platform placeholder directories with working delivery assets

## Not Started
### Backend
- [ ] User CRUD
- [ ] Enable / disable user
- [ ] Lock / unlock user
- [ ] User profile
- [ ] Search, pagination, sorting, and filtering
- [ ] Audit event emission for important actions
- [ ] Forgot password
- [ ] Email verification
- [ ] MFA
- [ ] OAuth2 / OIDC

### Frontend
- [ ] Identity frontend foundation
- [ ] Service-owned UI flows

### Platform / DevOps
- [ ] Harbor lifecycle policies such as retention and scanning conventions
- [ ] Helm chart
- [ ] Kubernetes manifests
- [ ] Ingress configuration
- [ ] Prometheus / Grafana / Loki

## Recommended Next Slice
- [ ] Add one narrow Helm slice: service chart, values, config/secret references, health probes, and image settings
