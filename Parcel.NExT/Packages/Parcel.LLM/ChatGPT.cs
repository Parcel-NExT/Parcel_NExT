using System.Net.Http.Headers;
using System.Text;

namespace Parcel.Services
{
    public sealed class ChatGPTConfiguration
    {
        #region Enums
        public enum ChatGPTModel
        {
            GPT4Turbo
        }
        #endregion

        #region Defaults
        internal const string OpenAIChatCompletionEndpoint = "https://api.openai.com/v1/chat/completions";
        internal const int DefaultTokenSizeLimit = 4000;

        internal const string GPT4TurboModel = "gpt-4-turbo-preview";
        #endregion

        #region Methods
        public static ChatGPTConfiguration ConfigureChatGPT(string aipToken, int defaultTokenSizeLimit, ChatGPTModel model)
        {
            return new ChatGPTConfiguration()
            {
                APIToken = aipToken,
                TokenSizeLimit = defaultTokenSizeLimit,
                Model = model switch
                {
                    ChatGPTModel.GPT4Turbo => GPT4TurboModel,
                    _ => throw new ArgumentOutOfRangeException($"Invalid model: {model}")
                }
            };
        }
        #endregion

        #region Configurations
        public string APIToken { get; set; }
        public int TokenSizeLimit { get; set; } = DefaultTokenSizeLimit;
        public string Model { get; set; } = GPT4TurboModel;
        #endregion
    }

    /// <summary>
    /// API for ChatGPT
    /// </summary>
    public static class ChatGPT
    {
        #region Methods
        public static string Complete(string query, ChatGPTConfiguration configuration)
        {
            return Complete(string.Empty, query, configuration.Model, configuration.APIToken, configuration.TokenSizeLimit);
        }
        public static string Complete(string system, string query, string model, string apiToken, int sizeLimit)
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = new Uri(ChatGPTConfiguration.OpenAIChatCompletionEndpoint)
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            using StringContent jsonContent = new(
                $$"""
                {
                    "messages": [
                        {
                            "role": "system",
                            "content": "{{system}}"
                        },
                        {
                            "role": "user",
                            "content": "{{query}}"
                        }
                    ],
                    "model": "{{model}}",
                    "max_tokens": {{sizeLimit}}
                }
                """, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = httpClient.PostAsync(string.Empty, jsonContent).Result;
            HttpResponseMessage message = response.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            return jsonResponse;
        }
        public static string Complete(string query, string token)
            => Complete(string.Empty, query, ChatGPTConfiguration.GPT4TurboModel, token, ChatGPTConfiguration.DefaultTokenSizeLimit);
        #endregion

        #region Higher Level Interface
        public static string AskChatGPTAboutData(string question, string dataCSV, string apiToken)
            => throw new NotImplementedException();
        public static string AskChatGPTAboutImage(string question, string imageReference, string apiToken)
            => throw new NotImplementedException();
        #endregion
    }
}
