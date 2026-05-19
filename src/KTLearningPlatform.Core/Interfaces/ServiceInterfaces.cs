using KTLearningPlatform.Core.DTOs;

namespace KTLearningPlatform.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request);
    }

    public interface IJoinerService
    {
        Task<JoinerDto> CreateJoinerAsync(CreateJoinerDto request);
        Task<IEnumerable<JoinerDto>> GetJoinersAsync();
        Task<JoinerDto?> GetJoinerByIdAsync(int joinerId);
        Task<bool> UpdateJoinerAsync(int joinerId, UpdateJoinerDto request);
        Task<bool> DeleteJoinerAsync(int joinerId);
        Task<JoinerDashboardDto> GetJoinerDashboardAsync(int joinerId);
    }

    public interface IKTContentService
    {
        Task<IEnumerable<KTModuleDto>> GetModulesAsync();
        Task<KTModuleDto?> GetModuleByIdAsync(int moduleId);
        Task<IEnumerable<KTAssetDto>> GetAssetsBySessionAsync(int sessionId);
    }

    public interface IQuizService
    {
        Task<QuizMasterDto> CreateQuizAsync(QuizMasterDto request);
        Task<QuizMasterDto?> GetQuizByIdAsync(int quizId);
        Task<IEnumerable<QuizQuestionDto>> GetQuizQuestionsAsync(int quizId);
        Task<QuizResultDto> SubmitQuizAsync(SubmitQuizDto request);
    }

    public interface IAssessmentService
    {
        Task<IEnumerable<JoinerApprovalDto>> GetPendingApprovalsAsync();
        Task<bool> ApproveJoinerAsync(int joinerId, ApprovalActionDto request);
        Task<bool> RejectJoinerAsync(int joinerId, ApprovalActionDto request);
    }

    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardMetricsAsync();
        Task<ComplianceReportDto> GetComplianceReportAsync();
    }
}
