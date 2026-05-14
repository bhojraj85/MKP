# KT Learning Platform - Component-Database-API Mapping Guide

## 📋 Complete Mapping Reference

This document maps every form/component to its corresponding database tables, stored procedures, API endpoints, DTOs, services, and business logic.

---

## 🎯 MASTER FORMS MAPPING

### 1. JOINER MASTER FORM (Admin)
**Purpose:** Create, view, edit, delete joiners

#### Frontend Components
```
├── joiner-master-form.component.ts
│   ├── Form Fields:
│   │   ├── First Name (Required, Min 2 chars)
│   │   ├── Last Name (Required, Min 2 chars)
│   │   ├── Email (Required, Unique, Email format)
│   │   ├── Mobile Number (Required, 10 digits)
│   │   ├── Department (Dropdown - Required)
│   │   ├── Role (Dropdown - Required)
│   │   └── Join Date (Date picker - Required)
│   │
│   ├── Form Actions:
│   │   ├── Create New Joiner [Submit]
│   │   ├── Reset Form [Reset]
│   │   ├── Cancel [Close]
│   │   ├── List View [Navigate]
│   │   └── Search/Filter [Search]
│   │
│   └── Validations:
│       ├── Email uniqueness check
│       ├── Mobile number format
│       └── Date not in future
│
└── joiner-report.component.ts (List View)
    ├── Table Columns:
    │   ├── Joiner Code
    │   ├── Full Name
    │   ├── Email
    │   ├── Mobile
    │   ├── Department
    │   ├── Role
    │   ├── Join Date
    │   └── Status
    │
    ├── Filters:
    │   ├── By Status
    │   ├── By Department
    │   ├── By Role
    │   └── Search by Name/Email
    │
    ├── Pagination:
    │   ├── Page Size (10, 20, 50)
    │   └── Total Records Count
    │
    └── Actions:
        ├── Edit [Open Form]
        ├── View Details
        ├── Delete [Confirmation]
        └── Export to Excel
```

#### Database Tables
```
tbl_Joiners
├── JoinerId (PK)
├── JoinerCode (Auto-generated)
├── FirstName
├── LastName
├── Email (Unique)
├── MobileNumber
├── DepartmentId (FK → tbl_Departments)
├── RoleId (FK → tbl_Roles)
├── JoinDate
├── JoiningStatus (INITIATED)
├── IsActive
├── CreatedOn
└── ModifiedOn

tbl_Departments
├── DepartmentId (PK)
├── DepartmentCode
├── DepartmentName
├── Description
├── IsActive
├── CreatedOn
└── ModifiedOn

tbl_Roles
├── RoleId (PK)
├── RoleCode
├── RoleName
├── Description
├── PassingScore
├── IsActive
├── CreatedOn
└── ModifiedOn

tbl_AuditLog
├── AuditId (PK)
├── UserId
├── ActionType (CREATE, UPDATE, DELETE)
├── EntityType (Joiner)
├── EntityId
├── OldValue
├── NewValue
└── ActionOn
```

#### Stored Procedures
```sql
-- CREATE
sp_CreateNewJoiner
  @FirstName, @LastName, @Email, @MobileNumber, 
  @DepartmentId, @RoleId, @JoinDate

-- READ
sp_GetAllJoiners (@PageNumber, @PageSize)
sp_GetJoinerById (@JoinerId)

-- UPDATE
sp_UpdateJoiner
  @JoinerId, @FirstName, @LastName, @Email, 
  @MobileNumber, @DepartmentId, @RoleId, @JoinDate

-- DELETE
sp_DeleteJoiner (@JoinerId)

-- AUDIT
sp_LogAuditTrail
  @UserId, @ActionType, @EntityType, @EntityId, 
  @OldValue, @NewValue
```

