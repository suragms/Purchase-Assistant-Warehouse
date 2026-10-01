using System;
using PurchaseAssistant.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PurchaseAssistant.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(72, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; } = Role.Staff;
    }

    public class UpdateUserDto
    {
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }
    }
}
