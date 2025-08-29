using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain
{

    public class DataMockup
    {

        private readonly DbContext dbContext;

        private readonly Random rnd = new Random();

        public DataMockup(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region Methods Public

        public void SeedMockup()
        {
            if (!dbContext.Set<Country>().Any(x => true))
            {
                CreateZones(1, 3, 3, 4);
            }

            var Aieps = dbContext.Set<Aiep>().Where(x => true).IgnoreQueryFilters().Include(d => d.Educationers)
                .ToList();

            foreach (var Aiep in Aieps)
            {
                foreach (var Educationer in Aiep.Educationers)
                {
                    var randomBuilders = rnd.Next(1, 5);

                    for (var currentBuilder = 0; currentBuilder < randomBuilders; currentBuilder++)
                    {
                        var builder = RandomizeProperties(new Builder());
                        dbContext.Set<Builder>().Add(builder);
                        dbContext.SaveChanges();

                        var builderEducationerAiep = new BuilderEducationerAiep
                        {
                            BuilderId = builder.Id,
                            Builder = builder,
                            AiepId = Aiep.Id,
                            Aiep = Aiep
                        };

                        dbContext.Set<BuilderEducationerAiep>().Add(builderEducationerAiep);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        #endregion

        #region Methods Private

        private void CreateZones(int countries, int regions, int areas, int Aieps)
        {
            var dbset = dbContext.Set<Country>();

            for (var countryNumber = 0; countryNumber < countries; countryNumber++)
            {
                var country = RandomizeProperties(new Country());
                country.KeyName = "gbr";
                dbset.Add(country);
                dbContext.SaveChanges();

                for (var regionNumber = 0; regionNumber < regions; regionNumber++)
                {
                    var region = RandomizeProperties(new Region());
                    country.Regions.Add(region);
                    dbContext.SaveChanges();

                    for (var areaNumber = 0; areaNumber < areas; areaNumber++)
                    {
                        var area = RandomizeProperties(new Area());
                        region.Areas.Add(area);
                        dbContext.SaveChanges();

                        for (var AiepNumber = 0; AiepNumber < Aieps; AiepNumber++)
                        {
                            var Aiep = RandomizeProperties(new Aiep());
                            Aiep.AiepCode = "D" + rnd.Next(000, 999);
                            area.Aieps.Add(Aiep);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private T RandomizeProperties<T>(T input)
        {
            var properties = input.GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "Id" || propertyInfo.Name.EndsWith("Id"))
                {
                    continue;
                }
                if (propertyInfo.PropertyType.IsSubclassOf(typeof(System.Enum)))
                {
                    var values = System.Enum.GetValues(propertyInfo.PropertyType);
                    propertyInfo.SetValue(input, values.GetValue(rnd.Next(0, values.Length)));
                }

                if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                    propertyInfo.SetValue(input, rnd.Next(int.MinValue, int.MaxValue));
                if (propertyInfo.PropertyType == typeof(string)) propertyInfo.SetValue(input, RandomString(1, 8));
                if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                {
                    var date = DateTime.Now.AddDays(rnd.Next(-600, 0));
                    date = date.AddHours(rnd.Next(-600, 600));
                    propertyInfo.SetValue(input, date);
                }

                if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                    propertyInfo.SetValue(input, rnd.Next(0, 1) >= 1);
            }

            return input;
        }

        private string RandomString(int minLength, int maxLength)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            var length = rnd.Next(minLength, maxLength);

            return new string(Enumerable.Range(1, length).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        }

        #endregion

    }

}

