using WTelegram;

namespace Parcel.Services
{
    /// <summary>
    /// Technical configurations
    /// </summary>
    /// <remarks>
    /// You can obtain your api_id/api_hash at https://my.telegram.org/apps
    /// </remarks>
    public sealed class TelegramServicesConfiguration
    { 
        public int APPAPIID { get; set; }
        public string APPAPIHash { get; set; }
        public string PhoneNumber { get; set; } // E.g. "+12025550156"
        public string VerificationCode { get; set; }

        #region If Sign Up Is Required
        public string FirstName { get; set; }
        public string LastName { get; set; }
        #endregion

        #region If User has Enabled 2FA
        public string Password { get; set; }
        #endregion
    }
    public static class TelegramServices
    {
        #region Client
        private static Client ConnectedClient { get; set; }
        private static bool IsConnected => ConnectedClient != null && ConnectedClient.User != null;
        #endregion

        #region Key Methods
        public static TelegramServicesConfiguration ConfigureTelegramService(int apiID, string apiHash, string phoneNumber, string verificationCode, string firstName, string lastName, string password)
        {
            return new TelegramServicesConfiguration()
            {
                APPAPIHash = apiHash,
                APPAPIID = apiID,
                PhoneNumber = phoneNumber,
                VerificationCode = verificationCode,
                FirstName = firstName,
                LastName = lastName,
                Password = password
            };
        }
        public static string[] GetContacts(TelegramServicesConfiguration configuration)
        {
            if (!IsConnected) 
                DoLogin(configuration);
            TL.Messages_Chats chats = ConnectedClient.Messages_GetAllChats().Result;

            return chats.chats.Select(c => $"ID: {c.Key} Title: {c.Value.Title} Chat: {c.Value}").ToArray();
        }
        public static string[] GetNotifications(TelegramServicesConfiguration configuration)
        {
            throw new NotImplementedException();
        }
        public static string[] GetMessages(string contact, TelegramServicesConfiguration configuration)
        {
            throw new NotImplementedException();
        }
        public static void SendMessage(string contact, string message, TelegramServicesConfiguration configuration)
        {
            if (!IsConnected)
                DoLogin(configuration);

            TL.Messages_Chats chats = ConnectedClient.Messages_GetAllChats().Result;

            // This user has joined the following
            Dictionary<long, TL.ChatBase> contacts = chats.chats.ToDictionary(c => c.Key, c => c.Value); // From ID to Chat

            // Select chat using ID
            long chatId = long.Parse(contact);
            TL.ChatBase target = chats.chats[chatId];
            // Console.WriteLine($"Sending a message in chat {chatId}: {target.Title}");
            ConnectedClient.SendMessageAsync(target, message).Wait();
        }
        #endregion

        #region Routines

        #endregion

        #region Helpers
        static string? ClientConfig(TelegramServicesConfiguration configuration, string what)
        {
            switch (what)
            {
                case "api_id": return configuration.APPAPIID.ToString();
                case "api_hash": return configuration.APPAPIHash;
                case "phone_number": return configuration.PhoneNumber;
                case "verification_code": return string.IsNullOrEmpty(configuration.VerificationCode) ? throw new ArgumentException("Need verification code.") : configuration.VerificationCode;
                case "first_name": return configuration.FirstName;
                case "last_name": return configuration.LastName;
                case "password": return configuration.Password;
                default: return null;                  // let WTelegramClient decide the default config
            }
        }
        static void DoLogin(TelegramServicesConfiguration configuration)
        {
            ConnectedClient = new Client(configuration.APPAPIID, configuration.APPAPIHash);
            string identity = ConnectedClient.Login(configuration.PhoneNumber).Result;

            if (ConnectedClient.User == null)
                throw new UnauthorizedAccessException($"More login information is needed: {identity}");
            else
                Console.WriteLine($"We are logged-in as {ConnectedClient.User} (id {ConnectedClient.User.id})");
        }
        #endregion
    }
}
