using AlohaFly.Utils;
using NLog;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AlohaFly.DataExtension
{
    public class PropertyHelper<T>
        where T : class, INotifyPropertyChanged, new()
    {
        string propName;
        object owner;
        Func<object, T> func;
        public PropertyHelper(object _owner, Func<object, T> _func)
        {
            func = _func;

            owner = _owner;
        }

        public void SetValue(T _value)
        {
            var prop = func(owner);
            if (prop != _value)
            {
                prop = _value;
                //owner. OnPropertyChanged(propName);
            }
        }
        T value;
        public T Value { get => value; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }


    public class FullyObservableDBDataSubsriber<TSource, TOut>
        where TSource : class, INotifyPropertyChanged, new()
        where TOut : class, INotifyPropertyChanged, new()
    {
        Func<TSource, long> sourceKeySelector = null;
        Func<TOut, long> keySelector = null;
        Func<TOut, long> orderKeySelector = null;
        Func<TOut, string> orderKeySelectorStr = null;
        Func<TSource, bool> selector = a => true;
        Action<TSource, TOut> transformer = null;
        Func<TSource, TOut> newFunc = null;
        Func<IEnumerable<TSource>, TOut> itemSelector = null;
        public FullyObservableDBDataSubsriber(Func<TOut, long> _keySelector)
        {
            keySelector = _keySelector;

        }

        private void SourceData_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //TSource item = sender as TSource;
            int idx = e.CollectionIndex;
            if (idx > -1)
            {
                TSource item = sourceData[idx];
                if (selector(item))
                {
                    if (outData != null)
                    {
                        if (outData.Any(a => keySelector(a) == sourceKeySelector(item)))
                        {
                            var itemOut = outData.FirstOrDefault(a => keySelector(a) == sourceKeySelector(item));
                            transformer?.Invoke(item, itemOut);

                        }
                        else  //Добавляем в выходную коллекцию, т.к. элемент стал удовлетворять селектору
                        {
                            if (newFunc != null)
                            {
                                outData.Add(newFunc(item));
                            }
                            else
                            {
                                if (transformer != null)
                                {
                                    TOut itmOut = new TOut();
                                    transformer(item, itmOut);

                                }
                                else
                                {
                                    outData.Add(item as TOut);
                                }
                            }
                        }
                    }
                    if (orderKeySelector != null && outData != null)
                    {
                        int chIdx = outData.IndexOf(e.ItemSender as TOut);
                        if (chIdx > 0)
                        {
                            int bIdx = Math.Max(0, chIdx - 1);
                            int aIdx = Math.Min(outData.Count-1, chIdx + 1);
                            if ((orderKeySelector(outData[chIdx]) < orderKeySelector(outData[bIdx])) || (orderKeySelector(outData[chIdx]) > orderKeySelector(outData[aIdx])))
                            {
                                outData.Sort(orderKeySelector);
                            }
                        }
                    }
                    if (orderKeySelectorStr != null && outData != null)
                    {
                        outData.Sort(orderKeySelectorStr);

                    }
                    if (itemSelector != null)
                    {

                        var selCol = sourceData.Where(a => selector(a)).ToList();
                        var selItem = itemSelector(selCol);
                        onChange(selItem);

                    }
                }
                else
                { //Удаляем из выходной коллекци, если он там был и перестал удовлетворять селектору
                    if (outData != null)
                    {
                        if (outData.Any(a => keySelector(a) == sourceKeySelector(item)))
                        {
                            var itemOut = outData.FirstOrDefault(a => keySelector(a) == sourceKeySelector(item));
                            outData.Remove(itemOut);
                        }
                    }
                }
            }
        }

        private void SourceData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (outData != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (TSource item in e.OldItems)
                    {
                        if (outData.Any(a => keySelector(a) == sourceKeySelector(item)))
                        {
                            outData.Remove(outData.FirstOrDefault(a => keySelector(a) == sourceKeySelector(item)));
                        }
                    }
                    if (itemSelector != null)
                    {

                        onChange(itemSelector(sourceData.Where(a => selector(a))));

                    }

                }

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (TSource item in e.NewItems)
                    {
                        if (!outData.Any(a => keySelector(a) == sourceKeySelector(item)))
                        {
                            if (selector(item))
                            {
                                if (newFunc != null)
                                {
                                    //outData.Add(newFunc(item));

                                    outData.AddWithSort(orderKeySelector, orderKeySelectorStr,newFunc(item));
                                }
                                else
                                {
                                    if (transformer != null)
                                    {
                                        TOut itmOut = new TOut();
                                        transformer(item, itmOut);

                                    }
                                    else
                                    {
                                        outData.AddWithSort(orderKeySelector, orderKeySelectorStr, item as TOut);
                                        //outData.Add(item as TOut);
                                    }
                                }
                            }
                        }
                    }
                    /*
                    if (orderKeySelector != null && outData != null)
                    {
                        outData.Sort(orderKeySelector);
                    }
                    if (orderKeySelectorStr != null && outData != null)
                    {
                        outData.Sort(orderKeySelectorStr);

                    }
                    */
                    if (itemSelector != null)
                    {
                        onChange(itemSelector(sourceData.Where(a => selector(a))));
                    }
                }

            }
        }


        public FullyObservableDBDataSubsriber<TSource, TOut> Transform(Action<TSource, TOut> action)
        {
            transformer = action;
            return this;
        }

        public FullyObservableDBDataSubsriber<TSource, TOut> Select(Func<TSource, bool> func)
        {
            selector = func;
            RefreshOutdata();
            return this;
        }

        public FullyObservableDBDataSubsriber<TSource, TOut> OrderBy(Func<TOut, long> _orderKeySelector)
        {
            orderKeySelector = _orderKeySelector;
            RefreshOutdata();
            return this;
        }
        public FullyObservableDBDataSubsriber<TSource, TOut> OrderBy(Func<TOut, DateTime> _orderKeySelector)
        {
            return OrderBy(new Func<TOut, long>((a) => _orderKeySelector(a).Ticks));
        }

        public FullyObservableDBDataSubsriber<TSource, TOut> OrderBy(Func<TOut, string> _orderKeySelector)
        {
            orderKeySelectorStr = _orderKeySelector;
            RefreshOutdata();
            return this;

        }


        public FullyObservableDBDataSubsriber<TSource, TOut> OrderByDesc(Func<TOut, DateTime> _orderKeySelector)
        {

            //orderKeySelector = ;
            return OrderBy(new Func<TOut, long>((a) => -_orderKeySelector(a).Ticks));
        }

        private FullyObservableCollection<TSource> sourceData;
        private FullyObservableCollection<TOut> outData;
        private Action<TOut> onChange;
        public FullyObservableDBDataSubsriber<TSource, TOut> SubsribeAction(FullyObservableDBData<TSource> source, Action<TOut> _onChange, Func<IEnumerable<TSource>, TOut> _selectFunc)
        {
            //newFunc = _newFunc;
            itemSelector = _selectFunc;
            onChange = _onChange;
            var selCol = sourceData.Where(a => selector(a)).ToList();
            var selItem = itemSelector(selCol);
            onChange(selItem);

            sourceData = source.Data;
            sourceKeySelector = source.KeySelector;
            sourceData.CollectionChanged += SourceData_CollectionChanged;
            sourceData.ItemPropertyChanged += SourceData_ItemPropertyChanged;
            return this;

        }

        private void RefreshOutdata()
        {
            if (sourceData == null) return;
            if (outData == null) return;
            outData.Clear();
            outData.SetEventsFreeze();
            foreach (var itm in sourceData)
            {

                if (selector(itm))
                {
                    if (newFunc != null)
                    {
                        outData.Add(newFunc(itm));
                    }
                    else
                    {

                        if (transformer == null)
                        {
                            outData.Add(itm as TOut);
                        }
                        else
                        {
                            var item = new TOut();
                            transformer(itm, item);
                            outData.Add(item);
                        }
                    }

                }
            }
            if (orderKeySelector != null && outData != null)
            {
                outData.Sort(orderKeySelector);
            }
            if (orderKeySelectorStr != null && outData != null)
            {
                outData.Sort(orderKeySelectorStr);
            }
            outData.UnSetEventsFreeze();
        }

        public FullyObservableDBDataSubsriber<TSource, TOut> Subsribe(FullyObservableDBData<TSource> source, FullyObservableCollection<TOut> subscriber, Func<TSource, TOut> _newFunc = null)
        {
            newFunc = _newFunc;
            outData = subscriber;
            sourceData = source.Data;
            sourceKeySelector = source.KeySelector;
            sourceData.CollectionChanged += SourceData_CollectionChanged;
            sourceData.ItemPropertyChanged += SourceData_ItemPropertyChanged;
            RefreshOutdata();
            return this;
        }



    }




    public class FullyObservableDBData<T>
        where T : class, INotifyPropertyChanged
    {

        public FullyObservableDBData()
        {

        }
        Logger logger = LogManager.GetCurrentClassLogger();

        public FullyObservableCollection<T> Data;
        public Func<T, long> KeySelector;
        private List<FullyObservableCollection<T>> subsribers = new List<FullyObservableCollection<T>>();
        //private BlockingCollection<FullyObservableCollection<T>> subsribers = new BlockingCollection<FullyObservableCollection<T>>();

        

        private Dictionary<long, T> data;//= new ConcurrentDictionary<long, T>();
        //private DataDBUpdaterFactory<T> updaterFactory;
        private LinkedData<T> linkedData;


        public bool exist = false;
        public bool Exist { get { return exist; } }

       // private List<long> changesIdsDuringFillUpdate = new List<long>();
        private bool fillUpdating = false;
        //private object fillUpdatingLock = new object();

        public DateTime? startDate;
        //private DateTime? endDate;

        public void Fill(Func<T, long> _keySelector, DateTime? _startDate = null)
        {
            try
            {
                KeySelector = _keySelector;
                startDate = _startDate;
                //endDate = DateTime.Now.AddMonths(1);
                var updaterFactory = new DataDBUpdaterFactory<T>(KeySelector);
                linkedData = updaterFactory.GetLinkedFullyObservableDBData();
            }
            catch 
            {
                return;
            }

            exist = true;
            if (data == null)
            {
                data = new Dictionary<long, T>();
                Data = new FullyObservableCollection<T>(data.Values);
                Subsribe(Data);
            }
            FillUpdate();
        }




        public void ChangeStartDate(DateTime dt)
        {
            DateTime dts = startDate == null ? dt : new DateTime(Math.Min(startDate.GetValueOrDefault().Ticks, dt.Ticks));
            if (dts != startDate)
            {
                DateTime? edt = startDate;
                startDate = dts;
                FillUpdate(edt);
            }

        }

        private void FillUpdate(DateTime? edt = null)
        {
            fillUpdating = true;
            var dbRes = linkedData.DBListFunc.Invoke(startDate, edt);

           // lock (fillUpdatingLock)
            {
                fillUpdating = false;
                if (dbRes.Success)
                {

                    //var res = dbRes.Result.Where(a => !changesIdsDuringFillUpdate.Contains(KeySelector(a))).ToList();// Убираем записи, которые обновились локально с начала апдейта
                    //changesIdsDuringFillUpdate.Clear();
                    
                    foreach (var item in dbRes.Result)
                    {
                        // var itemCh = linkedData.DBChildrenDataUpdater(item);
                        AddOrUpdateDataItem(item);
                    }
                }
            }

        }


        public void UpdateItems(List<T> items)
        {
            if (items == null) return;
            fillUpdating = true;
            //lock (fillUpdatingLock)
            {
                try
                {

                    fillUpdating = false;
                    //var res = items.Where(a => !changesIdsDuringFillUpdate.Contains(KeySelector(a))).ToList();// Убираем записи, которые обновились локально с начала апдейта
                    //changesIdsDuringFillUpdate.Clear();
                    foreach (var item in items)
                    {
                        //var itemCh = linkedData.DBChildrenDataUpdater(item);
                        //AddOrUpdateDataItem(itemCh);
                        AddOrUpdateDataItem(item);
                    }

                }
                catch
                { }
                }
            
        }


        object subsribersLocker = new object();
        private T AddOrUpdateDataItem(T item)
        {
            if (item == null) return null;
            T val=null;
          //  lock (locker)
            {

                MainClass.Dispatcher.Invoke(new Action(() =>
               {
               

                   if (data.TryGetValue(KeySelector(item), out val))
                   {
                       CopyAllPrimitiveProp(item, val);
                       val = linkedData.DBChildrenDataUpdater(val);

                      
                   }
                   else
                   {
                       //val = item;
                       item = linkedData.DBChildrenDataUpdater(item);
                       data.Add(KeySelector(item), item);
                       lock (subsribersLocker)
                       {
                           foreach (var subsriber in subsribers)
                           {
                               if (!subsriber.Any(a => KeySelector(a) == KeySelector(item)))
                               {
                                   subsriber.Add(item);
                               }
                           }
                       }
                       val = item;
                   }
               }));
                //   linkedData.SubClassesUpdater?.Invoke(item);


            }
            return val;
        }

        private void DeleteDataItem(T item)
        {
            if (item == null) return;
            
                data.Remove(KeySelector(item));
            lock (subsribersLocker)
            {
                foreach (var subsriber in subsribers)
                {
                    if (subsriber.Any(a => KeySelector(a) == KeySelector(item)))
                    {
                        var itm = subsriber.FirstOrDefault(a => KeySelector(a) == KeySelector(item));
                        subsriber.Remove(itm);
                    }
                }
                // linkedData.SubClassesDeleter?.Invoke(item);
            }
        }

        public static void SubscibeTransform<TOut>(ObservableCollection<TOut> subsriber, Func<T, TOut> transformer)
        {


        }

        public void Subsribe(FullyObservableCollection<T> subsriber)
        {
            if (subsriber == null) return;
            lock (subsribersLocker)
            {
                if (!subsribers.Contains(subsriber))
                {

                    subsribers.Add(subsriber);

                    foreach (var d in data.Values)
                    {
                        if (!subsriber.Any(a => KeySelector(a) == KeySelector(d)))
                        {
                            subsriber.Add(d);
                        }
                    }
                }
            }
        }
        public void Unsubsribe(FullyObservableCollection<T> subsriber)
        {
            if (subsriber == null) return;
            lock (subsribersLocker)
            {
                if (subsribers.Contains(subsriber))
                {
                    subsribers.Remove(subsriber);
                }
            }
        }

        //private long EditingItemId = 0;
        public FullyObservableDBDataUpdateResult<T> BeginEdit(long id)
        {

            var res = new FullyObservableDBDataUpdateResult<T>()
            {
                Succeess = true
            };
            return res;

        }
        
        public FullyObservableDBDataUpdateManyResult EndEditMany(List<T> deletedItems, List<T> addedorUpdatetdItems)
        {

            var res = new FullyObservableDBDataUpdateManyResult() { Succeess = true };
            try
            {

                if (addedorUpdatetdItems != null)
                {
                    foreach (var item in addedorUpdatetdItems)
                    {
                        var innerRes = EndEdit(item);
                        res.Succeess = res.Succeess & innerRes.Succeess;
                        res.ErrorMessage = res.ErrorMessage + innerRes.ErrorMessage;
                    }
                }
                if (addedorUpdatetdItems != null)
                {
                    foreach (var item in deletedItems)
                    {
                        var innerRes = DeleteItem(item);
                        res.Succeess = res.Succeess & innerRes.Succeess;
                        res.ErrorMessage = res.ErrorMessage + innerRes.ErrorMessage;
                    }
                }
            }
            catch(Exception e)
            {
                logger.Debug($"EndEditMany Error {e.Message}");
            }
            return res;
        }

        /*
        public FullyObservableDBDataUpdateResult<T> DeleteItem(Func<long,T> selector,)
        {

        }
        */
            public FullyObservableDBDataUpdateResult<T> DeleteItem(T item)
        {
           // lock (fillUpdatingLock)
            {
                /*
                if (fillUpdating)
                {
                    changesIdsDuringFillUpdate.Add(KeySelector(item));
                }
                */
                var res = new FullyObservableDBDataUpdateResult<T>();

                var dbUpdaterResult = linkedData.DBDeleter.Invoke(item);
                res.Succeess = dbUpdaterResult.Succeess;
                if (dbUpdaterResult.Succeess)
                {
                    DeleteDataItem(item);
                }
                else
                {
                    res.ErrorMessage = $"Ошибка при сохранении изменений в базе данных. {Environment.NewLine + dbUpdaterResult.ErrorMessage}";
                }
                return res;
            }
        }

        public FullyObservableDBDataUpdateResult<T> EndEdit(T item)
        {
            //lock (fillUpdatingLock)
            {
                var res = new FullyObservableDBDataUpdateResult<T>() { Succeess = false };
                try
                {
                    /*
                    if (fillUpdating)
                    {
                        changesIdsDuringFillUpdate.Add(KeySelector(item));
                    }
                    */
                    var dbUpdaterResult = linkedData.DBUpdater.Invoke(item);
                    res.Succeess = dbUpdaterResult.Succeess;
                    if (dbUpdaterResult.Succeess)
                    {
                        res.UpdatedItem = AddOrUpdateDataItem(dbUpdaterResult.UpdatedItem);
                    }
                    else
                    {
                        res.ErrorMessage = $"Ошибка при сохранении изменений в базе данных. {Environment.NewLine + dbUpdaterResult.ErrorMessage}";
                    }
                    return res;
                }
                catch (Exception e)
                {
                    logger.Debug($"EndEdit Error {e.Message}");      
                    return res;
                }
            }
        }




        public void CancelEdit(long Id)
        {

        }


        /*
        public void AddOrUpdate(T item)
        {
            
        }
        */




        private void CopyAllPrimitiveProp(T sourse, T destination, List<string> Except = null)
        {
            foreach (PropertyInfo PI in sourse.GetType().GetProperties())
            {
                if (Except != null && Except.Contains(PI.Name))
                {
                    continue;
                }
                /*
                if (PI.PropertyType.IsNonStringEnumerable())
                {
                    continue;
                }
                */
                //   if (PI.PropertyType.IsPrimitive || PI.PropertyType == typeof(Decimal) || PI.PropertyType == typeof(string))
                {
                    try
                    {
                        object val = sourse.GetType().GetProperty(PI.Name).GetValue(sourse);
                        destination.GetType().GetProperty(PI.Name).SetValue(destination, val);
                    }
                    catch
                    { }
                }


            }
        }

    }


    public class FullyObservableDBDataUpdateManyResult
    {
        public bool Succeess { set; get; }
        public string ErrorMessage { set; get; }
    }
    public class FullyObservableDBDataUpdateResult<T>
    {
        public FullyObservableDBDataUpdateResult()
        {

        }
        public bool Succeess { set; get; }
        public string ErrorMessage { set; get; }
        public T UpdatedItem { set; get; }
    }

    public static class FullyObservableDBDataExt
    {
        public static bool IsNonStringEnumerable(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}
