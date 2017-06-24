using System;

using Microsoft.WindowsAzure.Storage.Table;

namespace CosmosTableExample
{
    public class Person : TableEntity
    {
        public Person()
        {
        }

        public Person(string country, string personId)
        {
            this.PartitionKey = country;
            this.RowKey = personId;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime Birth { get; set; }
    }
}
