using Ardalis.SmartEnum;

namespace Interview.Domain.Certificates;

public sealed class InterviewGrade : SmartEnum<InterviewGrade>
{
    public static readonly InterviewGrade Awful = new InterviewGrade("Ужасно", 0);
    public static readonly InterviewGrade Bad = new InterviewGrade("Плохо", 1);
    public static readonly InterviewGrade NotBad = new InterviewGrade("Не плохо", 2);
    public static readonly InterviewGrade Good = new InterviewGrade("Хорошо", 3);
    public static readonly InterviewGrade Perfect = new InterviewGrade("Превосходно", 4);
    
    private InterviewGrade(string name, int value) : base(name, value)
    {
    }
}