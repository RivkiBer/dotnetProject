using BakeryServices.Interface;
using NamespaceBakery.Services;
using UserNamespace.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models; // נדרש עבור הגדרות Swagger מתקדמות

var builder = WebApplication.CreateBuilder(args);

// --- 1. רישום שירותים (Services) ---
builder.Services.AddPastryService();
builder.Services.AddUserService();
builder.Services.AddActiveUser();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// הגדרות לוגים
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// רישום Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // הוספת אפשרות להכניס Token בתוך ממשק ה-Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "נא להזין את ה-Token בלבד (ללא המילה Bearer)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// הגדרות אימות (Authentication)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = UserNamespace.Services.TokenService.GetTokenValidationParameters();
    });

// הגדרות הרשאות (Authorization)
builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("AllUsers", policy => policy.RequireClaim("type", "Admin", "Regular"));
    cfg.AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin"));
});

var app = builder.Build();

// --- 2. הגדרת צינור הבקשות (Middleware Pipeline) ---

if (app.Environment.IsDevelopment())
{
    // הערה: הסרנו את MapOpenApi כדי למנוע התנגשות עם Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        // אם את רוצה שה-Swagger יהיה דף הבית, בטלי את הערה בשורה הבאה:
        // options.RoutePrefix = string.Empty; 
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// חשוב: סדר ה-Middleware קריטי! קודם Authentication ואז Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();