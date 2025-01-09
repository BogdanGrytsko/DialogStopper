using Newtonsoft.Json;
using System.Text;

namespace HackatonEV2025;

public class OpenAIApiCaller
{
    //MINI:
    //private const string API_KEY = "0498b625c9cf4de2bf60c00fa0ee5fdd"; // Set your key here
    //private const string ENDPOINT = "https://openai-track06-hackathon.openai.azure.com/openai/deployments/gpt-4o-mini-deployment/chat/completions?api-version=2024-02-15-preview";

    //FULL:
    private const string API_KEY = "0498b625c9cf4de2bf60c00fa0ee5fdd"; // Set your key here
    private const string ENDPOINT = "https://openai-track06-hackathon.openai.azure.com/openai/deployments/gpt-4o-deployment/chat/completions?api-version=2024-08-01-preview";

    public async Task<OpenAIResponse> Execute(string prompt)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);
        var payload = new
        {
            messages = new object[]
            {
                  new {
                      role = "system",
                      content = new object[] {
                          new {
                              type = "text",
                              text = "You are an AI assistant that helps people analyze data."
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                          new {
                              type = "text",
                              text = prompt
                          }
                      }
                  }
            },
            temperature = 0.7,
            top_p = 0.95,
            max_tokens = 800,
            stream = false
        };

        var response = await httpClient.PostAsync(ENDPOINT, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));


        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<OpenAIResponse>(content);
            Console.WriteLine(JsonConvert.DeserializeObject<dynamic>(content));
            return responseData!;
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}, {responseBody}");
            throw new Exception(responseBody);
        }
    }
}