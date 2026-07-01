# Harbor

Harbor image registry conventions for the AccessHub Identity service live in this directory.

## Scope

This slice defines how the Jenkins pipeline can publish the Identity API image to Harbor. It does not configure Harbor projects, retention policies, vulnerability scanning, Helm deployment, Kubernetes deployment, or ingress.

## Image reference convention

Published images should use this format:

```text
${HARBOR_REGISTRY}/${HARBOR_PROJECT}/accesshub-identity-api:${TAG}
```

Placeholder example:

```text
harbor.example.internal/accesshub/accesshub-identity-api:build-123
```

`harbor.example.internal` is an example only. Do not commit a real registry host or credentials to this repository.

## Jenkins parameters

The Jenkins pipeline uses these parameters for optional Harbor publishing:

- `PUBLISH_TO_HARBOR` - enables Harbor publishing when set to `true`.
- `HARBOR_REGISTRY` - Harbor registry host, for example `harbor.example.internal`. Do not include `http://`, `https://`, or a path.
- `HARBOR_PROJECT` - Harbor project or namespace for this service.
- `HARBOR_CREDENTIALS_ID` - Jenkins credential ID for the Harbor robot account.

Publishing is disabled by default and no Harbor credentials are required when `PUBLISH_TO_HARBOR=false`.

## Credential convention

Recommended Jenkins credential ID:

```text
harbor-robot-accesshub-identity
```

Credential type:

```text
Username with password
```

Use a Harbor robot account scoped to the service project with the minimum required push and pull permissions. Do not use admin-level credentials.

## Tagging convention

When Harbor publishing is enabled, the Jenkins pipeline tags the already-built local image with remote Harbor tags.

Tags pushed:

- `build-${BUILD_NUMBER}` for every publish.
- `${safeBranch}-${BUILD_NUMBER}` when Jenkins provides `BRANCH_NAME`.
- `latest` only when `BRANCH_NAME` is `main`.

Feature branches must not push `latest`.

Branch names are lowercased and characters outside Docker tag-safe characters are replaced with `-`.

## Out of scope

The Harbor publishing slice does not include:

- Harbor project creation,
- retention policy configuration,
- vulnerability scanning policy,
- Helm chart publishing,
- Kubernetes deployment,
- ingress configuration,
- deployment smoke tests.
