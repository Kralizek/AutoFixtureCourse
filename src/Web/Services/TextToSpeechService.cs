
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.Extensions.Logging;

namespace Web.Services
{
    public interface ITextToSpeechService
    {
        Task<TextToSpeechResponse> ReadAloudAsync(TextToSpeechRequest request);
    }

    public class TextToSpeechRequest
    {
        public string Text { get; set; }
    }

    public class TextToSpeechResponse
    {
        public Stream AudioStream { get; set; }

        public string ContentType { get; set; }
    }

    public class PollyTextToSpeechService : ITextToSpeechService
    {
        private readonly IAmazonPolly polly;
        private readonly ILogger<PollyTextToSpeechService> logger;

        public PollyTextToSpeechService(IAmazonPolly polly, ILogger<PollyTextToSpeechService> logger)
        {
            this.polly = polly ?? throw new System.ArgumentNullException(nameof(polly));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task<TextToSpeechResponse> ReadAloudAsync(TextToSpeechRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            logger.LogInformation($"Processing {request.Text}");

            var response = await polly.SynthesizeSpeechAsync(new SynthesizeSpeechRequest {
                Text = request.Text,
		        OutputFormat = OutputFormat.Mp3,
                VoiceId = Amazon.Polly.VoiceId.Carla
            });

            var result = new TextToSpeechResponse
            {
                AudioStream = response.AudioStream,
                ContentType = response.ContentType
            };

            return result;
        }
    }
}