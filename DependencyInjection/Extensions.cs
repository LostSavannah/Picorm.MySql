using Microsoft.Extensions.DependencyInjection;
using Picorm.Common;
using Picorm.Common.Mapping;
using Picorm.Common.Mapping.Common;

namespace Picorm.MySql.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddMySql(this IServiceCollection services, string connectionString, IEntityMapperFactory? entityMapperFactory = null)
        {
            return services.AddSingleton<IMapper>(new Mapper(new MySqlDbInterface(connectionString), entityMapperFactory ?? new EntityMapperFactory()));
        }
    }
}
