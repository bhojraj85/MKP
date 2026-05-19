namespace KTLearningPlatform.Core.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }
    }

    public class JoinerDto
    {
        public int JoinerId { get; set; }
        public string JoinerCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime JoinDate { get; set; }
        public string JoiningStatus { get; set; } = null!;
    }

    public class CreateJoinerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public int DepartmentId { get; set; }
        public int RoleId { get; set; }
        public DateTime JoinDate { get; set; }
    }

    public class UpdateJoinerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public int DepartmentId { get; set; }
        public int RoleId { get; set; }
        public DateTime JoinDate { get; set; }
        public string JoiningStatus { get; set; } = null!;
    }

    public class JoinerDashboardDto
    {
        public int JoinerId { get; set; }
        public string FullName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string JoiningStatus { get; set; } = null!;
        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }
        public decimal OverallProgress { get; set; }
    }

    public class KTModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; } = null!;
        public string ModuleName { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class KTSessionDto
    {
        public int SessionId { get; set; }
        public int ModuleId { get; set; }
        public string SessionName { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class KTAssetDto
    {
        public int AssetId { get; set; }
        public int SessionId { get; set; }
        public string AssetType { get; set; } = null!;
        public string AssetTitle { get; set; } = null!;
        public string AssetUrl { get; set; } = null!;
    }

    public class QuizMasterDto
    {
        public int QuizId { get; set; }
        public int RoleId { get; set; }
        public string QuizCode { get; set; } = null!;
        public string QuizName { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int PassingScore { get; set; }
        public int DurationMinutes { get; set; }
        public int MaxAttempts { get; set; }
    }

    public class QuizQuestionDto
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string QuestionType { get; set; } = null!;
        public int Marks { get; set; }
        public List<QuizOptionDto> Options { get; set; } = new();
    }

    public class QuizOptionDto
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; } = null!;
        public bool IsCorrect { get; set; }
    }

    public class SubmitQuizDto
    {
        public int JoinerId { get; set; }
        public int QuizId { get; set; }
        public Dictionary<int, int> Answers { get; set; } = new();
    }

    public class QuizResultDto
    {
        public bool Passed { get; set; }
        public decimal Score { get; set; }
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public class ApprovalActionDto
    {
        public string Action { get; set; } = null!;
        public string Comments { get; set; } = null!;
    }

    public class JoinerApprovalDto
    {
        public int JoinerId { get; set; }
        public string JoinerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public decimal Score { get; set; }
    }

    public class AdminDashboardDto
    {
        public int NewJoinersCount { get; set; }
        public int InProgressCount { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int CompletedCount { get; set; }
        public decimal PassRate { get; set; }
        public decimal AverageTAT { get; set; }
    }

    public class ComplianceReportDto
    {
        public int TotalJoiners { get; set; }
        public int MetSLA { get; set; }
        public int MissedSLA { get; set; }
        public decimal CompliancePercentage { get; set; }
    }
}
