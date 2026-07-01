# AccessHub - Project Requirements

> Version: 1.0
>
> This document is the single source of truth for this repository.
>
> Status annotations reflect the current repository state as of 2026-07-01.

---

# 1. Vision
Build a production-quality Identity service for the AccessHub IAM platform.

**Status:** In Progress

---

# 2. Goals
Target qualities:
- Clean Architecture
- Domain Driven Design (where appropriate)
- Microservice-first architecture
- Infrastructure as Code
- Docker
- Kubernetes
- CI/CD
- Monitoring
- Logging
- Security
- Automated deployment
- Production readiness

**Status:** Partially met. Backend architecture, core authentication, initial RBAC runtime behavior, Docker/Compose, Jenkins CI, and gated Harbor publishing are in place. Deployment, frontend, full observability, and broader production-readiness work remain pending.

---

# 3. Technology Stack
## Backend
Target:
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- FluentValidation
- JWT Authentication
- Refresh Token
- Serilog
- Health Checks
- OpenAPI

**Current state:** All of the above are present except FluentValidation. Validation is currently implemented manually in the application layer.

## Frontend
Target:
- React
- TypeScript
- Vite
- React Router
- TanStack Query
- Zustand
- Tailwind CSS
- Axios

**Current state:** Not started.

## Database
Target:
- PostgreSQL running via Docker with persistent volumes

**Current state:** PostgreSQL provider integration and EF Core migration exist. An initial Docker Compose PostgreSQL setup exists for local development and has been verified with EF migrations and the containerized API register flow.

## Infrastructure
Target:
- Ubuntu Server
- Docker Engine
- Docker Compose
- k3s
- Helm
- Jenkins
- Harbor
- SonarQube
- NGINX Ingress
- Prometheus
- Grafana
- Loki

**Current state:** Docker, Docker Compose, Jenkins pipeline as code, and gated Harbor image publishing are partially implemented for the Identity service. k3s, Helm, SonarQube, NGINX Ingress, Prometheus, Grafana, and Loki remain planned only.

---

# 4. Architecture
This repository represents the **Identity** bounded context as the first **AccessHub microservice**.

It should be independently buildable, deployable, testable, and observable.

**Status:** Partially met. The backend is independently buildable and testable, containerized, runnable locally with PostgreSQL through Compose, and wired into an initial Jenkins/Harbor delivery path. Deployment and full observability are not yet implemented.

---

# 5. Authentication
Implement:
- [x] Register
- [x] Login
- [x] Logout
- [x] JWT Access Token
- [x] Refresh Token
- [x] Token Rotation
- [x] Password Hashing
- [x] Change Password

Future:
- [ ] Forgot Password
- [ ] Email Verification
- [ ] MFA
- [ ] OAuth2
- [ ] OIDC

**Status:** Implemented for the current authentication slice and covered by tests.

---

# 6. Authorization
Implement RBAC with policy-based authorization.

Entities:
- [x] User
- [x] Role
- [x] Permission
- [x] UserRole
- [x] RolePermission

Runtime behavior:
- [x] Policy-based authorization for the initial current-user policy
- [x] Permission evaluation through ASP.NET Core policies for one endpoint
- [x] No role checks inside controllers/endpoints for the implemented slice

**Status:** In Progress. The schema exists and one runtime permission policy protects `/api/auth/me`; broader RBAC administration and additional policies are not implemented yet.

---

# 7. User Management
Implement:
- [ ] User CRUD
- [ ] Enable User
- [ ] Disable User
- [ ] Lock User
- [ ] Unlock User
- [ ] User Profile
- [ ] Search Users
- [ ] Pagination
- [ ] Sorting
- [ ] Filtering

**Status:** Not started.

---

# 8. Audit Logging
Every important action must generate an audit record.

Examples include login, logout, create/update/delete user, assign/remove role, and password change.

Audit records should include timestamp, user, action, resource, IP address, and result.

**Status:** In Progress at the schema level only. `AuditLog` exists, but events are not being emitted yet.

---

# 9. Repository Structure
This repository should contain everything needed to build, deploy, and operate the **AccessHub Identity service**.

Expected areas:
- `src/`
- `platform/`
- `scripts/`
- `docs/`
- `README.md`

**Status:** In Progress. The structure exists, but many areas still contain placeholders.

---

# 10. Infrastructure as Code
Everything must be declarative and stored in Git.

Expected assets include Dockerfiles, Docker Compose, Helm charts, Kubernetes manifests, scripts, and configuration.

**Status:** In Progress. Dockerfile, Docker Compose, Jenkins pipeline, and Harbor publishing conventions are declarative and version controlled. Helm charts, Kubernetes manifests, ingress, monitoring assets, and operational scripts remain pending.

---

# 11. Docker
Required:
- [x] Dockerfile
- [x] Multi-stage build
- [x] Healthcheck
- [x] Environment variables
- [x] Small image
- [x] Pinned versions
- [x] Non-root user whenever possible

**Status:** In Progress. Initial API Dockerfile is present and the image build has been verified locally.

---

# 12. Docker Compose
Required for local development:
- [x] Backend
- [ ] Frontend
- [x] PostgreSQL
- [ ] PgAdmin (optional)

Future:
- [ ] RabbitMQ
- [ ] Redis

**Status:** In Progress. Initial API + PostgreSQL Compose stack is present and verified with health checks, EF migrations, and register flow. Frontend and optional PgAdmin are not included yet.

---

# 13. Kubernetes
Required:
- [ ] Deployment
- [ ] Service
- [ ] Ingress
- [ ] ConfigMap
- [ ] Secret
- [ ] PersistentVolumeClaim

Use Helm whenever appropriate.

**Status:** Not started.

---

# 14. CI/CD
Target pipeline:
GitHub → Jenkins → Restore → Build → Unit Test → SonarQube → Docker Build → Docker Push → Harbor → Helm Upgrade → k3s → Smoke Test

**Status:** In Progress. Initial Jenkins pipeline as code exists for restore, build, test, Docker image build, and gated Harbor image publishing. SonarQube, Helm upgrade, k3s deployment, and smoke test stages are not implemented yet.

---

# 15. Monitoring
Implement:
- [ ] Prometheus
- [ ] Grafana
- [ ] Loki
- [x] Application Health Checks
- [x] Structured Logging
- [ ] Metrics
- [ ] Prepare for OpenTelemetry

**Status:** In Progress. App health and logging exist; the monitoring stack and metrics do not.

---

# 16. Security
Never hardcode passwords, secrets, API keys, JWT secrets, or connection strings.

Use:
- Environment Variables
- `.env.example`
- Kubernetes Secrets

**Status:** In Progress. Runtime config uses environment variables and a Compose `.env.example` exists, but Kubernetes secret assets are not implemented.

---

# 17. Documentation
Generate documentation for architecture, folder structure, development, Docker, Compose, Kubernetes, Helm, CI/CD, monitoring, troubleshooting, backup, and restore.

Use Mermaid where appropriate.

**Status:** In Progress. Basic repository documentation exists; the operational documentation set is still pending.

---

# 18. Coding Principles
Prefer simplicity, avoid overengineering, keep APIs documented, and keep everything testable.

**Status:** Partially aligned. Recent backend work is incremental and tested, but documentation and some standards gaps remain.

---

# 19. Development Workflow
Follow: Analyze → Design → Explain → Wait for approval → Implement → Review → Summarize.

**Status:** Being followed for recent backend slices.

---

# 20. Working Rules for AI
Analyze first, understand the architecture, produce a roadmap, wait for approval, avoid unrelated refactors, explain trade-offs, and optimize for maintainability.

**Status:** Active operating rules.
