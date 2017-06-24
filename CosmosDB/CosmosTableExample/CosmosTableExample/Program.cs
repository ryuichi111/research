
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosTableExample
{
    class Program
    {
        TableManager tableManager = new TableManager();

        static void Main(string[] args)
        {
            Console.WriteLine("App.configのTableConnectionStringをあなたの環境の値に設定の上で実行してください。");
            Console.WriteLine("また、500 RUのテーブルを作成します。テーブルを作成すると、RUに比例したCosmos DB利用料金が課金されますのでご注意ください。");
            Console.WriteLine("処理を続行して、よろしいですか？(y/n)");
            if (Console.ReadLine().ToUpper() != "Y")
            {
                Console.WriteLine("処理を中断しました。");
                return;
            }

            var program = new Program();
            program.CallCreateTable();
            // 利用する機能をコメントアウトしてください。
            //program.CallInsertPerson();
            //program.CallInsertPersonDontSetRowKey(); 
            //program.CallInsertPersonBatch();
            //bool ret1 = program.CallFindByPartitionKeyAndRowKey().Result;
            //bool ret2 = program.CallFindByPartitionKey().Result;
            //bool ret3 = program.CallFindByCountryAndBirth().Result;
            //bool ret4 = program.CallReplacePerson().Result;
            //bool ret5 = program.CallDeletePerson().Result;
        }

        public void CallCreateTable()
        {
            tableManager.CreateTable("PersonTable");
        }

        public void CallInsertPerson()
        {
            Person person = new Person("Japan", "0000000001")
            {
                FirstName = "Ryuichi",
                LastName = "Daigo",
                Birth = new DateTime(2000, 9, 16),
                Email = "daigo@clearboxtechnology.net"
            };
            tableManager.InsertPerson("PersonTable", person);
        }

        public void CallInsertPersonDontSetRowKey()
        {
            Person person = new Person("Japan", "")
            {
                FirstName = "Sora",
                LastName = "Daigo",
                Birth = new DateTime(2002, 10, 16),
                Email = "sora@clearboxtechnology.net"
            };
            tableManager.InsertPerson("PersonTable", person);
        }

        public void CallInsertPersonBatch()
        {
            List<Person> persons = new List<Person>();

            string[] country = { "Japan", "UnitedKingdom", "America", "France", "Porland" };
            Random random = new Random();
            int totalIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                persons.Clear();

                for (int n = 0; n < 100; n++)
                {
                    Person person = new Person(country[i], string.Format("{0:1000000000}", totalIndex))
                    {
                        FirstName = "FirstTest" + totalIndex.ToString(),
                        LastName = "LastTest" + totalIndex.ToString(),
                        Birth = new DateTime(random.Next(1960, 2010), random.Next(1, 12), random.Next(1, 28)),
                        Email = string.Format("test{0}@test.jp", totalIndex)
                    };

                    persons.Add(person);
                    totalIndex++;
                }

                tableManager.InsertPersonBatch("PersonTable", persons);

                System.Threading.Thread.Sleep(1500);
            }

            
        }

        public async Task<bool> CallFindByPartitionKeyAndRowKey()
        {
            var person = await this.tableManager.FindByPartitionKeyAndRowKey("PersonTable", "America", "1000000296");

            // 結果をコンソール出力
            Console.WriteLine("FindByKey(\"PersonTable\", \"America\", \"1000000296\")の結果");
            Console.WriteLine(
                string.Format("PartitionKey={0} RowKey={1} FirstNam={2} LastName={3} EMail={4} Birth={5}",
                person.PartitionKey, person.RowKey, person.FirstName, person.LastName, person.Email, person.Birth.ToString("yyyy/MM/dd"))
                );

            return true;
        }

        public async Task<bool> CallFindByPartitionKey()
        {
            var persons = await this.tableManager.FindByPartitionKey("PersonTable", "Japan");
            //var persons = this.tableManager.FindByPartitionKey("PersonTable", "Japan");

            // 結果をコンソール出力
            Console.WriteLine("FindByPartitionKey(\"PersonTable\", \"Japan\")の結果");
            foreach (var person in persons)
            {
                Console.WriteLine(
                    string.Format("PartitionKey={0} RowKey={1} FirstNam={2} LastName={3} EMail={4} Birth={5}",
                    person.PartitionKey, person.RowKey, person.FirstName, person.LastName, person.Email, person.Birth.ToString("yyyy/MM/dd"))
                    );
            }
            return true;
        }

        public async Task<bool> CallFindByCountryAndBirth()
        {
            var persons = await this.tableManager.FindByCountryAndBirth("PersonTable", "France", new DateTime(1990,1,1,0,0,0,DateTimeKind.Utc));

            // 結果をコンソール出力
            Console.WriteLine("FindByCountryAndBirth(\"PersonTable\", \"France\", new DateTime(1990,1,1))の結果");
            foreach (var person in persons)
            {
                Console.WriteLine(
                    string.Format("PartitionKey={0} RowKey={1} FirstNam={2} LastName={3} EMail={4} Birth={5}",
                    person.PartitionKey, person.RowKey, person.FirstName, person.LastName, person.Email, person.Birth.ToString("yyyy/MM/dd"))
                    );
            }
            return true;
        }

        public async Task<bool> CallReplacePerson()
        {
            // エンティティを取得
            Person person = await tableManager.FindByPartitionKeyAndRowKey("PersonTable", "Japan", "0000000001");

            // エンティティ値を変更
            person.LastName = "Modify!!!";
            //person.LastName = "Daigo";
            //person.ETag = Guid.NewGuid().ToString();

            // エンティティを更新
            bool result = tableManager.ReplatePerson("PersonTable", person);

            return result;
        }

        public async Task<bool> CallDeletePerson()
        {
            // エンティティを取得
            Person person = await tableManager.FindByPartitionKeyAndRowKey("PersonTable", "Japan", "0000000001");

            // エンティティを削除
            bool result = tableManager.DeletePerson("PersonTable", person);

            return result;
        }
    }
}
