using Microsoft.EntityFrameworkCore;
using estudiantes_icinf.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configurar la conexión a PostgreSQL con EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CAMBIAR ESTO: Inyectar el repositorio de PostgreSQL en vez del JsonStudentRepository
builder.Services.AddScoped<IStudentRepository, PostgresStudentRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar endpoints existentes
app.MapStudentEndpoints();

app.Run();