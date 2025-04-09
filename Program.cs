using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración para Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Registro");
});

// Configuración de controladores para API
builder.Services.AddControllers().AddNewtonsoftJson();

// Configuración de autenticación (agrega esto)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
}).AddCookie("Cookies", options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccesoDenegado";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

// Configuración de autorización (agrega esto)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Administrador"));
});

// Configuración de sesiones optimizada
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "VideojuegosStore.Session";
});

var app = builder.Build();

// Configuración del pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agrega estos middlewares en ESTE ORDEN
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();
app.MapRazorPages();

app.Run();