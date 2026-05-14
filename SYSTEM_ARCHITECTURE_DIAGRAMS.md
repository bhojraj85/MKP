# System Architecture Diagrams & Visual Workflows

## 🎯 Complete System Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                        KT LEARNING PLATFORM - COMPLETE FLOW                     │
└─────────────────────────────────────────────────────────────────────────────────┘

                          ┌──────────────────────┐
                          │    ADMIN LOGIN       │
                          │   (AD/NT Domain)     │
                          └──────────┬───────────┘
                                     │
                    ┌────────────────┼────────────────┐
                    │                │                │
          ┌─────────▼────────┐  ┌────▼─────────┐  ┌──▼──────────────┐
          │  Create Joiner   │  │ Manage KT    │  │ Quiz Master     │
          │  (Master Form)   │  │ Content      │  │ (Master Form)   │
          │                  │  │ (Upload PDFs)│  │ (Questions)     │
          └────────┬─────────┘  └────┬─────────┘  └─────┬───────────┘
                   │                 │                  │
                   └─────────────────┼──────────────────┘
                                     │
                    ┌────────────────▼────────────────┐
                    │  DATABASE OPERATIONS            │
                    │  • tbl_Joiners                  │
                    │  • tbl_KTModules                │
                    │  • tbl_QuizMaster               │
                    │  • tbl_KTAssets                 │
                    └────────────────┬────────────────┘
                                     │
                    ┌────────────────▼────────────────┐
                    │  EMAIL NOTIFICATION             │
                    │  • Temp ID sent to Joiner       │
                    └────────────────┬────────────────┘
                                     │
                          ┌──────────▼─────────┐
                          │   JOINER LOGIN     │
                          │   (Temp ID Auth)   │
                          └──────────┬─────────┘
                                     │
                ┌────────────────────┼────────────────────┐
                │                    │                    │
       ┌────────▼────────┐  ┌────────▼────────┐  ┌────────▼────────┐
       │  DASHBOARD      │  │  KT LEARNING    │  │  QUIZ ATTEMPT   │
       │  • Progress     │  │  • Watch Videos │  │  • MCQ Questions│
       │  • Modules List │  │  • Read PDFs    │  │  • Timer        │
       │  • Status       │  │  • Complete Docs│  │  • Submit Ans   │
       └────────┬────────┘  └────────┬────────┘  └────────┬────────┘
                │                    │                    │
                └────────────────────┼────────────────────┘
                                     │
                    ┌────────────────▼────────────────┐
                    │  UPDATE PROGRESS & SCORE        │
                    │  • tbl_JoinerProgress           │
                    │  • tbl_QuizResponses            │
                    └────────────────┬────────────────┘
                                     │
                          ┌──────────▼─────────┐
                          │  DECISION GATE      │
                          │  Score >= 70% ?     │
                          └──────┬──────────┬──┘
                                 │          │
                        ┌────────▼────┐ ┌──▼────────────┐
                        │   PASS      │ │   FAIL        │
                        └────────┬────┘ └───┬───────────┘
                                 │          │
                    ┌────────────▼────┐  ┌─▼──────────────┐
                    │ Send Approval   │  │ Remedial KT    │
                    │ Email to Admin  │  │ Assignment     │
                    │ tbl_Joiner      │  │ Attempt Count  │
                    │ Assessment      │  └─┬──────────────┘
                    │ (PASSED)        │    │
                    └────────┬────────┘    │
                             │         ┌───▼──────────┐
                             │         │ Attempts < 3 │
                             │         └───┬──────────┘
                             │             │
                    ┌────────▼────────┐    │
                    │  ADMIN APPROVAL │◄───┘
                    │  (Action Form)  │
                    │  • Approve      │
                    │  • Reject       │
                    └────────┬────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
         ┌────▼──────┐              ┌─────▼──────┐
         │ APPROVED  │              │  REJECTED  │
         └────┬──────┘              └────────────┘
              │
    ┌─────────▼──────────┐
    │ IT System Integration
    │ Create NT ID
    │ tbl_PermanentAccess
    └─────────┬──────────┘
              │
    ┌─────────▼──────────┐
    │ Send Activation    │
    │ Email to Joiner    │
    └─────────┬──────────┘
              │
    ┌─────────▼──────────┐
    │ PROCESS COMPLETE   │
    │ Joiner Active      │
    └────────────────────┘
