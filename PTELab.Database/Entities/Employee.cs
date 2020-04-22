using System;

namespace PTELab.Database.Entities
{
    public class Employee : Entity
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual Company Company { get; set; }
    }
}