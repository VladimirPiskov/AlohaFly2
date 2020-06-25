Класс для работы со СторХаусом "TStoreHouse" имеет следующие методы:
1 - ConnectSH служит для подключения к СторХаусу, если подключение прошло удачно вернет true если нет то смотрим ошибку методом ErrMessage().
2 - GetGoodsTree() получает список товарных групп, возвращает обект типа TGoodsTree, если вернет NULL то смотрим ошибку методом ErrMessage().
3 - GetGoods(int Rid) получает список товаров (0 - товар, 1 - услуга, 2 - ссылка) в группе, входной параметр Rid уникальный идентификатор группы, если вернет NULL то смотрим ошибку методом ErrMessage().
4 - ErrMessage() возвращает текст ошибки или "Ok" если ошибок нет
5 - Units() возвращает список ед.измерения TListUnits, если вернет NULL то смотрим ошибку методом ErrMessage().
6 - Categoty1() возвращает категорию для товаров TListCategory, если вернет NULL то смотрим ошибку методом ErrMessage().
7 - Categoty2() возвращает бухгалтерскую категорию для товаров TListCategory, если вернет NULL то смотрим ошибку методом ErrMessage().
8 - AddGoods(int ParentRid, string Name, string Code, int iCode, int ExtCode, int Cat1, int Cat2, int MunitRid, double Price) добавление нового товара, результат добавления смотрим методом ErrMessage().
    ParentRid - номер товарной группы, посмотреть можно методом GetGoodsTree()
	Name - наименование товара
	Code - текстовая часть кода
	iCode - числовая часть кода
	ExtCode - внешний код
	Cat1 - категория для товара, выбрать нужную можно с помощью метода Categoty1()
	Cat2 - бухгалтерская категория для товара, выбрать нужную можно с помощью метода Categoty2()
	MunitRid - id ед.измерения, посмотреть можно методом Units()
	Price - цена товара
9 - CloseConection() - отключиться от сервера
10 - ExpenceDocumentCreate - создание документа расхода входной параметр объект типа TExpenceDocument
     TExpenceDocument.Prefix - префик документа, может быть пустым
	 TExpenceDocument.DocNum - номер документа
	 TExpenceDocument.Date - дата документа
	 TExpenceDocument.RidPlace - id места реализации, подобрать нужный параметр можно с помощью PlaceImpl()
	 TExpenceDocument.CatExpence - id категории расхода, подобрать нужный параметр можно с помощью ExpCtgs() 
	 TExpenceDocument.Coment - коментарий может быть пустым
	 TExpenceDocument.Type - тип учета 0-учет, 1-спец.учет
	 List<TItemDocument> ListItemDocument - список товаров
     TItemDocument.Rid - id товара, подобрать нужный параметр можно с помощью GetGoods()
	 TItemDocument.Quantity - количество

11 - ExpenceDocumentDelete - удалить документ расхода, входной параметр объект типа TExpenceDocument
     обязательные поля:
	 TExpenceDocument.Prefix - префик документа, может быть пустым
	 TExpenceDocument.DocNum - номер документа
	 TExpenceDocument.Date - дата документа

12 - AddExpCtgs(string name) - добавить новую категорию расхода, добавлена будет в раздел прочее. 
     name - название категории
13 - GetInvoices(DateTime Start, DateTime End) - получить приходные накладные
14 - ComplectGet(int Rid) -получить комплекты из группы комплектов с Rid, Rid группы комплектов можно узнать из метода GetComplectGroup
15 - InsGoodTree(int RID_Parent, string Code, string Name, out string ErrorMessage) создать группу для товаров
     RID_Parent - RID группы родителя, если RID_Parent = 0 группа создасться в корне. Code - код группы.  Name - наименование    
16 - UpdateGoods(int Rid, int ParentRid, string Name, string Code, int iCode, int ExtCode, int Cat1, int Cat2, int MunitRid, double Price) обновляет товар, результат добавления смотрим методом ErrMessage().
		Rid - уникальный идентификатор
		ParentRid - номер товарной группы, посмотреть можно методом GetGoodsTree()
		Name - наименование товара
		Code - текстовая часть кода
		iCode - числовая часть кода
		ExtCode - внешний код
		Cat1 - категория для товара, выбрать нужную можно с помощью метода Categoty1()
		Cat2 - бухгалтерская категория для товара, выбрать нужную можно с помощью метода Categoty2()
		MunitRid - id ед.измерения, посмотреть можно методом Units()
		Price - цена товара 	
17 - ExpenceDocumentCheck проверка наличия документа расхода, входной параметр объект типа TExpenceDocument
     обязательные поля:
	 TExpenceDocument.Prefix - префик документа, может быть пустым
	 TExpenceDocument.DocNum - номер документа
	 TExpenceDocument.Date - дата документа
	 TExpenceDocument.Type - тип учета 0-учет, 1-спец.учет
Дополнительные условия для работы библиотеки:
1 - необходимо зарегистрировать библиотеку  СторХауса Sh4Ole.dll
2 - наличие библиотеки StoreHouse.dll рядом с исполняемым файлом