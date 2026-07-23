using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;

namespace UrbanNoodleTest.TestHelpers
{
    public static class DbContextFactory
    {
        public static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }
    }
}
