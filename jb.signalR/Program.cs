using jb.signalR.hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(builder =>
    builder
        .SetIsOriginAllowed(origin => string.IsNullOrEmpty(origin) || origin == "http://localhost:8100") // Permite localhost y apps sin origen.
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
);


app.UseRouting();

app.UseAuthorization();
app.MapControllers();


// Configure the HTTP request pipeline.
app.UseEndpoints(endpoints => {
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.Run();


