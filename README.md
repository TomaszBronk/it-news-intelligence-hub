# IT News Intelligence Hub

A full-stack portfolio application for collecting IT news from RSS and public APIs, organizing them into a personal briefing, generating Polish summaries and preparing editable discussion-ready post drafts.

> The application is designed to demonstrate .NET, ASP.NET Core, React, SQL Server, external API integrations, background processing, testing and responsible AI-assisted content generation.

## Problem

Important technology news is spread across many sources. This application helps users collect selected news, filter and organize them, review source links and prepare verified Polish-language summaries or post drafts.

## Planned features

- RSS and public API sources management
- Scheduled news collection
- Normalization and deduplication
- Categories: .NET, Azure, AI, security, React, DevOps and data
- Daily and weekly IT briefings
- Original source links and publication dates
- Polish AI-assisted summaries
- Editable post drafts and discussion prompts
- Manual approval before copying or publishing any content
- User authentication and saved items

## Technology stack

- Backend: ASP.NET Core Web API
- Frontend: React + TypeScript
- Database: SQL Server / SQL Server LocalDB for local development
- Data access: Entity Framework Core
- Background processing: .NET BackgroundService
- Documentation: OpenAPI / Swagger
- Testing: xUnit and integration tests
- DevOps: Docker Compose and GitHub Actions
- Cloud target: Microsoft Azure

## Project status

🚧 In development — MVP planning phase.

## Source and content policy

The application stores only the metadata and excerpts needed to create a briefing.
Every summary or post draft must link to the original source and be reviewed by a human before use.
The application does not bypass paywalls or republish full articles.

## Local development

Detailed setup instructions will be added with the first MVP milestone.

## Roadmap

- [x] Create solution and local database
- [x] Add RSS source management
- [x] Implement manual RSS/Atom feed import
- [x] Store and display imported news items
- [ ] Add parser tests with local XML samples
- [ ] Add deduplication
- [ ] Implement scheduled feed collection
- [ ] Implement filters and saved items
- [ ] Add AI-assisted Polish summaries
- [ ] Add editable post drafts
- [ ] Add tests and CI
- [ ] Deploy to Azure

## Author

Tomasz Bronk  
[LinkedIn](https://www.linkedin.com/in/tomasz-bronk/) · [GitHub](https://github.com/TomaszBronk)
