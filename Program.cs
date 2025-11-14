using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IosAssignment2Backend.Data;
using IosAssignment2Backend.Models;
using IosAssignment2Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure TicketMaster API settings
builder.Services.Configure<TicketMasterApiSettings>(
    builder.Configuration.GetSection("TicketMasterApi")
);

builder.Services.AddHttpClient("TicketMasterClient");


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Stores.MaxLengthForKeys = 128; // Key/ID length

    // Password settings
    options.Password.RequireDigit = true; // at least one digit
    options.Password.RequireLowercase = true; // at least one lowercase letter
    options.Password.RequireNonAlphanumeric = true; // at least one special character required (examples: ! @ # $ % ^ & * ( ) - _ = + . , and also spaces)
    options.Password.RequireUppercase = true; // at least one uppercase letter
    options.Password.RequiredLength = 8; // minimum length
    options.Password.RequiredUniqueChars = 1; // at least one unique character
    
    // Sign-in settings - require confirmed account for login
    options.SignIn.RequireConfirmedAccount = false; // Set to false to allow immediate login after registration
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Add Controllers
builder.Services.AddControllers();

// Add JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Add Cors
builder.Services.AddCors(o => o.AddPolicy("AllowAllPolicy", builder => {
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllPolicy"); 
app.UseRouting();
// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    // Apply migrations
    context.Database.Migrate();

    // user manager type
    var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Call seed method
    IdentitySeedData.Initialize(context, userMgr).Wait();
}

app.Run();
