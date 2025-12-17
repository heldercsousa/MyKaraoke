# 🔌 MAUI Integration Guide

**Connecting MyVocaList (C#) to AI Services (Python)**  
**Clean Architecture | Type Safety | Async Patterns**

---

## 🎯 Integration Architecture

```
MyVocaList.csproj (Your existing MAUI app)
│
├── Services/
│   └── AI/                          ← New folder for AI services
│       ├── Abstractions/            ← Interfaces
│       │   ├── IAIService.cs
│       │   ├── IPitchAnalysisService.cs
│       │   └── IRecommendationService.cs
│       │
│       ├── Implementation/          ← Concrete classes
│       │   ├── PitchAnalysisService.cs
│       │   ├── RecommendationService.cs
│       │   └── LyricsSearchService.cs
│       │
│       ├── Models/                  ← DTOs
│       │   ├── PitchAnalysisRequest.cs
│       │   ├── PitchAnalysisResult.cs
│       │   ├── RecommendationRequest.cs
│       │   └── RecommendationResponse.cs
│       │
│       └── Configuration/           ← Settings
│           └── AIServiceConfig.cs
│
└── ViewModels/
    └── AI/                          ← AI-powered ViewModels
        ├── SingingViewModel.cs
        └── SongSelectionViewModel.cs
```

---

## 📦 Part 1: Setup (15 minutes)

### Add NuGet Packages

```xml
<!-- MyVocaList.csproj -->
<ItemGroup>
  <!-- HTTP Client extensions -->
  <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
  
  <!-- JSON serialization -->
  <PackageReference Include="System.Text.Json" Version="8.0.0" />
  
  <!-- Polly for resilience (retry logic) -->
  <PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.0" />
  
  <!-- Refit (optional - type-safe HTTP client) -->
  <PackageReference Include="Refit" Version="7.0.0" />
  <PackageReference Include="Refit.HttpClientFactory" Version="7.0.0" />
</ItemGroup>
```

---

## 🔧 Part 2: Configuration (20 minutes)

### API Configuration Class

**File: `Services/AI/Configuration/AIServiceConfig.cs`**

```csharp
namespace MyVocaList.Services.AI.Configuration;

/// <summary>
/// Configuration for AI service endpoints
/// Supports different environments (dev, staging, production)
/// </summary>
public class AIServiceConfig
{
    /// <summary>
    /// Base URLs for different deployment stages
    /// </summary>
    public static class ApiUrls
    {
#if DEBUG
        // Development: Local or ngrok
        public const string PitchAnalysis = "http://localhost:8000";
        public const string Recommendations = "http://localhost:8001";
        public const string LyricsSearch = "http://localhost:8002";
#else
        // Production: AWS Lambda
        public const string PitchAnalysis = "https://abc123.execute-api.us-east-1.amazonaws.com/Prod";
        public const string Recommendations = "https://def456.execute-api.us-east-1.amazonaws.com/Prod";
        public const string LyricsSearch = "https://ghi789.execute-api.us-east-1.amazonaws.com/Prod";
#endif
    }

    /// <summary>
    /// Timeout configuration
    /// </summary>
    public static class Timeouts
    {
        public static readonly TimeSpan Default = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan PitchAnalysis = TimeSpan.FromSeconds(60); // Audio processing takes longer
        public static readonly TimeSpan Recommendations = TimeSpan.FromSeconds(10);
    }

    /// <summary>
    /// Retry policy configuration
    /// </summary>
    public static class RetryPolicy
    {
        public const int MaxRetries = 3;
        public static readonly TimeSpan InitialDelay = TimeSpan.FromMilliseconds(500);
    }
}
```

---

## 📝 Part 3: Data Models (30 minutes)

### Request/Response DTOs

**File: `Services/AI/Models/PitchAnalysisModels.cs`**

```csharp
using System.Text.Json.Serialization;

namespace MyVocaList.Services.AI.Models;

/// <summary>
/// Request to analyze pitch from audio recording
/// </summary>
public record PitchAnalysisRequest
{
    /// <summary>
    /// Base64-encoded audio data (WAV format preferred)
    /// </summary>
    [JsonPropertyName("audio")]
    public required string AudioBase64 { get; init; }

    /// <summary>
    /// Optional: Reference pitch to compare against
    /// </summary>
    [JsonPropertyName("reference_pitch")]
    public double? ReferencePitch { get; init; }

    /// <summary>
    /// Optional: Expected note (for karaoke scoring)
    /// </summary>
    [JsonPropertyName("expected_note")]
    public string? ExpectedNote { get; init; }
}

/// <summary>
/// Result from pitch analysis
/// </summary>
public record PitchAnalysisResult
{
    [JsonPropertyName("average_pitch")]
    public required double AveragePitch { get; init; }

    [JsonPropertyName("pitch_range")]
    public required double[] PitchRange { get; init; }

    [JsonPropertyName("confidence")]
    public required double Confidence { get; init; }

    [JsonPropertyName("pitch_stability")]
    public double PitchStability { get; init; }

    [JsonPropertyName("notes_detected")]
    public string[]? NotesDetected { get; init; }

    /// <summary>
    /// Calculate score (0-100) based on pitch accuracy
    /// </summary>
    public int CalculateScore(double? referencePitch = null)
    {
        if (referencePitch == null)
            return (int)(Confidence * 100);

        // Calculate deviation from reference
        var deviation = Math.Abs(AveragePitch - referencePitch.Value);
        var maxDeviation = 100.0; // Hz
        var accuracy = Math.Max(0, 1 - (deviation / maxDeviation));

        // Combine confidence and accuracy
        var score = (Confidence * 0.4 + accuracy * 0.6) * 100;
        return (int)Math.Round(score);
    }
}

/// <summary>
/// Wrapper for API response
/// </summary>
public record AIApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>
    /// Throw exception if request failed
    /// </summary>
    public T GetDataOrThrow()
    {
        if (!Success || Data == null)
            throw new InvalidOperationException($"AI API request failed: {Error ?? "Unknown error"}");
        return Data;
    }
}
```

**File: `Services/AI/Models/RecommendationModels.cs`**

```csharp
using System.Text.Json.Serialization;

namespace MyVocaList.Services.AI.Models;

/// <summary>
/// Request for song recommendations
/// </summary>
public record RecommendationRequest
{
    /// <summary>
    /// Song title to find similar songs for
    /// </summary>
    [JsonPropertyName("song_title")]
    public string? SongTitle { get; init; }

    /// <summary>
    /// Or: List of recently played songs
    /// </summary>
    [JsonPropertyName("recent_songs")]
    public List<string>? RecentSongs { get; init; }

    /// <summary>
    /// Number of recommendations to return
    /// </summary>
    [JsonPropertyName("n")]
    public int Count { get; init; } = 5;

    /// <summary>
    /// Optional: Preferred genres
    /// </summary>
    [JsonPropertyName("genres")]
    public List<string>? Genres { get; init; }
}

/// <summary>
/// Recommended song
/// </summary>
public record RecommendedSong
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("artist")]
    public required string Artist { get; init; }

    [JsonPropertyName("genre")]
    public required string Genre { get; init; }

    [JsonPropertyName("bpm")]
    public required int Bpm { get; init; }

    [JsonPropertyName("similarity")]
    public double Similarity { get; init; }

    /// <summary>
    /// Convert to your domain model (Musica entity)
    /// </summary>
    public Musica ToMusica()
    {
        return new Musica
        {
            Titulo = Title,
            Artista = Artist,
            Genero = Genre,
            // Map other properties as needed
        };
    }
}

public record RecommendationResponse
{
    [JsonPropertyName("recommendations")]
    public required List<RecommendedSong> Recommendations { get; init; }

    [JsonPropertyName("query_song")]
    public string? QuerySong { get; init; }
}
```

---

## 🏗️ Part 4: Service Implementation (60 minutes)

### Interface Definitions

**File: `Services/AI/Abstractions/IPitchAnalysisService.cs`**

```csharp
namespace MyVocaList.Services.AI.Abstractions;

/// <summary>
/// Service for analyzing singing pitch and calculating scores
/// </summary>
public interface IPitchAnalysisService
{
    /// <summary>
    /// Analyze pitch from audio file
    /// </summary>
    /// <param name="audioFilePath">Path to audio file (WAV preferred)</param>
    /// <param name="referencePitch">Optional reference pitch for comparison</param>
    /// <returns>Pitch analysis result with score</returns>
    Task<PitchAnalysisResult> AnalyzePitchAsync(
        string audioFilePath,
        double? referencePitch = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyze pitch from base64 audio data
    /// </summary>
    Task<PitchAnalysisResult> AnalyzePitchFromBase64Async(
        string audioBase64,
        double? referencePitch = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if service is available (health check)
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
```

**File: `Services/AI/Abstractions/IRecommendationService.cs`**

```csharp
namespace MyVocaList.Services.AI.Abstractions;

/// <summary>
/// Service for AI-powered song recommendations
/// </summary>
public interface IRecommendationService
{
    /// <summary>
    /// Get song recommendations based on a single song
    /// </summary>
    Task<List<RecommendedSong>> GetRecommendationsAsync(
        string songTitle,
        int count = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recommendations based on queue history
    /// </summary>
    Task<List<RecommendedSong>> GetRecommendationsForQueueAsync(
        List<string> recentSongs,
        int count = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get diverse recommendations (avoid repetitive genres)
    /// </summary>
    Task<List<RecommendedSong>> GetDiverseRecommendationsAsync(
        List<string> recentSongs,
        int count = 5,
        CancellationToken cancellationToken = default);
}
```

---

### Service Implementation (Pitch Analysis)

**File: `Services/AI/Implementation/PitchAnalysisService.cs`**

```csharp
using MyVocaList.Services.AI.Abstractions;
using MyVocaList.Services.AI.Models;
using MyVocaList.Services.AI.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyVocaList.Services.AI.Implementation;

/// <summary>
/// ✅ PRODUCTION-READY: Pitch analysis with error handling, retry logic, and logging
/// </summary>
public class PitchAnalysisService : IPitchAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PitchAnalysisService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PitchAnalysisService(
        HttpClient httpClient,
        ILogger<PitchAnalysisService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(AIServiceConfig.ApiUrls.PitchAnalysis);
        _httpClient.Timeout = AIServiceConfig.Timeouts.PitchAnalysis;

        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<PitchAnalysisResult> AnalyzePitchAsync(
        string audioFilePath,
        double? referencePitch = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🎤 Analyzing pitch from file: {FilePath}", audioFilePath);

        try
        {
            // Read audio file and convert to base64
            var audioBytes = await File.ReadAllBytesAsync(audioFilePath, cancellationToken);
            var audioBase64 = Convert.ToBase64String(audioBytes);

            return await AnalyzePitchFromBase64Async(audioBase64, referencePitch, cancellationToken);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "❌ Audio file not found: {FilePath}", audioFilePath);
            throw new InvalidOperationException($"Audio file not found: {audioFilePath}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error reading audio file: {FilePath}", audioFilePath);
            throw;
        }
    }

    public async Task<PitchAnalysisResult> AnalyzePitchFromBase64Async(
        string audioBase64,
        double? referencePitch = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🎵 Analyzing pitch from base64 data (size: {Size} bytes)",
            audioBase64.Length);

        try
        {
            // Create request
            var request = new PitchAnalysisRequest
            {
                AudioBase64 = audioBase64,
                ReferencePitch = referencePitch
            };

            // Call API
            var response = await _httpClient.PostAsJsonAsync(
                "/analyze",
                request,
                _jsonOptions,
                cancellationToken);

            // Check response
            response.EnsureSuccessStatusCode();

            // Parse response
            var apiResponse = await response.Content
                .ReadFromJsonAsync<AIApiResponse<PitchAnalysisResult>>(
                    _jsonOptions,
                    cancellationToken);

            if (apiResponse == null)
                throw new InvalidOperationException("API returned null response");

            var result = apiResponse.GetDataOrThrow();

            _logger.LogInformation(
                "✅ Pitch analysis complete - Avg: {AvgPitch}Hz, Confidence: {Confidence}%",
                result.AveragePitch,
                result.Confidence * 100);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ HTTP error calling pitch analysis API");
            throw new InvalidOperationException(
                "Failed to connect to pitch analysis service. Check network connection.", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "❌ Pitch analysis request timed out");
            throw new TimeoutException("Pitch analysis took too long. Try with a shorter audio clip.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "❌ Invalid JSON response from API");
            throw new InvalidOperationException("Invalid response from pitch analysis service", ex);
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
```

---

### Service Implementation (Recommendations)

**File: `Services/AI/Implementation/RecommendationService.cs`**

```csharp
using MyVocaList.Services.AI.Abstractions;
using MyVocaList.Services.AI.Models;
using MyVocaList.Services.AI.Configuration;
using System.Net.Http.Json;

namespace MyVocaList.Services.AI.Implementation;

/// <summary>
/// ✅ PRODUCTION-READY: Song recommendations with caching and fallback
/// </summary>
public class RecommendationService : IRecommendationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecommendationService> _logger;
    
    // Simple in-memory cache (upgrade to IMemoryCache in production)
    private readonly Dictionary<string, (List<RecommendedSong> Songs, DateTime Expiry)> _cache = new();
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(30);

    public RecommendationService(
        HttpClient httpClient,
        ILogger<RecommendationService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(AIServiceConfig.ApiUrls.Recommendations);
        _httpClient.Timeout = AIServiceConfig.Timeouts.Recommendations;

        _logger = logger;
    }

    public async Task<List<RecommendedSong>> GetRecommendationsAsync(
        string songTitle,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🎵 Getting recommendations for: {Song}", songTitle);

        // Check cache first
        var cacheKey = $"single_{songTitle}_{count}";
        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            if (cached.Expiry > DateTime.UtcNow)
            {
                _logger.LogInformation("✅ Returning cached recommendations");
                return cached.Songs;
            }
            _cache.Remove(cacheKey); // Expired
        }

        try
        {
            var request = new RecommendationRequest
            {
                SongTitle = songTitle,
                Count = count
            };

            var response = await _httpClient.PostAsJsonAsync("/recommend", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content
                .ReadFromJsonAsync<AIApiResponse<RecommendationResponse>>(cancellationToken);

            var recommendations = apiResponse?.GetDataOrThrow()?.Recommendations
                ?? throw new InvalidOperationException("No recommendations returned");

            // Cache results
            _cache[cacheKey] = (recommendations, DateTime.UtcNow + _cacheExpiry);

            _logger.LogInformation("✅ Retrieved {Count} recommendations", recommendations.Count);
            return recommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting recommendations");
            
            // Fallback: Return empty list instead of crashing
            return new List<RecommendedSong>();
        }
    }

    public async Task<List<RecommendedSong>> GetRecommendationsForQueueAsync(
        List<string> recentSongs,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🎵 Getting queue-based recommendations for {Count} recent songs", recentSongs.Count);

        try
        {
            var request = new RecommendationRequest
            {
                RecentSongs = recentSongs,
                Count = count
            };

            var response = await _httpClient.PostAsJsonAsync("/recommend/queue", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content
                .ReadFromJsonAsync<AIApiResponse<RecommendationResponse>>(cancellationToken);

            return apiResponse?.GetDataOrThrow()?.Recommendations
                ?? new List<RecommendedSong>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting queue recommendations");
            return new List<RecommendedSong>();
        }
    }

    public async Task<List<RecommendedSong>> GetDiverseRecommendationsAsync(
        List<string> recentSongs,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        // Implementation: Call API with diversity parameter
        // Or: Post-process results to ensure genre diversity
        var recommendations = await GetRecommendationsForQueueAsync(recentSongs, count * 2, cancellationToken);

        // Filter for diversity (max 2 songs per genre)
        var diverse = recommendations
            .GroupBy(s => s.Genre)
            .SelectMany(g => g.Take(2))
            .Take(count)
            .ToList();

        return diverse;
    }
}
```

---

## 🔌 Part 5: Dependency Injection Registration (15 minutes)

**File: `MauiProgram.cs`** (update existing)

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Your existing fonts...
            });

        // ✅ REGISTER AI SERVICES
        RegisterAIServices(builder.Services);

        // Your existing registrations...
        
        return builder.Build();
    }

    private static void RegisterAIServices(IServiceCollection services)
    {
        // Configure HttpClient with Polly retry policy
        services.AddHttpClient<IPitchAnalysisService, PitchAnalysisService>()
            .AddTransientHttpErrorPolicy(policy => 
                policy.WaitAndRetryAsync(
                    retryCount: AIServiceConfig.RetryPolicy.MaxRetries,
                    sleepDurationProvider: retryAttempt => 
                        AIServiceConfig.RetryPolicy.InitialDelay * retryAttempt));

        services.AddHttpClient<IRecommendationService, RecommendationService>()
            .AddTransientHttpErrorPolicy(policy => 
                policy.WaitAndRetryAsync(3, retryAttempt => 
                    TimeSpan.FromMilliseconds(500 * retryAttempt)));

        // Register as Scoped (one instance per page navigation)
        services.AddScoped<IPitchAnalysisService, PitchAnalysisService>();
        services.AddScoped<IRecommendationService, RecommendationService>();

        System.Diagnostics.Debug.WriteLine("✅ AI Services registered");
    }
}
```

---

## 🎨 Part 6: ViewModel Integration (45 minutes)

**File: `ViewModels/AI/SingingViewModel.cs`**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyVocaList.Services.AI.Abstractions;
using MyVocaList.Services.AI.Models;

namespace MyVocaList.ViewModels.AI;

/// <summary>
/// ViewModel for karaoke singing with AI pitch analysis
/// </summary>
public partial class SingingViewModel : ObservableObject
{
    private readonly IPitchAnalysisService _pitchService;
    private readonly ILogger<SingingViewModel> _logger;

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private bool _isAnalyzing;

    [ObservableProperty]
    private int _pitchScore;

    [ObservableProperty]
    private string _feedbackMessage = "";

    [ObservableProperty]
    private string _recordingPath = "";

    public SingingViewModel(
        IPitchAnalysisService pitchService,
        ILogger<SingingViewModel> logger)
    {
        _pitchService = pitchService;
        _logger = logger;
    }

    [RelayCommand]
    async Task StartRecording()
    {
        try
        {
            IsRecording = true;
            FeedbackMessage = "Recording... 🎤";

            // Your existing recording logic
            // RecordingPath will be set by your audio service
            
            _logger.LogInformation("🎤 Started recording");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting recording");
            FeedbackMessage = "Failed to start recording";
        }
    }

    [RelayCommand]
    async Task StopRecording()
    {
        try
        {
            IsRecording = false;
            FeedbackMessage = "Processing... ⏳";

            // Your existing logic to stop and save recording
            // Assume RecordingPath is now set

            _logger.LogInformation("🎤 Stopped recording, path: {Path}", RecordingPath);

            // Automatically analyze
            await AnalyzeRecording();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping recording");
            FeedbackMessage = "Failed to process recording";
        }
    }

    [RelayCommand]
    async Task AnalyzeRecording()
    {
        if (string.IsNullOrEmpty(RecordingPath))
        {
            FeedbackMessage = "No recording to analyze";
            return;
        }

        try
        {
            IsAnalyzing = true;
            FeedbackMessage = "Analyzing your performance... 🎵";

            // Call AI service
            var result = await _pitchService.AnalyzePitchAsync(RecordingPath);

            // Calculate score
            PitchScore = result.CalculateScore();

            // Generate feedback
            FeedbackMessage = GenerateFeedback(result);

            _logger.LogInformation("✅ Analysis complete - Score: {Score}", PitchScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing recording");
            FeedbackMessage = "Analysis failed. Please try again.";
            PitchScore = 0;
        }
        finally
        {
            IsAnalyzing = false;
        }
    }

    private string GenerateFeedback(PitchAnalysisResult result)
    {
        var score = result.CalculateScore();

        return score switch
        {
            >= 90 => "🌟 Excellent! You nailed it!",
            >= 75 => "👍 Great job! Keep it up!",
            >= 60 => "🙂 Good effort! Practice makes perfect!",
            >= 40 => "💪 Keep practicing! You're improving!",
            _ => "🎵 Don't give up! Everyone starts somewhere!"
        };
    }
}
```

