using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using BPCalculator;

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddControllers();
    // 🔧 FIX: Add anti-forgery service
    builder.Services.AddAntiforgery();

    var app = builder.Build();

    // TELEMETRY MIDDLEWARE - Logs every request
    app.Use(async (context, next) =>
    {
        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        await next();
        
        stopwatch.Stop();
        
        Log.Information("API {Method} {Path} -> {StatusCode} in {ResponseTime}ms", 
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    });

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // HEALTH ENDPOINT
    app.MapGet("/health", () => 
    {
        Log.Information("Health check called - Status: Healthy");
        return new 
        { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            service = "BP Calculator + BMI Calculator",
            version = "1.0.0",
            environment = app.Environment.EnvironmentName
        };
    });

    // METRICS ENDPOINT
    app.MapGet("/metrics", () => 
    {
        Log.Information("Metrics checked - Memory: {MemoryMB}MB", 
            GC.GetTotalMemory(false) / 1024 / 1024);
            
        return new 
        {
            timestamp = DateTime.UtcNow,
            memory_used = GC.GetTotalMemory(false),
            uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
        };
    });

    // BP CALCULATOR API ENDPOINT
    app.MapPost("/api/bp/calculate", (BPRequest request) =>
    {
        Log.Information("BP Calculation - Systolic: {Systolic}, Diastolic: {Diastolic}", 
            request.Systolic, request.Diastolic);

        // Manual validation
        if (request.Systolic < 70 || request.Systolic > 190)
        {
            Log.Warning("BP Validation failed - Systolic out of range: {Systolic}", request.Systolic);
            return Results.BadRequest("Systolic must be between 70 and 190");
        }
            
        if (request.Diastolic < 40 || request.Diastolic > 100)
        {
            Log.Warning("BP Validation failed - Diastolic out of range: {Diastolic}", request.Diastolic);
            return Results.BadRequest("Diastolic must be between 40 and 100");
        }
            
        if (request.Systolic <= request.Diastolic)
        {
            Log.Warning("BP Validation failed - Systolic <= Diastolic: {Systolic}/{Diastolic}", 
                request.Systolic, request.Diastolic);
            return Results.BadRequest("Systolic must be greater than Diastolic");
        }

        var bp = new BloodPressure 
        { 
            Systolic = request.Systolic, 
            Diastolic = request.Diastolic 
        };
        
        Log.Information("BP Result - Category: {Category}", bp.Category);
        
        return Results.Ok(new 
        {
            Category = bp.Category.ToString(),
            Systolic = bp.Systolic,
            Diastolic = bp.Diastolic,
            Message = GetBPCategoryMessage(bp.Category)
        });
    });

    // BMI CALCULATOR API ENDPOINT
    app.MapPost("/api/bmi/calculate", (BMIRequest request) =>
    {
        Log.Information("BMI Calculation - Weight: {Weight}kg, Height: {Height}m", 
            request.Weight, request.Height);

        // Input validation
        if (request.Weight < 30 || request.Weight > 300)
        {
            Log.Warning("BMI Validation failed - Weight out of range: {Weight}", request.Weight);
            return Results.BadRequest("Weight must be between 30 and 300 kg");
        }
            
        if (request.Height < 1.0 || request.Height > 2.5)
        {
            Log.Warning("BMI Validation failed - Height out of range: {Height}", request.Height);
            return Results.BadRequest("Height must be between 1.0 and 2.5 meters");
        }

        var bmi = new BMI 
        { 
            Weight = request.Weight, 
            Height = request.Height 
        };
        
        Log.Information("BMI Result - Score: {Score}, Category: {Category}", 
            Math.Round(bmi.BMIScore, 2), bmi.Category);
        
        return Results.Ok(new 
        {
            Score = Math.Round(bmi.BMIScore, 2),
            Category = bmi.Category,
            Weight = bmi.Weight,
            Height = bmi.Height,
            Message = GetBMICategoryMessage(bmi.Category)
        });
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapRazorPages();

    Log.Information("Starting web host with BP + BMI calculators. Environment: {Env}", 
        app.Environment.EnvironmentName);
        
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Helper methods
static string GetBPCategoryMessage(BPCategory category)
{
    return category switch
    {
        BPCategory.Low => "Your blood pressure is lower than normal",
        BPCategory.Ideal => "Your blood pressure is ideal", 
        BPCategory.PreHigh => "Your blood pressure is pre-high",
        BPCategory.High => "Your blood pressure is high",
        _ => "Unable to determine blood pressure category"
    };
}

static string GetBMICategoryMessage(string category)
{
    return category switch
    {
        "Underweight" => "You are underweight",
        "Normal" => "Your weight is normal", 
        "Overweight" => "You are overweight",
        "Obese" => "You are obese",
        _ => "Unable to determine BMI category"
    };
}

// DTOs
public record BPRequest(int Systolic, int Diastolic);
public record BMIRequest(double Weight, double Height);
