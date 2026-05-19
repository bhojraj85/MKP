# .NET C# API Design Guide for KT Learning Platform

## 1. Overview

Your workspace currently contains requirement and architecture documentation, but does not yet contain a .NET solution or project files. Use this directory to create a new ASP.NET Core API solution under the same root.

Suggested root structure:
```
MKP/
├── src/
│   ├── KTLearningPlatform.API/
│   ├── KTLearningPlatform.Core/
│   ├── KTLearningPlatform.Infrastructure/
│   ├── KTLearningPlatform.Tests/
│
├── docs/
├── scripts/
├── Requirement.md
├── ARCHITECTURE.md
├── DOTNET_API_MODULES.md
└── README.md
```

## 2. Recommended .NET Solution and Projects

### Solution
- `KTLearningPlatform.sln`

### Projects
- `KTLearningPlatform.API` (ASP.NET Core Web API)
- `KTLearningPlatform.Core` (Domain entities, interfaces, DTOs)
- `KTLearningPlatform.Infrastructure` (EF Core, repositories, data access)
- `KTLearningPlatform.Tests` (unit and integration tests)

## 3. Suggested API Modules

### Module 1: Authentication
Responsibility: user login, token issuance, role management.

- Controller: `AuthController`
- Service: `IAuthService`, `AuthService`
- Model: `LoginRequestDto`, `LoginResponseDto`, `JwtTokenDto`
- Entity: `User`, `UserRole`, `UserClaim`
- Endpoint examples:
  - `POST /api/auth/login`
  - `POST /api/auth/refresh-token`
  - `POST /api/auth/logout`

### Module 2: Joiner Management
Responsibility: joiner onboarding, CRUD, temporary ID creation, status updates.

- Controller: `JoinersController`
- Service: `IJoinerService`, `JoinerService`
- Model: `CreateJoinerDto`, `JoinerDto`, `UpdateJoinerDto`, `JoinerDashboardDto`
- Entity: `Joiner`, `TemporaryAccess`, `PermanentAccess`
- Tables: `tbl_Joiners`, `tbl_TemporaryAccess`, `tbl_PermanentAccess`
- Endpoint examples:
  - `POST /api/joiners/create`
  - `GET /api/joiners/list`
  - `GET /api/joiners/{id}`
  - `PUT /api/joiners/{id}`
  - `DELETE /api/joiners/{id}`
  - `GET /api/joiners/{id}/dashboard`

### Module 3: KT Content Management
Responsibility: manage modules, sessions, assets, role-module mapping.

- Controller: `ContentController` or `KTContentController`
- Service: `IKTContentService`, `KTContentService`
- Model: `KTModuleDto`, `KTSessionDto`, `KTAssetDto`, `RoleModuleMappingDto`
- Entity: `KTModule`, `KTSession`, `KTAsset`, `RoleModuleMapping`
- Tables: `tbl_KTModules`, `tbl_KTSessions`, `tbl_KTAssets`, `tbl_RoleModuleMapping`
- Endpoint examples:
  - `POST /api/content/modules`
  - `GET /api/content/modules`
  - `GET /api/content/modules/{id}`
  - `PUT /api/content/modules/{id}`
  - `DELETE /api/content/modules/{id}`
  - `POST /api/content/sessions`
  - `POST /api/content/assets/upload`

### Module 4: Quiz Management
Responsibility: quiz creation, question management, quiz attempts, scoring.

- Controller: `QuizController`
- Service: `IQuizService`, `QuizService`
- Model: `QuizDto`, `QuizQuestionDto`, `QuizOptionDto`, `QuizAttemptDto`, `QuizResultDto`
- Entity: `QuizMaster`, `QuizQuestion`, `QuizOption`, `QuizResponse`, `QuizResponseDetail`
- Tables: `tbl_QuizMaster`, `tbl_QuizQuestions`, `tbl_QuizOptions`, `tbl_QuizResponses`, `tbl_QuizResponseDetails`
- Endpoint examples:
  - `POST /api/quiz/create`
  - `GET /api/quiz/{quizId}`
  - `GET /api/quiz/{quizId}/questions`
  - `POST /api/quiz/{quizId}/start`
  - `POST /api/quiz/submit`
  - `GET /api/quiz/{quizId}/results`

### Module 5: Assessment and Remedial
Responsibility: quiz decision logic, remedial assignment, manager escalation.

- Controller: `AssessmentController`
- Service: `IAssessmentService`, `AssessmentService`
- Model: `AssessmentDto`, `RemedialAssignmentDto`, `ApprovalActionDto`
- Entity: `JoinerAssessment`, `RemedialAssignment`
- Tables: `tbl_JoinerAssessment`, `tbl_RemedialAssignment`
- Endpoint examples:
  - `GET /api/assessment/pending`
  - `POST /api/assessment/approve/{joinerId}`
  - `POST /api/assessment/reject/{joinerId}`
  - `POST /api/assessment/remedial`

### Module 6: Admin Dashboard and Reports
Responsibility: KPI dashboards, reports, compliance, TAT.

- Controller: `AdminDashboardController`, `ReportsController`
- Service: `IAdminService`, `ReportsService`
- Model: `AdminDashboardDto`, `QuizReportDto`, `ComplianceReportDto`, `TATReportDto`
- Endpoint examples:
  - `GET /api/admin/dashboard/metrics`
  - `GET /api/admin/reports/quiz`
  - `GET /api/admin/reports/compliance`
  - `GET /api/admin/reports/tat`

