namespace SecureArchCore.Helper
{
    public static class PasswordHasher
    {
        public static string Hash(string input) => BCrypt.Net.BCrypt.HashPassword(input);
    }

}