#### API Endpoints
```
POST   /api/joiners/create
       Request: CreateJoinerActionFormDto
       Response: { JoinerId, JoinerCode, TemporaryId }

GET    /api/joiners/list?pageNumber=1&pageSize=10
       Response: PagedResult<JoinerMasterFormDto>

GET    /api/joiners/{id}
       Response: JoinerMasterFormDto

PUT    /api/joiners/{id}
       Request: CreateJoinerActionFormDto
       Response: { Success, Message }

DELETE /api/joiners/{id}
       Response: { Success, Message }
```

#### DTOs
```csharp
// Request
public class CreateJoinerActionFormDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public int DepartmentId { get; set; }
    public int RoleId { get; set; }
    public DateTime JoinDate { get; set; }
}

// Response
public class JoinerMasterFormDto
{
    public int JoinerId { get; set; }
    public string JoinerCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public DateTime JoinDate { get; set; }
    public string JoiningStatus { get; set; }
}
```

#### Services
```csharp
IJoinerService
├── CreateJoinerAsync(dto): Task<JoinerMasterFormDto>
├── GetAllJoinersAsync(pageNumber, pageSize): Task<PagedResult<JoinerMasterFormDto>>
├── GetJoinerByIdAsync(joinerId): Task<JoinerMasterFormDto>
├── UpdateJoinerAsync(joinerId, dto): Task<bool>
└── DeleteJoinerAsync(joinerId): Task<bool>

INotificationService
└── SendTemporaryIdEmailAsync(joinerId, tempId): Task<bool>
```

#### Business Logic
1. Generate unique JoinerCode: "JNR_" + yyyyMMddHHmmss
2. Generate Temporary ID: "TEMP_" + yyyyMMddHHmmss
3. Hash temporary password using bcrypt
4. Auto-assign KT modules based on role
5. Send email with temporary credentials
6. Set initial status to "INITIATED"
7. Create audit log entry

---

### 2. DEPARTMENT MASTER FORM (Admin)
**Purpose:** Manage departments

#### Frontend
- Simple CRUD form
- Dropdown in Joiner form

#### Tables
- tbl_Departments

#### API Endpoints
```
POST   /api/departments/create
GET    /api/departments/list
GET    /api/departments/{id}
PUT    /api/departments/{id}
DELETE /api/departments/{id}
```

#### DTO
```csharp
public class DepartmentDto
{
    public int DepartmentId { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
    public string Description { get; set; }
}
```

---

### 3. ROLE MASTER FORM (Admin)
**Purpose:** Manage roles and assign passing scores

#### Frontend
- CRUD form
- Assign modules to role

#### Tables
- tbl_Roles
- tbl_RoleModuleMapping

#### API Endpoints
```
POST   /api/roles/create
GET    /api/roles/list
GET    /api/roles/{id}
PUT    /api/roles/{id}
DELETE /api/roles/{id}
POST   /api/roles/{id}/modules/assign
```

---

### 4. KT MODULE MASTER FORM (Admin)
**Purpose:** Create and manage KT modules

#### Frontend
- Create/Edit form
- Module structure with sessions
- Drag & drop ordering

#### Tables
- tbl_KTModules
- tbl_KTSessions
- tbl_KTAssets
- tbl_RoleModuleMapping

#### Stored Procedures
```sql
sp_CreateKTModule
sp_UpdateKTModule
sp_DeleteKTModule
sp_GetModuleWithSessions
sp_AssignModuleToRole
```

#### API Endpoints
```
POST   /api/modules/create
GET    /api/modules/list
GET    /api/modules/{id}/full
PUT    /api/modules/{id}
DELETE /api/modules/{id}
POST   /api/modules/{id}/assign-role
```

---

## 🎯 ACTION FORMS MAPPING

### 1. QUIZ MASTER FORM (Admin)
**Purpose:** Create and manage quizzes

