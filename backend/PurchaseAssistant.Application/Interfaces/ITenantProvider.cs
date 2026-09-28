using System;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface ITenantProvider
    {
        Guid GetBusinessId();
    }
}