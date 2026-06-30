# Tasks

_Status snapshot: 2026-06-30_

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

## In Progress
- [ ] Implement policy-based RBAC on top of the existing role/permission schema
- [ ] Expand documentation beyond placeholders and intent notes
- [ ] Replace platform placeholder directories with working delivery assets

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
- [ ] Jenkins pipeline
- [ ] Harbor integration
- [ ] Helm chart
- [ ] Kubernetes manifests
- [ ] Ingress configuration
- [ ] Prometheus / Grafana / Loki

## Recommended Next Slice
- [ ] Add one narrow authorization slice: one real policy, one protected endpoint, and tests for 401 vs 403