```

---

## 🗄️ Database Schema Relationship Diagram

```
┌──────────────────┐
│  tbl_Departments │
└────────┬─────────┘
         │ 1
         │
         │ n
    ┌────▼──────────────┐      ┌─────────────────┐
    │  tbl_Joiners      │◄─────┤  tbl_Roles      │
    │                   │      │                 │
    │ • JoinerId (PK)   │      │ • RoleId (PK)   │
    │ • DepartmentId(FK)│      │ • RoleName      │
    │ • RoleId (FK)     │      │ • PassingScore  │
    │ • JoiningStatus   │      └────────┬────────┘
    │ • Email           │               │ 1
    └────┬──────────────┘               │ n
         │ 1                            │
         │ n            ┌───────────────▼──────────┐
         │              │  tbl_RoleModuleMapping   │
         │              │                          │
         │              │ • RoleId (FK)            │
         ├──────────────┼─ • ModuleId (FK)         │
         │              └──────────┬────────────────┘
         │                         │
         │         ┌───────────────▼────────────┐
         │         │  tbl_KTModules             │
         │         │                            │
         │         │ • ModuleId (PK)            │
         │         │ • ModuleName               │
         │         │ • DurationMinutes          │
         │         └───────────┬────────────────┘
         │                     │ 1
         │                     │ n
         │         ┌───────────▼──────────┐
         │         │  tbl_KTSessions      │
         │         │                      │
         │         │ • SessionId (PK)     │
         │         │ • ModuleId (FK)      │
         │         │ • SessionName        │
         │         └───────────┬──────────┘
         │                     │ 1
         │                     │ n
         │         ┌───────────▼──────────┐
         │         │  tbl_KTAssets        │
         │         │                      │
         │         │ • AssetId (PK)       │
         │         │ • SessionId (FK)     │
         │         │ • AssetType          │
         │         │ • AssetUrl           │
         │         └─────────────────────┘
         │
         │         ┌──────────────────────────┐
         └─────────┤  tbl_JoinerProgress      │
                   │                          │
                   │ • ProgressId (PK)        │
                   │ • JoinerId (FK)          │
                   │ • ModuleId (FK)          │
                   │ • SessionId (FK)         │
                   │ • AssetId (FK)           │
                   │ • CompletionStatus       │
                   │ • CompletionPercentage   │
                   └──────────────────────────┘

    ┌──────────────────┐
    │  tbl_QuizMaster  │
    │                  │
    │ • QuizId (PK)    │
    │ • RoleId (FK)────┼──────────────┐
    │ • PassingScore   │              │ n
    │ • TotalQuestions │              │
    └────────┬─────────┘              │
             │ 1                      │ 1
             │ n                 ┌────▼──────────────┐
             │      ┌────────────┤  tbl_Roles       │
        ┌────▼──────┤            └──────────────────┘
        │            │
    ┌───▼───────────────────┐
    │  tbl_QuizQuestions    │
    │                       │
    │ • QuestionId (PK)     │
    │ • QuizId (FK)         │
    │ • QuestionText        │
    └────────┬──────────────┘
             │ 1
             │ n
    ┌────────▼──────────────┐
    │  tbl_QuizOptions      │
    │                       │
    │ • OptionId (PK)       │
    │ • QuestionId (FK)     │
    │ • OptionText          │
    │ • IsCorrect           │
    └───────────────────────┘

    ┌──────────────────────────┐
    │  tbl_QuizResponses       │
    │                          │
    │ • ResponseId (PK)        │
    │ • JoinerId (FK)──────────┼──────┐
    │ • QuizId (FK)────────────┼─┐    │
    │ • AttemptNumber          │ │    │
    │ • ObtainedMarks          │ │    │
    │ • Score                  │ │    │
    │ • Status                 │ │    │
    │ • IsLatest               │ │    │
    └────────┬─────────────────┘ │    │
             │ 1                 │    │
             │ n                 │    │ n
             │      ┌────────────▼───┐
        ┌────▼──────────┐            │
        │ tbl_Joiners   │◄───────────┘
        └───────────────┘

    ┌──────────────────────────┐
    │  tbl_QuizResponseDetails │
    │                          │
    │ • DetailId (PK)          │
    │ • ResponseId (FK)        │
    │ • QuestionId (FK)        │
    │ • SelectedOptionId (FK)  │
    │ • IsCorrect              │
    │ • MarksObtained          │
    └──────────────────────────┘
