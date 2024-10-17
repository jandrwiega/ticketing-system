using TicketingSystem.Repositories;
using TicketingSystem.Services;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Core.Database;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Core.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto>, TicketsDbRepository>();
builder.Services.AddScoped<ITagsRepository, TicketsTagsDbRepository>();
builder.Services.AddScoped<IMetadataRepository, TicketsMetadataDbRepository>();
builder.Services.AddScoped<ITicketsConfigurationService, TicketsConfigurationService>();
builder.Services.AddScoped<ITicketsConfigurationRepository, TicketsConfigurationRepository>();
builder.Services.AddScoped<ITicketsDependenciesRepository, TicketsDependenciesDbRepository>();
builder.Services.AddScoped<DependeciesValidatorFactory, DependeciesValidatorFactory>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DB")));

builder.Services.AddRazorPages();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

    DbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
        dbContext.Database.Migrate();
    };
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();