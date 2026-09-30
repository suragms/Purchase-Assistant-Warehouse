namespace PurchaseAssistant.Application.DTOs.AI
{
    public class AiOptions
    {
        public bool Enabled { get; set; } = true;
        public string DefaultProvider { get; set; } = "OpenAI";
        // Add other settings as needed
    }
}
