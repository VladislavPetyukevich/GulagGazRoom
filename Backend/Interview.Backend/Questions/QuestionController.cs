using Interview.Backend.Shared;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Questions;

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
    public Task<IPagedList<Question>> GetPage([FromQuery] PageRequest request)
    {
        return _questionRepository.GetPage(request.PageNumber, request.PageSize);
    }

    [HttpPost(nameof(Create))]
    public Task Create(Question room)
    {
        return _questionRepository.CreateAsync(room);
    }
}
