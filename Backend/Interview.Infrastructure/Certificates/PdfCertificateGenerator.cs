using Interview.Domain.Certificates;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Interview.Infrastructure.Certificates;

public sealed class PdfCertificateGenerator : ICertificateGenerator
{
    public Task<Stream> GenerateAsync(CertificateDetail detail, CancellationToken cancellationToken = default)
    {
        var document = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.DefaultTextStyle(x => x.FontFamily("Times New Roman"));
                page.Margin(1, Unit.Inch);

                page.Header()
                    .Text(detail.CandidateFullName)
                    .FontSize(36);
                
                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(0.1f, Unit.Inch);
                        column.Item()
                            .Text("Результат прохождения: " + detail.Grade.Name)
                            .FontSize(18);

                        column.Item()
                            .Text(detail.Description)
                            .FontSize(18);
                    });

                page.Footer()
                    .Text(text =>
                    {
                        text.Line(detail.Sign).FontSize(18);
                        text.AlignRight();
                    });
            });
        });
        
        return Task.FromResult<Stream>(new MemoryStream(document.GeneratePdf()));
    }
}