```

---

## 🔄 API Call Flow Diagram

```
╔════════════════════════════════════════════════════════════════╗
║              ANGULAR FRONTEND → ASP.NET CORE API               ║
╚════════════════════════════════════════════════════════════════╝

JOINER PORTAL:
┌─────────────────────┐
│  Joiner Login       │──► POST /api/auth/login
└─────────────────────┘     [TemporaryId, Password]
        │                   ◄── JWT Token
        │
┌───────▼────────────────┐
│  Joiner Dashboard      │──► GET /api/joiners/{id}/dashboard
└────────────────────────┘     ◄── JoinerDashboardDto
        │
┌───────▼────────────────┐
│  Load KT Modules       │──► GET /api/contentManagement/modules/{moduleId}
└────────────────────────┘     ◄── KTModuleDto[]
        │
┌───────▼────────────────┐
│  View Learning Content │──► GET /api/contentManagement/assets/session/{sessionId}
└────────────────────────┘     ◄── KTAssetDto[]
        │
┌───────▼────────────────┐
│  Update Progress       │──► PUT /api/joiners/{id}/progress
└────────────────────────┘     [ModuleId, SessionId, AssetId, Status]
        │
┌───────▼────────────────┐
│  Start Quiz            │──► POST /api/quiz/{quizId}/start
└────────────────────────┘     ◄── QuizMasterDto
        │
┌───────▼────────────────┐
│  Attempt Quiz          │──► GET /api/quiz/{quizId}/attempt
└────────────────────────┘     ◄── QuizQuestionDto[]
        │
┌───────▼────────────────┐
│  Submit Responses      │──► POST /api/quiz/submit
└────────────────────────┘     [JoinerId, QuizId, Responses]
        │                       ◄── { Score, Status }
        │
        ├─► IF PASS:
        │   POST /api/admin/approval-pending
        │       [JoinerId → Admin Approval Queue]
        │
        └─► IF FAIL:
            POST /api/admin/remedial-assignment
                [JoinerId → Remedial Assignment]


ADMIN PORTAL:
┌─────────────────────┐
│   Admin Login       │──► POST /api/auth/login
└─────────────────────┘     [NTId, Password]
        │                   ◄── JWT Token
        │
┌───────▼────────────────┐
│  Admin Dashboard       │──► GET /api/adminDashboard/metrics
└────────────────────────┘     ◄── AdminDashboardDto
        │
