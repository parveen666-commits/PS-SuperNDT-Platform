using System;

namespace PS.SuperNDT.UI.Database;

public static class DatabaseInitializer
{
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
            return;

        try
        {
            using var db = new SuperNDTDbContext();

            db.Initialize();

            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to initialize PS SuperNDT database.",
                ex);
        }
    }
}