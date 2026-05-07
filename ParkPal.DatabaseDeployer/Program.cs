using DbUp;
using System.Reflection;

Console.WriteLine("🐘 ParkPal Database Deployer starting up, buddy...");

// Grab the connection string from Docker's environment variables
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DatabaseConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ ERROR: No connection string found!");
    Console.ResetColor();
    return -1;
}

// Ensure the database itself exists before running scripts
EnsureDatabase.For.PostgresqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .WithVariablesDisabled()
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"❌ ERROR: Upgrade failed: {result.Error}");
    Console.ResetColor();
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ SUCCESS: Database is fully upgraded and ready to rock!");
Console.ResetColor();

return 0; // Returning 0 tells Docker it finished successfully!