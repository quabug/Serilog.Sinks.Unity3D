using Serilog;
using Serilog.Sinks.Unity3D;
using System;
using System.Diagnostics;
using System.Threading;
using Serilog.Context;
using Serilog.Core;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLogger : MonoBehaviour
{
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _warningButton;
    [SerializeField] private Button _errorButton;
    [SerializeField] private Button _threadButton;

    private Serilog.ILogger _logger;

    private void Awake() =>
        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Destructure.With<UnityObjectDestructuringPolicy>()
            .Enrich.FromLogContext()
            .Enrich.WithProperty(Constant.CATEGORY_NAME, "Project")
            .Enrich.WithProperty(Constant.UNITY_OBJECT_PROPERTY, "Unknown")
            .WriteTo.Unity3D(outputTemplate: "[{Level:u3}] {SourceContext}: <{__Object__}> {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

    private void Start()
    {
        _infoButton.onClick.AddListener(() =>
        {
            using (UnityContext.PushObject(this))
            {
                _logger.Information("This is an info");
            }
        });

        var testLogger = _logger.ForContext(Constant.CATEGORY_NAME, "Test");
        _warningButton.onClick.AddListener(() => testLogger.Warning("This is a warning"));
        _errorButton.onClick.AddListener(() =>
        {
            try
            {
                throw new InvalidOperationException("Invalid stuff");
            }
            catch (Exception e)
            {
                _logger.Error(e, "This is an error");
            }
        });
        _threadButton.onClick.AddListener(() =>
        {
            var stopWatch = Stopwatch.StartNew();

            ThreadPool.QueueUserWorkItem(state =>
            {
                stopWatch.Stop();
                using (UnityContext.PushObject(this))
                {
                    _logger.Information("Log from thread {Id}, Invoke took: {Elapsed}",
                        Thread.CurrentThread.ManagedThreadId, stopWatch.Elapsed);
                }
            });
        });
    }
}