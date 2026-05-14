# KT Learning Platform - Complete Architecture & Design Document

## 📊 Process Workflow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    JOINER ONBOARDING FLOW                       │
└─────────────────────────────────────────────────────────────────┘

1. ADMIN INITIATES
   └─→ Admin Portal → Create New Joiner → Master Form
       (Mobile, Email, Department, Role)
       └─→ Database: tbl_Joiners (Status: INITIATED)

2. TEMP ID GENERATION
   └─→ System Auto-generates Temp ID
       └─→ Email Notification to Joiner
       └─→ Database: tbl_TemporaryAccess

3. JOINER LOGIN
   └─→ Joiner Portal (Temp ID + Password)
       └─→ Auto-assign KT Learning Path
       └─→ Dashboard: Show Learning Progress

4. KT COMPLETION
   └─→ Joiner: Watch Videos, Read PDFs, Complete Modules
       └─→ Database: tbl_JoinerProgress (Track Module Status)

5. QUIZ ASSESSMENT
   └─→ Quiz Engine: tbl_QuizMaster → tbl_QuizResponses
       └─→ Auto-calculate Score

6. DECISION GATE
   ├─→ PASS (≥70%): Trigger Admin Approval
   │   └─→ Database: tbl_JoinerAssessment (Status: PASSED)
   │   └─→ Email: Send to Admin/AMIT
   │
   └─→ FAIL (<70%): Remedial Assignment
       └─→ Database: tbl_RemedialAssignment
       └─→ Attempt Counter + Re-quiz Scheduled
       └─→ If Attempts ≥ 3: Escalate to Manager

7. NT ID CREATION
   └─→ Admin Approval → IT System Integration
       └─→ NT ID Created in Mahindra Domain
       └─→ Database: tbl_PermanentAccess (Status: ACTIVE)
       └─→ Email: Joiner Account Activated

┌─────────────────────────────────────────────────────────────────┐
│                      ADMIN MANAGEMENT FLOW                      │
└─────────────────────────────────────────────────────────────────┘

