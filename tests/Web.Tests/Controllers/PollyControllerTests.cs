using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Web.Controllers;
using Web.Services;

namespace Tests.Controllers
{
    [TestFixture]
    public class PollyControllerTests 
    {
        IFixture fixture = new Fixture();

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture();

            fixture.Register<System.IO.Stream>(() => System.IO.Stream.Null);
        }

        private PollyController CreateSystemUnderTest()
        {
            return new PollyController(Mock.Of<ILogger<PollyController>>());
        }

        [Test]
        [InlineAutoMoqData]
        public async Task TextToSpeech_uses_service(ITextToSpeechService service, TextToSpeechRequest request)
        {
            var sut = CreateSystemUnderTest();

            var response = fixture.Build<TextToSpeechResponse>()
                                    .With(p => p.ContentType, "some/valid-mime")
                                    .Create();

            Mock.Get(service).Setup(p => p.ReadAloudAsync(It.IsAny<TextToSpeechRequest>()))
                            .ReturnsAsync(response);

            var result = await sut.TextToSpeech(service, request);

            Assert.That(result, Is.InstanceOf<FileResult>());
        }
    }
}