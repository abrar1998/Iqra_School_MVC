using SchoolProj.Auth_Filters;
using SchoolProj.DAL.AccountRepository;
using SchoolProj.DAL.DownloadRepository;
using SchoolProj.DAL.GalleryRepository;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.DAL.PageLoadService;
using SchoolProj.DAL.PageRepository;
using SchoolProj.DAL.SliderRepository;
using SchoolProj.DAL.UserRepository;
using SchoolProj.General;
using System.Globalization;

namespace SchoolProj
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Set the default culture globally only for date and purpose to avoid environment errors
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");


            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // Configure ApiSettings
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
            builder.Services.AddHttpClient();

            // Register repositories and services
            builder.Services.AddScoped<INoticeRepo, NoticeRepo>();
            builder.Services.AddScoped<IPageRepo, PageRepo>();
            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<ISliderRepo, SliderRepo>(); // Changed to Scoped for consistency
            builder.Services.AddScoped<IDownloadRepo, DownloadRepo>();
            builder.Services.AddSingleton<IPageLoadRepo, PageLoadRepo>(); // Singleton for page load optimization
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IGalleryRepo, GalleryRepo>();

            // Apply the SessionAuthorize attribute globally
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<SessionAuthorizeAttribute>();
            });

            // Configure session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1); // Set session timeout to 1 day
                options.Cookie.HttpOnly = true;            // Enhance security
                options.Cookie.IsEssential = true;         // Mark cookie as essential
            });

            var app = builder.Build();

            // Load all pages at application startup
            try
            {
                var pageService = app.Services.GetRequiredService<IPageLoadRepo>();
                pageService.LoadAllPagesOnceAsync().GetAwaiter().GetResult();
                pageService.GetNoticeListWithFilesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error occurred while loading pages during startup.");
            }

            // Configure the HTTP request pipeline
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts(); // Enforce strict transport security
            //}

            if (app.Environment.IsProduction())
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession(); // Enable session middleware
            app.UseAuthentication(); // Authentication middleware
            app.UseAuthorization();  // Authorization middleware

            // Default route configuration
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Default}/{id?}");

            app.Run();
        }
    }
}
