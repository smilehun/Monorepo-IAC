# Jenkins

Jenkins CI/CD assets for the AccessHub Identity service live in this directory.

## Current scope

The current pipeline:

- checks out the repository,
- prints .NET and Docker toolchain information,
- restores the backend solution,
- builds the backend solution in Release configuration,
- runs the backend test suite,
- archives TRX test result artifacts,
- builds the Identity API Docker image locally on the Jenkins agent,
- optionally publishes the image to Harbor when `PUBLISH_TO_HARBOR=true`.

Harbor publishing is disabled by default.

## Out of scope

This slice does not:

- deploy with Helm,
- deploy to Kubernetes or k3s,
- configure ingress,
- run deployment smoke tests.

## Jenkins job setup

Create a Jenkins Pipeline job with:

- Definition: `Pipeline script from SCM`
- Script Path: `platform/jenkins/Jenkinsfile`

## Required agent capabilities

The Jenkins agent that runs this pipeline needs:

- Git access to this repository,
- .NET 9 SDK,
- Docker CLI,
- access to a Docker daemon,
- network access for NuGet package restore.

No registry, cluster, or application secrets are required when `PUBLISH_TO_HARBOR=false`.

## Optional Harbor publishing

Set these Jenkins parameters to publish the image to Harbor after restore, build, tests, and Docker build succeed:

- `PUBLISH_TO_HARBOR=true`
- `HARBOR_REGISTRY=<registry-host>`
- `HARBOR_PROJECT=<project-name>`
- `HARBOR_CREDENTIALS_ID=harbor-robot-accesshub-identity`

`HARBOR_REGISTRY` must be a registry host only. Do not include `http://`, `https://`, or a path.

The Jenkins credential should be a `Username with password` credential for a Harbor robot account with minimum push and pull permissions for the service project.

When publishing is enabled, the pipeline pushes:

- `build-${BUILD_NUMBER}` for every publish,
- `${safeBranch}-${BUILD_NUMBER}` when Jenkins provides `BRANCH_NAME`,
- `latest` only from `main`.

Harbor naming and tagging conventions are documented in `platform/harbor/README.md`.

## Local command equivalents

Run these from the repository root to execute the same core checks locally:

```bash
dotnet restore src/backend/AccessHub.Identity.sln
dotnet build src/backend/AccessHub.Identity.sln --configuration Release --no-restore
dotnet test src/backend/AccessHub.Identity.sln --configuration Release --no-restore
docker build -f platform/docker/Dockerfile -t accesshub-identity-api:local .
```

## Future slices

Future CI/CD slices should add Helm/Kubernetes deployment and deployment smoke tests after Harbor publishing is verified.
