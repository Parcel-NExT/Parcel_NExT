using System.Text;

namespace Parcel.Services
{
    public sealed class OllamaConfiguration
    {
        #region Defaults
        internal const string DefaultOllamaEndpoint = "http://localhost:11434/api";
        internal const string DefaultOllamaModel = "phi3";
        #endregion

        #region Methods
        public static OllamaConfiguration ConfigureOllama(string model = DefaultOllamaModel, string? endpoint = DefaultOllamaEndpoint)
        {
            return new OllamaConfiguration()
            {
                Model = model,
                Endpoint = endpoint ?? DefaultOllamaEndpoint,
            };
        }
        #endregion

        #region Configurations
        public string Model { get; set; } = DefaultOllamaModel;
        public string Endpoint { get; set; } = DefaultOllamaEndpoint;
        #endregion
    }

    /// <summary>
    /// Provides non-streamed API to Ollama
    /// </summary>
    public class Ollama
    {
        #region REST API Wrapper
        public static string OllamaComplete(string prompt, OllamaConfiguration? configuration = null)
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = new Uri($"{configuration.Endpoint}/generate")
            };

            using StringContent jsonContent = new(
                $$"""
                {
                  "model": "{{configuration.Model}}",
                  "prompt": "{{prompt}}",
                  "stream": false
                }
                """, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = httpClient.PostAsync(string.Empty, jsonContent).Result;
            HttpResponseMessage message = response.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            return jsonResponse;
        }
        public static string OllamaChat(string system, string user, OllamaConfiguration? configuration = null)
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = new Uri($"{configuration.Endpoint}/chat")
            };

            using StringContent jsonContent = new(
                $$"""
                {
                  "model": "{{configuration.Model}}",
                  "messages": [
                    { "role": "system", "content": "{{system}}" },
                    { "role": "user", "content": "{{user}}" }
                  ],
                  "stream": false
                }
                """, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = httpClient.PostAsync(string.Empty, jsonContent).Result;
            HttpResponseMessage message = response.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            return jsonResponse;
        }
        #endregion

        #region Higher Level Functions

        #endregion

        #region Managed Chat Sessions

        #endregion
    }
}
