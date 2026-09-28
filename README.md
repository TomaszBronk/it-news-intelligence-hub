# IT News Intelligence Hub

[![Continuous Integration](https://github.com/TomaszBronk/it-news-intelligence-hub/actions/workflows/ci.yml/badge.svg)](https://github.com/TomaszBronk/it-news-intelligence-hub/actions/workflows/ci.yml)

A full-stack application for collecting IT news from RSS and public APIs, organizing it into a personal briefing, generating Polish summaries, and preparing editable discussion-ready post drafts.

> The project demonstrates practical software engineering skills in .NET, ASP.NET Core, React, SQL Server, external API integrations, background processing, automated testing, CI and responsible AI-assisted content generation.

## Problem

Important technology news is distributed across many sources. Following relevant updates about .NET, Azure, AI, security, React, DevOps and data platforms can be time-consuming.

IT News Intelligence Hub helps users collect selected news, organize it in one place, review original sources and later create verified Polish-language summaries or post drafts.

## Implemented features

- ASP.NET Core Web API.
- React frontend built with JavaScript and JSX.
- React Router navigation between News sources and News items.
- Axios HTTP client with centralized API error mapping.
- TanStack Query for server-state caching, mutations, invalidation and automatic polling.
- Modular monolith backend using Clean Architecture principles.
- SQL Server Express LocalDB persistence for local development.
- Entity Framework Core migrations.
- REST API for RSS and Atom source management.
- Create, list, view, update and delete news sources.
- Validation of required fields and feed URLs.
- Unique RSS/Atom feed URL validation.
- Manual RSS/Atom import using the `Fetch now` action.
- Automatic import of active news sources through a .NET `BackgroundService`.
- Feed import status tracking:
  - Last import attempt.
  - Last successful import.
  - Last import error.
- Duplicate news detection by external feed ID and original article URL.
- Imported news list with links to original articles.
- Automatic UI refresh for source status and imported news.
- OpenAPI / Swagger documentation.
- Health check endpoint:
  ```text
  GET /health
  ```
- RFC 7807 `ProblemDetails` API error responses.
- xUnit unit tests for the RSS feed import use case.
- xUnit API integration tests using SQLite in-memory database.
- GitHub Actions CI:
  - .NET restore, build and tests.
  - React dependency installation and production build.

## Planned features

- RSS and public API source management improvements.
- Categories, filters and saved news items.
- Status workflow:
  ```text
  New → Read → Saved → Dismissed
  ```
- Personal notes for saved news items.
- Daily and weekly IT briefing views.
- Search, filtering and sorting.
- Polish AI-assisted summaries.
- Developer-impact and business-impact summary formats.
- Editable post drafts and discussion prompts.
- Manual review and approval before copying or publishing content.
- User authentication and user-specific saved items.
- Docker Compose local environment.
- Azure deployment.
- Azure-oriented monitoring and observability.

## Technology stack

| Area | Technology |
|---|---|
| Backend | C#, .NET, ASP.NET Core Web API |
| Frontend | React, JavaScript, JSX |
| Client-side routing | React Router |
| HTTP client | Axios |
| Server state | TanStack Query |
| Database | SQL Server Express LocalDB |
| Test database | SQLite in-memory |
| Data access | Entity Framework Core |
| RSS and Atom | `System.ServiceModel.Syndication` |
| Background processing | .NET `BackgroundService` |
| API documentation | OpenAPI / Swagger |
| API health | ASP.NET Core Health Checks |
| Error format | RFC 7807 `ProblemDetails` |
| Testing | xUnit, NSubstitute, WebApplicationFactory |
| CI | GitHub Actions |
| Development environment | Visual Studio 2026, Node.js LTS |
| Planned cloud target | Microsoft Azure |

## Project status

🚧 In development — Milestone 3 completed.

The application currently supports RSS/Atom source management, manual and scheduled feed imports, duplicate detection, import status tracking, a React-based interface, automated tests and GitHub Actions CI.

## Architecture

The application follows a modular monolith approach with Clean Architecture principles.

```text
React client
      |
      | Axios + TanStack Query / HTTP
      v
ASP.NET Core Server
      |
      v
Application use cases and abstractions
      |
      +---------------------------+
      |                           |
      v                           v
Domain entities              Infrastructure
                              - EF Core / SQL Server
                              - RSS and Atom reader
                              - background import worker
```

### Backend

- **Domain** contains business entities and rules. It has no dependency on frameworks, databases or infrastructure.
- **Application** contains use cases, commands, queries, interfaces and application-level business flow.
- **Infrastructure** contains EF Core persistence, SQL Server integration, RSS/Atom HTTP communication, repositories and technical implementations.
- **Server** exposes HTTP endpoints, configures dependency injection, handles exceptions globally and hosts background services.

### Frontend

- **Axios** centralizes HTTP configuration, timeout configuration and API error mapping.
- **TanStack Query** manages server state, cache invalidation, mutations, loading states, error states and automatic polling.
- **React Router** provides navigation between application pages.
- The frontend is organized by features, including `news-sources` and `news-items`.
- Local component state is used only for UI concerns such as forms, visibility and temporary user interactions.

## Repository structure

```text
it-news-intelligence-hub/
├─ src/
│  ├─ ItNewsIntelligenceHub.Domain/
│  ├─ ItNewsIntelligenceHub.Application/
│  ├─ ItNewsIntelligenceHub.Infrastructure/
│  ├─ ItNewsIntelligenceHub.Server/
│  └─ itnewsintelligencehub.client/
├─ tests/
│  ├─ ItNewsIntelligenceHub.Application.Tests/
│  └─ ItNewsIntelligenceHub.Server.IntegrationTests/
├─ docs/
│  ├─ adr/
│  └─ screenshots/
├─ samples/
├─ infra/
├─ .github/
│  └─ workflows/
│     └─ ci.yml
├─ README.md
├─ SECURITY.md
├─ THIRD-PARTY-NOTICES.md
├─ LICENSE
├─ .editorconfig
├─ .gitignore
├─ global.json
└─ ItNewsIntelligenceHub.sln
```

## Source and content policy

- The application is intended for public RSS/Atom feeds and permitted public APIs.
- Original articles remain the property of their publishers.
- The application stores only metadata and excerpts needed to create a briefing.
- The application does not bypass paywalls or republish full articles.
- Every future AI-generated summary or post draft must show the original source URL.
- AI-generated content must be reviewed, edited and verified by a human before use or publication.
- The MVP does not publish content automatically to LinkedIn or other platforms.
- Do not commit API keys, tokens, passwords, confidential documents or customer data.

## Local development

### Prerequisites

- Visual Studio 2026 with the **ASP.NET and web development** workload.
- .NET SDK version configured in `global.json`.
- Node.js LTS.
- SQL Server Express LocalDB.
- Git.

### 1. Clone the repository

```powershell
git clone [https://github.com/TomaszBronk/it-news-intelligence-hub.git](https://github.com/TomaszBronk/it-news-intelligence-hub.git)
cd it-news-intelligence-hub
```

### 2. Configure local API settings

Copy the example configuration file:

```powershell
Copy-Item `
  src/ItNewsIntelligenceHub.Server/appsettings.Development.example.json `
  src/ItNewsIntelligenceHub.Server/appsettings.Development.json
```

The local development configuration file is ignored by Git. Do not commit secrets, tokens or passwords.

### 3. Start SQL Server LocalDB

```powershell
sqllocaldb start MSSQLLocalDB
```

To verify LocalDB installation:

```powershell
sqllocaldb info
```

### 4. Apply database migrations

Run from the repository root:

```powershell
dotnet ef database update `
  --project src/ItNewsIntelligenceHub.Infrastructure `
  --startup-project src/ItNewsIntelligenceHub.Server
```

This creates or updates the local database:

```text
ItNewsIntelligenceHubDb
```

### 5. Install frontend dependencies

```powershell
cd src/itnewsintelligencehub.client
npm ci
```

### 6. Run the application

Start `ItNewsIntelligenceHub.Server` in Visual Studio 2026 using `F5`.

Start the React frontend:

```powershell
npm run dev
```

The frontend is typically available at:

```text
http://localhost:5173
```

The API Swagger/OpenAPI page is available at:

```text
[https://localhost](https://localhost):<api-port>/openapi
```

or, depending on the configured OpenAPI UI:

```text
[https://localhost](https://localhost):<api-port>/swagger
```

Use the actual API port from:

```text
src/ItNewsIntelligenceHub.Server/Properties/launchSettings.json
```

### 7. Health check

After starting the API, verify application health:

```text
GET [https://localhost](https://localhost):<api-port>/health
```

Expected response:

```text
Healthy
```

## Testing

Run all backend unit and integration tests from the repository root:

```powershell
dotnet test
```

Build the React production bundle:

```powershell
cd src/itnewsintelligencehub.client
npm run build
```

## Current API endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/news-sources` | Lists configured RSS/Atom sources |
| GET | `/api/news-sources/{id}` | Gets one news source |
| POST | `/api/news-sources` | Creates a news source |
| PUT | `/api/news-sources/{id}` | Updates a news source |
| DELETE | `/api/news-sources/{id}` | Deletes a news source |
| POST | `/api/news-sources/{id}/fetch` | Imports news from one source |
| GET | `/api/news-items` | Lists imported news items |
| GET | `/health` | Returns application health status |

## Example news source request

```json
{
  "name": "Microsoft .NET Blog",
  "feedUrl": "[https://devblogs.microsoft.com/dotnet/feed/](https://devblogs.microsoft.com/dotnet/feed/)",
  "websiteUrl": "[https://devblogs.microsoft.com/dotnet/](https://devblogs.microsoft.com/dotnet/)",
  "category": "DotNet",
  "isActive": true
}
```

## Roadmap

- [x] Create solution and local database.
- [x] Add RSS source management.
- [x] Implement manual RSS/Atom feed import.
- [x] Store and display imported news items.
- [x] Add duplicate detection.
- [x] Implement scheduled feed collection.
- [x] Add import status tracking and health check.
- [x] Introduce Clean Architecture boundaries.
- [x] Add unit tests for the RSS feed import use case.
- [x] Add API integration tests.
- [x] Add GitHub Actions CI workflow.
- [ ] Add categories, filters and saved items.
- [ ] Add daily and weekly briefing views.
- [ ] Add Polish AI-assisted summaries.
- [ ] Add editable post drafts and discussion prompts.
- [ ] Add frontend component tests.
- [ ] Add Docker Compose local environment.
- [ ] Deploy to Azure.

## License

This project is licensed under the [MIT License](LICENSE).


## Author

Tomasz Bronk

- LinkedIn: [linkedin.com/in/tomasz-bronk](https://www.linkedin.com/in/tomasz-bronk/)
- GitHub: [github.com/TomaszBronk](https://github.com/TomaszBronk)
