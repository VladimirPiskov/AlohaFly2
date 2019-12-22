using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace AlohaFlyAdmin
{
    public class DishModif
    {
        public DishModif()
        {}

        public List<long> GetDisableDish()
        {
            List<long> DId = new List<long>();
            using (StreamReader sr = new StreamReader("DisableDish.txt"))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        DId.Add(Convert.ToInt32(sr.ReadLine()));
                    }
                    catch
                    { }
                }
            }
            return DId;
        }

        public void DisableDish()
        {
            List<long> dd = GetDisableDish();

             var r =  AlohaFly.DBProvider.Client.GetDishList().Result.Where(a=>dd.Contains(a.Id)).ToList();
            foreach (var v in r) { v.IsActive = false; }


            AlohaFly.DBProvider.UpdateDishList(r);
           
        }

        public void CreatePaymentsLink()
        { }

        /*
         * Id	CompanyName	PaymentId
1	Avcon Jet                                                                                           	6
2	Aviapartner                                                                                         	12
3	Cl-4                                                                                                	6
4	FTConsulting                                                                                        	6
5	Global Jet                                                                                          	6
6	Golden Wings                                                                                        	12
7	Jet Aviation                                                                                        	6
8	Net Jets                                                                                            	6
9	Nomad Aviation                                                                                      	6
10	Private Flight                                                                                      	6
11	Qatar Executive                                                                                     	6
12	Restaero                                                                                            	12
13	Rus Aero                                                                                            	12
14	Sirius Aero                                                                                         	12
15	Tag Aviation                                                                                        	6
16	Universal aviation                                                                                  	12
17	Vip Jet Service                                                                                     	12
18	Vip-Port                                                                                            	12
19	АВАНГАРД-АВИА                                                                                       	12
20	АО Скай Атлас                                                                                       	12
21	Бизнес Авиэйшн Центр                                                                                	12
22	Вельталь-авиа                                                                                       	12
23	Главная линия                                                                                       	12
24	Джет Кетеринг                                                                                       	12
25	Джет Прогресс                                                                                       	12
26	ДжетЭйрГрупп                                                                                        	12
27	Ист Юнион                                                                                           	12
28	МБК-С                                                                                               	12
29	МСкай                                                                                               	12
30	ООО Авиа Групп                                                                                      	12
31	Премьер Авиа                                                                                        	12
32	Частное лицо                                                                                        	7
33	Rus Jet                                                                                             	12
34	Меридиан                                                                                            	12
39	Aviation Consulting                                                                                 	6
40	Satcom                                                                                              	12
41	Flight Catering                                                                                     	12
42	Streamline                                                                                          	12
43	ООО Транспит Северо-Запад                                                                           	12

         * 
         * 
         * 
         * 
         * */

        public void CreatePayments()
        {
            /*
            0   Неизв False   0
6   Наличные 7  True    0
7   Кредитки 7  False   1
11  наличные 7  True    0
12  Безналичные 7   False   0
NULL NULL    NULL NULL
*/
            List<AlohaService.ServiceDataContracts.Payment> Pp = new List<AlohaService.ServiceDataContracts.Payment>()
                    {
                       new AlohaService.ServiceDataContracts.Payment ()
                       {
                           Code = 0,
                           IsActive=true,
                           FiskalId=0,
                           IsCash =false,
                           Name="Неизв"
                       },
                       new AlohaService.ServiceDataContracts.Payment ()
                       {
                           Code = 6,
                           IsActive=true,
                           FiskalId=0,
                           IsCash =true,
                           Name="Наличные 7"
                       }
                       ,
                       new AlohaService.ServiceDataContracts.Payment ()
                       {
                           Code = 7,
                           IsActive=true,
                           FiskalId=0,
                           IsCash =false,
                           Name="Кредитки 7"
                       }
                       ,
                       new AlohaService.ServiceDataContracts.Payment ()
                       {
                           Code = 11,
                           IsActive=true,
                           FiskalId=0,
                           IsCash =true,
                           Name="наличные 7"
                       }
                       ,
                       new AlohaService.ServiceDataContracts.Payment ()
                       {
                           Code = 12,
                           IsActive=true,
                           FiskalId=0,
                           IsCash =false,
                           Name="Безналичные 7"
                       }

                    };
            foreach (var p in Pp)
            {
                var rres = AlohaFly.DBProvider.Client.CreatePayment(p);
            }
            

        }


        public void CreateDiscounts()
        {

            //CreateDiscounts00();
            CreateDiscounts10();
            /*
            CreateDiscounts0();
            CreateDiscounts1();
            CreateDiscounts2();
            CreateDiscounts3();
            CreateDiscounts4();
            CreateDiscounts5();
            CreateDiscounts6();
            */
        }


        public void CreateDiscounts0()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(39);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=0,
                            End = null
                        }
                    }
                };

                AddDiscToBase(d, airc.Result);
            }

        }

        public void CreateDiscounts1()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(10);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;
                
            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "5%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=5,
                            Start=0,
                            End = null
                        }
                    }
                };

                AddDiscToBase(d, airc.Result);
            }
            
        }

        public void CreateDiscounts2()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(25);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "0-600000-5% "+Environment.NewLine+ " более 600000-10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=5,
                            Start=0,
                            End = 600000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=600000,
                            End = null
                        }
                    }
                };
                AddDiscToBase(d, airc.Result);
            }

        }


        public void CreateDiscounts3()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(11);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "0-400000-5% " + Environment.NewLine + "400000-800000-8% " + Environment.NewLine + " более 800000-10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=5,
                            Start=0,
                            End = 400000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=8,
                            Start=400000,
                            End = 800000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=800000,
                            End = null
                        }
                    }
                };
                AddDiscToBase(d, airc.Result);
            }

        }
        public void CreateDiscounts4()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(9);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "0-500000-5% " + Environment.NewLine + "500000-1000000-8% " + Environment.NewLine + " более 1000000-10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=5,
                            Start=0,
                            End = 500000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=8,
                            Start=500000,
                            End = 1000000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=1000000,
                            End = null
                        }
                    }
                };
                AddDiscToBase(d, airc.Result);
            }

        }

        public void CreateDiscounts5()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(12);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=0,
                            End = null
                        }
                    }
                };

                AddDiscToBase(d, airc.Result);
            }

        }

        public void CreateDiscounts00()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(13);
            bool create = true;
            /*
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            */
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "Нет",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=0,
                            Start=0,
                            End = null
                        }
                    }
                };

                AddDiscToBase(d, airc.Result);
            }




        }

        public void CreateDiscounts6()
        {
            var airc = AlohaFly.DBProvider.Client.GetAirCompany(3);
            bool create = true;
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=0,
                            End = null
                        }
                    }
                };

                AddDiscToBase(d, airc.Result);
            }

        }


        public void CreateDiscounts10()
        {
            
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "8%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=8,
                            Start=0,
                            End = null
                        }
                    }
                };

                AddDiscToBase(d, null);
            

        }


        public void CreateDiscounts7()
        {

            var airc = AlohaFly.DBProvider.Client.GetAirCompany(19);
            bool create = true;
            /*
            if (airc.Result.DiscountId != null)
            {
                var disc = AlohaFly.DBProvider.Client.GetDiscount(airc.Result.DiscountId.Value);
                create = !disc.Success;

            }
            */
            if (create)
            {
                AlohaService.ServiceDataContracts.Discount d = new AlohaService.ServiceDataContracts.Discount()
                {
                    Name = "0-500000-5% " + Environment.NewLine + "0-800000-8% " + Environment.NewLine + " более 800000-10%",
                    Ranges = new List<AlohaService.ServiceDataContracts.DiscountRange>()
                    {
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=5,
                            Start=0,
                            End = 500000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=8,
                            Start=0,
                            End = 800000
                        },
                        new AlohaService.ServiceDataContracts.DiscountRange ()
                        {
                            DiscountPercent=10,
                            Start=0,
                            End = null
                        }
                    }
                };
                AddDiscToBase(d, airc.Result);
            }

        }


        private void  AddDiscToBase(AlohaService.ServiceDataContracts.Discount d, AlohaService.ServiceDataContracts.AirCompany arc)
        {
            var dres = AlohaFly.DBProvider.Client.CreateDiscount(d);
            foreach (var r in d.Ranges)
            {
                try
                {
                    var rres = AlohaFly.DBProvider.Client.CreateDiscountRange(r);
                    r.Id = rres.CreatedObjectId;
                    AlohaFly.DBProvider.Client.AddRangeToDiscount(dres.CreatedObjectId, rres.CreatedObjectId);
                }
                catch { }
            }

            if (arc != null)
            {
                arc.DiscountId = dres.CreatedObjectId;
                var acres = AlohaFly.DBProvider.Client.UpdateAirCompany(arc);
            }
        }
    }
}
