using System.Collections;
using System.Globalization;
using Avalanche.Message;
using Avalanche.StatusCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

class logging
{
    public static void Run()
    {
        {
            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection();
            // Add logging
            InitializeLogging(serviceCollection);
            // Create service
            IServiceProvider service = serviceCollection.BuildServiceProvider();
            // Create ILogger
            var logger = service.GetRequiredService<Microsoft.Extensions.Logging.ILogger<example>>();
            // Create message
            IMessage message = SystemMessages.ArgumentNull.Generic.New("argumentName");
            // Log message
            logger.LogMessage(message);
        }
        {
            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection();
            // Add logging
            InitializeLogging(serviceCollection);
            // Create service
            IServiceProvider service = serviceCollection.BuildServiceProvider();
            // Create ILogger
            var logger = service.GetRequiredService<Microsoft.Extensions.Logging.ILogger<example>>();
            // Create message
            IMessage message = SystemMessages.ArgumentNull.Generic.New("argumentName");
            // Log message
            message.LogTo(logger);
        }
        {
            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection();
            // Add logging
            InitializeLogging(serviceCollection);
            // Create service
            IServiceProvider service = serviceCollection.BuildServiceProvider();
            // Create ILogger
            var logger = service.GetRequiredService<Microsoft.Extensions.Logging.ILogger<example>>();
            SystemMessages.ArgumentNull.Generic.New("argumentName").SetSeverity(LogLevel.Critical).LogTo(logger);
        }
        {
            // Service collection
            IServiceCollection serviceCollection = new ServiceCollection();
            // Add logging
            InitializeLogging(serviceCollection);
            // Create service
            IServiceProvider service = serviceCollection.BuildServiceProvider();
            // Create ILogger
            var logger = service.GetRequiredService<Microsoft.Extensions.Logging.ILogger<example>>();
            IMessage msg = SystemMessages.ArgumentNull.Generic.New("argumentName");
            logger.LogMessage(msg, LogLevel.Critical);
            msg.LogTo(logger, LogLevel.Critical);
        }
    }

    static IServiceCollection InitializeLogging(IServiceCollection serviceCollection)
    {
        // Initial configuration
        MemoryConfiguration memConfig = new MemoryConfiguration()
            .Set("Logging:LogLevel:Default", "Debug")
            .Set("Serilog:WriteTo:0:Name", "Console")
            .Set("Serilog:WriteTo:0:Args:OutputTemplate", "[{EventIdHex} {Level:u1}] {Message:lj}{NewLine}{Exception}")
            .Set("Serilog:WriteTo:0:Args:RestrictedToMinimumLevel", "Verbose")
            .Set("Serilog:WriteTo:0:Args:Theme", "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console");

        // Read configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .Add(memConfig)
            .Build();

        //
        var switchableLogger = new Serilog.SwitchableLogger();
        // Logging
        serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder
#if DEBUG
                    .SetMinimumLevel(LogLevel.Trace)
#else
                    .SetMinimumLevel(LogLevel.Information)
#endif
                    .AddSerilog(switchableLogger, true)
                    .AddSerilogConfigurationLoader(configuration, switchableLogger,
                        c => new Serilog.LoggerConfiguration()
#if DEBUG
                                .MinimumLevel.Verbose()
#else
                                .MinimumLevel.Information()
#endif
                                .Enrich.With(new EventIdEnricher())
                            .ReadFrom.Configuration(configuration)
                            .CreateLogger())
                );
        //
        return serviceCollection;
    }

    /// <summary>Memory configuration</summary>
    public class MemoryConfiguration : ConfigurationProvider, IEnumerable<KeyValuePair<string, string>>, IConfigurationSource
    {
        /// <summary>Expose inner configuration data</summary>
        public new IDictionary<String, String> Data => base.Data;
        /// <summary>Configuration data</summary>
        public string this[string key] { get => base.Data[key]; set => base.Data[key] = value; }

        /// <summary>Create memory configuration</summary>
        public MemoryConfiguration() : base() { }

        /// <summary>Assign <paramref name="value"/> to <paramref name="key"/></summary>
        public new MemoryConfiguration Set(string key, string value) { base.Data[key] = value; return this; }
        /// <summary>Build configuration provider.</summary>
        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
        /// <summary>Enumerate</summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Data.GetEnumerator();
        /// <summary>Enumerate</summary>
        IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
    }

    /// <summary>Serilog enricher that reduces "EventId" to its "id" field.</summary>
    public class EventIdEnricher : ILogEventEnricher
    {
        /// <summary>Reduce to EventId.id</summary>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // Get properties
            if (!logEvent.Properties.TryGetValue("EventId", out LogEventPropertyValue? eventIdStructure)) return;
            // Not structure field
            if (eventIdStructure is not StructureValue structureValue || structureValue.Properties == null) return;
            // Get list
            IReadOnlyList<LogEventProperty> list = structureValue.Properties;
            // Process each
            for (int i = 0; i < list.Count; i++)
            {
                // Get property
                LogEventProperty logEventProperty = list[i];
                // Not id
                if (logEventProperty.Name != "Id") continue;
                // New property
                LogEventProperty eventId = propertyFactory.CreateProperty("EventId", logEventProperty.Value);
                // Add as new key
                logEvent.AddOrUpdateProperty(eventId);
                // Print as hex
                string hex = logEventProperty.Value.ToString("X8", CultureInfo.InvariantCulture);
                // New property
                LogEventProperty eventIdHex = propertyFactory.CreateProperty("EventIdHex", hex);
                // Add as new key
                logEvent.AddOrUpdateProperty(eventIdHex);
                // Completed
                return;
            }
        }
    }
}
