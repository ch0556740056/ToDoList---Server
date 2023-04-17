
using Microsoft.OpenApi.Models;
using TodoApi;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDBContext>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
  options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
app.MapGet("/items", (ToDoDBContext context) =>
{
    return context.Items.ToList();
});

app.MapPost("/items", async(ToDoDBContext context, Item item)=>
{
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/items/{id}", async(ToDoDBContext context,[FromBody]Item item, int id)=>
{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();
    existItem.IsComplete =true;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async(ToDoDBContext context, int id)=>
{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();
    context.Items.Remove(existItem);
    await context.SaveChangesAsync();
    return Results.NoContent();
});
app.Run();
