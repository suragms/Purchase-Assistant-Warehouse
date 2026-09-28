using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PurchaseAssistant.Application.DTOs.Auth
{
    public class BusinessSummaryDto
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}