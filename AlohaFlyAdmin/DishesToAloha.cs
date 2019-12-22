using StoreHouseConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using AlohaService.ServiceDataContracts;
using System.Reflection;

namespace AlohaFlyAdmin
{
  public static  class DishesToAloha
    {
        public static List<Dish> GetSVODishes()
        {
            var tmp = new List<Dish>();
            tmp = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.Name!=null && a.Name.StartsWith("SVO")).ToList();
            return tmp;
        }


        private static List<AirCompany> GetAirs()
        {
            var tmp = new List<AirCompany>();
            tmp = AlohaFly.DBProvider.Client.GetAirCompanyList().Result.ToList();
            return tmp;
        }

        public static void InsertIntoCFCSQL()
        {
            var dList = GetSVODishes();
            AddToSql(dList);
        }

        public static void InsertTendersIntoCFCSQL()
        {
            var dList = GetAirs();
            AddAirsToSql(dList);
        }


        private static void AddAirsToSql(List<AirCompany> Tmp)
        {
            String SQLPL = @"Data Source=192.168.222.116\SQLEXPRESS;Initial Catalog=CFCStandaloneDB;User ID=manager;Password=manager ";
            //string pStr = @"TERM1\SQLEXPRESS";
            var db = new CFCdbDataContext(SQLPL);
            foreach (AirCompany d in Tmp.Where(a=>a.Id>3))
            {
                Tender oldtndr = db.Tender.FirstOrDefault(a => a.Number == 101);
                Tender tndr = new Tender();
                foreach (PropertyInfo PI in tndr.GetType().GetProperties())
                {
                    
                    object val = tndr.GetType().GetProperty(PI.Name).GetValue(oldtndr);


                    tndr.GetType().GetProperty(PI.Name).SetValue(tndr, val);
                }
                tndr.Id = Guid.NewGuid();
                tndr.Number = (int)d.Id + 100;
                tndr.Name = d.Name.Substring(0,Math.Min(d.Name.Length,9));
                
                db.Tender.InsertOnSubmit(tndr);
                db.SubmitChanges();
                //tndr.FK_ReportAsTender = tndr.Id;
                //db.SubmitChanges();
            }
            }

            public static void AddToSql(List<Dish> Tmp)
        {
            String SQLPL = @"Data Source=192.168.0.101\SQLEXPRESS;Initial Catalog=CFCStandaloneDB;User ID=manager;Password=manager ";
            //string pStr = @"TERM1\SQLEXPRESS";
            var db = new CFCdbDataContext(SQLPL);
            foreach (Dish d in Tmp)
            {
                Category cat = db.Category.Where(a => a.Number==2).First();
                /*
                Category SaleCat = GetCat(d.SaleCat);
                if (SaleCat == null)
                {
                    MessageBox.Show("Error SaleCat" + d.Id);
                }
                Category GenCat = GetCat(d.GeneralCat);
                if (GenCat == null)
                {
                    MessageBox.Show("Error GenCat" + d.Id);
                }
                */
                Item OldIt = db.Item.Where(a => a.Number == 1).First();
                
                Item it = new Item();
                
                foreach (PropertyInfo PI in it.GetType().GetProperties())
                {
                    /*
                    if (PI.Name == "FK_SalesCategory")
                    {
                        continue;
                    }
                    if (PI.Name == "Category")
                    {
                        continue;
                    }
                    if (PI.Name == "CategoryItem")
                    {
                        continue;
                    }
                    */
                    object val = it.GetType().GetProperty(PI.Name).GetValue(OldIt);


                    it.GetType().GetProperty(PI.Name).SetValue(it, val);
                }
                
                it.Id = Guid.NewGuid();
                it.Number = (int)d.Barcode;
                it.DefaultPrice = 0;
                d.Name = d.Name.Substring(4);
                if (d.Name.Length > 25)
                {
                    d.Name = d.Name.Substring(0, 25);
                }
                it.LongName = d.Name;
                it.ShortName = d.Name.Substring(0,Math.Min(d.Name.Length, 14));
                //it.FK_SalesCategory = SaleCat.Id;
                it.ChitName = d.Name.Substring(0, Math.Min(d.Name.Length, 14));
                if (d.EnglishName.Length > 25)
                {
                    d.EnglishName = d.EnglishName.Substring(0, 25);
                }
                it.LongNameAlternate = d.EnglishName;
                db.Item.InsertOnSubmit(it);
                db.SubmitChanges();
                /*
                CategoryItem CI = new CategoryItem();
                CI.Id = Guid.NewGuid();
                //CI.Item = it;
                //CI.Category = GenCat;
                CI.FK_CategoryId = GenCat.Id;
                CI.FK_ItemId = it.Id;
                CI.FK_Owner = OldIt.FK_Owner;


                db.CategoryItem.InsertOnSubmit(CI);
                db.SubmitChanges();
                */

            }
        }
    }
}
