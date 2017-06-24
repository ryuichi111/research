using System;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CosmosTableExample
{
    public class TableManager
    {
        private CloudStorageAccount cloudStorageAccount = null;
        private CloudTableClient cloudTableClient = null;

        public TableManager()
        {
            string connectionString = ConfigurationManager.AppSettings["TableConnectionString"];

            this.cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            this.cloudTableClient = this.cloudStorageAccount.CreateCloudTableClient();

        }

        public bool CreateTable(string tableName)
        {
            bool result = false;

            CloudTable table = this.cloudTableClient.GetTableReference(tableName);
            result = table.CreateIfNotExists();

            return result;
        }

        public bool InsertPerson(string tableName, Person person)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);
            // INSERT操作オブジェクトを取得
            TableOperation insertOperation = TableOperation.Insert(person);
            // 実行
            TableResult result = table.Execute(insertOperation);

            return true;
        }

        public bool InsertPersonBatch(string tableName, List<Person> persons)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);

            // バッチ操作オブジェクトを作成
            TableBatchOperation tableBatchOperation = new TableBatchOperation();

            // INSERT操作オブジェクトを作成
            foreach (Person person in persons)
            {
                TableOperation insertOperation = TableOperation.Insert(person);
                tableBatchOperation.Add(insertOperation);
            }

            // バッチ実行
            IList<TableResult> results = table.ExecuteBatch(tableBatchOperation);

            return true;
        }

        public async Task<Person> FindByPartitionKeyAndRowKey(string tableName, string partitionKey, string rowKey)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);

            // テーブルオペレーションを作成
            TableOperation tableOperation = TableOperation.Retrieve<Person>(partitionKey, rowKey);

            // クエリー実行
            TableResult person = await table.ExecuteAsync(tableOperation);

            if (person.Result != null)
                return person.Result as Person;

            return null;
        }


        public async Task<List<Person>> FindByPartitionKey(string tableName, string partitionKey)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);

            // クエリーオブジェクトを作成
            TableQuery<Person> query = new TableQuery<Person>()
              .Where(
              TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                partitionKey)
              );

            // 実行
            var persons = await table.ExecuteQuerySegmentedAsync<Person>(query, null);

            return persons.Results;
        }

        public async Task<List<Person>> FindByCountryAndBirth(string tableName, string country, DateTime birth)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);

            // 日付を20桁のTicksに変換
            string sBirth = birth.Ticks.ToString("00000000000000000000");
            
            // テーブルオペレーションを作成
            TableQuery<Person> query = new TableQuery<Person>()
                .Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, country),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("Birth", QueryComparisons.GreaterThanOrEqual, sBirth)
                    //TableQuery.GenerateFilterConditionForDate("Birth", QueryComparisons.GreaterThanOrEqual, new DateTimeOffset(birth))
                    ));
            
            var persons = await table.ExecuteQuerySegmentedAsync<Person>(query, null);

            return persons.Results;
        }

        public bool ReplatePerson(string tableName, Person person)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);
            // REPLACE操作オブジェクトを取得
            TableOperation replaceOperation = TableOperation.Replace(person);
            // 実行
            TableResult result = table.Execute(replaceOperation);

            return true;
        }

        public bool DeletePerson(string tableName, Person person)
        {
            // テーブルオブジェクトを取得
            CloudTable table = this.cloudTableClient.GetTableReference(tableName);
            // DELETE操作オブジェクトを取得
            TableOperation deleteOperation = TableOperation.Delete(person);
            // 実行
            TableResult result = table.Execute(deleteOperation);

            return true;
        }
    }
}