#### Frontend Components
```
quiz-master-form.component.ts
├── Basic Info Section:
│   ├── Role (Dropdown - Required)
│   ├── Quiz Name (Required)
│   ├── Quiz Code (Auto-generated)
│   ├── Description (Textarea)
│   ├── Passing Score % (Default: 70)
│   ├── Duration Minutes (Default: 60)
│   ├── Max Attempts (Default: 3)
│   └── Is Active (Toggle)
│
├── Questions Section:
│   ├── Add Question Button
│   ├── Question List:
│   │   ├── Question Text
│   │   ├── Question Type (MCQ/True-False/Fill Blank)
│   │   ├── Marks (Default: 1)
│   │   ├── Options:
│   │   │   ├── Option Text
│   │   │   ├── Mark as Correct (Radio)
│   │   │   └── Remove Option Button
│   │   ├── Add Option Button
│   │   ├── Remove Question Button
│   │   └── Duplicate Question Button
│   │
│   └── Summary:
│       ├── Total Questions Count
│       ├── Total Marks
│       └── Validate All Options Have Correct Answer
│
└── Form Actions:
    ├── Save Quiz
    ├── Preview Quiz
    ├── Publish Quiz
    └── Cancel
```

#### Database Tables
```
tbl_QuizMaster
├── QuizId (PK)
├── RoleId (FK)
├── QuizCode (Auto-generated)
├── QuizName
├── Description
├── TotalQuestions
├── PassingScore
├── DurationMinutes
├── MaxAttempts
├── IsActive
├── CreatedOn
└── ModifiedOn

tbl_QuizQuestions
├── QuestionId (PK)
├── QuizId (FK)
├── QuestionText
├── QuestionType (MCQ, TRUE_FALSE, FILL_BLANK)
├── DisplayOrder
├── Marks
├── IsActive
├── CreatedOn
└── ModifiedOn

tbl_QuizOptions
├── OptionId (PK)
├── QuestionId (FK)
├── OptionText
├── IsCorrect
├── DisplayOrder
└── CreatedOn
```

#### Stored Procedures
```sql
sp_CreateQuizMaster
  @RoleId, @QuizName, @QuizCode, @PassingScore, 
  @DurationMinutes, @MaxAttempts

sp_AddQuestionToQuiz
  @QuizId, @QuestionText, @QuestionType, @Marks, @DisplayOrder

sp_AddQuestionOptions
  @QuestionId, @OptionText, @IsCorrect, @DisplayOrder

sp_UpdateQuiz
sp_PublishQuiz
sp_DeleteQuiz
sp_GetQuizWithQuestions
```

#### API Endpoints
```
POST   /api/quiz/create
       Request: CreateQuizMasterDto
       Response: { QuizId }

PUT    /api/quiz/{id}
       Request: CreateQuizMasterDto

POST   /api/quiz/{id}/publish
       Response: { Success }

GET    /api/quiz/{id}/full
       Response: QuizWithQuestionsDto

DELETE /api/quiz/{id}

POST   /api/quiz/{id}/questions/add
       Request: AddQuestionDto

DELETE /api/quiz/{id}/questions/{questionId}
```

#### DTOs
```csharp
public class CreateQuizMasterDto
{
    public int RoleId { get; set; }
    public string QuizName { get; set; }
    public string Description { get; set; }
    public int PassingScore { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxAttempts { get; set; }
}

public class QuizQuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string QuestionType { get; set; }
    public int Marks { get; set; }
    public List<QuizOptionDto> Options { get; set; }
}

public class QuizOptionDto
{
    public int OptionId { get; set; }
    public string OptionText { get; set; }
}
```

---

### 2. APPROVAL ACTION FORM (Admin)
**Purpose:** Approve or reject joiner based on quiz performance