**File: `ViewModels/AI/SongSelectionViewModel.cs`**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyVocaList.Services.AI.Abstractions;
using MyVocaList.Services.AI.Models;
using System.Collections.ObjectModel;

namespace MyVocaList.ViewModels.AI;

/// <summary>
/// ViewModel for song selection with AI recommendations
/// </summary>
public partial class SongSelectionViewModel : ObservableObject
{
    private readonly IRecommendationService _recommendationService;
    private readonly ILogger<SongSelectionViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<RecommendedSong> _recommendations = new();

    [ObservableProperty]
    private bool _isLoadingRecommendations;

    [ObservableProperty]
    private string _selectedSongTitle = "";

    public SongSelectionViewModel(
        IRecommendationService recommendationService,
        ILogger<SongSelectionViewModel> logger)
    {
        _recommendationService = recommendationService;
        _logger = logger;
    }

    [RelayCommand]
    async Task LoadRecommendations(string songTitle)
    {
        if (string.IsNullOrEmpty(songTitle))
            return;

        try
        {
            IsLoadingRecommendations = true;
            SelectedSongTitle = songTitle;

            _logger.LogInformation("🎵 Loading recommendations for: {Song}", songTitle);

            var recommendations = await _recommendationService
                .GetRecommendationsAsync(songTitle, count: 10);

            Recommendations.Clear();
            foreach (var song in recommendations)
            {
                Recommendations.Add(song);
            }

            _logger.LogInformation("✅ Loaded {Count} recommendations", recommendations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading recommendations");
            // Show error to user via your notification system
        }
        finally
        {
            IsLoadingRecommendations = false;
        }
    }

    [RelayCommand]
    async Task LoadSmartRecommendations(List<string> recentSongs)
    {
        try
        {
            IsLoadingRecommendations = true;

            _logger.LogInformation("🎵 Loading smart recommendations based on queue");

            var recommendations = await _recommendationService
                .GetDiverseRecommendationsAsync(recentSongs, count: 10);

            Recommendations.Clear();
            foreach (var song in recommendations)
            {
                Recommendations.Add(song);
            }

            _logger.LogInformation("✅ Loaded {Count} diverse recommendations", recommendations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading smart recommendations");
        }
        finally
        {
            IsLoadingRecommendations = false;
        }
    }
}
```

---

## 🧪 Part 7: Testing (30 minutes)

**File: `Tests/Services/AI/PitchAnalysisServiceTests.cs`**

```csharp
using Xunit;
using Moq;
using MyVocaList.Services.AI.Implementation;
using Microsoft.Extensions.Logging;

namespace MyVocaList.Tests.Services.AI;

public class PitchAnalysisServiceTests
{
    private readonly Mock<ILogger<PitchAnalysisService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly HttpClient _httpClient;

    public PitchAnalysisServiceTests()
    {
        _loggerMock = new Mock<ILogger<PitchAnalysisService>>();
        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:8000")
        };
    }

    [Fact]
    public async Task AnalyzePitchAsync_ValidFile_ReturnsResult()
    {
        // Arrange
        var service = new PitchAnalysisService(_httpClient, _loggerMock.Object);
        var audioPath = "test_audio.wav";
        
        // Mock HTTP response
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(@"{
                ""success"": true,
                ""data"": {
                    ""average_pitch"": 220.0,
                    ""pitch_range"": [180, 280],
                    ""confidence"": 0.85,
                    ""pitch_stability"": 0.92
                }
            }")
        };

        // Setup mock to return response
        _httpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        // Act
        var result = await service.AnalyzePitchAsync(audioPath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(220.0, result.AveragePitch);
        Assert.Equal(0.85, result.Confidence);
    }

    [Fact]
    public async Task AnalyzePitchAsync_InvalidFile_ThrowsException()
    {
        // Arrange
        var service = new PitchAnalysisService(_httpClient, _loggerMock.Object);
        var invalidPath = "nonexistent.wav";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AnalyzePitchAsync(invalidPath));
    }
}
```

---

## ✅ Integration Checklist

Use this for each AI feature you add:

- [ ] Define request/response models
- [ ] Create service interface
- [ ] Implement service with error handling
- [ ] Register in DI container
- [ ] Create ViewModel
- [ ] Wire up to existing pages
- [ ] Add unit tests
- [ ] Test with mock data locally
- [ ] Test with real API (ngrok/Render)
- [ ] Test error scenarios (no network, timeout)
- [ ] Add loading indicators in UI
- [ ] Document API endpoints

---

## 🚀 Quick Start: Add First AI Feature (Today!)

1. **Add packages** (5 min)
2. **Copy `AIServiceConfig.cs`** (2 min)
3. **Copy pitch analysis models** (5 min)
4. **Copy `PitchAnalysisService.cs`** (10 min)
5. **Register in `MauiProgram.cs`** (5 min)
6. **Test with ngrok** (10 min)

**Total: 37 minutes to first working AI integration! 🎉**

---

Next guide: `WEEKLY_IMPLEMENTATION_PLAN.md` for detailed week-by-week tasks.
