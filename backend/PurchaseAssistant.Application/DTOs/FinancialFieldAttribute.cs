namespace PurchaseAssistant.Application.DTOs;

// Response contracts explicitly classify decimal values; new unclassified values are hidden from nonowners.
[AttributeUsage(AttributeTargets.Property)]
public sealed class FinancialFieldAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public sealed class OperationalNumericAttribute : Attribute { }