#### Frontend Components
```
approval-action-form.component.ts
├── Pending List Section:
│   ├── List of joiners with PASSED status
│   ├── Display:
│   │   ├── Joiner Name
│   │   ├── Quiz Score
│   │   ├── Passing Score Required
│   │   └── Approval Status
│   │
│   └── Action: Select for Review
│
├── Approval Form Section:
│   ├── Joiner Information (Read-only):
│   │   ├── Name
│   │   ├── Email
│   │   ├── Department
│   │   ├── Role
│   │   └── Quiz Score
│   │
│   ├── Approval Form:
│   │   ├── Comments (Textarea)
│   │   ├── Approval Status:
│   │   │   ├── Approve Button
│   │   │   └── Reject Button
│   │   │
│   │   └── Form Actions:
│   │       ├── Submit
│   │       └── Cancel
│   │
│   └── Audit Trail:
│       ├── Timestamp
│       ├── Admin Name
│       └── Action
│
└── Auto-refresh List (Poll every 30 seconds)
```

#### Database Tables
```
tbl_JoinerAssessment
├── AssessmentId (PK)
├── JoinerId (FK)
├── FinalScore
├── Status
├── ApprovalStatus (PENDING, APPROVED, REJECTED)
├── ApprovedBy (UserId - FK)
├── ApprovedOn
├── Comments
└── CreatedOn

tbl_PermanentAccess
├── PermanentAccessId (PK)
├── JoinerId (FK)
├── NTId
├── NTIdStatus (PENDING, ACTIVE)
├── CreatedOn
└── ActivatedOn

tbl_AuditLog
└── (Log approval action)
```

#### Stored Procedures
```sql
sp_GetPendingApprovals

sp_ApproveJoiner
  @JoinerId, @ApprovedBy, @Comments

sp_RejectJoiner
  @JoinerId, @ApprovedBy, @RejectionReason

sp_CreatePermanentAccess
  @JoinerId, @NTId
```

#### API Endpoints
```
GET    /api/admin/approval-pending
       Response: JoinerApprovalDto[]

POST   /api/admin/approval/{joinerId}
       Request: ApprovalActionDto
       Response: { Success, NTId? }

GET    /api/admin/approval/{joinerId}/details
       Response: JoinerApprovalDetailDto
```

#### DTOs
```csharp
public class JoinerApprovalDto
{
    public int JoinerId { get; set; }
    public string JoinerName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public string Role { get; set; }
    public decimal Score { get; set; }
    public int PassingScore { get; set; }
}

public class ApprovalActionDto
{
    public string Action { get; set; } // APPROVE, REJECT
    public string Comments { get; set; }
}
```

---

### 3. CONTENT MANAGEMENT ACTION FORM (Admin)
**Purpose:** Upload, manage, and delete KT assets

#### Frontend Components
```
content-management.component.ts
├── Session Selection:
│   ├── Module Dropdown
│   ├── Session Dropdown
│   └── Session Details (Display)
│
├── Upload Section:
│   ├── File Upload Area (Drag & Drop):
│   │   ├── Asset Type (Video, PDF, Document)
│   │   ├── Asset Title
│   │   ├── Select File
│   │   └── Upload Button
│   │
│   ├── Progress Bar
│   │
│   └── Upload Validation:
│       ├── File size max 100MB
│       ├── Allowed formats (mp4, pdf, docx, etc)
│       └── Duplicate check
│
├── Assets List Section:
│   ├── Table:
│   │   ├── Asset Icon
│   │   ├── Asset Title
│   │   ├── Asset Type
│   │   ├── File Size
│   │   ├── Upload Date
│   │   ├── Display Order (Drag-able)
│   │   └── Actions (Edit, Preview, Delete)
│   │
│   └── Bulk Actions:
│       ├── Select All
│       ├── Delete Selected
│       └── Reorder
│
└── Asset Preview:
    ├── Video Player
    ├── PDF Viewer
    └── Document Previewer
```

#### Database Tables
```
tbl_KTAssets
├── AssetId (PK)
├── SessionId (FK)
├── AssetType (VIDEO, PDF, DOCUMENT)
├── AssetTitle
├── AssetUrl
├── AssetPath (Local storage path)
├── FileSize
├── DisplayOrder
├── IsActive
├── CreatedOn
└── ModifiedOn
```