├───────┼────────────┬──────────┬─────────────┐
│       │            │          │             │
│   ┌───▼─────┐  ┌──▼──┐   ┌───▼──┐      ┌──▼──────┐
│   │ Joiner  │  │Quiz │   │Content│     │Approval │
│   │ Master  │  │Master   │Mgmt    │     │Actions  │
│   │ Form    │  │Form    │        │     │         │
│   └───┬─────┘  └──┬──┘   └───┬──┘      └──┬──────┘
│       │           │           │           │
│   POST │      POST │       POST│       POST│
│  /api/ │    /api/  │     /api/ │      /api/
│joiners/│   quiz/   │content/   │    admin/
│ create │  create   │  assets/  │ approval/
│        │           │  upload   │    {id}
│        │           │           │
└────────┴───────────┴───────────┴───────────

        GET /api/joiners/list (Master Report)
        GET /api/quiz/reports (Quiz Report)
        GET /api/adminDashboard/approval-pending (Approval List)
        GET /api/adminDashboard/reports/compliance (Compliance Report)
```

---

## 💾 Database to API to Frontend Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    DATA FLOW ARCHITECTURE                   │
└─────────────────────────────────────────────────────────────┘

LAYER 1: DATABASE (SQL Server)
┌─────────────────────────────────────┐
│  Tables                             │
│  ├─ tbl_Joiners                     │
│  ├─ tbl_Departments                 │
│  ├─ tbl_Roles                       │
│  ├─ tbl_KTModules                   │
│  ├─ tbl_QuizMaster                  │
│  ├─ tbl_QuizResponses               │
│  └─ [13 more tables]                │
└────────────┬────────────────────────┘
             │
    ┌────────▼──────────────────────┐
    │  Stored Procedures            │
    │  sp_CreateNewJoiner()         │
    │  sp_GetJoinerDashboard()      │
    │  sp_SubmitQuizResponse()      │
    │  sp_GetAdminDashboard()       │
    │  [10+ more SPs]               │
    └────────┬──────────────────────┘
             │
LAYER 2: BACKEND (ASP.NET Core)
             │
    ┌────────▼──────────────────────┐
    │  Data Access Layer            │
    │  • DbContext                  │
    │  • Repositories               │
    │  • UnitOfWork Pattern         │
    └────────┬──────────────────────┘
             │
    ┌────────▼──────────────────────┐
    │  Business Logic Layer         │
    │  • Services (Interfaces)      │
    │  • Service Implementations    │
    │  • Validation Logic           │
    │  • Business Rules             │
    └────────┬──────────────────────┘
             │
    ┌────────▼──────────────────────┐
    │  API Layer                    │
    │  • Controllers                │
    │  • Action Methods             │
    │  • Request/Response DTOs      │
    │  • Middleware                 │
    │  • Authorization              │
    └────────┬──────────────────────┘
             │
LAYER 3: FRONTEND (Angular)
             │
    ┌────────▼──────────────────────┐
    │  Services                     │
    │  • HttpClient Calls           │
    │  • API Communication          │
    │  • Token Management           │
    │  • Error Handling             │
    └────────┬──────────────────────┘
             │
    ┌────────▼──────────────────────┐
    │  Components                   │
    │  • Joiner Dashboard           │
    │  • Admin Dashboard            │
    │  • Forms (Master/Action)      │
    │  • Reports                    │
    │  • Quiz Attempt               │
    └────────┬──────────────────────┘
             │
    ┌────────▼──────────────────────┐
    │  View Layer (HTML/CSS)        │
    │  • User Interface             │
    │  • User Experience            │
    │  • Charts & Visualizations    │
    └────────────────────────────────┘
```

---

## 🎨 Frontend Component Hierarchy

