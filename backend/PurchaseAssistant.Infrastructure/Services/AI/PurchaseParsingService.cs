using System.Text.Json;
using Microsoft.Extensions.Logging;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class PurchaseParsingService : IPurchaseParsingService
{
    private readonly IAIRoutingService _routingService;
    private readonly ICatalogService _catalogService;
    private readonly ISupplierService _supplierService;
    private readonly ILogger<PurchaseParsingService> _logger;

    public PurchaseParsingService(
        IAIRoutingService routingService,
        ICatalogService catalogService,
        ISupplierService supplierService,
        ILogger<PurchaseParsingService> logger)
    {
        _routingService = routingService;
        _catalogService = catalogService;
        _supplierService = supplierService;
        _logger = logger;
    }

        public async Task<PurchaseIntentCandidateDto> ParseAsync(string prompt, CancellationToken ct = default)
        {
            var systemPrompt = @"You are a purchase assistant for a warehouse.
Parse the user request into a structured JSON representation of the purchase intent.

Return ONLY JSON matching this structure:
{
  ""Status"": ""Success"" | ""AmbiguousMatch"" | ""MissingInformation"" | ""Error"",
  ""Message"": string,
  ""SupplierId"": Guid | null,
  ""SupplierName"": string | null,
  ""Items"": [
    {
      ""ItemCode"": string | null,
      ""CatalogItemName"": string | null,
      ""RequestedQuantity"": decimal,
      ""UnitOfMeasure"": string | null
    }
  ],
  ""Notes"": string | null
}

Do not calculate totals, prices, or stock.
If supplier or items are ambiguous, set status to ""AmbiguousMatch"".
If information is missing, use ""MissingInformation"" status.";

            var aiRequest = new AIRequest(prompt, systemPrompt);
            var aiResponse = await _routingService.ExecuteWithFailoverAsync(aiRequest, ct);

            if (!aiResponse.Success)
            {
                return new PurchaseIntentCandidateDto(IntentStatus.Error, aiResponse.Error, null, null, new(), null, null);
            }

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var candidate = JsonSerializer.Deserialize<PurchaseIntentCandidateDto>(aiResponse.Content!, options);

                if (candidate == null) throw new JsonException("Failed to deserialize");

                // Validate and resolve IDs
                return await ValidateAndResolveAsync(candidate, ct);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse AI response: {Content}", aiResponse.Content);
                return new PurchaseIntentCandidateDto(IntentStatus.Error, "Parsing error", null, null, new(), null, null);
            }
        }

        private async Task<PurchaseIntentCandidateDto> ValidateAndResolveAsync(PurchaseIntentCandidateDto candidate, CancellationToken ct)
        {
            var validatedItems = new List<PurchaseIntentItemCandidateDto>();
            Guid? supplierId = null;

            // Resolve Supplier
            var suppliers = await _supplierService.GetAllAsync(ct);
            if (!string.IsNullOrEmpty(candidate.SupplierName))
            {
                var match = suppliers.FirstOrDefault(s => s.Name.Equals(candidate.SupplierName, StringComparison.OrdinalIgnoreCase));
                if (match != null) supplierId = match.Id;
                else return candidate with { Status = IntentStatus.MissingInformation, Message = $"Supplier '{candidate.SupplierName}' not found." };
            }
            else if (candidate.SupplierId.HasValue)
            {
                if (!suppliers.Any(s => s.Id == candidate.SupplierId.Value))
                    return candidate with { Status = IntentStatus.MissingInformation, Message = "Supplier not found." };
                supplierId = candidate.SupplierId;
            }

            // Resolve Items
            foreach (var item in candidate.Items)
            {
                var catalogResult = await _catalogService.GetAllAsync(1, 50, item.ItemCode ?? item.CatalogItemName, null, ct);

                if (catalogResult.Data.Count == 0)
                {
                    validatedItems.Add(item with { IsAmbiguous = true });
                    continue;
                }

                if (catalogResult.Data.Count == 1)
                {
                    var match = catalogResult.Data[0];
                    validatedItems.Add(new PurchaseIntentItemCandidateDto(
                        match.Id,
                        match.ItemCode,
                        match.Name,
                        item.RequestedQuantity,
                        item.UnitOfMeasure,
                        false,
                        null));
                }
                else
                {
                    // Ambiguous
                    var options = catalogResult.Data.Select(d => new CatalogItemOptionDto(d.Id, d.Name, d.CategoryName)).ToList();
                    validatedItems.Add(new PurchaseIntentItemCandidateDto(
                        null,
                        item.ItemCode,
                        item.CatalogItemName,
                        item.RequestedQuantity,
                        item.UnitOfMeasure,
                        true,
                        options));
                }
            }

            var status = validatedItems.Any(i => i.IsAmbiguous) ? IntentStatus.AmbiguousMatch : IntentStatus.Success;
            return new PurchaseIntentCandidateDto(
                status,
                status == IntentStatus.AmbiguousMatch ? "Some items are ambiguous." : null,
                supplierId,
                candidate.SupplierName,
                validatedItems,
                candidate.Notes,
                null
            );
        }
}
