using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTLearningPlatform.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class Department
    {
        public int DepartmentId { get; set; }
        [Required]
        public string DepartmentCode { get; set; } = null!;
        [Required]
        public string DepartmentName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class Role
    {
        public int RoleId { get; set; }
        [Required]
        public string RoleCode { get; set; } = null!;
        [Required]
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
        public int PassingScore { get; set; } = 70;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class Joiner
    {
        public int JoinerId { get; set; }
        [Required]
        public string JoinerCode { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string MobileNumber { get; set; } = null!;
        public int DepartmentId { get; set; }
        public int RoleId { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public string JoiningStatus { get; set; } = "INITIATED";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class TemporaryAccess
    {
        public int TemporaryAccessId { get; set; }
        public int JoinerId { get; set; }
        [Required]
        public string TemporaryId { get; set; } = null!;
        [Required]
        public string TemporaryPasswordHash { get; set; } = null!;
        public bool IsUsed { get; set; }
        public DateTime? FirstLoginOn { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PermanentAccess
    {
        public int PermanentAccessId { get; set; }
        public int JoinerId { get; set; }
        [Required]
        public string NTId { get; set; } = null!;
        public string NTIdStatus { get; set; } = "PENDING";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ActivatedOn { get; set; }
    }

    public class JoinerProgress
    {
        public int ProgressId { get; set; }
        public int JoinerId { get; set; }
        public int ModuleId { get; set; }
        public int SessionId { get; set; }
        public int AssetId { get; set; }
        public string CompletionStatus { get; set; } = "NOT_STARTED";
        public decimal CompletionPercentage { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public int DurationSpentSeconds { get; set; }
    }

    public class KTModule
    {
        public int ModuleId { get; set; }
        [Required]
        public string ModuleCode { get; set; } = null!;
        [Required]
        public string ModuleName { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class KTSession
    {
        public int SessionId { get; set; }
        public int ModuleId { get; set; }
        [Required]
        public string SessionName { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class KTAsset
    {
        public int AssetId { get; set; }
        public int SessionId { get; set; }
        [Required]
        public string AssetType { get; set; } = null!;
        [Required]
        public string AssetTitle { get; set; } = null!;
        [Required]
        public string AssetUrl { get; set; } = null!;
        public string? AssetPath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class RoleModuleMapping
    {
        public int MappingId { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class QuizMaster
    {
        public int QuizId { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string QuizCode { get; set; } = null!;
        [Required]
        public string QuizName { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int PassingScore { get; set; } = 70;
        public int DurationMinutes { get; set; }
        public int MaxAttempts { get; set; } = 3;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class QuizQuestion
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        [Required]
        public string QuestionText { get; set; } = null!;
        [Required]
        public string QuestionType { get; set; } = "MCQ";
        public int DisplayOrder { get; set; }
        public int Marks { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class QuizOption
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        [Required]
        public string OptionText { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class QuizResponse
    {
        public int ResponseId { get; set; }
        public int JoinerId { get; set; }
        public int QuizId { get; set; }
        public int AttemptNumber { get; set; }
        public int TotalMarks { get; set; }
        public int ObtainedMarks { get; set; }
        public decimal Score { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime StartedOn { get; set; } = DateTime.UtcNow;
        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
        public bool IsLatest { get; set; } = true;
    }

    public class QuizResponseDetail
    {
        public int DetailId { get; set; }
        public int ResponseId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? SelectedText { get; set; }
        public bool IsCorrect { get; set; }
        public int MarksObtained { get; set; }
    }

    public class JoinerAssessment
    {
        public int AssessmentId { get; set; }
        public int JoinerId { get; set; }
        public decimal FinalScore { get; set; }
        public string Status { get; set; } = "PENDING";
        public string ApprovalStatus { get; set; } = "PENDING";
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class RemedialAssignment
    {
        public int RemedialId { get; set; }
        public int JoinerId { get; set; }
        public int QuizId { get; set; }
        public string? AssignmentReason { get; set; }
        public decimal PreviousScore { get; set; }
        public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedOn { get; set; }
        public string Status { get; set; } = "PENDING";
        public int EscalationLevel { get; set; }
        public int? EscalatedToManagerId { get; set; }
    }

    public class AuditLog
    {
        public int AuditId { get; set; }
        public int? UserId { get; set; }
        public string ActionType { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public int? EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ActionOn { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
    }
}
