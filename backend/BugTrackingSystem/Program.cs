using BugTrackingSystem.Data;
using BugTrackingSystem.Exceptions;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Repositories.Implementation;
using BugTrackingSystem.Repositories.Implementations;
using BugTrackingSystem.Repositories.Interfaces;
using BugTrackingSystem.Services;
using BugTrackingSystem.Services.Implementations;
using BugTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- CONTROLLERS --------------------

builder.Services.AddControllers();


// -------------------- GLOBAL ERROR HANDLING --------------------

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


// -------------------- CORS --------------------

var frontendUrl = builder.Configuration["FrontendUrl"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var allowedOrigins = new List<string>
        {
            "http://localhost:5173",
            "https://localhost:5173"
        };

        if (!string.IsNullOrWhiteSpace(frontendUrl))
        {
            allowedOrigins.Add(frontendUrl.TrimEnd('/'));
        }

        policy
            .WithOrigins(allowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// -------------------- SWAGGER --------------------

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bug Tracking System API",
        Version = "v1"
    });

    options.AddSecurityDefinition(
        "bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter only your JWT token. Do not type Bearer.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "bearer",
                    document)
            ] = []
        });
});


// -------------------- DATABASE --------------------

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});


// -------------------- DEPENDENCY INJECTION --------------------

// User and Authentication
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<JwtTokenGenerator>();


// Projects
builder.Services.AddScoped<
    IProjectRepository,
    ProjectRepository>();

builder.Services.AddScoped<
    IProjectService,
    ProjectService>();


// Project Members
builder.Services.AddScoped<
    IProjectMemberRepository,
    ProjectMemberRepository>();

builder.Services.AddScoped<
    IProjectMemberService,
    ProjectMemberService>();


// Bugs
builder.Services.AddScoped<
    IBugRepository,
    BugRepository>();

builder.Services.AddScoped<
    IBugService,
    BugService>();


// Comments
builder.Services.AddScoped<
    ICommentRepository,
    CommentRepository>();

builder.Services.AddScoped<
    ICommentService,
    CommentService>();


// Dashboards
builder.Services.AddScoped<
    IDashboardRepository,
    DashboardRepository>();

builder.Services.AddScoped<
    IDashboardService,
    DashboardService>();


// -------------------- JWT AUTHENTICATION --------------------

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        ))
            };
    });

builder.Services.AddAuthorization();


// -------------------- BUILD APPLICATION --------------------

// Build must appear only once and after all service registrations.
var app = builder.Build();


// -------------------- HTTP MIDDLEWARE PIPELINE --------------------

// Keep this near the beginning so it can catch errors
// from middleware and controllers that run after it.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await DatabaseSeeder.SeedAdminAsync(app);


app.Run();