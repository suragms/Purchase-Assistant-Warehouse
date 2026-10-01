using PurchaseAssistant.Domain.Entities;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(User user, Membership? activeMembership, Guid sessionId);
        string GenerateRandomToken();
    }
}
