using Microsoft.AspNetCore.Authentication.Cookies;
using SQLChecker.Models;
using SQLChecker.Models.Services;
using SQLChecker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddServerSideBlazor();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMoodle", policy =>
    {
        policy.WithOrigins(
            "https://learn.hamk.fi"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


DB_Services.Initialize(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<TaskCheckingService>();
builder.Services.AddScoped<IDatabaseService,DatabaseService>();

var connectionString = builder.Configuration.GetConnectionString("SqlDatabaseConnection");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.LogoutPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/Index";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.Cookie.Name = "REDACTED_PROJECT_NAME";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NoAdminAccess", policy =>
    {
        policy.RequireAssertion(context =>
        !(context.User.Identity.IsAuthenticated && context.User.IsInRole("admin")));
    });
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowMoodle");

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
