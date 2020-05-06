using System;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class TestDataCreator
    {
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private Random random = new Random();  
        private readonly DatabaseHelperProvider _provider;

        public TestDataCreator(DatabaseHelperProvider provider)
        {
            this._provider = provider;
        }

        public void CreateUserData(int amount = 250)
        {
            for (int i = 0; i < amount; i++)
            {
                UserData user = _provider.Users.NewUser();
                user.Username = $"TestUser-{i}";
                _provider.Authorization.AddAuthorizedUser(user.UserId, GenerateRandomString(), (ServiceType) random.Next(0, 3));
            }
        }

        private string GenerateRandomString(int length = 16)
        {
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

    }
}