#### Stored Procedures
```sql
sp_CreateKTAsset
  @SessionId, @AssetType, @AssetTitle, @AssetUrl, @AssetPath, @DisplayOrder

sp_UpdateKTAsset
  @AssetId, @AssetTitle, @AssetType, @DisplayOrder

sp_DeleteKTAsset
  @AssetId

sp_GetAssetsBySession
  @SessionId

sp_ReorderAssets
  @SessionId
```

#### API Endpoints
```
POST   /api/content/assets/upload
       Form Data: file, sessionId, assetType, assetTitle
       Response: { AssetId, AssetUrl }

GET    /api/content/assets/session/{sessionId}
       Response: KTAssetDto[]

GET    /api/content/assets/{assetId}
       Response: KTAssetDto

PUT    /api/content/assets/{assetId}
       Request: UpdateAssetDto

DELETE /api/content/assets/{assetId}

PUT    /api/content/assets/reorder
       Request: ReorderAssetDto[]
```

---

## 📊 REPORT FORMS MAPPING

### 1. JOINER REPORT (Admin)
**Purpose:** View all joiners with filtering and search

#### Frontend
```
joiner-report.component.ts
├── Filters:
│   ├── By Status
│   ├── By Department
│   ├── By Role
│   ├── Join Date Range
│   └── Search (Name, Email, Code)
│
├── Table:
│   ├── Joiner Code
│   ├── Full Name
│   ├── Email
│   ├── Mobile
│   ├── Department
│   ├── Role
│   ├── Join Date
│   ├── Joining Status
│   └── Actions (Edit, View, Delete)
│
├── Pagination:
│   ├── Page Size Selector
│   └── Page Navigation
│
└── Export:
    ├── Export to Excel
    └── Export to PDF
```

#### API Endpoint
```
GET /api/joiners/list?status=&department=&role=&search=&pageNumber=1&pageSize=10
```

### 2. QUIZ REPORT (Admin)
**Purpose:** View quiz statistics and performance

#### Frontend
```
quiz-report.component.ts
├── Quiz Selection:
│   └── Quiz Dropdown
│
├── Statistics:
│   ├── Total Attempts
│   ├── Passed Count
│   ├── Failed Count
│   ├── Pass Rate (%)
│   ├── Average Score
│   └── Highest Score
│
├── Chart:
│   └── Pass/Fail Distribution (Pie Chart)
│
└── Detailed Table:
    ├── Joiner Name
    ├── Attempt Number
    ├── Score
    ├── Status
    └── Timestamp
```

#### API Endpoint
```
GET /api/quiz/{id}/reports
    Response: QuizReportDto
```

### 3. COMPLIANCE REPORT (Admin)
**Purpose:** Track SLA and compliance metrics

#### Frontend
```
compliance-report.component.ts
├── Metrics:
│   ├── Temp ID Creation TAT
│   ├── KT Assignment TAT
│   ├── First Attempt Pass Rate
│   ├── Remedial Assignment Rate
│   ├── NT ID Creation TAT
│   ├── Total Compliance Score
│   └── Audit Trail Count
│
├── Charts:
│   ├── TAT Trend (Line Chart)
│   ├── Pass Rate Trend (Line Chart)
│   └── Status Distribution (Bar Chart)
│
└── Table:
    ├── Joiner Name
    ├── Days to Onboarding Complete
    ├── SLA Met (Yes/No)
    └── Remarks
```

#### API Endpoint
```
GET /api/admin/reports/compliance
    Response: ComplianceReportDto
```

---

## 🎯 JOINER PORTAL FORMS MAPPING

### 1. JOINER DASHBOARD COMPONENT

