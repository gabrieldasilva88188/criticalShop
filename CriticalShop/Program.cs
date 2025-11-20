using Microsoft.EntityFrameworkCore;
using CriticalShop.Data; // ajuste para o seu namespace real
using Microsoft.AspNetCore.Http; // Adicionando para configurações de cookie

var builder = WebApplication.CreateBuilder(args);

// ==============================================
// Serviços
// ==============================================
builder.Services.AddControllersWithViews();

// Configuração de sessão
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Em desenvolvimento permitir SameAsRequest para facilitar testes sem HTTPS
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax; // Permitir que o cookie seja enviado em requisições de outros sites
    options.Cookie.Name = ".CriticalShop.Session"; // Nome personalizado para o cookie
});

// EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Serviços customizados
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CriticalShop.Services.AuthService>();
builder.Services.AddScoped<CriticalShop.Services.FreteService>();
builder.Services.AddHttpClient(); // Para chamadas HTTP (ViaCEP)

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

// Middleware de sessão (deve vir depois de UseRouting e antes de UseEndpoints)
app.UseSession();

// Adiciona suporte a cookies de sessão
app.UseCookiePolicy();

// Middleware para verificar a sessão em cada requisição
app.Use(async (context, next) =>
{
    // Força a criação da sessão se ainda não existir
    await context.Session.LoadAsync();
    
    // Log para depuração
    var sessionId = context.Session.Id;
    var usuarioNome = context.Session.GetString("UsuarioNome") ?? "[não autenticado]";
    Console.WriteLine($"Sessão: {sessionId}, Usuário: {usuarioNome}, Path: {context.Request.Path}");
    
    await next(context);
});

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
