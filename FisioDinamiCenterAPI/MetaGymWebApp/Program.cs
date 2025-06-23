using LogicaDatos;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using Microsoft.EntityFrameworkCore;
using LogicaNegocio.Interfaces.Servicios;
using LogicaApp.Servicios;
using LogicaNegocio.Servicios;
using LogicaNegocio.Interfaces.Repositorios;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
//Bd
var EnlaceSQL = builder.Configuration.GetConnectionString("ConexionInicial");

builder.Services.AddDbContext<DbContextApp>(options =>
    options.UseSqlServer(EnlaceSQL,
        b => b.MigrationsAssembly("LogicaDatos")));

//Inyeccion de repos
// Repositorios
builder.Services.AddScoped<IRepositorioCliente, RepoClientes>();
builder.Services.AddScoped<IRepositorioProfesional, RepoProfesional>();
builder.Services.AddScoped<IRepositorioAdmin, RepoAdmin>();
builder.Services.AddScoped<IRepositorioCita, RepoCitas>();
builder.Services.AddScoped<IRepositorioExtra,RepoExtras>();
builder.Services.AddScoped<IRepositorioRutina, RepoRutinas>();
builder.Services.AddScoped<IRepositorioEjercicio, RepoEjercicios>();
builder.Services.AddScoped<IRepositorioMedia, RepoMedias>();

// Servicios
builder.Services.AddScoped<IClienteServicio, ServicioCliente>();
builder.Services.AddScoped<IAdminServicio, ServicioAdmin>();
builder.Services.AddScoped<IUsuarioServicio, ServicioUsuario>();
builder.Services.AddScoped<ICitaServicio, ServicioCita>();
builder.Services.AddScoped<IExtraServicio, ServicioExtras>();
builder.Services.AddScoped<IProfesionalServicio,ProfesionalServicio>();
builder.Services.AddScoped<IRutinaServicio, ServicioRutina>();
builder.Services.AddScoped<IMediaServicio, ServicioMedia>();

builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=AcercaDe}/{id?}");

app.Run();