#### Frontend
```
joiner-dashboard.component.ts
├── Welcome Card:
│   ├── Welcome Message (First Name)
│   ├── Role
│   └── Department
│
├── Progress Summary:
│   ├── Overall Progress % (Progress Bar)
│   ├── Modules Completed (e.g., 2/5)
│   ├── Days Remaining (If applicable)
│   └── Current Status
│
├── Modules Grid/List:
│   ├── For each module:
│   │   ├── Module Name
│   │   ├── Progress Bar
│   │   ├── Completion Status
│   │   ├── Sessions Count
│   │   ├── Duration
│   │   └── Start/Continue Button
│   │
│   └── Module Navigation: Next → Quiz
│
├── Quick Links:
│   ├── View Progress
│   ├── Start Quiz
│   ├── View Results
│   └── Profile
│
└── Notifications:
    ├── Quiz Assigned
    ├── Remedial Assigned
    ├── Result Declared
    └── Approval Status
```

#### Database Query
```sql
sp_GetJoinerDashboard @JoinerId
```

#### API Endpoint
```
GET /api/joiners/{id}/dashboard
    Response: JoinerDashboardDto
```

#### DTO
```csharp
public class JoinerDashboardDto
{
    public int JoinerId { get; set; }
    public string FullName { get; set; }
    public string DepartmentName { get; set; }
    public string RoleName { get; set; }
    public string JoiningStatus { get; set; }
    public int TotalModules { get; set; }
    public int CompletedModules { get; set; }
    public decimal OverallProgress { get; set; }
    public List<ModuleProgressDto> Modules { get; set; }
}
```

### 2. LEARNING PROGRESS COMPONENT

#### Frontend
```
learning-progress.component.ts
├── Module Header:
│   ├── Module Name
│   ├── Module Description
│   └── Total Duration
│
├── Sessions List:
│   ├── For each session:
│   │   ├── Session Name
│   │   ├── Session Description
│   │   ├── Duration
│   │   │
│   │   └── Assets List:
│   │       ├── For each asset:
│   │       │   ├── Asset Icon (Video/PDF/Doc)
│   │       │   ├── Asset Title
│   │       │   ├── Asset Size
│   │       │   └── Action (View/Download)
│   │       │
│   │       └── Session Completion Checkbox
│   │
│   └── Session Navigation: Previous → Next
│
├── Content Viewer:
│   ├── Video Player (mp4)
│   ├── PDF Viewer
│   └── Document Previewer
│
├── Progress Tracking:
│   ├── Time Spent Counter
│   ├── Completion Status
│   └── Mark as Complete Button
│
└── Navigation:
    ├── Back to Dashboard
    ├── Next Module
    └── Go to Quiz
```

#### Database Tables
```
tbl_JoinerProgress (Read/Update)
tbl_KTSessions (Read)
tbl_KTAssets (Read)
```

#### API Endpoints
```
GET /api/content/modules/{moduleId}
    Response: ModuleContentDto

PUT /api/joiners/{joinerId}/progress
    Request: UpdateProgressDto
    Response: { Success }

GET /api/content/assets/session/{sessionId}
    Response: AssetDto[]
```

### 3. QUIZ ATTEMPT COMPONENT

#### Frontend
```
quiz-attempt.component.ts
├── Quiz Header:
│   ├── Quiz Name
│   ├── Quiz Description
│   ├── Timer (MM:SS countdown)
│   ├── Questions Counter (e.g., 5/20)
│   └── Progress Bar
│
├── Question Display:
│   ├── Question Number (e.g., Question 5 of 20)
│   ├── Question Text
│   ├── Question Type Icon
│   ├── Question Marks
│   │
│   └── Answer Options:
│       ├── MCQ: Radio buttons
│       ├── True/False: Two buttons
│       └── Fill Blank: Text input
│
├── Navigation:
│   ├── Previous Button (Disabled on Q1)
│   ├── Next Button (Disabled on Last Q)
│   ├── Question Palette (Click to jump)
│   └── Submit Button (Visible on Last Q)
│
├── Timer Alert:
│   ├── Warning at 5 minutes remaining
│   ├── Critical at 1 minute remaining
│   └── Auto-submit when time expires
│
└── Features:
    ├── Save answers
    ├── Clear answer
    ├── Review mode (after submission)
    └── Show correct answers (only after completion)
```

