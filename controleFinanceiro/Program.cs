using ControleFinanceiro.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Adiciona suporte a MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// ✅ Configura o DbContext com SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=localhost;Database=ControleFinanceiro;Trusted_Connection=True;TrustServerCertificate=True;"));

// ✅ Adiciona suporte a Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo da sessão
    options.Cookie.HttpOnly = true; // Protege contra XSS
    options.Cookie.IsEssential = true; // Necessário para funcionar sem consentimento de cookies
});

// ✅ Registra o HttpContextAccessor (para acessar sessão dentro dos controllers)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ✅ Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Ativa sessão ANTES de MapControllerRoute
app.UseSession();

// ✅ Autenticação/Autorização (se for usar no futuro)
app.UseAuthorization();

// ✅ Roteamento padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
