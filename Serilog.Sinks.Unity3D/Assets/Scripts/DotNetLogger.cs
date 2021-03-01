using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Unity3D;
using UnityEngine;
using UnityEngine.UI;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class DotNetLogger : MonoBehaviour
{
    public Button Self;
    public Button Other;

    private ILogger _logger;

    private void Awake()
    {
        var seriLogger = new LoggerConfiguration()
            .MinimumLevel.Is(LevelConvert.ToSerilogLevel(LogLevel.Debug))
            .Destructure.With<UnityObjectDestructuringPolicy>()
            .Enrich.FromLogContext()
            .Enrich.WithProperty(Constant.CATEGORY_NAME, "Project")
            .Enrich.WithProperty(Constant.UNITY_OBJECT_PROPERTY, "Unknown")
            .WriteTo.Unity3D(outputTemplate: "[{Level:u3}] {SourceContext}: {Message:lj}")
            .CreateLogger()
        ;
        var loggerFactory = new SerilogLoggerFactory(seriLogger);
        _logger = loggerFactory.CreateLogger(this);
    }

    private void Start()
    {
        _logger.LogInformation("Start");
        Self.onClick.AddListener(() => _logger.LogInformation("Click"));
        Other.onClick.AddListener(() => _logger.LogInformation(Other, "Click on other"));
    }
}