using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.SeedData
{
    public static class GymDbContextSeeding
    {
        public static bool SeedData(GymDbContext dbContext)
        {
            try
            {
                var hasPlan = dbContext.Plans.Any();
                var hasCategories = dbContext.Categories.Any();
                if (hasPlan && hasCategories)
                    return false;

                if (!hasPlan)
                {
                    var plans = LoadDataFromJson<Plan>("plans.json");
                    if (plans.Any())
                        dbContext.Plans.AddRange(plans);

                }


                if (!hasCategories)
                {
                    var categories = LoadDataFromJson<Category>("categories.json");
                    if (categories.Any())
                        dbContext.Categories.AddRange(categories);

                }

                return dbContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                Console.WriteLine("seeding faild");
                return false;
            }
        }


        #region Helper Methods

        private static List<T> LoadDataFromJson<T>(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\Files", fileName);

            if (!File.Exists(filePath))
                return [];

            var jsonData = File.ReadAllText(filePath);
           
            var options = new JsonSerializerOptions ()
            { 
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<T>>(jsonData , options) ?? new List<T>();
                                   ;
        }

        #endregion
    }
}
