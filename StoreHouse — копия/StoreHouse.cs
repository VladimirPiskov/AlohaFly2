using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;



namespace StoreHouseConnect
{
    public class TStoreHouse
    {
        const int _RC_NEAR = 0x00000000;
        const int _PC_53 = 0x00010000;
        const int _EM_INVALID = 0x00000010;
        const int _EM_UNDERFLOW = 0x00000002;
        const int _EM_ZERODIVIDE = 0x00000008;
        const int _EM_OVERFLOW = 0x00000004;
        const int _EM_INEXACT = 0x00000001;
        const int _EM_DENORMAL = 0x00080000;
        const int _CW_DEFAULT = _RC_NEAR + _PC_53 + _EM_INVALID + _EM_ZERODIVIDE + _EM_OVERFLOW + _EM_UNDERFLOW + _EM_INEXACT + _EM_DENORMAL;

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _controlfp(int newControl, int mask);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _fpreset();

        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string GetErrorMessage();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetErrorCode();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void Connect(string ServerName, int Port, string UserName, string Password);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GoodsTree();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string Goods(int Rid);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void AddItem(int ParentRid, string Name, string Code, int iCode, int ExtCode, int Cat1, int Cat2, int MunitRid, double Price);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void UpdateItem(int Rid, int ParentRid, string Name, string Code, int iCode, int ExtCode, int Cat1, int Cat2, int MunitRid, double Price);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetCategory1();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetCategory2();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetUnits();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void Disconect();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetExpCtgs();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetPlaceImpl();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void CreateExpenceDocument(string json_str);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void DeleteExpenceDocument(string json_str);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern bool CheckExpenceDocument(string json_str);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void CreateExpCtgs(string name);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetInvoice(string Start, string End);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetComplect(int Rid);
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern string GetGroupComplect();
        [DllImport("StoreHouse.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void InsertGoodsTree(int RID_Parent, string Code, string Name);


        //Подключение к StoreHouse
        public bool ConnectSH(string ServerName, int Port, string UserName, string Password, out int ErrCode, out string ErrorMessage)
        {
            Connect(ServerName, Port, UserName, Password);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();
            if (ErrCode != 0) return false;

            return true;
        }
        public void CloseConection()
        {
            Disconect();
            //_controlfp(_CW_DEFAULT, 0xfffff);
            _fpreset();
        }

        //Список всех товарных груп 
        public TGoodsTree GetGoodsTree(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GoodsTree();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();
            if (ErrCode != 0) { return null; }


            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TGoodsTree));
            Mstream.Position = 0;
            TGoodsTree goods_trre = (TGoodsTree)Serializer.ReadObject(Mstream);

            return goods_trre;
        }
        public string GetGoodsToJSONstr(int Rid, out int ErrCode, out string ErrorMessage)
        {
            string json_value = Goods(Rid);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            return json_value;

        }
        //Список товаров в группе, входной параметр уникальный инлефикатор (Rid) группы 
        public TGoodsList GetGoods(int Rid,out int ErrCode, out string ErrorMessage)
        {
            string json_value = Goods(Rid);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }

            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TGoodsList));
            Mstream.Position = 0;
            TGoodsList goods_list = (TGoodsList)Serializer.ReadObject(Mstream);

            return goods_list;

        }

        //Список ед.изм.
        public TListUnits Units(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetUnits();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }

            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListUnits));
            Mstream.Position = 0;
            TListUnits units_list = (TListUnits)Serializer.ReadObject(Mstream);

            return units_list;
        }
        //Категория товаров
        public TListCategory Categoty1(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetCategory1();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }

            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListCategory));
            Mstream.Position = 0;
            TListCategory category_list = (TListCategory)Serializer.ReadObject(Mstream);

            return category_list;
        }

        //Категория товаров бухгалтерская
        public TListCategory Categoty2(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetCategory2();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }

            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListCategory));
            Mstream.Position = 0;
            TListCategory category_list = (TListCategory)Serializer.ReadObject(Mstream);

            return category_list;
        }

        //Новый товар
        public bool AddGoods(int ParentRid, string Name, string Code, int iCode, int ExtCode, int Cat1, int Cat2, int MunitRid, double Price, out int ErrCode, out string ErrorMessage)
        {
            AddItem(ParentRid, Name, Code, iCode, ExtCode, Cat1, Cat2, MunitRid, Price);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();


            if (ErrCode != 0) { return false; }

            return true;
        }
        //Обновить товар
        public bool UpdateGoods(int Rid, int ParentRid, string Name, string Code, int iCode, int ExtCode, int Cat1, int Cat2, int MunitRid, double Price, out int ErrCode, out string ErrorMessage)
        {
            UpdateItem(Rid, ParentRid, Name, Code, iCode, ExtCode, Cat1, Cat2, MunitRid, Price);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();


            if (ErrCode != 0) { return false; }

            return true;
        }
        //Возвращает ошибку или "Ok" если все хорошо
        public string ErrMessage()
        {
            string err_mes = GetErrorMessage();
            _controlfp(_CW_DEFAULT, 0xfffff);
            return err_mes;
        }

        public TListExpCtgs ExpCtgs(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetExpCtgs();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }


            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListExpCtgs));
            Mstream.Position = 0;
            TListExpCtgs ExpCtgs_list = (TListExpCtgs)Serializer.ReadObject(Mstream);

            return ExpCtgs_list;
        }


        public TListPlaceImpl PlaceImpl(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetPlaceImpl();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }


            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListPlaceImpl));
            Mstream.Position = 0;
            TListPlaceImpl PlaceImpl_list = (TListPlaceImpl)Serializer.ReadObject(Mstream);

            return PlaceImpl_list;
        }

        public bool ExpenceDocumentCreate(TExpenceDocument expenceDocument, out int ErrCode, out string ErrorMessage)
        {
            MemoryStream Mstream = new MemoryStream();

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TExpenceDocument), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("dd.MM.yyyy HH:mm:ss")
            });

            Serializer.WriteObject(Mstream, expenceDocument);

            Mstream.Position = 0;
            StreamReader sr = new StreamReader(Mstream);

            string json_str = sr.ReadToEnd();

            CreateExpenceDocument(json_str);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();


            if (ErrCode != 0) { return false; }

            return true;
        }
        public bool ExpenceDocumentDelete(TExpenceDocument expenceDocument, out int ErrCode, out string ErrorMessage)
        {
            MemoryStream Mstream = new MemoryStream();

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TExpenceDocument), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("dd.MM.yyyy HH:mm:ss")
            });

            Serializer.WriteObject(Mstream, expenceDocument);

            Mstream.Position = 0;
            StreamReader sr = new StreamReader(Mstream);

            string json_str = sr.ReadToEnd();

            DeleteExpenceDocument(json_str);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();


            if (ErrCode != 0) { return false; }

            return true;
        }
        public bool ExpenceDocumentCheck(TExpenceDocument expenceDocument, out int ErrCode, out string ErrorMessage)
        {
            MemoryStream Mstream = new MemoryStream(); 

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TExpenceDocument), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("dd.MM.yyyy HH:mm:ss")
            });

            Serializer.WriteObject(Mstream, expenceDocument);

            Mstream.Position = 0;
            StreamReader sr = new StreamReader(Mstream);

            string json_str = sr.ReadToEnd();

            bool res = CheckExpenceDocument(json_str);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();
           
            return res;
        }
        public bool AddExpCtgs(string name, out int ErrCode, out string ErrorMessage)
        {
            CreateExpCtgs(name);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return false; }

            return true;
        }

        public TListInvoice GetInvoices(DateTime Start, DateTime End, out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetInvoice(Start.ToString(), End.ToString());
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }


            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListInvoice), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("dd.MM.yyyy HH:mm:ss")
            });
            Mstream.Position = 0;
            TListInvoice invoice_list = (TListInvoice)Serializer.ReadObject(Mstream);

            return invoice_list;
        }

        public TComplectList ComplectGet(int Rid, out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetComplect(Rid);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }


            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TComplectList));
            Mstream.Position = 0;
            TComplectList complect_list = (TComplectList)Serializer.ReadObject(Mstream);

            return complect_list;
        }

        public TListGroupComplect GetComplectGroup(out int ErrCode, out string ErrorMessage)
        {
            string json_value = GetGroupComplect();
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();

            if (ErrCode != 0) { return null; }

            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] bytes = encoding.GetBytes(json_value);

            MemoryStream Mstream = new MemoryStream(bytes);

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(TListGroupComplect));
            Mstream.Position = 0;
            TListGroupComplect complect_group_list = (TListGroupComplect)Serializer.ReadObject(Mstream);

            return complect_group_list;

        }

        public bool InsGoodTree(int RID_Parent, string Code, string Name, out int ErrCode, out string ErrorMessage)
        {
             InsertGoodsTree(RID_Parent, Code, Name);
            ErrCode = GetErrorCode();
            ErrorMessage = GetErrorMessage();
            _fpreset();


            if (ErrCode != 0) { return false; }

            return true;
        }


    }
}
