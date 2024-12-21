using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TPreseau3.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la chaîne de connexion
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable ou vide.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

// Configuration JWT
var key = Encoding.ASCII.GetBytes("YourSecretKeyHere"); // Remplace par une clé sécurisée
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Ajouter les services Swagger pour la documentation API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuration conditionnelle pour Swagger en développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ajouter le middleware pour servir les fichiers statiques
app.UseDefaultFiles(); // Sert index.html par défaut
app.UseStaticFiles();  // Permet de servir les fichiers CSS, JS, etc.

// Configurer le pipeline
app.UseHttpsRedirection();
app.UseAuthentication(); // Activer l'authentification JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
