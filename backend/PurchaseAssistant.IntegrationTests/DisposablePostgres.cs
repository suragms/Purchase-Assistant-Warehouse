using Npgsql;
using Xunit;

namespace PurchaseAssistant.IntegrationTests;

internal static class DisposablePostgres
{
    private const string VariableName = "PURCHASE_ASSISTANT_TEST_DATABASE";
    private const string DatabasePrefix = "wa_test_";

    public static bool IsConfiguredSafely
    {
        get
        {
            var configured = Environment.GetEnvironmentVariable(VariableName);
            if (string.IsNullOrWhiteSpace(configured)) return false;
            try
            {
                return new NpgsqlConnectionStringBuilder(configured).Database!
                    .StartsWith(DatabasePrefix, StringComparison.OrdinalIgnoreCase);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    public static string ConnectionString
    {
        get
        {
            var configured = Environment.GetEnvironmentVariable(VariableName);
            if (string.IsNullOrWhiteSpace(configured))
                throw new InvalidOperationException($"Set {VariableName} to a dedicated test database before running PostgreSQL integration tests.");

            var parsed = new NpgsqlConnectionStringBuilder(configured);
            if (!parsed.Database!.StartsWith(DatabasePrefix, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"The PostgreSQL integration database name must start with {DatabasePrefix}; no other database is safe for this suite.");

            return parsed.ConnectionString;
        }
    }
}

public sealed class RequiresDisposablePostgresFactAttribute : FactAttribute
{
    public RequiresDisposablePostgresFactAttribute()
    {
        if (!DisposablePostgres.IsConfiguredSafely)
            Skip = "Set PURCHASE_ASSISTANT_TEST_DATABASE to a dedicated wa_test_* PostgreSQL database to run this test.";
    }
}

public sealed class RequiresDisposablePostgresTheoryAttribute : TheoryAttribute
{
    public RequiresDisposablePostgresTheoryAttribute()
    {
        if (!DisposablePostgres.IsConfiguredSafely)
            Skip = "Set PURCHASE_ASSISTANT_TEST_DATABASE to a dedicated wa_test_* PostgreSQL database to run this test.";
    }
}
