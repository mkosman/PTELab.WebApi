using System.Collections.Generic;

namespace PTELab.Database.Entities
{
    public class Company : Entity
    {
        public virtual string Name { get; set; }
        public virtual int EstablishmentYear { get; set; }
        public virtual IList<Employee> Employees { get; set; }
    }
}
