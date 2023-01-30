namespace Interview.Domain.Certificates;

public sealed record CertificateDetail(string CandidateFullName, InterviewGrade Grade, string Description, string Sign = "GulagGazRoom");