```
AppComponent
├── AuthModule
│   ├── LoginComponent
│   │   └── JoinerLoginComponent
│   │   └── AdminLoginComponent
│   └── AuthGuard
│
├── JoinerModule (Route: /joiner)
│   ├── JoinerDashboardComponent
│   │   ├── WelcomeCardComponent
│   │   ├── ProgressSummaryComponent
│   │   └── ModulesGridComponent
│   │
│   ├── LearningProgressComponent
│   │   ├── SessionsListComponent
│   │   └── AssetsPlayerComponent
│   │
│   ├── QuizAttemptComponent
│   │   ├── QuestionDisplayComponent
│   │   ├── OptionsComponent
│   │   ├── TimerComponent
│   │   └── NavigationComponent
│   │
│   └── ProfileComponent
│       └── PersonalDetailsComponent
│
└── AdminModule (Route: /admin)
    ├── AdminDashboardComponent
    │   ├── KPICardsComponent
    │   ├── MetricsComponent
    │   └── ChartsComponent
    │
    ├── JoinerMasterFormComponent
    │   └── FormComponent
    │
    ├── JoinerActionFormComponent
    │   └── FormComponent
    │
    ├── QuizMasterFormComponent
    │   ├── BasicInfoComponent
    │   ├── QuestionsComponent
    │   └── OptionsComponent
    │
    ├── QuizReportComponent
    │   ├── FilterComponent
    │   └── TableComponent
    │
    ├── ContentManagementComponent
    │   ├── UploadComponent
    │   └── AssetListComponent
    │
    ├── ApprovalActionFormComponent
    │   ├── PendingListComponent
    │   └── ApprovalFormComponent
    │
    └── ComplianceReportComponent
        └── TableComponent
```

---

## 🔐 Authentication & Authorization Flow

```
┌─────────────────────────────────────────────────┐
│         AUTHENTICATION & AUTHORIZATION FLOW      │
└─────────────────────────────────────────────────┘

LOGIN REQUEST:
┌─────────────────┐
│  User Provides  │
│  Credentials    │
└────────┬────────┘
         │
    ┌────▼─────────────────┐
    │ POST /api/auth/login │
    │ [TemporaryId/NTId,   │
    │  Password]           │
    └────────┬─────────────┘
             │
    ┌────────▼──────────────────────┐
    │  Backend Validation           │
    │  1. Check if user exists      │
    │  2. Hash & verify password    │
    │  3. Verify role/permissions   │
    └────────┬──────────────────────┘
             │
        ┌────▼────┐
        │  VALID? │
        └────┬────┘
             │
        ┌────┴─────────────┐
        │                  │
    ┌───▼──┐          ┌────▼──┐
    │ YES  │          │  NO   │
    └───┬──┘          └───┬───┘
        │                 │
        │          ┌──────▼────────┐
        │          │  401 Error    │
        │          │  "Invalid     │
        │          │  Credentials" │
        │          └───────────────┘
        │
    ┌───▼────────────────────┐
    │  Generate JWT Token    │
    │  Payload:              │
    │  • JoinerId/AdminId    │
    │  • Username            │
    │  • Role                │
    │  • Expiry              │
    └────────┬───────────────┘
             │
    ┌────────▼──────────────┐
    │ Return JWT Token      │
    │ { token: "xxx..." }   │
    └────────┬──────────────┘
             │
    ┌────────▼──────────────┐
    │  Store Token in       │
    │  • LocalStorage       │
    │  • Angular Service    │
    └────────┬──────────────┘
             │
AUTHORIZED REQUESTS:
    ┌────────▼──────────────┐
    │  Add JWT to Header    │
    │  Authorization:       │
    │  Bearer <token>       │
    └────────┬──────────────┘
             │
    ┌────────▼──────────────┐
    │  Backend Receives     │
    │  Request             │
    └────────┬──────────────┘
             │
    ┌────────▼──────────────┐
    │  Middleware Validates │
    │  • Verify Token       │
    │  • Extract Claims     │
    │  • Check Expiry       │
    └────────┬──────────────┘
             │
        ┌────▼────┐
        │ VALID?  │
        └────┬────┘
             │
        ┌────┴──────────┐
        │               │
    ┌───▼──┐        ┌───▼───┐
    │ YES  │        │  NO   │
    └───┬──┘        └───┬───┘
        │               │
        │        ┌──────▼────────┐
        │        │  401 Error    │
        │        │  "Unauthorized│
        │        │  Token"       │
        │        └───────────────┘
        │
    ┌───▼───────────────────┐
    │  Extract Role & User  │
    │  from Token Claims    │
    └───────┬───────────────┘
            │
    ┌───────▼──────────────────┐
    │  Check Authorization     │
    │  [Authorize(Roles="Admin")]
    │  [Authorize(Roles="Joiner")]
    └───────┬──────────────────┘
            │
        ┌───▼────┐
        │ MATCH? │
        └───┬────┘
            │
        ┌───┴──────────┐
        │              │
    ┌───▼──┐       ┌───▼───┐
    │ YES  │       │  NO   │
    └───┬──┘       └───┬───┘
        │              │
        │       ┌──────▼────────┐
        │       │  403 Error    │
        │       │  "Forbidden"  │
        │       └───────────────┘
        │
    ┌───▼────────────────────┐
    │  Allow Request         │
    │  Execute Action        │
    │  Return 200 Response   │
    └────────────────────────┘
```

