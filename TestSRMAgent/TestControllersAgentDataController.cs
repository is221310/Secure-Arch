using SRMAgent.Controllers;

namespace TestSRMAgent
{
    public class TestControllersAgentDataController
    {
        [Fact]
        public void Get_ReturnsUpdatedAgentData()
        {
            // Arrange
            var controller = new AgentDataController();
            var beforeCall = DateTime.Now;

            // Act
            var result = controller.Get();
            var afterCall = DateTime.Now;

            // Assert
            Assert.NotNull(result);

            // Temperatur muss im Bereich [10, 30) liegen
            Assert.InRange(result.CurrentTemp, 10, 29);

            // KeepAliveTimestamp sollte zwischen beforeCall und afterCall liegen
            Assert.True(result.KeepAliveTimestamp >= beforeCall && result.KeepAliveTimestamp <= afterCall,
                $"KeepAliveTimestamp {result.KeepAliveTimestamp} liegt nicht im erwarteten Zeitfenster.");
        }
    }
}