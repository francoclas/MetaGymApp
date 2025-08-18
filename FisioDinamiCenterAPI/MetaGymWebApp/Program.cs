using LogicaDatos;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using Microsoft.EntityFrameworkCore;
using LogicaNegocio.Interfaces.Servicios;
using LogicaApp.Servicios;
using LogicaNegocio.Servicios;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaDatos.Precarga;
using LogicaNegocio.Extra;
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
builder.Services.AddScoped<IRepositorioComentario, RepoComentario>();
builder.Services.AddScoped<IRepositorioPublicacion, RepoPublicacion>();
builder.Services.AddScoped<IRepositorioNotificacion, RepoNotificacion>();
builder.Services.AddScoped<IRepositorioAgenda, RepoAgenda>();

// Servicios
builder.Services.AddScoped<IClienteServicio, ServicioCliente>();
builder.Services.AddScoped<IAdminServicio, ServicioAdmin>();
builder.Services.AddScoped<IUsuarioServicio, ServicioUsuario>();
builder.Services.AddScoped<ICitaServicio, ServicioCita>();
builder.Services.AddScoped<IExtraServicio, ServicioExtras>();
builder.Services.AddScoped<IProfesionalServicio,ServicioProfesional>();
builder.Services.AddScoped<IRutinaServicio, ServicioRutina>();
builder.Services.AddScoped<IMediaServicio, ServicioMedia>();
builder.Services.AddScoped<IComentarioServicio,ServicioComentario>();
builder.Services.AddScoped<IPublicacionServicio,ServicioPublicacion>();
builder.Services.AddScoped<INotificacionServicio, ServicioNotificacion>();
builder.Services.AddScoped<IAgendaServicio, ServicioAgenda>();
builder.Services.AddScoped(typeof(Lazy<>), typeof(LazyResolver<>));


builder.Services.AddSession();
var app = builder.Build();
//Precarga
//Migracion
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbContextApp>();
    context.Database.Migrate();
}
CargaAdmin.CargarAdminBase(app.Services);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next();

    // Si venció la sesión y devolvió un redirect al login
    if (context.Response.StatusCode == 302 &&
        context.Request.Headers.ContainsKey("X-Requested-With") &&
        context.Request.Headers["X-Requested-With"] == "Fetch")
    {
        // Forzar 401 para que JS pueda detectarlo
        context.Response.StatusCode = 401;
    }
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=AcercaDe}/{id?}");

app.Run();
