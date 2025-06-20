// this class was created following this tutorial https://www.youtube.com/watch?v=aumcaBkprsA&t=61s and with the help of KI
using Blazored.LocalStorage;
using Moq;
using Moq.Protected;
using SecureArchApp.Services;
using System.Net;
using System.Net.Http.Json;

namespace TestSecureArchApp
{
    public class TestServicesAuthService
    {
        [Fact]
        public async Task LoginAsync_ReturnsTrueAndStoresToken_WhenSuccessful()
        {
            // Arrange
            // Mock für den HttpMessageHandler
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            // Mock für den lokalen Speicher-Service
            var mockLocalStorage = new Mock<ILocalStorageService>();

            // Definition Token und Ablaufzeit
            var expectedToken = "test_token";
            var expectedExpiration = DateTime.UtcNow.AddMinutes(10);

            // Simulation positive AuthService Response
            var authResponse = new AuthService.AuthResponse
            {
                Username = "testuser",
                Access_Token = expectedToken,
                Access_Token_Expiration = expectedExpiration,
                Refresh_Token = "refresh",
                Refresh_Token_Expiration = expectedExpiration.AddDays(1),
                Token_Type = "Bearer"
            };

            // Erstelle eine HttpResponseMessage mit Status 200 OK und der simulierten Antwort als JSON
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(authResponse)
            };

            // Konfiguration wie sich MockHttpMessageHandler verhalten soll --> immer obige Antwort senden
            mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            //  Erstellen einer AuthService-Instanz mit dem HttpClient und dem gemockten lokalen Speicher
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:8000/")
            };
            var authService = new AuthService(httpClient, mockLocalStorage.Object);

            // Act
            // Login mit username und passwort
            var result = await authService.LoginAsync("testuser", "securepassword");

            // Assert
            // Prüfen ob als Ergebnis true zurückkommt = erfolgreicher Login
            Assert.True(result);
            // Prüfen ob der Access Token und das Ablaufdatum richtig gespeichert wurden
            mockLocalStorage.Verify(s => s.SetItemAsync("access_token", expectedToken, It.IsAny<CancellationToken>()), Times.Once);
            mockLocalStorage.Verify(s => s.SetItemAsync("access_token_expiration", expectedExpiration, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}