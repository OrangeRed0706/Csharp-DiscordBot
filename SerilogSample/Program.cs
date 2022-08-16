using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Verbose("Hello");
Log.Debug("Hello");
Log.Information("Hello");
Log.Warning("Hello");
Log.Error("Hello");
Log.Fatal("Hello");