using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using FluentNHibernate.Cfg;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using PTELab.Database.Entities;
// ReSharper disable UnusedParameter.Global

namespace PTELab.Database.Configurations
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, IConfiguration configuration)
        {
            // todo:: configuration for other db connection
            services.AddSingleton(factory => ConfigureSQliteDb());

            services.AddScoped<ISession>(factory =>
                factory.GetServices<ISessionFactory>()
                    .First()
                    .OpenSession()
            );
            return services;
        }

        private static ISessionFactory ConfigureInMemoryDb()
        {
            var factory = Fluently.Configure()
                .Database(() => FluentNHibernate.Cfg.Db.SQLiteConfiguration
                    .Standard
                    .InMemory()
                    .ShowSql())
                .Mappings(m=>
                    m.FluentMappings.AddFromAssemblyOf<Company>()
                    )
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory()
                ;
            return factory;
        }

        private static ISessionFactory ConfigureSQliteDb()
        {
            var factory = Fluently.Configure()
                .Database(() => FluentNHibernate.Cfg.Db.SQLiteConfiguration
                    .Standard
                    .UsingFile("coompanies.db")
                    .ShowSql())
                .Mappings(m=>
                    m.FluentMappings.AddFromAssemblyOf<Company>()
                    )
                .ExposeConfiguration(BuildSchema)
               .BuildSessionFactory()
                ;
            return factory;
        }
        private static void BuildSchema(Configuration config)
        {
            if (!File.Exists("coompanies.db"))
            {
                new SchemaExport(config).SetOutputFile("schema.sql").Create(true, true);
                return;
            }

            var needUpdateSchema = false;
            try
            {
                new SchemaValidator(config).Validate();
            }
            catch
            {
                needUpdateSchema = true;
            }

            if (!needUpdateSchema) return;

            try
            {
                new SchemaUpdate(config).Execute(true, true);
            }
            catch (Exception)
            {
            }
        }
    }
}
