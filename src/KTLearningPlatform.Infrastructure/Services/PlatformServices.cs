using KTLearningPlatform.Core.DTOs;
using KTLearningPlatform.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace KTLearningPlatform.Infrastructure.Services
{
    public class PlatformServices :
        IAuthService,
        IJoinerService,
        IKTContentService,
        IQuizService,
        IAdminDashboardService
    {
        private readonly string _connectionString;

        public PlatformServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private static void AddParameter(SqlCommand command, string name, object? value)
        {
            command.Parameters.Add(new SqlParameter(name, value ?? DBNull.Value));
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT TOP 1 Username, Role FROM [Users] WHERE Username = @Username AND PasswordHash = @PasswordHash", connection);
            AddParameter(command, "@Username", request.Username);
            AddParameter(command, "@PasswordHash", request.Password);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return new LoginResponseDto { Success = false, Message = "Invalid credentials" };
            }

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = Guid.NewGuid().ToString()
            };
        }

        public async Task<JoinerDto> CreateJoinerAsync(CreateJoinerDto request)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "INSERT INTO Joiners (JoinerCode, FirstName, LastName, Email, MobileNumber, DepartmentId, RoleId, JoinDate, JoiningStatus, IsActive, CreatedOn) " +
                "OUTPUT INSERTED.JoinerId " +
                "VALUES (@JoinerCode, @FirstName, @LastName, @Email, @MobileNumber, @DepartmentId, @RoleId, @JoinDate, @JoiningStatus, @IsActive, @CreatedOn)", connection);

            var joinerCode = $"JN{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            AddParameter(command, "@JoinerCode", joinerCode);
            AddParameter(command, "@FirstName", request.FirstName);
            AddParameter(command, "@LastName", request.LastName);
            AddParameter(command, "@Email", request.Email);
            AddParameter(command, "@MobileNumber", request.MobileNumber);
            AddParameter(command, "@DepartmentId", request.DepartmentId);
            AddParameter(command, "@RoleId", request.RoleId);
            AddParameter(command, "@JoinDate", request.JoinDate == default ? DateTime.UtcNow : request.JoinDate);
            AddParameter(command, "@JoiningStatus", "INITIATED");
            AddParameter(command, "@IsActive", true);
            AddParameter(command, "@CreatedOn", DateTime.UtcNow);

            var joinerId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return new JoinerDto
            {
                JoinerId = joinerId,
                JoinerCode = joinerCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                JoinDate = request.JoinDate == default ? DateTime.UtcNow : request.JoinDate,
                JoiningStatus = "INITIATED"
            };
        }

        public async Task<IEnumerable<JoinerDto>> GetJoinersAsync()
        {
            var joiners = new List<JoinerDto>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT JoinerId, JoinerCode, FirstName, LastName, Email, MobileNumber, DepartmentId, RoleId, JoinDate, JoiningStatus FROM Joiners", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                joiners.Add(new JoinerDto
                {
                    JoinerId = reader.GetInt32(0),
                    JoinerCode = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Email = reader.GetString(4),
                    MobileNumber = reader.GetString(5),
                    DepartmentId = reader.GetInt32(6),
                    RoleId = reader.GetInt32(7),
                    JoinDate = reader.GetDateTime(8),
                    JoiningStatus = reader.GetString(9)
                });
            }

            return joiners;
        }

        public async Task<JoinerDto?> GetJoinerByIdAsync(int joinerId)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT JoinerId, JoinerCode, FirstName, LastName, Email, MobileNumber, DepartmentId, RoleId, JoinDate, JoiningStatus FROM Joiners WHERE JoinerId = @JoinerId", connection);
            AddParameter(command, "@JoinerId", joinerId);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new JoinerDto
            {
                JoinerId = reader.GetInt32(0),
                JoinerCode = reader.GetString(1),
                FirstName = reader.GetString(2),
                LastName = reader.GetString(3),
                Email = reader.GetString(4),
                MobileNumber = reader.GetString(5),
                DepartmentId = reader.GetInt32(6),
                RoleId = reader.GetInt32(7),
                JoinDate = reader.GetDateTime(8),
                JoiningStatus = reader.GetString(9)
            };
        }

        public async Task<bool> UpdateJoinerAsync(int joinerId, UpdateJoinerDto request)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "UPDATE Joiners SET FirstName = @FirstName, LastName = @LastName, Email = @Email, MobileNumber = @MobileNumber, DepartmentId = @DepartmentId, RoleId = @RoleId, JoinDate = @JoinDate, JoiningStatus = @JoiningStatus WHERE JoinerId = @JoinerId", connection);
            AddParameter(command, "@JoinerId", joinerId);
            AddParameter(command, "@FirstName", request.FirstName);
            AddParameter(command, "@LastName", request.LastName);
            AddParameter(command, "@Email", request.Email);
            AddParameter(command, "@MobileNumber", request.MobileNumber);
            AddParameter(command, "@DepartmentId", request.DepartmentId);
            AddParameter(command, "@RoleId", request.RoleId);
            AddParameter(command, "@JoinDate", request.JoinDate);
            AddParameter(command, "@JoiningStatus", request.JoiningStatus);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteJoinerAsync(int joinerId)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("DELETE FROM Joiners WHERE JoinerId = @JoinerId", connection);
            AddParameter(command, "@JoinerId", joinerId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<JoinerDashboardDto> GetJoinerDashboardAsync(int joinerId)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var dashboard = new JoinerDashboardDto { JoinerId = joinerId };

            using (var command = new SqlCommand("SELECT FirstName, LastName, DepartmentId, RoleId, JoiningStatus FROM Joiners WHERE JoinerId = @JoinerId", connection))
            {
                AddParameter(command, "@JoinerId", joinerId);
                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return dashboard;
                }

                dashboard.FullName = $"{reader.GetString(0)} {reader.GetString(1)}";
                dashboard.DepartmentName = string.Empty;
                dashboard.RoleName = string.Empty;
                dashboard.JoiningStatus = reader.GetString(4);
                var departmentId = reader.GetInt32(2);
                var roleId = reader.GetInt32(3);

                reader.Close();

                using var deptCommand = new SqlCommand("SELECT DepartmentName FROM Departments WHERE DepartmentId = @DepartmentId", connection);
                AddParameter(deptCommand, "@DepartmentId", departmentId);
                dashboard.DepartmentName = (await deptCommand.ExecuteScalarAsync() as string) ?? string.Empty;

                using var roleCommand = new SqlCommand("SELECT RoleName FROM Roles WHERE RoleId = @RoleId", connection);
                AddParameter(roleCommand, "@RoleId", roleId);
                dashboard.RoleName = (await roleCommand.ExecuteScalarAsync() as string) ?? string.Empty;
            }

            using (var moduleCountCommand = new SqlCommand("SELECT COUNT(*) FROM KTModules", connection))
            {
                dashboard.TotalModules = Convert.ToInt32(await moduleCountCommand.ExecuteScalarAsync());
            }

            using (var completedModuleCommand = new SqlCommand("SELECT COUNT(*) FROM JoinerProgress WHERE JoinerId = @JoinerId AND CompletionStatus = 'COMPLETED'", connection))
            {
                AddParameter(completedModuleCommand, "@JoinerId", joinerId);
                dashboard.CompletedModules = Convert.ToInt32(await completedModuleCommand.ExecuteScalarAsync());
            }

            dashboard.OverallProgress = dashboard.TotalModules == 0 ? 0 : Math.Min(100m, (decimal)dashboard.CompletedModules / dashboard.TotalModules * 100);
            return dashboard;
        }

        public async Task<IEnumerable<KTModuleDto>> GetModulesAsync()
        {
            var modules = new List<KTModuleDto>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT ModuleId, ModuleCode, ModuleName, Description, DurationMinutes FROM KTModules ORDER BY DisplayOrder", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                modules.Add(new KTModuleDto
                {
                    ModuleId = reader.GetInt32(0),
                    ModuleCode = reader.GetString(1),
                    ModuleName = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    DurationMinutes = reader.GetInt32(4)
                });
            }

            return modules;
        }

        public async Task<KTModuleDto?> GetModuleByIdAsync(int moduleId)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT ModuleId, ModuleCode, ModuleName, Description, DurationMinutes FROM KTModules WHERE ModuleId = @ModuleId", connection);
            AddParameter(command, "@ModuleId", moduleId);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new KTModuleDto
            {
                ModuleId = reader.GetInt32(0),
                ModuleCode = reader.GetString(1),
                ModuleName = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                DurationMinutes = reader.GetInt32(4)
            };
        }

        public async Task<IEnumerable<KTAssetDto>> GetAssetsBySessionAsync(int sessionId)
        {
            var assets = new List<KTAssetDto>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT AssetId, SessionId, AssetType, AssetTitle, AssetUrl FROM KTAssets WHERE SessionId = @SessionId", connection);
            AddParameter(command, "@SessionId", sessionId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                assets.Add(new KTAssetDto
                {
                    AssetId = reader.GetInt32(0),
                    SessionId = reader.GetInt32(1),
                    AssetType = reader.GetString(2),
                    AssetTitle = reader.GetString(3),
                    AssetUrl = reader.GetString(4)
                });
            }

            return assets;
        }

        public async Task<KTModuleDto> CreateModuleAsync(KTModuleDto request)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "INSERT INTO KTModules (ModuleCode, ModuleName, Description, DurationMinutes, DisplayOrder, IsActive, CreatedOn) " +
                "OUTPUT INSERTED.ModuleId VALUES (@ModuleCode, @ModuleName, @Description, @DurationMinutes, @DisplayOrder, @IsActive, @CreatedOn)", connection);
            AddParameter(command, "@ModuleCode", request.ModuleCode);
            AddParameter(command, "@ModuleName", request.ModuleName);
            AddParameter(command, "@Description", request.Description);
            AddParameter(command, "@DurationMinutes", request.DurationMinutes);
            AddParameter(command, "@DisplayOrder", 0);
            AddParameter(command, "@IsActive", true);
            AddParameter(command, "@CreatedOn", DateTime.UtcNow);

            request.ModuleId = Convert.ToInt32(await command.ExecuteScalarAsync());
            return request;
        }

        public async Task<QuizMasterDto> CreateQuizAsync(QuizMasterDto request)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "INSERT INTO QuizMasters (RoleId, QuizCode, QuizName, Description, TotalQuestions, PassingScore, DurationMinutes, MaxAttempts, IsActive, CreatedOn) " +
                "OUTPUT INSERTED.QuizId VALUES (@RoleId, @QuizCode, @QuizName, @Description, @TotalQuestions, @PassingScore, @DurationMinutes, @MaxAttempts, @IsActive, @CreatedOn)", connection);
            AddParameter(command, "@RoleId", request.RoleId);
            AddParameter(command, "@QuizCode", request.QuizCode);
            AddParameter(command, "@QuizName", request.QuizName);
            AddParameter(command, "@Description", request.Description);
            AddParameter(command, "@TotalQuestions", request.TotalQuestions);
            AddParameter(command, "@PassingScore", request.PassingScore);
            AddParameter(command, "@DurationMinutes", request.DurationMinutes);
            AddParameter(command, "@MaxAttempts", request.MaxAttempts);
            AddParameter(command, "@IsActive", true);
            AddParameter(command, "@CreatedOn", DateTime.UtcNow);

            request.QuizId = Convert.ToInt32(await command.ExecuteScalarAsync());
            return request;
        }

        public async Task<QuizMasterDto?> GetQuizByIdAsync(int quizId)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT QuizId, RoleId, QuizCode, QuizName, Description, PassingScore, DurationMinutes, MaxAttempts FROM QuizMasters WHERE QuizId = @QuizId", connection);
            AddParameter(command, "@QuizId", quizId);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new QuizMasterDto
            {
                QuizId = reader.GetInt32(0),
                RoleId = reader.GetInt32(1),
                QuizCode = reader.GetString(2),
                QuizName = reader.GetString(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                PassingScore = reader.GetInt32(5),
                DurationMinutes = reader.GetInt32(6),
                MaxAttempts = reader.GetInt32(7)
            };
        }

        public async Task<IEnumerable<QuizQuestionDto>> GetQuizQuestionsAsync(int quizId)
        {
            var questions = new List<QuizQuestionDto>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT QuestionId, QuizId, QuestionText, QuestionType, Marks FROM QuizQuestions WHERE QuizId = @QuizId ORDER BY DisplayOrder", connection);
            AddParameter(command, "@QuizId", quizId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                questions.Add(new QuizQuestionDto
                {
                    QuestionId = reader.GetInt32(0),
                    QuizId = reader.GetInt32(1),
                    QuestionText = reader.GetString(2),
                    QuestionType = reader.GetString(3),
                    Marks = reader.GetInt32(4),
                    Options = new List<QuizOptionDto>()
                });
            }

            foreach (var question in questions)
            {
                using var optionCommand = new SqlCommand(
                    "SELECT OptionId, QuestionId, OptionText, IsCorrect FROM QuizOptions WHERE QuestionId = @QuestionId ORDER BY DisplayOrder", connection);
                AddParameter(optionCommand, "@QuestionId", question.QuestionId);

                using var optionReader = await optionCommand.ExecuteReaderAsync();
                while (await optionReader.ReadAsync())
                {
                    question.Options.Add(new QuizOptionDto
                    {
                        OptionId = optionReader.GetInt32(0),
                        QuestionId = optionReader.GetInt32(1),
                        OptionText = optionReader.GetString(2),
                        IsCorrect = optionReader.GetBoolean(3)
                    });
                }
            }

            return questions;
        }

        public async Task<QuizResultDto> SubmitQuizAsync(SubmitQuizDto request)
        {
            var questions = await GetQuizQuestionsAsync(request.QuizId);
            var totalMarks = questions.Sum(q => q.Marks);
            var obtainedMarks = 0;

            foreach (var question in questions)
            {
                if (request.Answers.TryGetValue(question.QuestionId, out var selectedOptionId))
                {
                    var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == selectedOptionId);
                    if (selectedOption?.IsCorrect == true)
                    {
                        obtainedMarks += question.Marks;
                    }
                }
            }

            var score = totalMarks == 0 ? 0 : Math.Round((decimal)obtainedMarks / totalMarks * 100, 2);
            var quiz = await GetQuizByIdAsync(request.QuizId);
            var passingScore = quiz?.PassingScore ?? 0;
            var status = score >= passingScore ? "PASSED" : "FAILED";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var insertCommand = new SqlCommand(
                "INSERT INTO QuizResponses (JoinerId, QuizId, AttemptNumber, TotalMarks, ObtainedMarks, Score, Status, StartedOn, SubmittedOn, IsLatest) " +
                "VALUES (@JoinerId, @QuizId, @AttemptNumber, @TotalMarks, @ObtainedMarks, @Score, @Status, @StartedOn, @SubmittedOn, @IsLatest)", connection);
            AddParameter(insertCommand, "@JoinerId", request.JoinerId);
            AddParameter(insertCommand, "@QuizId", request.QuizId);
            AddParameter(insertCommand, "@AttemptNumber", 1);
            AddParameter(insertCommand, "@TotalMarks", totalMarks);
            AddParameter(insertCommand, "@ObtainedMarks", obtainedMarks);
            AddParameter(insertCommand, "@Score", score);
            AddParameter(insertCommand, "@Status", status);
            AddParameter(insertCommand, "@StartedOn", DateTime.UtcNow);
            AddParameter(insertCommand, "@SubmittedOn", DateTime.UtcNow);
            AddParameter(insertCommand, "@IsLatest", true);
            await insertCommand.ExecuteNonQueryAsync();

            return new QuizResultDto
            {
                Passed = status == "PASSED",
                Score = score,
                Status = status,
                Message = status == "PASSED" ? "Quiz passed" : "Quiz failed"
            };
        }

        public async Task<AdminDashboardDto> GetDashboardMetricsAsync()
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var metrics = new AdminDashboardDto();

            using (var command = new SqlCommand("SELECT COUNT(*) FROM Joiners", connection))
            {
                metrics.NewJoinersCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            using (var command = new SqlCommand("SELECT COUNT(*) FROM Joiners WHERE JoiningStatus = 'IN_PROGRESS'", connection))
            {
                metrics.InProgressCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            using (var command = new SqlCommand("SELECT COUNT(*) FROM QuizResponses WHERE Status = 'PASSED'", connection))
            {
                metrics.PassedCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            using (var command = new SqlCommand("SELECT COUNT(*) FROM QuizResponses WHERE Status = 'FAILED'", connection))
            {
                metrics.FailedCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            using (var command = new SqlCommand("SELECT COUNT(*) FROM JoinerProgress WHERE CompletionStatus = 'COMPLETED'", connection))
            {
                metrics.CompletedCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            metrics.PassRate = metrics.NewJoinersCount == 0 ? 0 : Math.Round((decimal)metrics.PassedCount / metrics.NewJoinersCount * 100, 2);
            metrics.AverageTAT = 0m;
            return metrics;
        }

        public async Task<ComplianceReportDto> GetComplianceReportAsync()
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var report = new ComplianceReportDto();

            using (var command = new SqlCommand("SELECT COUNT(*) FROM Joiners", connection))
            {
                report.TotalJoiners = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            using (var command = new SqlCommand("SELECT COUNT(*) FROM JoinerProgress WHERE CompletionStatus = 'COMPLETED'", connection))
            {
                report.MetSLA = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            report.MissedSLA = report.TotalJoiners - report.MetSLA;
            report.CompliancePercentage = report.TotalJoiners == 0 ? 0 : Math.Round((decimal)report.MetSLA / report.TotalJoiners * 100, 2);
            return report;
        }
    }
}
