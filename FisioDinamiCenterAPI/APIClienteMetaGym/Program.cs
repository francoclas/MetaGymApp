using LogicaDatos;
using LogicaDatos.Interfaces.Repos;
using LogicaDatos.Repositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using LogicaNegocio.Interfaces.Servicios;
using LogicaApp.Servicios;
using LogicaNegocio.Servicios;
using LogicaNegocio.Interfaces.Repositorios;
using LogicaDatos.Precarga;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios base ----------
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

//------------- Token -------------
// Configuración de JWT
var clave = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(clave);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
// ---------- Base de datos ----------
var EnlaceSQL = builder.Configuration.GetConnectionString("ConexionInicial");

builder.Services.AddDbContext<DbContextApp>(options =>
    options.UseSqlServer(EnlaceSQL,
        b => b.MigrationsAssembly("LogicaDatos")));

// ---------- Repositorios ----------
builder.Services.AddScoped<IRepositorioCliente, RepoClientes>();
builder.Services.AddScoped<IRepositorioProfesional, RepoProfesional>();
builder.Services.AddScoped<IRepositorioAdmin, RepoAdmin>();
builder.Services.AddScoped<IRepositorioCita, RepoCitas>();
builder.Services.AddScoped<IRepositorioExtra, RepoExtras>();
builder.Services.AddScoped<IRepositorioRutina, RepoRutinas>();
builder.Services.AddScoped<IRepositorioEjercicio, RepoEjercicios>();
builder.Services.AddScoped<IRepositorioMedia, RepoMedias>();
builder.Services.AddScoped<IRepositorioComentario, RepoComentario>();
builder.Services.AddScoped<IRepositorioPublicacion, RepoPublicacion>();
builder.Services.AddScoped<IRepositorioNotificacion, RepoNotificacion>();


// ---------- Servicios ----------
builder.Services.AddScoped<IClienteServicio, ServicioCliente>();
builder.Services.AddScoped<IAdminServicio, ServicioAdmin>();
builder.Services.AddScoped<IUsuarioServicio, ServicioUsuario>();
builder.Services.AddScoped<ICitaServicio, ServicioCita>();
builder.Services.AddScoped<IExtraServicio, ServicioExtras>();
builder.Services.AddScoped<IProfesionalServicio, ServicioProfesional>();
builder.Services.AddScoped<IRutinaServicio, ServicioRutina>();
builder.Services.AddScoped<IMediaServicio, ServicioMedia>();
builder.Services.AddScoped<IComentarioServicio, ServicioComentario>();
builder.Services.AddScoped<IPublicacionServicio, ServicioPublicacion>();
builder.Services.AddScoped<INotificacionServicio, ServicioNotificacion>();


// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientePolicy", policy =>
    {
        policy.AllowAnyOrigin() // SOLO para desarrollo
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
object value = builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MetaGym Cliente API",
        Version = "v1",
        Description = "API para clientes externos con autenticación JWT"
    });

    // Definición del esquema JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Token JWT. Escribí: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ---------- Build ----------
var app = builder.Build();

// ---------- Migración DB ----------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbContextApp>();
    context.Database.Migrate();
}

// ---------- Precarga ----------
CargaAdmin.CargarAdminBase(app.Services);
app.UseStaticFiles();
// ---------- Middleware ----------
app.UseCors("ClientePolicy");
app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();

// ---------- Swagger UI ----------
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
