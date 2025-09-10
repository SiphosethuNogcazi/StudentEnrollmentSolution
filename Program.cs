using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentEnrollment.Api .Data;
using StudentEnrollment.Api.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with InMemory Database
builder.Services.AddDbContext<StudentEnrollmentDbContext>(options =>
    options.UseInMemoryDatabase("StudentEnrollmentDb"));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<StudentEnrollmentDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKey12345!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentEnrollmentIssuer";


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
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course Enrollment API", Version = "v1" });

    // Add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityReq = new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityReq);
});


var app = builder.Build();

// Seed sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudentEnrollmentDbContext>();

    if (!context.Courses.Any())
    {
        context.Courses.AddRange(
            new Course { Title = "Introduction to Blazor", Description = "Learn the basics of Blazor WebAssembly." },
            new Course { Title = "Advanced C#", Description = "Deep dive into advanced C# concepts." },
            new Course { Title = "Entity Framework Core", Description = "Master EF Core with hands-on projects." }
        );
        context.SaveChanges();
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
