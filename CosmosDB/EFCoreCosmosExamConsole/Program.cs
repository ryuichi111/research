using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EFCoreCosmosExamConsole.Models;

namespace EFCoreCosmosExamConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // テスト用の家族オブジェクトを作成（世帯主＋パートナー＋子供×2）
            Person takashi = new Person() { PersonId = Guid.NewGuid(), FirstName = "Takashi", LastName = "Tanaka", Birth = new DateTime(1985, 4, 5) };
            Person sawako = new Person() { PersonId = Guid.NewGuid(), FirstName = "Sawako", LastName = "Tanaka", Birth = new DateTime(1983, 7, 12) };
            Person chiyori = new Person() { PersonId = Guid.NewGuid(), FirstName = "Chiyori", LastName = "Tanaka", Birth = new DateTime(2001, 10, 4) };
            Person mamoru = new Person() { PersonId = Guid.NewGuid(), FirstName = "Mamoru", LastName = "Tanaka", Birth = new DateTime(2002, 11, 20) };
            Family family = new Family()
            {
                FamilyId = Guid.NewGuid(),
                HeadOfHousehold = takashi,
                Partner = sawako,
            };
            family.Children = new List<Person>();
            family.Children.Add(chiyori);
            family.Children.Add(mamoru);

            // CosmosDBに保存
            using (var context = new PeopleContext())
            {
                // Database / Collectionを（無ければ）作成
                context.Database.EnsureCreated();

                // Familyをコンテキストに追加
                context.Families.Add(family);

                // SaveChanges()の裏側でオブジェクトをJSON変換、シャドウプロパティの追加、CosmosDBへの保存、が行われる
                context.SaveChanges();
            }

            // CosmosDBから読み込み
            using (var context = new PeopleContext())
            {
                // Familyを関連オブジェクトごと取得
                var loladedFamily = context.Families
                    .Include(f => f.HeadOfHousehold)
                    .Include(f => f.Partner)
                    .Include(f => f.Children)
                    .Where(f => f.FamilyId == family.FamilyId).FirstOrDefault();

                // Personを単独で取得
                var loadedSawako = context.Persons
                    .Where(p => p.PersonId == sawako.PersonId).FirstOrDefault();
            }
        }
    }
}
