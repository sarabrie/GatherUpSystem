# GatherUp – Event Management System

A backend system for planning and managing events, built with **C# and ASP.NET Web API (.NET 8)** using an N-tier architecture.

Event hosts create events and invite participants. Participants respond with RSVPs and answer polls. Event managers handle the budget, suppliers and receipts, and the system sends email updates and reminders.

> Team project developed as part of the Software Engineering program.

## Features

- **Events** – create, edit and view events, invite participants by email, and coordinate the schedule with a poll.
- **Participants & RSVP** – add participants, track attendance and payment status, manage notification preferences, and send reminders.
- **Budget & suppliers** – allocate the budget to vendors, record receipts, and view the financial status and a receipts report for each event.
- **Polls & surveys** – create polls with multiple questions, submit answers, and view the results per question.
- **Notifications** – email updates through SMTP, with a file-based mail service for local development.
- **Authentication** – login with JWT tokens.
- **Web client** – HTML, CSS and JavaScript pages for the host, the manager and the participant, served from `wwwroot`.

## Architecture

The solution is split into four projects with a strict separation of responsibilities:

| Layer | Project | Responsibility |
| --- | --- | --- |
| API | `GatherUp.API` | REST controllers, JWT authentication, global exception middleware, static web client |
| Business logic | `GatherUp.BL` | Services for auth, events, finance, polls and notifications |
| Core | `GatherUp.Core` | Domain objects, enums and interfaces (`IRepository`, `IMailService`) |
| Infrastructure | `GatherUp.Infrastructure` | XML-based repositories, data initialization and mail services |

Dependencies point inward. The API and BL layers work against the interfaces defined in Core, and the concrete implementations from Infrastructure are registered with **Dependency Injection** in `Program.cs`. This makes it possible to replace the XML storage or the mail provider without changing the business logic.

## API overview

| Area | Example endpoints |
| --- | --- |
| Auth | `POST /api/auth/login` |
| Events | `POST /api/events/create` · `GET /api/events/my-events` · `PUT /api/events/{id}/edit` · `POST /api/events/{id}/coordinate-schedule` |
| Participants | `GET /api/participants/event/{id}` · `PATCH /api/participants/event/{id}/attendance` · `POST /api/participants/event/{id}/remind` |
| Financial | `GET /api/financial/{id}/suppliers` · `POST /api/financial/{id}/receipts` · `GET /api/financial/{id}/status` |
| Polls | `POST /api/polls/event/{id}` · `POST /api/polls/{pollId}/answer` · `GET /api/polls/{pollId}/questions/{qId}/results` |

## Tech stack

- C#, .NET 8, ASP.NET Web API
- JWT Bearer authentication
- Swagger (Swashbuckle)
- XML file storage (custom repository and serializer)
- SMTP email
- HTML, CSS, JavaScript

## Getting started

1. Open `GatherUpSystem.sln` in Visual Studio 2022, or use the .NET 8 SDK.
2. In `GatherUp.API/appsettings.json`, set your own values for `Jwt:Key` and the `Smtp` section. Do not commit real passwords.
3. Run the API project:

   ```bash
   cd GatherUp.API
   dotnet run
   ```

4. Open the Swagger UI or the web client pages from `wwwroot` in the browser.
