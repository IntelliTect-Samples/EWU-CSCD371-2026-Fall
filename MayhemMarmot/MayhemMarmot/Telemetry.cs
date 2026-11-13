using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

/// <summary>
/// App-specific telemetry sources plus the OpenTelemetry wiring. Everything is exported over OTLP, which is
/// what the Aspire dashboard (and any other OTLP backend) ingests. Export is enabled only when
/// OTEL_EXPORTER_OTLP_ENDPOINT is set (Aspire's AppHost and the standalone dashboard container both set it).
/// </summary>
internal static class Telemetry
{
    public const string ServiceName = "MayhemMarmot";

    public static readonly ActivitySource ActivitySource = new(ServiceName);
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> ChaosInjected = Meter.CreateCounter<long>(
        "marmot.chaos.injected", unit: "{event}", description: "Chaos modes applied to requests.");

    // Tagged by user (the seed source). User is caller-supplied, so cardinality is unbounded;
    // the OpenTelemetry SDK caps each metric at 2000 series and folds the rest into an overflow series.
    public static readonly Counter<long> PeopleRequests = Meter.CreateCounter<long>(
        "marmot.people.requests", unit: "{request}", description: "GET /people/{user} attempts, tagged by user.");

    public static readonly Counter<long> PeopleGenerated = Meter.CreateCounter<long>(
        "marmot.people.generated", unit: "{person}", description: "Fake people written to responses.");

    public static readonly Counter<long> ZipLookups = Meter.CreateCounter<long>(
        "marmot.zip.lookups", unit: "{lookup}", description: "Zip lookups, tagged by outcome (hit/miss).");

    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        var otel = builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(ServiceName))
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter(ServiceName))
            .WithTracing(t => t
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource(ServiceName));

        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        {
            otel.UseOtlpExporter();
        }

        return builder;
    }
}

/// <summary>Source-generated (AOT-safe) structured log messages; parameters become log attributes.</summary>
internal static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning,
        Message = "Chaos mode {ChaosMode} applied to {Endpoint}")]
    public static partial void ChaosApplied(this ILogger logger, ChaosModes chaosMode, string endpoint);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Generated {PersonCount} people for user {User}")]
    public static partial void PeopleGenerated(this ILogger logger, int personCount, string user);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
        Message = "Zip lookup {Zip} found={Found}")]
    public static partial void ZipLookup(this ILogger logger, string zip, bool found);
}
