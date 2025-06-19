using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using Moq.Protected;

namespace TestSecureArchAppClient
{
    public class TestServicesCredentialsHandler
    {
        [Fact]
        public async Task SendAsync_TriggersRedirectToLogin_OnUnauthorizedResponse()
        {
            // Arrange

            // Aktuelle URL simulieren
            var currentUrl = "https://localhost:5001/protected";

            // Mock NavigationManager – mit einer Ableitung, da es abstrakt ist
            var mockNavManager = new MockNavigationManager(currentUrl);

            // JSRuntime wird nicht verwendet, aber muss im Konstruktor übergeben werden
            var mockJsRuntime = new Mock<IJSRuntime>();

            // Mock Handler, das eine Unauthorized-Antwort zurückgibt
            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

            // Erstelle CredentialsHandler mit dem gemockten Handler als InnerHandler
            var handler = new CredentialsHandler(mockJsRuntime.Object, mockNavManager)
            {
                InnerHandler = mockInnerHandler.Object
            };

            var invoker = new HttpMessageInvoker(handler);

            // Dummy-Request
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/data");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.True(mockNavManager.NavigateCalled, "Navigation to login page was not triggered.");
            Assert.Equal("/login?returnUrl=https%3A%2F%2Flocalhost%3A5001%2Fprotected", mockNavManager.NavigatedTo);
        }

    }
    public class MockNavigationManager : NavigationManager
    {
        public bool NavigateCalled { get; private set; } = false;
        public string? NavigatedTo { get; private set; }

        public MockNavigationManager(string baseUri)
        {
            Initialize(baseUri, baseUri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NavigateCalled = true;
            NavigatedTo = uri;
        }
    }
}