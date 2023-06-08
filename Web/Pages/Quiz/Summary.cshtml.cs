using ApplicationCore.Interfaces.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.Xml.Linq;

namespace Web.Pages;

public class Summary : PageModel
{   
    private readonly IQuizUserService _userService;
    private readonly PdfGenerator.PdfGeneratorClient _client;

    public int CorrectAnswerCount { get; set; }
    public Summary(IQuizUserService userService, PdfGenerator.PdfGeneratorClient client)
    {
        _userService = userService;
        _client = client;
    }

    public void OnGet(int quizId, int userId)
    {
        CorrectAnswerCount = _userService.CountCorrectAnswersForQuizFilledByUser(quizId, userId);
    }
    public async Task<ActionResult> OnPostAsync()
    {
        var document = await _client.GenerateAsync(new HtmlDocumentRequest()
            { Content = $"<html><p>Points: {CorrectAnswerCount}</p></html>", Name = "Certificate" });

        var stream = new MemoryStream(document.Content.ToByteArray());
        return new FileStreamResult(stream, new MediaTypeHeaderValue("application/pdf"))
        {

            FileDownloadName = "result.pdf"
        };
    }
}