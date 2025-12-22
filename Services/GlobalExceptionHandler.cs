using Serilog;

namespace MyVocaList.Services;

/// <summary>
/// Centralized exception handling for the entire application.
/// Hooks into unhandled exceptions and logs them via Serilog.
/// </summary>
public static class GlobalExceptionHandler
{
    private static bool _initialized;

    /// <summary>
    /// Initialize global exception handlers. Call this FIRST in MauiProgram.CreateMauiApp().
    /// </summary>
    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        // Hook AppDomain unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        // Hook Task unobserved exceptions
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

#if ANDROID
        // Hook Android-specific exceptions
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#endif

        Log.Information("GlobalExceptionHandler initialized");
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            Log.Fatal(exception, "Unhandled exception in AppDomain. IsTerminating: {IsTerminating}", e.IsTerminating);
        }
        else
        {
            Log.Fatal("Unhandled non-exception object: {ExceptionObject}. IsTerminating: {IsTerminating}",
                e.ExceptionObject, e.IsTerminating);
        }
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Unobserved task exception");

        // Mark as observed to prevent app termination
        e.SetObserved();
    }

#if ANDROID
    private static void OnAndroidUnhandledException(object? sender, Android.Runtime.RaiseThrowableEventArgs e)
    {
        Log.Fatal(e.Exception, "Android unhandled exception");

        // Mark as handled to prevent immediate crash (allows logging to complete)
        e.Handled = true;
    }
#endif
}
