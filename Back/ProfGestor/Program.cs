using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ProfGestor.Data;
using ProfGestor.Middlewares;
using ProfGestor.Repositories;
using ProfGestor.Services;

namespace ProfGestor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddDbContext<ProfGestorContext>(options =>
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.21-mysql")));

            // Configurar JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%";
            var issuer = jwtSettings["Issuer"] ?? "ProfGestor";
            var audience = jwtSettings["Audience"] ?? "ProfGestorUsers";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                // Adicionar suporte a cookies para o token
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Tentar obter token do cookie se não estiver no header Authorization
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            context.Token = context.Request.Cookies["authToken"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            // Configurar CORS para React
            var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() 
                ?? new[] { "http://localhost:3000", "http://localhost:5173" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins(corsOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Necessário para enviar cookies/tokens
                });
            });

            // Registrar AutoMapper
            builder.Services.AddAutoMapper(typeof(Mappings.MappingProfile));

            // Registrar Repositories
            builder.Services.AddScoped<IProfessorRepository, ProfessorRepository>();
            builder.Services.AddScoped<ITurmaRepository, TurmaRepository>();
            builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
            builder.Services.AddScoped<IPlanejamentoAulaRepository, PlanejamentoAulaRepository>();
            builder.Services.AddScoped<ILogRepository, LogRepository>();
            builder.Services.AddScoped<IAulaRepository, AulaRepository>();
            builder.Services.AddScoped<IFrequenciaRepository, FrequenciaRepository>();
            builder.Services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
            builder.Services.AddScoped<INotaAvaliacaoRepository, NotaAvaliacaoRepository>();
            builder.Services.AddScoped<IGabaritoQuestaoRepository, GabaritoQuestaoRepository>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Registrar Services
            builder.Services.AddScoped<IProfessorService, ProfessorService>();
            builder.Services.AddScoped<ITurmaService, TurmaService>();
            builder.Services.AddScoped<IAlunoService, AlunoService>();
            builder.Services.AddScoped<IPlanejamentoAulaService, PlanejamentoAulaService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IDisciplinaService, DisciplinaService>();
            builder.Services.AddScoped<IAulaService, AulaService>();
            builder.Services.AddScoped<IFrequenciaService, FrequenciaService>();
            builder.Services.AddScoped<IAvaliacaoService, AvaliacaoService>();
            builder.Services.AddScoped<INotaAvaliacaoService, NotaAvaliacaoService>();
            builder.Services.AddScoped<IGabaritoService, GabaritoService>();
            builder.Services.AddScoped<IRelatorioService, RelatorioService>();

            // Configurar JSON para usar camelCase (padrão JavaScript/TypeScript)
            builder.Services.AddControllers(options =>
                {
                    // Adicionar ModelBinderProvider para DateOnly em rotas
                    options.ModelBinderProviders.Insert(0, new ProfGestor.Binders.DateOnlyModelBinderProvider());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = false;
                    // Adicionar conversor para DateOnly (formato YYYY-MM-DD)
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new ProfGestor.Converters.DateOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new ProfGestor.Converters.CharJsonConverter());
                });

            // Configurar rotas para serem case-insensitive (importante para Linux)
            // Isso força todas as URLs a serem minúsculas, evitando problemas de case sensitivity
            builder.Services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options =>
            {
                options.LowercaseUrls = true; // URLs sempre em minúsculas
                options.LowercaseQueryStrings = true;
            });
            
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ProfGestor API",
                    Version = "v1",
                    Description = "API para gestão de professores e turmas"
                });

                // Configurar JWT no Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProfGestor API V1");
                    c.RoutePrefix = "swagger";
                });
            //}

            // Middleware de tratamento de erros global
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            // CORS deve vir antes de UseAuthentication e UseAuthorization
            app.UseCors("AllowReactApp");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
