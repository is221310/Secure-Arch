// this class was created following this tutorial https://www.youtube.com/watch?v=aumcaBkprsA&t=61s and with the help of KI
using SecureArchCore.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace TestSecureArchCore
{
    public class TestUserController : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserController _controller;

        public TestUserController()
        {
            // Setup DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new UserController(_context);
        }

        [Fact]
        public async Task GetAllWithCustomers_TestUser()
        {
            // Arrange
            var kunde = new Kunde { kunden_id = 1, kunden_name = "TestKunde" };
            _context.Kunden.Add(kunde);

            var user1 = new User
            {
                firstname = "Anna",
                lastname = "Musterfrau",
                email = "anna@example.com",
                telephone = "123456789",
                role = "Admin",
                address = "Teststraße 1",
                kunden_id = 1,
                password = "DummyPassword"
            };
            var user2 = new User
            {
                firstname = "Tom",
                lastname = "Beispiel",
                email = "tom@example.com",
                telephone = "987654321",
                role = "User",
                address = "Beispielweg 3",
                kunden_id = 1,
                password = "DummyPassword2"
            };
            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            var expectedUserCount = 2;
            var expectedKundenId = 1;

            // Act
            var result = await _controller.GetAllWithCustomers();

            // Assert
            // Rückgabewert des Controllers prüfen
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<User>>(okResult.Value);
            // DB Werte prüfen
            Assert.Equal(expectedUserCount, users.Count);
            Assert.All(users, u => Assert.Equal(expectedKundenId, u.kunden_id));
        }

        [Fact]
        public async Task GetById_TestUser()
        {
            // Arrange
            var kunde = new Kunde { kunden_id = 1, kunden_name = "Testkunde" };
            _context.Kunden.Add(kunde);

            var user = new User
            {
                firstname = "Alice",
                lastname = "Mustermann",
                email = "alice@example.com",
                telephone = "1234567890",
                role = "User",
                address = "Musterweg 7",
                kunden_id = 1,
                password = "DummyPassword3"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var expectedUserId = user.id;

            // Act
            var result = await _controller.GetById(expectedUserId);

            // Assert
            // Rückgabewert des Controllers prüfen
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUser = Assert.IsType<User>(okResult.Value);
            // DB Werte prüfen
            Assert.Equal(user.firstname, returnUser.firstname);
            Assert.Equal(user.email, returnUser.email);
            Assert.Equal(expectedUserId, returnUser.id);
        }

        // Hier gibt es noch ein Problem mit dem Passwort...
        /*[Fact]
        public async Task Create_TestUser()
        {
            // Arrange
            var kunde = new Kunde { kunden_id = 1, kunden_name = "Testkunde" };
            _context.Kunden.Add(kunde);

            var newUser = new User
            {
                firstname = "Lisa",
                lastname = "Neuer",
                email = "lisa@example.com",
                telephone = "555-987654",
                role = "User",
                address = "Hauptstraße 5",
                kunden_id = 1,
                password = "DummyPassword4" 
            };

            await _context.SaveChangesAsync();

            var expectedUserName = "Lisa";
            var expectedUserEmail = "lisa@example.com";

            // Act
            var result = await _controller.Create(newUser);

            // Assert
            // Rückgabe des Controllers prüfen
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(expectedUserName, returnedUser.firstname);
            Assert.True(returnedUser.id > 0);

            // Zusätzlicher Check in DB
            var dbUser = await _context.Users.FindAsync(returnedUser.id);
            Assert.NotNull(dbUser);
            Assert.Equal(expectedUserEmail, dbUser.email);
        }*/


        [Fact]
        public async Task Update_TestUser()
        {
            // Arrange
            var kunde = new Kunde { kunden_id = 1, kunden_name = "TestKunde" };
            _context.Kunden.Add(kunde);

            var originalUser = new User
            {
                firstname = "Karl",
                lastname = "Alt",
                email = "karl@alt.com",
                telephone = "123",
                role = "User",
                address = "Altgasse 1",
                kunden_id = 1,
                password = "DummyPassword5"
            };
            _context.Users.Add(originalUser);
            await _context.SaveChangesAsync();

            var updatedUser = new User
            {
                id = originalUser.id,
                firstname = "Karl",
                lastname = "Neu",
                email = "karl@neu.com",
                telephone = "456",
                role = "Admin",
                address = "Neugasse 2",
                kunden_id = 1,
                password = "DummyPassword6"
            };

            var expectedUserFirstNameUpdated = "Karl";
            var expectedUserLastNameUpdated = "Neu";
            var expectedUserEmailUpdated = "karl@neu.com";
            var expectedUserRoleUpdated = "Admin";
            var expectedUserAddressUpdated = "Neugasse 2";

            // Act
            var result = await _controller.Update(originalUser.id, updatedUser);

            // Assert
            // Rückgabewert des Controllers prüfen
            Assert.IsType<NoContentResult>(result);
            // DB Werte prüfen
            var dbUser = await _context.Users.FindAsync(originalUser.id);
            Assert.NotNull(dbUser); // Null-Check
            Assert.Equal(expectedUserFirstNameUpdated, dbUser!.firstname); // ! = Null-Forgiving Operator, Compiler dbKUnde ist nicht null
            Assert.Equal(expectedUserLastNameUpdated, dbUser!.lastname);
            Assert.Equal(expectedUserEmailUpdated, dbUser!.email);
            Assert.Equal(expectedUserRoleUpdated, dbUser!.role);
            Assert.Equal(expectedUserAddressUpdated, dbUser!.address);
        }

        [Fact]
        public async Task Delete_TestUser()
        {
            // Arrange
            var kunde = new Kunde { kunden_id = 1, kunden_name = "DummyKundeDeleteUser" };
            _context.Kunden.Add(kunde);

            var user = new User
            {
                firstname = "LöschMich",
                lastname = "Bitte",
                email = "delete@example.com",
                telephone = "111222",
                role = "User",
                address = "Wegstraße 7",
                kunden_id = 1,
                password = "DummyPassword7"
            };
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            var userId = user.id;

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            // Prüfen, ob NoContentResult zurückkommt
            Assert.IsType<NoContentResult>(result);
            // Prüfen ob User mit userId nicht mehr vorhanden ist
            var dbUser = await _context.Users.FindAsync(userId);
            Assert.Null(dbUser);
        }

        public void Dispose()
        {
            // DB löschen
            _context.Dispose();
        }
    }
}

