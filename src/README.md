# src

Application code for the AccessHub Identity microservice belongs under `src/`.

## Intended layout

- `backend/` - ASP.NET Core backend for the Identity service
- `frontend/` - React frontend UI owned by the Identity service

## Scope

This repository is not the whole IAM platform.

It owns one service boundary: **Identity**.

Backend and frontend work under `src/` should stay focused on Identity capabilities such as registration, login, logout, token handling, and password-related flows.
