using System.ComponentModel.DataAnnotations;
using Interview.Domain.Questions;
using Interview.Infrastructure.Constants;
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
    public Task<IPagedList<Question>> GetPage([Range(1, int.MaxValue)] int pageNumber,
        [Range(1, PageProperty.DefaultSize)] int pageSize)
    {
        return _questionRepository.GetPage(pageNumber, pageSize);
    }

    [HttpPost(nameof(Create))]
    public Task Create(Question room)
    {
        return _questionRepository.CreateAsync(room);
    }
}