#### Database Tables
```
tbl_QuizMaster (Read)
tbl_QuizQuestions (Read)
tbl_QuizOptions (Read)
tbl_QuizResponses (Create/Update)
tbl_QuizResponseDetails (Create)
```

#### API Endpoints
```
POST /api/quiz/{quizId}/start
     Response: { SessionId, DurationMinutes }

GET /api/quiz/{quizId}/attempt
    Response: QuizQuestionsDto

POST /api/quiz/submit
     Request: SubmitQuizResponseDto
     Response: { Score, Status, Message }
```

#### DTOs
```csharp
public class SubmitQuizResponseDto
{
    public int JoinerId { get; set; }
    public int QuizId { get; set; }
    public int AttemptNumber { get; set; }
    public Dictionary<int, int> Answers { get; set; } // QuestionId -> SelectedOptionId
    public int DurationSeconds { get; set; }
}

public class QuizResponseDto
{
    public decimal Score { get; set; }
    public string Status { get; set; } // PASSED, FAILED
    public string Message { get; set; }
}
```

---

## 📈 ADMIN DASHBOARD COMPONENT

#### Frontend
```
admin-dashboard.component.ts
├── KPI Cards (Real-time):
│   ├── New Joiners (Count)
│   ├── In Progress (Count)
│   ├── Passed (Count)
│   ├── Failed (Count)
│   ├── Completed (Count)
│   ├── Pending Approval (Count)
│   └── With color coding & icons
│
├── Metrics Section:
│   ├── Pass Rate (%)
│   ├── Average TAT (Days)
│   ├── Remedial Assignment Rate (%)
│   └── Compliance Score (%)
│
├── Charts:
│   ├── Status Distribution (Pie Chart)
│   ├── Pass Rate Trend (Line Chart)
│   ├── TAT Trend (Line Chart)
│   └── Department-wise Performance (Bar Chart)
│
├── Quick Actions:
│   ├── Create Joiner
│   ├── Create Quiz
│   ├── Manage Content
│   ├── View Approvals
│   └── Generate Reports
│
├── Recent Activities:
│   ├── Recent Approvals
│   ├── Recent Failures
│   ├── Recent Completions
│   └── Last 10 activities
│
└── Alerts:
    ├── SLA Warnings
    ├── Pending Approvals Count
    ├── System Health
    └── Data Quality Issues
```

#### Database Query
```sql
sp_GetAdminDashboard
```

#### API Endpoint
```
GET /api/admin/dashboard/metrics
    Response: AdminDashboardDto
```

---

## 🔄 DATA FLOW EXAMPLES

### Example 1: Create Joiner Flow
```
1. Admin fills Joiner Master Form
   ↓
2. Form validation (Client-side)
   ↓
3. POST /api/joiners/create with CreateJoinerActionFormDto
   ↓
4. Backend validates input
   ↓
5. sp_CreateNewJoiner Stored Procedure executes:
   ├── Insert into tbl_Joiners
   ├── Generate Temporary ID
   ├── Insert into tbl_TemporaryAccess
   ├── Assign KT Modules (sp_AssignKTModulesToJoiner)
   ├── Insert into tbl_JoinerProgress
   └── Insert Audit Log
   ↓
6. Email sent to Joiner (Temporary ID)
   ↓
7. Response: { JoinerId, JoinerCode, TemporaryId }
   ↓
8. UI shows success message
   ↓
9. Joiner receives email with login credentials
```

