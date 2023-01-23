﻿using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.AzureDataExplorer.Sinks;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.AzureDataExplorer.Extensions
{
    public static class AzureDataExplorerSinkExtensions
    {
        public static LoggerConfiguration AzureDataExplorerSink(
            this LoggerSinkConfiguration loggerConfiguration,
            AzureDataExplorerSinkOptions options,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = options.BatchPostingLimit,
                Period = options.Period,
                EagerlyEmitFirstEvent = true,
                QueueLimit = options.QueueSizeLimit
            };

            var azureDataExplorerSink = new AzureDataExplorerSink(options);
            var batchingSink = new PeriodicBatchingSink(azureDataExplorerSink, batchingOptions);

            var sink = string.IsNullOrWhiteSpace(options.BufferBaseFileName) ? (ILogEventSink)batchingSink : new AzureDataExplorerDurableSink(options);
            return loggerConfiguration.Sink(sink,
                restrictedToMinimumLevel,
                options.BufferFileLoggingLevelSwitch);
        }
    }
}
