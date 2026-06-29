# Project Requirements

## Vision

Build a production-quality portfolio project.

The repository must demonstrate:

- Senior .NET development
- Senior React development
- DevOps
- Kubernetes
- Infrastructure as Code
- CI/CD
- Observability

The repository should look like a real enterprise project rather than a tutorial.

---

# Principles

The repository is the single source of truth.

Everything should be reproducible.

Everything should be automated whenever possible.

Everything should be version controlled.

Avoid manual configuration.

---

# Target Platform

Ubuntu Server

Docker Engine

Docker Compose

k3s

Jenkins

Harbor

SonarQube

Prometheus

Grafana

Loki

NGINX Ingress

---

# Backend

.NET 9

ASP.NET Core

Clean Architecture

Entity Framework Core

Serilog

FluentValidation

Health Checks

OpenAPI

---

# Frontend

React

TypeScript

Vite

TanStack Query

TailwindCSS

React Router

---

# Deployment

Docker

Docker Compose

Helm

Kubernetes

---

# CI/CD

GitHub

↓

Jenkins

↓

SonarQube

↓

Build

↓

Test

↓

Docker Build

↓

Harbor

↓

Helm Upgrade

↓

k3s

---

# Infrastructure as Code

Everything must be stored in Git.

Never rely on manual configuration.

Infrastructure should be declarative.

---

# Quality

Prefer maintainability over speed.

Do not introduce technical debt.

Do not generate unnecessary abstractions.

Always explain architectural decisions.

---

# Working Style

Implement incrementally.

Never perform huge refactors.

Wait for approval after each phase.