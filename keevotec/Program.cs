using keevotec.Data;
using keevotec.Middleware;
using keevotec.Services.Movimento;
using keevotec.Services.Produto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IMovimentoService, MovimentoService>();
builder.Services.AddDbContext<KeevotecContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("keevotecConnection")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:5173");
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KeevotecContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors();

app.MapControllers();
app.Run();
