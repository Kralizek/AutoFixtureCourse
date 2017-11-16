using System.Threading;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Web.Services;

namespace Tests.Services
{
    [TestFixture]
    public class PollyTextToSpeechServiceTests
    {
        IFixture fixture;
        Mock<IAmazonPolly> mockPolly;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture();

            fixture.Register<System.IO.Stream>(() => System.IO.Stream.Null);

            mockPolly = new Mock<IAmazonPolly>();
        }

        private PollyTextToSpeechService CreateSystemUnderTest()
        {
            return new PollyTextToSpeechService(mockPolly.Object, Mock.Of<ILogger<PollyTextToSpeechService>>());
        }

        [Test]
        public async Task ReadAloudAsync_forwards_request_to_Polly()
        {
            var sut = CreateSystemUnderTest();

            var request = fixture.Create<TextToSpeechRequest>();

            var response = fixture.Create<SynthesizeSpeechResponse>();

            mockPolly.Setup(p => p.SynthesizeSpeechAsync(It.IsAny<SynthesizeSpeechRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response)
                        .Verifiable();

            var actual = await sut.ReadAloudAsync(request);

            Assert.That(actual.AudioStream, Is.SameAs(response.AudioStream));
            Assert.That(actual.ContentType, Is.EqualTo(response.ContentType));

            mockPolly.VerifyAll();
        }
    }
}