### Module 7: Notifications
Responsibility: email/SMS for temporary ID, quiz result, approval, escalation.

- Service: `INotificationService`, `NotificationService`
- Model: `NotificationRequestDto`
- Components: email templates, SMTP config
- Endpoint examples: internal only, not public.

## 4. Suggested Folder Changes

### Current workspace does not include a source folder.
Recommended structure:
```
MKP/src/KTLearningPlatform.API
MKP/src/KTLearningPlatform.Core
MKP/src/KTLearningPlatform.Infrastructure
MKP/src/KTLearningPlatform.Tests
MKP/docs
MKP/scripts
```

### Use `src/` for code and keep documentation at root.
This keeps the workspace clean and avoids mixing docs with implementation files.

## 5. Suggested Changes and Improvements

### Change 1: Add a solution file and project templates
- Create `KTLearningPlatform.sln`
- Add `KTLearningPlatform.API.csproj`, `KTLearningPlatform.Core.csproj`, `KTLearningPlatform.Infrastructure.csproj`, `KTLearningPlatform.Tests.csproj`

### Change 2: Separate domain, persistence, and web layers
- `Core` contains entities, DTOs, interfaces, constants.
- `Infrastructure` contains DbContext, EF Core configuration, repositories, migrations.
- `API` contains controllers, middleware, dependency injection.
- `Tests` contains unit/integration tests.

### Change 3: Use EF Core with repository/unit-of-work
- `AppDbContext` in Infrastructure
- `IRepository<T>` and `Repository<T>`
- `IUnitOfWork` for transactional operations

### Change 4: Add API versioning and Swagger
- Add `AddApiVersioning()`
- Add `AddSwaggerGen()` with `OpenApiInfo`
- Use versioned routes like `/api/v1/joiners`

### Change 5: Add authentication/authorization early
- Use `JwtBearer` authentication
- Setup roles: `Admin`, `Joiner`, `ContentAdmin`, `Manager`
- Protect controllers with `[Authorize(Roles = "Admin")]` and `[Authorize(Roles = "Joiner")]`

### Change 6: Add config-driven values
- `PassingScore` and `MaxAttempts` should be configurable in `appsettings.json`
- Use application settings for email, JWT, file storage paths

### Change 7: Add audit logging
- Save audit metadata in `tbl_AuditLog`
- Capture `UserId`, `ActionType`, `EntityType`, `OldValue`, `NewValue`

### Change 8: Use clean DTOs for all request/response payloads
- Avoid exposing EF entities directly
- Use AutoMapper for mapping

### Change 9: Store uploaded files in a separate assets folder
- Use `wwwroot/assets/kt` for videos/documents
- Save metadata in DB and file path relative to `wwwroot`

### Change 10: Add database migration scripts
- Use EF Core migrations or SQL scripts under `scripts/db`

## 6. Recommended Module-to-API Mapping

| Module | Controller | Service | Core Entities | Key Tables | Key Endpoints |
|---|---|---|---|---|---|
| Authentication | `AuthController` | `AuthService` | `User`, `Role` | `tbl_Users`, `tbl_Roles` | `POST /api/auth/login` |
| Joiner Management | `JoinersController` | `JoinerService` | `Joiner`, `TemporaryAccess` | `tbl_Joiners`, `tbl_TemporaryAccess` | `POST /api/joiners/create` |
| KT Content | `ContentController` | `KTContentService` | `KTModule`, `KTSession`, `KTAsset` | `tbl_KTModules`, `tbl_KTAssets` | `GET /api/content/modules` |
| Quiz | `QuizController` | `QuizService` | `QuizMaster`, `QuizQuestion` | `tbl_QuizMaster`, `tbl_QuizResponses` | `POST /api/quiz/submit` |
| Assessment | `AssessmentController` | `AssessmentService` | `JoinerAssessment`, `RemedialAssignment` | `tbl_JoinerAssessment` | `POST /api/assessment/approve/{id}` |
| Reports | `AdminDashboardController` / `ReportsController` | `ReportsService` | none | multiple | `GET /api/admin/reports/compliance` |
| Notifications | internal | `NotificationService` | none | none | internal service only |

## 7. Recommended Naming Conventions

- Controllers: `{Resource}Controller` (e.g. `JoinersController`)
- Services: `I{Resource}Service`, `{Resource}Service`
- Models: singular nouns (`Joiner`, `KTModule`)
- DTOs: `{Resource}Dto`, `{Action}{Resource}Dto`
- Repositories: `I{Resource}Repository`, `{Resource}Repository`
- Stored procedures: `sp_{Action}{Resource}`
- Tables: `tbl_{Resource}`

## 8. Next Step for Implementation

1. Create `src/KTLearningPlatform.sln`
2. Generate the four project templates
3. Implement `Core` entities and DTOs first
4. Implement `Infrastructure` DbContext and repository layer
5. Scaffold `API` controllers with stubbed service methods
6. Add `AuthService` and JWT configuration
7. Add module-wise controllers and services one by one
8. Add `Angular` frontend afterwards using same module separation

## 9. Suggested Improvements to Existing Docs

- Move architecture docs into `docs/`
- Create a `docs/ERD.md` and `docs/API_REFERENCE.md`
- Keep `Requirement.md` as source of truth and add version history
- Add `README.md` with development setup instructions for .NET and Angular

---

This module-wise API guide gives you a clear .NET C# architecture to implement in your current workspace. Use the `src/` layout and the suggested changes to build a maintainable ASP.NET Core backend.
