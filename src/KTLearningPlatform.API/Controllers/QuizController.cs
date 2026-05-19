using KTLearningPlatform.Core.DTOs;
using KTLearningPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KTLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuizMasterDto request)
        {
            var quiz = await _quizService.CreateQuizAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = quiz.QuizId }, quiz);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetQuestions(int id)
        {
            var questions = await _quizService.GetQuizQuestionsAsync(id);
            return Ok(questions);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitQuizDto request)
        {
            var result = await _quizService.SubmitQuizAsync(request);
            return Ok(result);
        }
    }
}
