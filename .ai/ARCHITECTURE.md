# Architecture

Monorepo

src/
    backend/
    frontend/

platform/
    docker/
    compose/
    kubernetes/
    helm/
    monitoring/
    jenkins/

Deployment

GitHub

↓

Jenkins

↓

Harbor

↓

k3s

Monitoring

Prometheus

↓

Grafana

Logging

Loki