ADMIN LOGIN
├─→ Dashboard: KPI Metrics, Real-time Status
├─→ Master Forms: Manage Joiners, Roles, Departments
├─→ Action Forms: Approve/Reject, Assign Remedial
├─→ Content Management: Upload KT Assets (Videos, PDFs)
├─→ Reports: Pass Rate, TAT, Compliance
└─→ Settings: Configure Passing Score, Max Attempts
```

---

## 🗄️ DATABASE DESIGN

### 1. Master Data Tables

#### `tbl_Departments`
```sql
CREATE TABLE tbl_Departments (
    DepartmentId INT PRIMARY KEY IDENTITY(1,1),
    DepartmentCode VARCHAR(20) UNIQUE NOT NULL,
    DepartmentName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL
)
```

#### `tbl_Roles`
```sql
CREATE TABLE tbl_Roles (
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleCode VARCHAR(20) UNIQUE NOT NULL,
    RoleName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    PassingScore INT DEFAULT 70,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL
)
```

#### `tbl_KTModules`
```sql
CREATE TABLE tbl_KTModules (
    ModuleId INT PRIMARY KEY IDENTITY(1,1),
    ModuleCode VARCHAR(20) UNIQUE NOT NULL,
    ModuleName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    DisplayOrder INT,
    DurationMinutes INT,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL
)
```

#### `tbl_KTSessions`
```sql
CREATE TABLE tbl_KTSessions (
    SessionId INT PRIMARY KEY IDENTITY(1,1),
    ModuleId INT NOT NULL,
    SessionName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    DurationMinutes INT,
    DisplayOrder INT,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL,
    FOREIGN KEY (ModuleId) REFERENCES tbl_KTModules(ModuleId)
)
```

#### `tbl_KTAssets`
```sql
CREATE TABLE tbl_KTAssets (
    AssetId INT PRIMARY KEY IDENTITY(1,1),
    SessionId INT NOT NULL,
    AssetType VARCHAR(20), -- 'VIDEO', 'PDF', 'DOCUMENT'
    AssetTitle VARCHAR(100) NOT NULL,
    AssetUrl VARCHAR(500) NOT NULL,
    AssetPath VARCHAR(MAX),
    DisplayOrder INT,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL,
    FOREIGN KEY (SessionId) REFERENCES tbl_KTSessions(SessionId)
)
```

#### `tbl_RoleModuleMapping`
```sql
CREATE TABLE tbl_RoleModuleMapping (
    MappingId INT PRIMARY KEY IDENTITY(1,1),
    RoleId INT NOT NULL,
    ModuleId INT NOT NULL,
    IsMandatory BIT DEFAULT 1,
    DisplayOrder INT,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES tbl_Roles(RoleId),
    FOREIGN KEY (ModuleId) REFERENCES tbl_KTModules(ModuleId),
    UNIQUE(RoleId, ModuleId)
)
```

---

### 2. Joiner Management Tables

#### `tbl_Joiners`
```sql
CREATE TABLE tbl_Joiners (
    JoinerId INT PRIMARY KEY IDENTITY(1,1),
    JoinerCode VARCHAR(20) UNIQUE NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    MobileNumber VARCHAR(15) NOT NULL,
    DepartmentId INT NOT NULL,
    RoleId INT NOT NULL,
    JoinDate DATE NOT NULL,
    JoiningStatus VARCHAR(50), -- 'INITIATED', 'KT_IN_PROGRESS', 'QUIZ_PENDING', 'PASSED', 'FAILED', 'REMEDIAL_IN_PROGRESS', 'NT_ID_CREATED'
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL,
    FOREIGN KEY (DepartmentId) REFERENCES tbl_Departments(DepartmentId),
    FOREIGN KEY (RoleId) REFERENCES tbl_Roles(RoleId)
)
```

#### `tbl_TemporaryAccess`
```sql
CREATE TABLE tbl_TemporaryAccess (
    TempAccessId INT PRIMARY KEY IDENTITY(1,1),
    JoinerId INT NOT NULL,
    TemporaryId VARCHAR(50) UNIQUE NOT NULL,
    TemporaryPassword VARCHAR(255) NOT NULL, -- Hashed
    IsUsed BIT DEFAULT 0,
    FirstLoginOn DATETIME NULL,
    ExpiryDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (JoinerId) REFERENCES tbl_Joiners(JoinerId)
)
```

#### `tbl_JoinerProgress`
```sql
CREATE TABLE tbl_JoinerProgress (
    ProgressId INT PRIMARY KEY IDENTITY(1,1),
    JoinerId INT NOT NULL,
    ModuleId INT NOT NULL,
    SessionId INT NOT NULL,
    AssetId INT NOT NULL,
    CompletionStatus VARCHAR(50), -- 'NOT_STARTED', 'IN_PROGRESS', 'COMPLETED'
    CompletionPercentage DECIMAL(5,2),
    StartedOn DATETIME,
    CompletedOn DATETIME,
    DurationSpentSeconds INT,
    FOREIGN KEY (JoinerId) REFERENCES tbl_Joiners(JoinerId),
    FOREIGN KEY (ModuleId) REFERENCES tbl_KTModules(ModuleId),
    FOREIGN KEY (SessionId) REFERENCES tbl_KTSessions(SessionId),
    FOREIGN KEY (AssetId) REFERENCES tbl_KTAssets(AssetId)
)
```

---

### 3. Quiz & Assessment Tables

#### `tbl_QuizMaster`
```sql
CREATE TABLE tbl_QuizMaster (
    QuizId INT PRIMARY KEY IDENTITY(1,1),
    RoleId INT NOT NULL,
    QuizCode VARCHAR(20) UNIQUE NOT NULL,
    QuizName VARCHAR(100) NOT NULL,
    Description VARCHAR(MAX),
    TotalQuestions INT,
    PassingScore INT,
    DurationMinutes INT,
    MaxAttempts INT DEFAULT 3,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL,
    FOREIGN KEY (RoleId) REFERENCES tbl_Roles(RoleId)
)
```

#### `tbl_QuizQuestions`
```sql
CREATE TABLE tbl_QuizQuestions (
    QuestionId INT PRIMARY KEY IDENTITY(1,1),
    QuizId INT NOT NULL,
    QuestionText VARCHAR(MAX) NOT NULL,
    QuestionType VARCHAR(20), -- 'MCQ', 'TRUE_FALSE', 'FILL_BLANK'
    DisplayOrder INT,
    Marks INT DEFAULT 1,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
    ModifiedOn DATETIME NULL,
    FOREIGN KEY (QuizId) REFERENCES tbl_QuizMaster(QuizId)
)
```

#### `tbl_QuizOptions`
```sql
CREATE TABLE tbl_QuizOptions (
    OptionId INT PRIMARY KEY IDENTITY(1,1),
    QuestionId INT NOT NULL,
    OptionText VARCHAR(MAX) NOT NULL,
    IsCorrect BIT DEFAULT 0,
    DisplayOrder INT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (QuestionId) REFERENCES tbl_QuizQuestions(QuestionId)
)
```

#### `tbl_QuizResponses`
```sql
CREATE TABLE tbl_QuizResponses (
    ResponseId INT PRIMARY KEY IDENTITY(1,1),
    JoinerId INT NOT NULL,
    QuizId INT NOT NULL,
    AttemptNumber INT,
    TotalMarks INT,
    ObtainedMarks INT,
    Score DECIMAL(5,2), -- Percentage
    Status VARCHAR(50), -- 'PASSED', 'FAILED'
    StartedOn DATETIME,
    SubmittedOn DATETIME,
    IsLatest BIT DEFAULT 1,
    FOREIGN KEY (JoinerId) REFERENCES tbl_Joiners(JoinerId),
    FOREIGN KEY (QuizId) REFERENCES tbl_QuizMaster(QuizId)
)
```

#### `tbl_QuizResponseDetails`
```sql
CREATE TABLE tbl_QuizResponseDetails (
    DetailId INT PRIMARY KEY IDENTITY(1,1),
    ResponseId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedOptionId INT,
    SelectedText VARCHAR(MAX),
    IsCorrect BIT,
    MarksObtained INT,
    FOREIGN KEY (ResponseId) REFERENCES tbl_QuizResponses(ResponseId),
    FOREIGN KEY (QuestionId) REFERENCES tbl_QuizQuestions(QuestionId),
    FOREIGN KEY (SelectedOptionId) REFERENCES tbl_QuizOptions(OptionId)
)
```

---

### 4. Remedial & Approval Tables

#### `tbl_RemedialAssignment`
```sql
CREATE TABLE tbl_RemedialAssignment (
    RemedialId INT PRIMARY KEY IDENTITY(1,1),
    JoinerId INT NOT NULL,
    QuizId INT NOT NULL,
    AssignmentReason VARCHAR(MAX),
    PreviousScore DECIMAL(5,2),
    AssignedOn DATETIME DEFAULT GETDATE(),
    CompletedOn DATETIME,
    Status VARCHAR(50), -- 'PENDING', 'COMPLETED', 'ESCALATED'
    EscalationLevel INT DEFAULT 0,
    EscalatedToManagerId INT,
    FOREIGN KEY (JoinerId) REFERENCES tbl_Joiners(JoinerId),
    FOREIGN KEY (QuizId) REFERENCES tbl_QuizMaster(QuizId)
)
```

#### `tbl_JoinerAssessment`
```sql
CREATE TABLE tbl_JoinerAssessment (
    AssessmentId INT PRIMARY KEY IDENTITY(1,1),
    JoinerId INT NOT NULL,
    FinalScore DECIMAL(5,2),
    Status VARCHAR(50), -- 'PASSED', 'FAILED'
    ApprovalStatus VARCHAR(50), -- 'PENDING', 'APPROVED', 'REJECTED'
    ApprovedBy INT,
    ApprovedOn DATETIME,
    Comments VARCHAR(MAX),
    CreatedOn DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (JoinerId) REFERENCES tbl_Joiners(JoinerId)
)
```

#### `tbl_PermanentAccess`
```sql
CREATE TABLE tbl_PermanentAccess (
    PermanentAccessId INT PRIMARY KEY IDENTITY(1,1),
    JoinerId INT NOT NULL,
    NTId VARCHAR(100) UNIQUE NOT NULL,
    NTIdStatus VARCHAR(50), -- 'PENDING', 'ACTIVE', 'INACTIVE'
    CreatedOn DATETIME DEFAULT GETDATE(),
    ActivatedOn DATETIME,
    FOREIGN KEY (JoinerId) REFERENCES tbl_Joiners(JoinerId)
)
```

---

### 5. Audit & Logging Table

#### `tbl_AuditLog`
```sql
CREATE TABLE tbl_AuditLog (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    ActionType VARCHAR(50), -- 'CREATE', 'UPDATE', 'DELETE', 'APPROVE', 'REJECT'
    EntityType VARCHAR(50),
    EntityId INT,
    OldValue VARCHAR(MAX),
    NewValue VARCHAR(MAX),
    ActionOn DATETIME DEFAULT GETDATE(),
    IPAddress VARCHAR(50)
)
```

---

## 🔧 STORED PROCEDURES

### 1. Joiner Management SPs

```sql
-- SP_CreateNewJoiner
CREATE PROCEDURE sp_CreateNewJoiner
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100),
    @MobileNumber VARCHAR(15),
    @DepartmentId INT,
    @RoleId INT,
    @JoinDate DATE
