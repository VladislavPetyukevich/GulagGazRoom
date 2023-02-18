using Ardalis.SmartEnum;

namespace Interview.Domain.Certificates;

public sealed class InterviewGrade : SmartEnum<InterviewGrade>
{
    public static readonly InterviewGrade Awful = new ("Ужасно", 0);
    public static readonly InterviewGrade Bad = new ("Плохо", 1);
    public static readonly InterviewGrade NotBad = new ("Не плохо", 2);
    public static readonly InterviewGrade Good = new ("Хорошо", 3);
    public static readonly InterviewGrade Perfect = new ("Превосходно", 4);

    private InterviewGrade(string name, int value)
        : base(name, value)
    {
    }
}
