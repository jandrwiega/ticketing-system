using Microsoft.EntityFrameworkCore;
using TicketingSystem.Core;
using TicketingSystem.Repositories;
using TicketingSystem.Services;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using AutoMapper;
using TicketingSystem.Core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto>, TicketsDbRepository>();
builder.Services.AddScoped<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<TicketsMiddleware>();

app.Run();