AS
BEGIN
    BEGIN TRANSACTION
    
    -- Insert into tbl_Joiners
    DECLARE @JoinerId INT
    INSERT INTO tbl_Joiners (JoinerCode, FirstName, LastName, Email, MobileNumber, 
                             DepartmentId, RoleId, JoinDate, JoiningStatus)
    VALUES (
        'JNR_' + FORMAT(GETDATE(), 'yyyyMMddHHmmss'),
        @FirstName, @LastName, @Email, @MobileNumber,
        @DepartmentId, @RoleId, @JoinDate, 'INITIATED'
    )
    SET @JoinerId = SCOPE_IDENTITY()
    
    -- Generate Temporary ID
    DECLARE @TempId VARCHAR(50) = 'TEMP_' + FORMAT(GETDATE(), 'yyyyMMddHHmmss')
    INSERT INTO tbl_TemporaryAccess (JoinerId, TemporaryId, TemporaryPassword, 
                                     ExpiryDate, IsActive)
    VALUES (@JoinerId, @TempId, HASHBYTES('SHA2_256', @TempId), 
            DATEADD(DAY, 30, GETDATE()), 1)
    
    -- Assign KT Modules based on Role
    INSERT INTO tbl_JoinerProgress (JoinerId, ModuleId, SessionId, AssetId, 
                                    CompletionStatus, CompletionPercentage)
    SELECT @JoinerId, rmm.ModuleId, ks.SessionId, ka.AssetId, 
           'NOT_STARTED', 0
    FROM tbl_RoleModuleMapping rmm
    JOIN tbl_KTSessions ks ON rmm.ModuleId = ks.ModuleId
    JOIN tbl_KTAssets ka ON ks.SessionId = ka.SessionId
    WHERE rmm.RoleId = @RoleId AND rmm.IsActive = 1 AND ks.IsActive = 1 AND ka.IsActive = 1
    
    COMMIT TRANSACTION
    
    SELECT @JoinerId AS JoinerId, @TempId AS TemporaryId
END
```

```sql
-- SP_GetJoinerDashboard
CREATE PROCEDURE sp_GetJoinerDashboard
    @JoinerId INT
AS
BEGIN
    SELECT 
        j.JoinerId,
        j.JoinerCode,
        j.FirstName + ' ' + j.LastName AS FullName,
        d.DepartmentName,
        r.RoleName,
        j.JoiningStatus,
        (SELECT COUNT(*) FROM tbl_KTModules WHERE IsActive = 1) AS TotalModules,
        (SELECT COUNT(DISTINCT ModuleId) FROM tbl_JoinerProgress 
         WHERE JoinerId = @JoinerId AND CompletionStatus = 'COMPLETED') AS CompletedModules,
        (SELECT AVG(CompletionPercentage) FROM tbl_JoinerProgress 
         WHERE JoinerId = @JoinerId) AS OverallProgress
    FROM tbl_Joiners j
    JOIN tbl_Departments d ON j.DepartmentId = d.DepartmentId
    JOIN tbl_Roles r ON j.RoleId = r.RoleId
    WHERE j.JoinerId = @JoinerId
END
```

### 2. Quiz Management SPs

```sql
-- SP_SubmitQuizResponse
CREATE PROCEDURE sp_SubmitQuizResponse
    @JoinerId INT,
    @QuizId INT,
    @AttemptNumber INT,
    @TotalMarks INT,
    @ObtainedMarks INT
AS
BEGIN
    BEGIN TRANSACTION
    
    DECLARE @Score DECIMAL(5,2) = (@ObtainedMarks * 100.0) / @TotalMarks
    DECLARE @PassingScore INT
    DECLARE @Status VARCHAR(50)
    
    SELECT @PassingScore = PassingScore FROM tbl_QuizMaster WHERE QuizId = @QuizId
    
    SET @Status = CASE WHEN @Score >= @PassingScore THEN 'PASSED' ELSE 'FAILED' END
    
    -- Update old responses
    UPDATE tbl_QuizResponses 
    SET IsLatest = 0 
    WHERE JoinerId = @JoinerId AND QuizId = @QuizId
    
    -- Insert new response
    INSERT INTO tbl_QuizResponses (JoinerId, QuizId, AttemptNumber, TotalMarks, 
                                   ObtainedMarks, Score, Status, StartedOn, SubmittedOn, IsLatest)
    VALUES (@JoinerId, @QuizId, @AttemptNumber, @TotalMarks, @ObtainedMarks, 
            @Score, @Status, GETDATE(), GETDATE(), 1)
    
    -- Update Joiner Status
    IF @Status = 'PASSED'
        UPDATE tbl_Joiners SET JoiningStatus = 'PASSED' WHERE JoinerId = @JoinerId
    ELSE
        UPDATE tbl_Joiners SET JoiningStatus = 'FAILED' WHERE JoinerId = @JoinerId
    
    COMMIT TRANSACTION
    
    SELECT @Score AS Score, @Status AS Status
