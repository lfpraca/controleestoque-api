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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KeevotecContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapControllers();
app.Run();
