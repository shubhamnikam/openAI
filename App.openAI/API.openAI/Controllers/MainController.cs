using API.openAI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Configuration;
using System.Globalization;
using System.Net.Http.Headers;

namespace API.openAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> logger;
        private readonly OpenAIConfigOption openAIConfigOption;

        public MainController(ILogger<MainController> logger, IOptions<OpenAIConfigOption> openAIConfigOption)
        {
            this.logger = logger;
            this.openAIConfigOption = openAIConfigOption.Value;
        }
        [HttpPost("GetAnswer")]
        public async Task<IActionResult> GetAnswer([FromBody] RequestModel requestModel)
        {
            try
            {
                if (string.IsNullOrEmpty(requestModel?.Question))
                {
                    throw new Exception("question is empty");
                }

                //call the open ai
                var answer = await CallOpenAI(requestModel?.Question);

                if (string.IsNullOrEmpty(answer))
                {
                    throw new Exception("answer is empty");
                }

                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = $"{ex.Message}" });
            }
        }

        private async Task<string> CallOpenAI(string question)
        {

            var apiCall = $"{openAIConfigOption?.BaseUrl}{openAIConfigOption?.Engine}/completions";

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAIConfigOption.ApiKey);
                    request.Content = new StringContent("{\n  \"prompt\": \"" + question + "\",\n  \"temperature\": " +
                                                        openAIConfigOption.Temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + openAIConfigOption.Tokens + ",\n  \"top_p\": " + openAIConfigOption.TopP +
                                                        ",\n  \"frequency_penalty\": " + openAIConfigOption.FrequencyPenalty + ",\n  \"presence_penalty\": " + openAIConfigOption.PresencePenalty + "\n}");

                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();

                    dynamic dynObj = JsonConvert.DeserializeObject<dynamic>(json);

                    if (dynObj != null)
                    {
                        return dynObj.choices[0].text.ToString();
                    }
                }
            }
            return null;

        }
    }
}
