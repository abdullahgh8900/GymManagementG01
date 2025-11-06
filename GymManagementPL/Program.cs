using GymManagementBLL.Mapping;
using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Data.SeedData;
using GymManagementDAL.Repositories.Implementaion;
using GymManagementDAL.Repositories.Interfaces;
using GymManagementDAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace GymManagementPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region DI Regisration
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                // options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            //  builder.Services.AddScoped(typeof(IPlanRepository), typeof(PlanRepository));
            builder.Services.AddScoped(typeof(ISessionRepository), typeof(SessionRepository));
            builder.Services.AddAutoMapper(X=>X.AddProfile(new MappingProfile()));

            #endregion

            var app = builder.Build();

            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var pendingMigration = dbContext.Database.GetPendingMigrations();
            if (pendingMigration?.Any() ?? false)
            {
                dbContext.Database.Migrate(); 
            }

            GymDbContextSeeding.SeedData(dbContext);

            #region Configure Pipeline [MiddelWares]

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            #endregion

            app.Run();
        }
    }
}
