using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class PollyController : Controller
    {
        private readonly ILogger<PollyController> logger;

        public PollyController(ILogger<PollyController> logger)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Text"] = "Ciao! Mi chiamo Carla e mi piace la pizza!";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TextToSpeech([FromServices] ITextToSpeechService tts, TextToSpeechRequest request)
        {
            logger.LogInformation($"Received {request.Text}");

            var response = await tts.ReadAloudAsync(request);

            return File(response.AudioStream, response.ContentType, "polly.mp3");
        }
    }
}
