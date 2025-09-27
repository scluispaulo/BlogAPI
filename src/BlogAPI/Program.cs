using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnectionString")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogApi", Version = "v1" });
});

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreatePostDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCommentDtoValidator>();

var app = builder.Build();

// Global error handling
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        var error = new
        {
            message = "An unexpected error occurred.",
            detail = app.Environment.IsDevelopment()
                ? exceptionHandlerPathFeature?.Error.Message
                : null,
            path = exceptionHandlerPathFeature?.Path
        };

        await context.Response.WriteAsJsonAsync(error);
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapPostEndpoints();
app.MapCommentEndpoints();

app.Run();