### Example 2: Quiz Submission Flow
```
1. Joiner submits Quiz
   ↓
2. POST /api/quiz/submit with SubmitQuizResponseDto
   ↓
3. Backend validates:
   ├── Check if Quiz exists
   ├── Check if Joiner exists
   ├── Check attempt count < max attempts
   └── Validate all answers provided
   ↓
4. sp_SubmitQuizResponse executes:
   ├── Calculate score
   ├── Determine PASS/FAIL (>= passing score?)
   ├── Insert into tbl_QuizResponses
   ├── Insert into tbl_QuizResponseDetails
   └── Update tbl_Joiners status
   ↓
5. Decision Logic:
   ├── IF PASSED:
   │  ├── Update JoiningStatus = "PASSED"
   │  ├── Create entry in tbl_JoinerAssessment
   │  └── Send approval email to Admin
   │
   └── IF FAILED:
      ├── Check if attempts remaining
      ├── IF attempts < 3:
      │  ├── Create RemedialAssignment
      │  ├── Send remedial email to Joiner
      │  └── JoiningStatus = "REMEDIAL_IN_PROGRESS"
      │
      └── IF attempts >= 3:
         ├── Escalate to Manager
         ├── JoiningStatus = "ESCALATED"
         └── Send escalation email
   ↓
6. Response: { Score, Status, NextSteps }
```

### Example 3: Approval Flow
```
1. Admin views Approval Pending List
   ├── GET /api/admin/approval-pending
   └── Display list of PASSED joiners
   ↓
2. Admin selects joiner for approval
   ├── Load JoinerApprovalDto
   └── Display details (score, name, role, etc)
   ↓
3. Admin enters comments and clicks APPROVE
   ↓
4. POST /api/admin/approval/{joinerId}
   with ApprovalActionDto (Action="APPROVE", Comments)
   ↓
5. Backend:
   ├── Validate joiner status = "PASSED"
   ├── sp_ApproveJoiner:
   │  ├── Update tbl_JoinerAssessment (Status = "APPROVED")
   │  ├── Generate NT ID
   │  ├── Insert into tbl_PermanentAccess
   │  └── Update tbl_Joiners (Status = "NT_ID_CREATED")
   │
   ├── Call IT Integration Service (if external)
   ├── Send NT ID email to Joiner
   ├── Log audit trail
   └── Update approvalStatus
   ↓
6. Response: { Success, NTId, Message }
   ↓
7. UI refreshes list
```

---

## 🔗 Complete Request-Response Examples

### Create Joiner - Request & Response
```
REQUEST:
POST /api/joiners/create
Content-Type: application/json

{
  "firstName": "Rajesh",
  "lastName": "Kumar",
  "email": "rajesh.kumar@mahindra.com",
  "mobileNumber": "9876543210",
  "departmentId": 1,
  "roleId": 2,
  "joinDate": "2026-05-20"
}

RESPONSE (201 Created):
{
  "success": true,
  "message": "Joiner created successfully",
  "data": {
    "joinerId": 1,
    "joinerCode": "JNR_20260514093045",
    "tempId": "TEMP_20260514093045",
    "firstName": "Rajesh",
    "lastName": "Kumar",
    "email": "rajesh.kumar@mahindra.com",
    "departmentName": "IT",
    "roleName": "Developer",
    "joiningStatus": "INITIATED"
  }
}
```

### Get Joiner Dashboard - Response
```
RESPONSE (200 OK):
{
  "success": true,
  "data": {
    "joinerId": 1,
    "joinerCode": "JNR_20260514093045",
    "fullName": "Rajesh Kumar",
    "email": "rajesh.kumar@mahindra.com",
    "departmentName": "IT",
    "roleName": "Developer",
    "joiningStatus": "KT_IN_PROGRESS",
    "totalModules": 5,
    "completedModules": 2,
    "overallProgress": 40.0,
    "modules": [
      {
        "moduleId": 1,
        "moduleName": "Core Java",
        "progress": 100,
        "status": "COMPLETED",
        "sessions": 3,
        "completedSessions": 3
      },
      {
        "moduleId": 2,
        "moduleName": "Spring Framework",
        "progress": 50,
        "status": "IN_PROGRESS",
        "sessions": 4,
        "completedSessions": 2
      }
    ]
  }
}
```

---

This comprehensive mapping guide provides complete reference for developers during implementation!
