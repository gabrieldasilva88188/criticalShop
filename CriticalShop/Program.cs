using Microsoft.EntityFrameworkCore;
using CriticalShop.Data; // ajuste para o seu namespace real

var builder = WebApplication.CreateBuilder(args);

// ==============================================
// Serviços
// ==============================================
builder.Services.AddControllersWithViews();

// EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// ==============================================
// Pipeline HTTP
// ==============================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Arquivos estáticos com logging + sem cache
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.File.PhysicalPath;
        var url = ctx.Context.Request.Path;
        Console.WriteLine($"Arquivo estático solicitado: {url} -> {path}");

        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
        ctx.Context.Response.Headers.Append("Expires", "0");
    }
});

app.UseRouting();

// Se ainda não tem autenticação configurada, você pode remover a linha abaixo.
// app.UseAuthentication();
app.UseAuthorization();

// (Opcional em DEV) aplica migrations automaticamente e executa seeder
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.SeedData(db);
}

// Rotas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