---

## 📊 Data Transformation Pipeline

```
DATABASE → API → FRONTEND

Example: Joiner Dashboard

Step 1: Database Query
┌──────────────────────────────────────┐
│ EXEC sp_GetJoinerDashboard @JoinerId │
└──────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────┐
│ SQL Result Set:                      │
│ ┌────────────────────────────────┐  │
│ │ JoinerId    │ 123              │  │
│ │ JoinerCode  │ JNR_20260514...  │  │
│ │ FirstName   │ John             │  │
│ │ LastName    │ Doe              │  │
│ │ Department  │ IT               │  │
│ │ TotalModules│ 5                │  │
│ │ Completed   │ 2                │  │
│ │ Progress    │ 40.50            │  │
│ └────────────────────────────────┘  │
└──────────────────────────────────────┘
           │
Step 2: Map to Entity
           ▼
┌──────────────────────────────────────┐
│ Backend Repository:                  │
│ Joiner entity = mapper.Map(          │
│     sqlResult                        │
│ )                                    │
└──────────────────────────────────────┘
           │
Step 3: Map to DTO
           ▼
┌──────────────────────────────────────┐
│ Backend Service:                     │
│ JoinerDashboardDto dto = mapper.Map( │
│     joiner entity                    │
│ )                                    │
└──────────────────────────────────────┘
           │
Step 4: Return from API
           ▼
┌──────────────────────────────────────┐
│ API Response (JSON):                 │
│ {                                    │
│   "joinerId": 123,                   │
│   "fullName": "John Doe",            │
│   "departmentName": "IT",            │
│   "totalModules": 5,                 │
│   "completedModules": 2,             │
│   "overallProgress": 40.50           │
│ }                                    │
└──────────────────────────────────────┘
           │
Step 5: Receive in Angular
           ▼
┌──────────────────────────────────────┐
│ Angular Service:                     │
│ this.http.get<JoinerDashboardDto>(   │
│     '/api/joiners/{id}/dashboard'    │
│ ).subscribe(data => {                │
│     this.dashboard = data;           │
│ })                                   │
└──────────────────────────────────────┘
           │
Step 6: Bind to Component
           ▼
┌──────────────────────────────────────┐
│ Component Property:                  │
│ dashboard: JoinerDashboardDto        │
│ {                                    │
│   joinerId: 123,                     │
│   fullName: "John Doe",              │
│   departmentName: "IT",              │
│   totalModules: 5,                   │
│   completedModules: 2,               │
│   overallProgress: 40.50             │
│ }                                    │
└──────────────────────────────────────┘
           │
Step 7: Display in Template
           ▼
┌──────────────────────────────────────┐
│ HTML/Template:                       │
│ <h1>Welcome, {{ dashboard.fullName }}│
│ <p>{{ completedModules }}/          │
│    {{ totalModules }}</p>            │
│ <div class="progress"                │
│      [style.width]="overallProgress" │
│ </div>                               │
└──────────────────────────────────────┘
```

This comprehensive visual architecture will help your team understand the complete system!
