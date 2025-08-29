using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecialEducationPlanning
.Api.WebTest
{
    public static class DbContextExtensions
    {
        public static void Seed(this DataContext context)
        {
            var testUser1 = new User
            {
                FirstName = "Luke",
                Surname = "Skywalker"
            };

            context.Add(testUser1);

            context.SaveChanges();
        }
    }
}
