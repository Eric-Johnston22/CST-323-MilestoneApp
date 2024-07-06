using Microsoft.ApplicationInsights.Channel;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using static Mysqlx.Notice.Warning.Types;

namespace CST_323_MilestoneApp.Utilities
{
    public class TemplateTraceTelemetryConverter : TraceTelemetryConverter
    {
        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var templateParser = new MessageTemplateParser();
            var template = templateParser.Parse("{SourceContext}.{MemberName}() - " + $"{logEvent.MessageTemplate.Text}");
            LogEvent newLogEvent = new LogEvent(logEvent.Timestamp
                , logEvent.Level
                , logEvent.Exception
                , template
                , logEvent.Properties.Select(p => new LogEventProperty(p.Key, p.Value)));
            return base.Convert(newLogEvent, formatProvider);
        }
    }
}
