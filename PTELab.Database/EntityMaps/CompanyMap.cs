using FluentNHibernate.Mapping;
using PTELab.Database.Entities;

namespace PTELab.Database.EntityMaps
{
    public class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Id(x => x.Id)
                .GeneratedBy.Identity()
                ;
            
            Map(x => x.Name)
                .Not.Nullable()
                .Length(100)
                ;

            Map(x => x.EstablishmentYear)
                ;

            HasMany(x => x.Employees)
                .Cascade.All()
                ;

            Table("Company");
        }
    }
}