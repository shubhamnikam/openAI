namespace API.openAI.Models
{
    public class OpenAIConfigOption
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string Engine { get; set; }
        public int Tokens { get; set; }
        public double Temperature { get; set; }
        public int TopP { get; set; }
        public int FrequencyPenalty { get; set; }
        public int PresencePenalty { get; set; }
    }
}
