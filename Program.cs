﻿using Microsoft.EntityFrameworkCore;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);



// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<DataContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    });

    //--------------------------------------------------...Hangfire...-----------------------------------------------

    //builder.Services.AddHangfire(configuration => configuration
    //.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    //.UseSimpleAssemblyNameTypeSerializer()
    //.UseRecommendedSerializerSettings()
    //.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

    ////services.AddHangfire(config => config.UseSqlServerStorage("DefaultConnection"));
    //services.AddHangfire((container, configuration) => configuration
    // .UseSqlServerStorage(container.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")));



// Add Hangfire server
//services.AddHangfireServer();

    services.AddCors();

    // configure automapper with all automapper profiles from this assembly
    services.AddAutoMapper(typeof(Program));

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<PatientsController>();
    services.AddScoped<IPatientService, PatientServices>();


    services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


    services.AddScoped<FcmNotificationService>();  // Register as scoped
    services.AddHostedService<FcmNotificationService>();


    // this code i got from this :
    //https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
    builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });
}


var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

//// Use Hangfire Dashboard
//using (var scope = app.Services.CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    var patientServices = serviceProvider.GetRequiredService<IPatientService>();
//    RecurringJob.AddOrUpdate("send-notifications-job", () => patientServices.SendMedicineNotifications(), Cron.MinuteInterval(1));
//}


var firebaseCredential = GoogleCredential.FromFile("SDK.json");
var firebaseApp = FirebaseApp.Create(new AppOptions
{
    Credential = firebaseCredential,
});


// configure HTTP request pipeline

{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });


    //app.UseHangfireDashboard("/hangfire");
    //// Use Hangfire Server Signal r fi
    //app.UseHangfireServer();



}

app.Run();