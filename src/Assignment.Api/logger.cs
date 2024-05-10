using Serilog;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using Amazon;
using Serilog.Formatting.Compact;
using Amazon.Runtime;

namespace Assignment.Api
{
    public static class Logger
    {
        public static Serilog.Core.Logger CreateLogger(IConfiguration configuration)
        {
            var awsCredentialsSection = configuration.GetSection("AwsCredentials");
            var logGroupName = awsCredentialsSection["LogGroupName"];
            var logStreamName = awsCredentialsSection["LogStreamName"];
            var accessKey = Environment.GetEnvironmentVariable("AccessKey");
            var secretKey = Environment.GetEnvironmentVariable("SecretKey");
            var region = Environment.GetEnvironmentVariable("Region");

            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            var awsRegion = RegionEndpoint.GetBySystemName(region);
            var cloudWatchClient = new AmazonCloudWatchLogsClient(awsCredentials, awsRegion);

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.AmazonCloudWatch(new CloudWatchSinkOptions
                {
                    LogGroupName = logGroupName,
                    LogStreamNameProvider = new ConstantLogStreamNameProvider(logStreamName),
                    BatchSizeLimit = 100,
                    QueueSizeLimit = 10000,
                    LogGroupRetentionPolicy = LogGroupRetentionPolicy.OneMonth,
                    TextFormatter = new CompactJsonFormatter()
                }, cloudWatchClient)
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, fileSizeLimitBytes: 10 * 1024 * 1024)
                .CreateLogger();

            return loggerConfig;
        }
    }
}