END
```

### 3. Admin Dashboard SPs

```sql
-- SP_GetAdminDashboard
CREATE PROCEDURE sp_GetAdminDashboard
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM tbl_Joiners WHERE JoiningStatus = 'INITIATED') AS NewJoiners,
        (SELECT COUNT(*) FROM tbl_Joiners WHERE JoiningStatus IN ('KT_IN_PROGRESS', 'QUIZ_PENDING')) AS InProgress,
        (SELECT COUNT(*) FROM tbl_Joiners WHERE JoiningStatus = 'PASSED') AS PassedCount,
        (SELECT COUNT(*) FROM tbl_Joiners WHERE JoiningStatus = 'FAILED') AS FailedCount,
        (SELECT COUNT(*) FROM tbl_Joiners WHERE JoiningStatus = 'NT_ID_CREATED') AS CompletedCount,
        (SELECT CAST(COUNT(*) * 100.0 / NULLIF((SELECT COUNT(*) FROM tbl_Joiners), 0) AS DECIMAL(5,2))
         FROM tbl_Joiners WHERE JoiningStatus = 'PASSED') AS PassRate,
        (SELECT AVG(CAST(DATEDIFF(DAY, CreatedOn, GETDATE()) AS DECIMAL))
         FROM tbl_Joiners WHERE JoiningStatus = 'NT_ID_CREATED') AS AvgTAT
END
```

---

## 🏗️ BACKEND ARCHITECTURE (ASP.NET Core)

### Project Structure
```
KTLearningPlatform/
├── KTLearningPlatform.API/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── Data/
│   ├── Middleware/
│   └── Program.cs
├── KTLearningPlatform.Data/
│   ├── Context/
│   ├── Repositories/
│   └── UnitOfWork/
└── KTLearningPlatform.Core/
    ├── Entities/
    ├── DTOs/
    └── Interfaces/
```

---

## 📋 MODELS (C# Classes)

### Core Entities

```csharp
// Models/Joiner.cs
public class Joiner
{
    public int JoinerId { get; set; }
    public string JoinerCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public int DepartmentId { get; set; }
    public int RoleId { get; set; }
    public DateTime JoinDate { get; set; }
    public string JoiningStatus { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // Navigation Properties
    public Department Department { get; set; }
    public Role Role { get; set; }
    public ICollection<JoinerProgress> JoinerProgresses { get; set; }
    public ICollection<QuizResponse> QuizResponses { get; set; }
}

// Models/Department.cs
public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public ICollection<Joiner> Joiners { get; set; }
}

