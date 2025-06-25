// this class was created following this tutorial https://www.youtube.com/watch?v=aumcaBkprsA&t=61s and with the help of KI
using SecureArchCore.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SecureArchCore.Models;

namespace TestSecureArchCore
{
    public class TestKundeController : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly KundeController _controller;

        public TestKundeController()
        {
            // Setup DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new KundeController(_context);
        }

        [Fact]
        public async Task GetAll_ReturnsAllKunden()
        {
            // Arrange
            // Testkunden anlegen
            _context.Kunden.AddRange( 
                new Kunde { kunden_id = 1, kunden_name = "Kunde A" },
                new Kunde { kunden_id = 2, kunden_name = "Kunde B" }
            );
            _context.SaveChanges();

            var expectedReturnValue = 2;

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Kunde>>(okResult.Value);
            Assert.Equal(expectedReturnValue, returnValue.Count);
        }

        [Fact]
        public async Task Update_Kunde_UpdatesAndReturnsNoContent()
        {
            // Arrange
            // Kunde mit ID 1 anlegen
            var existing = new Kunde 
            {
                kunden_id = 1,
                kunden_name = "Kunde Neu",
                created_at = DateTime.UtcNow
            };
            _context.Kunden.Add(existing);
            await _context.SaveChangesAsync();

            // Neuer Kunde mit ID 1 mit geänderten Daten
            var updated = new Kunde 
            {
                kunden_id = 1,
                kunden_name = "Kunde Updated",
                created_at = existing.created_at
            };

            var expectedKundenNameNachUpdate = "Kunde Updated";

            // Act
            var result = await _controller.Update(1, updated);

            // Assert
            // Prüfen, ob NoContentResult zurückkommt
            Assert.IsType<NoContentResult>(result);

            // Kunde mit ID 1 aus DB laden und Namen prüfen
            var dbKunde = await _context.Kunden.FindAsync(1);
            Assert.NotNull(dbKunde); // Null-Check
            Assert.Equal(expectedKundenNameNachUpdate, dbKunde!.kunden_name); // ! = Null-Forgiving Operator, Compiler dbKUnde ist nicht null
        }

        [Fact]
        public async Task Create_Kunde()
        {
            // Arrange
            var newKunde = new Kunde
            {
                kunden_id = 1,
                kunden_name = "Neuester Kunde",
                created_at = DateTime.UtcNow
            };

            var expectedKundenNameNachCreate = "Neuester Kunde";

            // Act
            var result = await _controller.Create(newKunde);

            // Assert
            // Rückgabewert des Controllers prüfen
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<Kunde>(createdResult.Value);
            Assert.Equal(expectedKundenNameNachCreate, returnValue.kunden_name);

            // Eintrag in DB prüfen
            var dbKunde = await _context.Kunden.FindAsync(1);
            Assert.NotNull(dbKunde);
            Assert.Equal(expectedKundenNameNachCreate, dbKunde.kunden_name);
        }

        [Fact]
        public async Task Delete_Kunde()
        {
            // Arrange
            // Kunde für Löschung anlegen
            var existing = new Kunde
            {
                kunden_id = 1,
                kunden_name = "Zulöschender Kunde",
                created_at = DateTime.UtcNow
            };
            _context.Kunden.Add(existing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(1);

            // Assert
            // Prüfen, ob NoContentResult zurückkommt
            Assert.IsType<NoContentResult>(result);

            // Prüfen ob Kunde mit ID 1 nicht mehr vorhanden ist
            var dbKunde = await _context.Kunden.FindAsync(1);
            Assert.Null(dbKunde);
        }

        public void Dispose()
        {
            // DB löschen
            _context.Dispose();
        }
    }
}
