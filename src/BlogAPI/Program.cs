using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnectionString")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
