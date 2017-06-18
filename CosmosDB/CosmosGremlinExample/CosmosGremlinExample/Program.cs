using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosGremlinExample
{
    class Program
    {
        private GremlinManager manager = new GremlinManager();

        static void Main(string[] args)
        {
            Console.WriteLine("GremlinManagerクラスのEndpointUrlとAuthKey(PrimaryKey)をあなたの環境の値に設定の上で実行してください。");
            Console.WriteLine("また、400 RUのコレクションを作成します。コレクションを作成すると、RUに比例したCosmos DB利用料金が課金されますのでご注意ください。");
            Console.WriteLine("処理を続行して、よろしいですか？(y/n)");
            if (Console.ReadLine().ToUpper() != "Y")
            {
                Console.WriteLine("処理を中断しました。");
                return;
            }

            var program = new Program();

            var res1 = program.CreateDatabaseAndCollection().Result;
            var res2 = program.ClearDatabaseCollection().Result;
            var res3 = program.CreateVertex().Result;
            var res4 = program.GetAllVertex().Result;
            var res5 = program.GetVertexById().Result;
            var res6 = program.GetAllEdge().Result;
            var res7 = program.GetSameOrderCustomers().Result;
            var res8 = program.GetRecomendBooks().Result;

        }

        public async Task<bool> ClearDatabaseCollection()
        {
            var ret = await manager.DropVertexAndEdge();

            return true;
        }
        
        public async Task<bool> CreateDatabaseAndCollection()
        {
            var database = await manager.CreateDatabase();
            var collection = await manager.CreateCollection();

            return true;
        }

        public async Task<bool> CreateVertex()
        {
            // add Customer Vertex
            var cv1 = await manager.AddVertex("Customer", "daigo");
            var cv2 = await manager.AddVertex("Customer", "tanaka");
            var cv3 = await manager.AddVertex("Customer", "kido");
            var cv4 = await manager.AddVertex("Customer", "sakai");

            // add Book Vertex
            // AddVertex() + SetProperties()呼び出し
            var bv1 = await manager.AddVertex("Book", "978-4101339115");
            var prop1 = new Dictionary<string, string>();
            prop1.Add("title", "きらきらひかる");
            prop1.Add("author", "江國香織");
            var bp1 = await manager.SetProperties("978-4101339115", prop1);

            // もう1つのAddVertex()オーバーロード呼び出し
            var prop2 = new Dictionary<string, string>();
            prop2.Add("title", "流しのしたの骨");
            prop2.Add("author", "江國香織");
            var bv2 = await manager.AddVertex("Book", "978-4101339153", prop2);

            var prop3 = new Dictionary<string, string>();
            prop3.Add("title", "キッチン");
            prop3.Add("author", "吉本ばなな");
            var bv3 = await manager.AddVertex("Book", "978-4041800089", prop3);

            var prop4 = new Dictionary<string, string>();
            prop4.Add("title", "月に吠える");
            prop4.Add("author", "萩原朔太郎");
            var bv4 = await manager.AddVertex("Book", "978-4903620510", prop4);

            var prop5 = new Dictionary<string, string>();
            prop5.Add("title", "抱擁、あるいはライスには塩を");
            prop5.Add("author", "江國香織");
            var bv5 = await manager.AddVertex("Book", "978-4087713664", prop5);
            
            // add order Edge
            // daigo -> きらきらひかる
            var e1 = await manager.AddEdge("order", "daigo", "978-4101339115");
            // tanaka -> きらきらひかる
            var e2 = await manager.AddEdge("order", "tanaka", "978-4101339115");
            // tanaka -> 月に吠える
            var e3 = await manager.AddEdge("order", "tanaka", "978-4903620510");
            // sasaki -> キッチン
            var e4 = await manager.AddEdge("order", "sasaki", "978-4041800089");
            // sasaki -> 流しのしたの骨
            var e5 = await manager.AddEdge("order", "sasaki", "978-4101339153");
            // kido -> きらきらひかる
            var e6 = await manager.AddEdge("order", "kido", "978-4101339115");
            // kido -> 抱擁、あるいはライスには塩を
            var e7 = await manager.AddEdge("order", "kido", "978-4087713664");
            // kido -> 流しのしたの骨
            var e8 = await manager.AddEdge("order", "kido", "978-4101339153");

            return true;
        }

        public async Task<bool> GetAllVertex()
        {
            Console.WriteLine("");
            Console.WriteLine("-start- すべてのVertexをリストします");

            var ret = await manager.GetAllVertex();
            foreach (var vertex in ret)
            {
                Console.WriteLine("---");
                Console.WriteLine("id: " + vertex.id);

                string label = vertex.label;
                if (label == "Book")
                {
                    Console.WriteLine("title: " + vertex.properties.title[0].value);
                    Console.WriteLine("author: " + vertex.properties.author[0].value);
                }
            }

            Console.WriteLine("-end-");

            return true;
        }

        public async Task<bool> GetVertexById()
        {
            dynamic vertex = await this.manager.GetVertexById("978-4087713664");

            Console.WriteLine("");
            Console.WriteLine("-start- 978-4087713664のVertexを検索します");

            Console.WriteLine("id: " + vertex.id);
            Console.WriteLine("title: " + vertex.properties.title[0].value);
            Console.WriteLine("author: " + vertex.properties.author[0].value);

            Console.WriteLine("-end-");

            return true;
        }

        public async Task<bool> GetAllEdge()
        {
            Console.WriteLine("");
            Console.WriteLine("-start- すべてのEdgeをリストします");

            var ret = await manager.GetAllEdge();
            foreach (var vertex in ret)
            {
                Console.WriteLine("id: " + vertex.id);
                Console.WriteLine("label: " + vertex.label);
            }

            Console.WriteLine("-end-");

            return true;
        }

        public async Task<bool> GetSameOrderCustomers()
        {
            Console.WriteLine("");
            Console.WriteLine("-start- daigoと同じ書籍を購入したCustomerをリストします");

            var ret = await manager.GetSameOrderCustomers("daigo");
            foreach (var vertex in ret)
            {
                Console.WriteLine(vertex.id);
            }

            Console.WriteLine("-end-");

            return true;
        }

        public async Task<bool> GetRecomendBooks()
        {
            Console.WriteLine("");
            Console.WriteLine("-start- daigoにおすすめの書籍をリストします");

            var ret = await manager.GetRecomendBooks("daigo");
            foreach (var vertex in ret)
            {
                Console.WriteLine("id: " + vertex.id);
                Console.WriteLine("title: " + vertex.properties.title[0].value);
                Console.WriteLine("author: " + vertex.properties.author[0].value);
            }

            Console.WriteLine("-end-");

            return true;
        }
    }
}
