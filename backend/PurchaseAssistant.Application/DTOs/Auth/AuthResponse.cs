using System;
using System.Collections.Generic;

namespace PurchaseAssistant.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public long ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public BusinessContextDto? CurrentBusiness { get; set; }
        public List<BusinessSummaryDto> Businesses { get; set; } = new();
    }

    public class BusinessContextDto
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}