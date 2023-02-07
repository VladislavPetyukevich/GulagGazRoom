using System.ComponentModel.DataAnnotations;
using Interview.Domain.Questions;
using Interview.Infrastructure.Questions;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Controllers.Questions;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{

    private readonly IQuestionRepository _questionRepository;

    public QuestionController(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    
    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<Question>> GetPage([Range(1, Int32.MaxValue)] int pageNumber)
    {
        return _questionRepository.GetPage(pageNumber, 30);
    }

    [HttpPost(nameof(Create))]
    public Task Create(Question room)
    {
        return _questionRepository.CreateAsync(room);
    }

    
}