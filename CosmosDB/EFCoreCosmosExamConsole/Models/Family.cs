using System;
using System.Collections.Generic;

namespace EFCoreCosmosExamConsole.Models
{
    public class Family
    {
        public Guid FamilyId { get; set; }

        public Person HeadOfHousehold { get; set; }

        public Person Partner { get; set; }

        public List<Person> Children { get; set; }
    }

    public class Person
    {
        public Guid PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birth { get; set; }
    }
}