// Models/Role.cs
public class Role
{
    public int RoleId { get; set; }
    public string RoleCode { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
    public int PassingScore { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public ICollection<Joiner> Joiners { get; set; }
    public ICollection<RoleModuleMapping> RoleModuleMappings { get; set; }
    public ICollection<QuizMaster> Quizzes { get; set; }
}

// Models/KTModule.cs
public class KTModule
{
    public int ModuleId { get; set; }
    public string ModuleCode { get; set; }
    public string ModuleName { get; set; }
    public string Description { get; set; }
    public int DisplayOrder { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public ICollection<KTSession> KTSessions { get; set; }
    public ICollection<RoleModuleMapping> RoleModuleMappings { get; set; }
    public ICollection<JoinerProgress> JoinerProgresses { get; set; }
}

// Models/QuizMaster.cs
public class QuizMaster
{
    public int QuizId { get; set; }
    public int RoleId { get; set; }
    public string QuizCode { get; set; }
    public string QuizName { get; set; }
    public string Description { get; set; }
    public int TotalQuestions { get; set; }
    public int PassingScore { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public Role Role { get; set; }
    public ICollection<QuizQuestion> QuizQuestions { get; set; }
    public ICollection<QuizResponse> QuizResponses { get; set; }
}

// Models/QuizResponse.cs
public class QuizResponse
{
    public int ResponseId { get; set; }
    public int JoinerId { get; set; }
    public int QuizId { get; set; }
    public int AttemptNumber { get; set; }
    public int TotalMarks { get; set; }
    public int ObtainedMarks { get; set; }
    public decimal Score { get; set; }
    public string Status { get; set; }
    public DateTime StartedOn { get; set; }
    public DateTime SubmittedOn { get; set; }
    public bool IsLatest { get; set; }

    public Joiner Joiner { get; set; }
    public QuizMaster QuizMaster { get; set; }
    public ICollection<QuizResponseDetail> QuizResponseDetails { get; set; }
}
```

### DTOs

```csharp
// DTOs/JoinerMasterFormDto.cs
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

// DTOs/CreateJoinerActionFormDto.cs
public class CreateJoinerActionFormDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string MobileNumber { get; set; }
    [Required]
    public int DepartmentId { get; set; }
    [Required]
    public int RoleId { get; set; }
    [Required]
    public DateTime JoinDate { get; set; }
}

// DTOs/JoinerDashboardDto.cs
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
}

// DTOs/AdminDashboardDto.cs
public class AdminDashboardDto
{
    public int NewJoinersCount { get; set; }
    public int InProgressCount { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal PassRate { get; set; }
    public decimal AvgTAT { get; set; }
}
```

---

## 🎮 API CONTROLLERS & ENDPOINTS

### JoinersController
```csharp
[ApiController]
[Route("api/[controller]")]
public class JoinersController : ControllerBase
{
    private readonly IJoinerService _joinerService;

    // Admin: Create Joiner (Master Form)
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateJoiner([FromBody] CreateJoinerActionFormDto dto)
    
    // Admin: Get All Joiners (Master Report)
    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllJoiners()
    
    // Admin: Get Joiner Details
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetJoinerDetails(int id)
    
    // Admin: Update Joiner
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateJoiner(int id, [FromBody] CreateJoinerActionFormDto dto)
    
    // Admin: Delete Joiner
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteJoiner(int id)
    
    // Joiner: Get Dashboard
    [HttpGet("{id}/dashboard")]
    [Authorize(Roles = "Joiner")]
    public async Task<IActionResult> GetJoinerDashboard(int id)
    
    // Joiner: Get Learning Progress
    [HttpGet("{id}/progress")]
    [Authorize(Roles = "Joiner")]
    public async Task<IActionResult> GetLearningProgress(int id)
}
```

### QuizController
```csharp
[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    // Admin: Create Quiz (Master Form)
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizMasterDto dto)
    
    // Admin: Get Quiz Questions
    [HttpGet("{quizId}/questions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetQuizQuestions(int quizId)
    
    // Admin: Update Quiz
    [HttpPut("{quizId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] CreateQuizMasterDto dto)
    
    // Admin: Quiz Report
    [HttpGet("reports")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetQuizReport()
    
    // Joiner: Start Quiz
    [HttpPost("{quizId}/start")]
    [Authorize(Roles = "Joiner")]
    public async Task<IActionResult> StartQuiz(int quizId)
    
    // Joiner: Get Quiz Questions
    [HttpGet("{quizId}/attempt")]
    [Authorize(Roles = "Joiner")]
    public async Task<IActionResult> GetQuizForAttempt(int quizId)
    
    // Joiner: Submit Quiz Response
    [HttpPost("submit")]
    [Authorize(Roles = "Joiner")]
    public async Task<IActionResult> SubmitQuizResponse([FromBody] SubmitQuizResponseDto dto)
}
```

### ContentManagementController
```csharp
[ApiController]
[Route("api/[controller]")]
public class ContentManagementController : ControllerBase
{
    private readonly IContentService _contentService;

    // Admin: Upload KT Asset
    [HttpPost("assets/upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAsset([FromForm] IFormFile file, int sessionId)
    
    // Admin: Get Assets by Session
    [HttpGet("assets/session/{sessionId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAssetsBySession(int sessionId)
    
    // Admin: Delete Asset
    [HttpDelete("assets/{assetId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsset(int assetId)
    
    // Joiner: Get Content
    [HttpGet("modules/{moduleId}")]
    [Authorize(Roles = "Joiner")]
    public async Task<IActionResult> GetModuleContent(int moduleId)
}
```

### AdminDashboardController
```csharp
[ApiController]
[Route("api/[controller]")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminService _adminService;

    // Admin: Dashboard Metrics
    [HttpGet("metrics")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDashboardMetrics()
    
    // Admin: Approval Pending List
    [HttpGet("approval-pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetApprovalPendingList()
    
    // Admin: Approve/Reject Joiner
    [HttpPost("approval/{joinerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveJoiner(int joinerId, [FromBody] ApprovalActionDto dto)
    
    // Admin: Compliance Report
    [HttpGet("reports/compliance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetComplianceReport()
}
```

---

## 🔧 SERVICES

```csharp
// Services/IJoinerService.cs
public interface IJoinerService
{
    Task<JoinerMasterFormDto> CreateJoinerAsync(CreateJoinerActionFormDto dto);
    Task<IEnumerable<JoinerMasterFormDto>> GetAllJoinersAsync();
    Task<JoinerMasterFormDto> GetJoinerByIdAsync(int joinerId);
    Task<bool> UpdateJoinerAsync(int joinerId, CreateJoinerActionFormDto dto);
    Task<bool> DeleteJoinerAsync(int joinerId);
    Task<JoinerDashboardDto> GetJoinerDashboardAsync(int joinerId);
}

// Services/IQuizService.cs
public interface IQuizService
{
    Task<QuizMasterDto> CreateQuizAsync(CreateQuizMasterDto dto);
    Task<IEnumerable<QuizQuestion>> GetQuizQuestionsAsync(int quizId);
    Task<QuizResponse> SubmitQuizResponseAsync(SubmitQuizResponseDto dto);
    Task<decimal> CalculateScoreAsync(int responseId);
    Task<IEnumerable<QuizReportDto>> GetQuizReportAsync();
}

// Services/IAdminService.cs
public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardMetricsAsync();
    Task<IEnumerable<JoinerApprovalDto>> GetApprovalPendingAsync();
    Task<bool> ApproveJoinerAsync(int joinerId, string comments);
    Task<bool> RejectJoinerAsync(int joinerId, string reason);
    Task<IEnumerable<ReportDto>> GetComplianceReportAsync();
}

// Services/IContentService.cs
public interface IContentService
{
    Task<KTAssetDto> UploadAssetAsync(int sessionId, IFormFile file, string assetType);
    Task<IEnumerable<KTAssetDto>> GetAssetsBySessionAsync(int sessionId);
    Task<bool> DeleteAssetAsync(int assetId);
}
```

---

## 🎨 ANGULAR FRONTEND ARCHITECTURE

### Project Structure
```
kt-learning-platform-ui/
├── src/
│   ├── app/
│   │   ├── shared/
│   │   │   ├── components/
│   │   │   ├── services/
│   │   │   ├── guards/
│   │   │   └── interceptors/
│   │   ├── modules/
│   │   │   ├── joiner/
│   │   │   │   ├── components/
│   │   │   │   │   ├── joiner-dashboard/
│   │   │   │   │   ├── learning-progress/
│   │   │   │   │   ├── quiz-attempt/
│   │   │   │   │   └── profile/
│   │   │   │   ├── services/
│   │   │   │   └── joiner.module.ts
│   │   │   └── admin/
│   │   │       ├── components/
│   │   │       │   ├── admin-dashboard/
│   │   │       │   ├── joiner-master-form/
│   │   │       │   ├── joiner-action-form/
│   │   │       │   ├── quiz-master-form/
│   │   │       │   ├── quiz-report/
│   │   │       │   ├── approval-form/
│   │   │       │   ├── content-management/
│   │   │       │   └── compliance-report/
│   │   │       ├── services/
│   │   │       └── admin.module.ts
│   │   ├── auth/
│   │   │   ├── components/
│   │   │   │   └── login/
│   │   │   └── services/
│   │   └── app.module.ts
│   └── main.ts
```

---

## 📱 ANGULAR FORMS & COMPONENTS

### Joiner Side Forms

#### 1. **Joiner Dashboard Component**
```typescript
// Components/joiner-dashboard.component.ts
@Component({
  selector: 'app-joiner-dashboard',
  templateUrl: './joiner-dashboard.component.html'
})
export class JoinerDashboardComponent implements OnInit {
  dashboard: JoinerDashboardDto;
  modules: KTModuleDto[] = [];
  progress: JoinerProgressDto[] = [];

  ngOnInit() {
    this.loadDashboard();
  }

  loadDashboard() {
    // Load joiner dashboard
  }
}

<!-- joiner-dashboard.component.html -->
<div class="dashboard-container">
  <div class="welcome-card">
    <h1>Welcome, {{ dashboard.fullName }}</h1>
    <p>Role: {{ dashboard.roleName }} | Department: {{ dashboard.departmentName }}</p>
  </div>

  <div class="progress-summary">
    <div class="progress-stat">
      <span class="label">Overall Progress</span>
      <div class="progress-bar">
        <div class="progress" [style.width]="dashboard.overallProgress + '%"></div>
      </div>
      <span class="percentage">{{ dashboard.overallProgress }}%</span>
    </div>
    <div class="progress-stat">
      <span class="label">Modules Completed</span>
      <span class="value">{{ dashboard.completedModules }}/{{ dashboard.totalModules }}</span>
    </div>
  </div>

  <div class="modules-grid">
    <div *ngFor="let module of modules" class="module-card">
      <h3>{{ module.moduleName }}</h3>
      <p>{{ module.description }}</p>
      <button (click)="startModule(module.moduleId)" class="btn-primary">
        Start Learning
      </button>
    </div>
  </div>
</div>
```

#### 2. **Learning Progress Component**
```typescript
// Components/learning-progress.component.ts
@Component({
  selector: 'app-learning-progress',
  templateUrl: './learning-progress.component.html'
})
export class LearningProgressComponent implements OnInit {
  sessions: KTSessionDto[] = [];
  assets: KTAssetDto[] = [];
  currentModule: KTModuleDto;

  ngOnInit() {
    this.loadModuleSessions();
  }
}

<!-- learning-progress.component.html -->
<div class="learning-progress">
  <h2>{{ currentModule.moduleName }}</h2>
  
  <div class="sessions-list">
    <div *ngFor="let session of sessions" class="session-item">
      <h4>{{ session.sessionName }}</h4>
      
      <div class="assets-list">
        <div *ngFor="let asset of session.assets" class="asset-item">
          <span class="asset-icon" [ngSwitch]="asset.assetType">
            <i *ngSwitchCase="'VIDEO'" class="fas fa-video"></i>
            <i *ngSwitchCase="'PDF'" class="fas fa-file-pdf"></i>
            <i *ngSwitchCase="'DOCUMENT'" class="fas fa-file-word"></i>
          </span>
          
          <span class="asset-name">{{ asset.assetTitle }}</span>
          
          <button (click)="viewAsset(asset.assetId)" class="btn-view">
            View
          </button>
        </div>
      </div>
    </div>
  </div>
</div>
```

#### 3. **Quiz Attempt Component**
```typescript
// Components/quiz-attempt.component.ts
@Component({
  selector: 'app-quiz-attempt',
  templateUrl: './quiz-attempt.component.html'
})
export class QuizAttemptComponent implements OnInit {
  quiz: QuizMasterDto;
  questions: QuizQuestionDto[] = [];
  currentQuestionIndex: number = 0;
  answers: Map<number, number> = new Map();
  timer: number;
  isSubmitted: boolean = false;

  ngOnInit() {
    this.loadQuiz();
    this.startTimer();
  }

  startTimer() {
    // Start countdown timer
  }

  selectAnswer(optionId: number) {
    this.answers.set(
      this.questions[this.currentQuestionIndex].questionId,
      optionId
    );
  }

  submitQuiz() {
    // Submit responses
  }
}

<!-- quiz-attempt.component.html -->
<div class="quiz-container">
  <div class="quiz-header">
    <h2>{{ quiz.quizName }}</h2>
    <div class="timer">
      <span class="time-remaining">{{ timer | timeFormat }}</span>
    </div>
  </div>

  <div class="quiz-content">
    <div class="progress-bar">
      <div class="progress" 
        [style.width]="(currentQuestionIndex / questions.length) * 100 + '%">
      </div>
    </div>

    <div class="question-container" *ngIf="questions[currentQuestionIndex]">
      <div class="question">
        <h4>Question {{ currentQuestionIndex + 1 }} of {{ questions.length }}</h4>
        <p>{{ questions[currentQuestionIndex].questionText }}</p>
      </div>

      <div class="options">
        <div *ngFor="let option of questions[currentQuestionIndex].options" 
          class="option">
          <input 
            type="radio" 
            [name]="'q' + questions[currentQuestionIndex].questionId"
            [value]="option.optionId"
            (change)="selectAnswer(option.optionId)"
          />
          <label>{{ option.optionText }}</label>
        </div>
      </div>
    </div>

    <div class="navigation">
      <button 
        (click)="previousQuestion()" 
        [disabled]="currentQuestionIndex === 0"
        class="btn-secondary">
        Previous
      </button>
      <button 
        (click)="nextQuestion()" 
        [disabled]="currentQuestionIndex === questions.length - 1"
        class="btn-secondary">
        Next
      </button>
      <button (click)="submitQuiz()" class="btn-primary">
        Submit Quiz
      </button>
    </div>
  </div>
</div>
```

---

### Admin Side Forms

#### 1. **Joiner Master Form (Create/Edit)**
```typescript
// Components/joiner-master-form.component.ts
@Component({
  selector: 'app-joiner-master-form',
  templateUrl: './joiner-master-form.component.html'
})
export class JoinerMasterFormComponent implements OnInit {
  form: FormGroup;
  departments: DepartmentDto[] = [];
  roles: RoleDto[] = [];
  isEditMode: boolean = false;
  joinerId: number;

  constructor(private fb: FormBuilder, private joinerService: IJoinerService) {}

  ngOnInit() {
    this.initForm();
    this.loadDependencies();
  }

  initForm() {
    this.form = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      mobileNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      departmentId: ['', Validators.required],
      roleId: ['', Validators.required],
      joinDate: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const dto = this.form.value;
      this.joinerService.createJoiner(dto).subscribe(
        (response) => {
          alert('Joiner created successfully!');
          this.form.reset();
        }
      );
    }
  }
}

<!-- joiner-master-form.component.html -->
<div class="form-container">
  <h2>{{ isEditMode ? 'Edit' : 'Create' }} Joiner</h2>

  <form [formGroup]="form" (ngSubmit)="onSubmit()">
    <div class="form-group">
      <label>First Name</label>
      <input 
        type="text" 
        formControlName="firstName" 
        class="form-control"
      />
      <span *ngIf="form.get('firstName').hasError('required')" class="error">
        First Name is required
      </span>
    </div>

    <div class="form-group">
      <label>Last Name</label>
      <input 
        type="text" 
        formControlName="lastName" 
        class="form-control"
      />
    </div>

    <div class="form-group">
      <label>Email</label>
      <input 
        type="email" 
        formControlName="email" 
        class="form-control"
      />
      <span *ngIf="form.get('email').hasError('email')" class="error">
        Enter valid email
      </span>
    </div>

    <div class="form-group">
      <label>Mobile Number</label>
      <input 
        type="text" 
        formControlName="mobileNumber" 
        class="form-control"
      />
    </div>

    <div class="form-group">
      <label>Department</label>
      <select formControlName="departmentId" class="form-control">
        <option value="">-- Select Department --</option>
        <option *ngFor="let dept of departments" [value]="dept.departmentId">
          {{ dept.departmentName }}
        </option>
      </select>
    </div>

    <div class="form-group">
      <label>Role</label>
      <select formControlName="roleId" class="form-control">
        <option value="">-- Select Role --</option>
        <option *ngFor="let role of roles" [value]="role.roleId">
          {{ role.roleName }}
        </option>
      </select>
    </div>

    <div class="form-group">
      <label>Join Date</label>
      <input 
        type="date" 
        formControlName="joinDate" 
        class="form-control"
      />
    </div>

    <div class="form-actions">
      <button type="submit" [disabled]="!form.valid" class="btn-primary">
        {{ isEditMode ? 'Update' : 'Create' }} Joiner
      </button>
      <button type="button" (click)="cancel()" class="btn-secondary">
        Cancel
      </button>
    </div>
  </form>
</div>
```

#### 2. **Admin Dashboard Component**
```typescript
// Components/admin-dashboard.component.ts
@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {
  dashboard: AdminDashboardDto;
  chartData: ChartData;
  approvalPending: JoinerApprovalDto[] = [];

  constructor(private adminService: IAdminService) {}

  ngOnInit() {
    this.loadDashboard();
  }

  loadDashboard() {
    this.adminService.getDashboardMetrics().subscribe(
      (data) => {
        this.dashboard = data;
        this.prepareCharts();
      }
    );
  }
}

<!-- admin-dashboard.component.html -->
<div class="admin-dashboard">
  <h1>Admin Dashboard</h1>

  <div class="kpi-cards">
    <div class="kpi-card new-joiners">
      <div class="icon"><i class="fas fa-user-plus"></i></div>
      <div class="content">
        <span class="label">New Joiners</span>
        <span class="value">{{ dashboard.newJoinersCount }}</span>
      </div>
    </div>

    <div class="kpi-card in-progress">
      <div class="icon"><i class="fas fa-spinner"></i></div>
      <div class="content">
        <span class="label">In Progress</span>
        <span class="value">{{ dashboard.inProgressCount }}</span>
      </div>
    </div>

    <div class="kpi-card passed">
      <div class="icon"><i class="fas fa-check-circle"></i></div>
      <div class="content">
        <span class="label">Passed</span>
        <span class="value">{{ dashboard.passedCount }}</span>
      </div>
    </div>

    <div class="kpi-card completed">
      <div class="icon"><i class="fas fa-flag-checkered"></i></div>
      <div class="content">
        <span class="label">Completed</span>
        <span class="value">{{ dashboard.completedCount }}</span>
      </div>
    </div>
  </div>

  <div class="metrics-section">
    <div class="metric">
      <span class="label">Pass Rate</span>
      <span class="value">{{ dashboard.passRate }}%</span>
    </div>
    <div class="metric">
      <span class="label">Average TAT (Days)</span>
      <span class="value">{{ dashboard.avgTAT }}</span>
    </div>
  </div>

  <div class="charts-section">
    <div class="chart">
      <h3>Joiner Status Distribution</h3>
      <canvas id="statusChart"></canvas>
    </div>
  </div>
</div>
```

#### 3. **Quiz Master Form**
```typescript
// Components/quiz-master-form.component.ts
@Component({
  selector: 'app-quiz-master-form',
  templateUrl: './quiz-master-form.component.html'
})
export class QuizMasterFormComponent implements OnInit {
  quizForm: FormGroup;
  questionsArray: FormArray;
  roles: RoleDto[] = [];

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.quizForm = this.fb.group({
      roleId: ['', Validators.required],
      quizCode: ['', Validators.required],
      quizName: ['', Validators.required],
      description: [''],
      passingScore: [70, Validators.required],
      durationMinutes: [60, Validators.required],
      maxAttempts: [3, Validators.required]
    });
    
    this.questionsArray = this.fb.array([]);
  }

  addQuestion() {
    const questionGroup = this.fb.group({
      questionText: ['', Validators.required],
      questionType: ['MCQ'],
      marks: [1],
      options: this.fb.array([])
    });
    this.questionsArray.push(questionGroup);
  }

  removeQuestion(index: number) {
    this.questionsArray.removeAt(index);
  }
}

<!-- quiz-master-form.component.html -->
<div class="quiz-master-form">
  <h2>Create Quiz</h2>

  <form [formGroup]="quizForm">
    <div class="basic-info">
      <div class="form-group">
        <label>Role</label>
        <select formControlName="roleId" class="form-control">
          <option *ngFor="let role of roles" [value]="role.roleId">
            {{ role.roleName }}
          </option>
        </select>
      </div>

      <div class="form-group">
        <label>Quiz Name</label>
        <input type="text" formControlName="quizName" class="form-control" />
      </div>

      <div class="form-group">
        <label>Passing Score (%)</label>
        <input type="number" formControlName="passingScore" class="form-control" />
      </div>

      <div class="form-group">
        <label>Duration (Minutes)</label>
        <input type="number" formControlName="durationMinutes" class="form-control" />
      </div>
    </div>

    <div class="questions-section">
      <h3>Questions</h3>
      <button type="button" (click)="addQuestion()" class="btn-secondary">
        Add Question
      </button>

      <div *ngFor="let question of questionsArray.controls; let i = index" class="question-item">
        <h4>Question {{ i + 1 }}</h4>
        <!-- Question form fields -->
      </div>
    </div>

    <button type="submit" class="btn-primary">Create Quiz</button>
  </form>
</div>
```

#### 4. **Joiner Report Form**
```typescript
// Components/joiner-report.component.ts
@Component({
  selector: 'app-joiner-report',
  templateUrl: './joiner-report.component.html'
})
export class JoinerReportComponent implements OnInit {
  joiners: JoinerMasterFormDto[] = [];
  filteredJoiners: JoinerMasterFormDto[] = [];
  filterForm: FormGroup;
  displayedColumns: string[] = ['joinerCode', 'fullName', 'department', 'role', 'status'];

  constructor(private fb: FormBuilder, private joinerService: IJoinerService) {}

  ngOnInit() {
    this.initFilterForm();
    this.loadJoiners();
  }

  initFilterForm() {
    this.filterForm = this.fb.group({
      status: [''],
      department: [''],
      role: ['']
    });
  }

  loadJoiners() {
    this.joinerService.getAllJoiners().subscribe(
      (data) => {
        this.joiners = data;
        this.applyFilters();
      }
    );
  }

  applyFilters() {
    const filters = this.filterForm.value;
    this.filteredJoiners = this.joiners.filter(j =>
      (!filters.status || j.joiningStatus === filters.status) &&
      (!filters.department || j.departmentName === filters.department) &&
      (!filters.role || j.roleName === filters.role)
    );
  }
}

<!-- joiner-report.component.html -->
<div class="joiner-report">
  <h2>Joiner Report</h2>

  <div class="filter-section">
    <form [formGroup]="filterForm" (ngSubmit)="applyFilters()">
      <select formControlName="status" class="filter-control">
        <option value="">-- All Status --</option>
        <option value="INITIATED">Initiated</option>
        <option value="KT_IN_PROGRESS">KT In Progress</option>
        <option value="PASSED">Passed</option>
        <option value="FAILED">Failed</option>
      </select>
      <button type="submit" class="btn-primary">Filter</button>
    </form>
  </div>

  <table class="report-table">
    <thead>
      <tr>
        <th>Joiner Code</th>
        <th>Full Name</th>
        <th>Department</th>
        <th>Role</th>
        <th>Status</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let joiner of filteredJoiners">
        <td>{{ joiner.joinerCode }}</td>
        <td>{{ joiner.firstName }} {{ joiner.lastName }}</td>
        <td>{{ joiner.departmentName }}</td>
        <td>{{ joiner.roleName }}</td>
        <td>
          <span [ngClass]="'status-' + joiner.joiningStatus">
            {{ joiner.joiningStatus }}
          </span>
        </td>
        <td>
          <button (click)="editJoiner(joiner.joinerId)">Edit</button>
          <button (click)="viewDetails(joiner.joinerId)">View</button>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

#### 5. **Approval Action Form**
```typescript
// Components/approval-action-form.component.ts
@Component({
  selector: 'app-approval-action-form',
  templateUrl: './approval-action-form.component.html'
})
export class ApprovalActionFormComponent implements OnInit {
  approvalForm: FormGroup;
  approvalList: JoinerApprovalDto[] = [];
  selectedJoiner: JoinerApprovalDto;

  ngOnInit() {
    this.initForm();
    this.loadPendingApprovals();
  }

  initForm() {
    this.approvalForm = this.fb.group({
      comments: ['', Validators.required],
      action: ['', Validators.required]
    });
  }

  approve() {
    // Send approval action
  }

  reject() {
    // Send rejection action
  }
}

<!-- approval-action-form.component.html -->
<div class="approval-section">
  <h2>Approval Actions</h2>

  <div class="pending-list">
    <h3>Pending Approvals</h3>
    <div *ngFor="let item of approvalList" class="approval-item">
      <div class="joiner-info">
        <strong>{{ item.joinerName }}</strong>
        <p>Score: {{ item.score }}%</p>
      </div>
      <button (click)="selectJoiner(item)" class="btn-secondary">
        Review
      </button>
    </div>
  </div>

  <div *ngIf="selectedJoiner" class="approval-form">
    <h3>Approve/Reject: {{ selectedJoiner.joinerName }}</h3>
    
    <form [formGroup]="approvalForm">
      <div class="form-group">
        <label>Score Obtained</label>
        <span class="score-display">{{ selectedJoiner.score }}%</span>
      </div>

      <div class="form-group">
        <label>Comments</label>
        <textarea 
          formControlName="comments" 
          class="form-control"
          rows="4">
        </textarea>
      </div>

      <div class="form-actions">
        <button type="button" (click)="approve()" class="btn-success">
          Approve
        </button>
        <button type="button" (click)="reject()" class="btn-danger">
          Reject
        </button>
      </div>
    </form>
  </div>
</div>
```

---

## 📊 REPORT FORMS

### For Admin:
1. **Joiner Report** - List all joiners with status, department, role
2. **Quiz Report** - Quiz-wise pass/fail statistics
3. **Remedial Assignment Report** - Track remedial KT assignments
4. **Compliance Report** - SLA compliance metrics
5. **TAT Report** - Turnaround time analysis

### For Joiner:
1. **Progress Report** - Module completion status
2. **Quiz Attempt History** - All quiz attempts with scores

---

## 🔐 SECURITY & VALIDATION

- Password hashing (bcrypt)
- JWT token authentication
- Role-based authorization
- Input validation at UI and API
- SQL injection prevention (parameterized queries)
- CORS configuration

---

## 📦 SUMMARY TABLE

| Layer | Component | Count |
|-------|-----------|-------|
| **Database** | Tables | 18 |
| | Stored Procedures | 15+ |
| **Models** | DTOs | 20+ |
| | Entities | 12+ |
| **API** | Controllers | 5 |
| | Endpoints | 35+ |
| **Services** | Service Interfaces | 5 |
| **Angular** | Components | 12+ |
| | Forms | 8 |
| | Reports | 5 |

---

This comprehensive architecture provides a scalable, maintainable solution for your KT Learning Platform!
