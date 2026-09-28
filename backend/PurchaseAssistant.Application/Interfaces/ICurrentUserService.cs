using System;
using System.Collections.Generic;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string Email { get; }
        Guid? BusinessId { get; }
        string Role { get; }
        IEnumerable<string> Permissions { get; }
        bool HasPermission(string permission);
    }
}
