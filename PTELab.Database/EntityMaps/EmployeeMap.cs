using FluentNHibernate.Mapping;
using PTELab.Database.Entities;

namespace PTELab.Database.EntityMaps
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id)
                .GeneratedBy.Identity()
                ;

            Map(x => x.FirstName)
                .Not.Nullable()
                .Length(100)
                ;
           
            Map(x => x.LastName)
                .Not.Nullable()
                .Length(100)
                ;

            Map(x => x.DateOfBirth)
                ;

            Map(x => x.JobTitle)
                .Not.Nullable()
                ;

            References(x => x.Company)
                //.Not.Nullable()
                ;

            Table("Employees");
        }
    }
}