﻿using AlohaService.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Utils
{

    public class QueryableEditableCollectionView<T> : QueryableCollectionView
        where T : IPrimaryUnik, IFocusable, new()


    {
        public QueryableEditableCollectionView(IEnumerable<T> source) : base(source)
        {

        }

        public ICommand AddItemCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {

                    var ph = new T();
                    InternalList.Insert(0, ph);
                    MoveCurrentToFirst();

                    ph.IsFocused = true;
                    ph.IsPrimary = true;
                }
            );
            }
        }


        public ICommand RemoveItemCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {
                    bool isPr = ((T)CurrentEditItem).IsPrimary;
                  
                    Remove(CurrentEditItem);
                   
                    if (isPr) ((T)CurrentEditItem).IsPrimary = true;

                   


                });
            }
        }
    }

    public class FullyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property is changed within an item.
        /// </summary>
        public event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged;

        public FullyObservableCollection() : base()
        { }

        public FullyObservableCollection(List<T> list) : base(list)
        {
            ObserveAll();
        }

        public FullyObservableCollection(IEnumerable<T> enumerable) : base(enumerable)
        {
            ObserveAll();
        }

        public Type GetElementType()
        {

            return typeof(T);
        }

        private bool eventsFreeze = false;

        public void SetEventsFreeze()
        {
            eventsFreeze = true;
        }
        public void UnSetEventsFreeze()
        {
            eventsFreeze = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }



        public void AddWithSort(Func<T, long> orderFunc, Func<T, string> orderFuncStr, T elem)
        {
            this.Add(elem);
            if (((orderFunc == null) && (orderFuncStr == null))  || (orderFuncStr!=null && orderFuncStr(elem)==null)) return;
            for (int i = 0; i < this.Count - 1; i++)
            {
                if (orderFunc != null)
                {
                    if (orderFunc(this[i]) >= orderFunc(elem))
                    {
                        this.Move(this.IndexOf(elem), i);
                        break;

                    }
                }
                else
                {
                    if (orderFuncStr(this[i]).CompareTo(orderFuncStr(elem)) > 0)
                    {
                        this.Move(this.IndexOf(elem), i);
                        break;
                    }
                }
            }
        }
        public void AddWithSort(Func<T, string> orderFunc, T elem)
        {
            this.Add(elem);
            if (orderFunc == null) return;
            for (int i = 0; i < this.Count - 1; i++)
            {
                if (orderFunc(this[i]).CompareTo(orderFunc(elem))>0)
                {
                    this.Move(this.IndexOf(elem), i);
                    break;
                }
            }
        }

        //public void Sort(Comparison<T> comparison)
        public void Sort(Func<T, long> orderFunc)
        {
            int comparison(T a, T b) => orderFunc(a).CompareTo(orderFunc(b));


            var sortableList = new List<T>(this);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                this.Move(this.IndexOf(sortableList[i]), i);
            }
        }

        public void Sort(Func<T, string> orderFunc)
        {
            


            var sortableList = new List<T>(this);
            sortableList.Sort(
                (a, b) => 
                {
                    if (orderFunc(a) == null)
                    {
                        if (orderFunc(b) == null)
                        {
                            // If x is null and y is null, they're
                            // equal. 
                            return 0;
                        }
                        else
                        {
                            // If x is null and y is not null, y
                            // is greater. 
                            return -1;
                        }
                    }
                    else
                    {
                        // If x is not null...
                        //
                        if (orderFunc(b) == null)
                        // ...and y is null, x is greater.
                        {
                            return 1;
                        }
                        else
                        {
                            return orderFunc(a).CompareTo(orderFunc(b));
                        }
                    }
                }
                );

            for (int i = 0; i < sortableList.Count; i++)
            {
                this.Move(this.IndexOf(sortableList[i]), i);
            }
        }



        
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {


            if (e.Action == NotifyCollectionChangedAction.Remove
            //||                e.Action == NotifyCollectionChangedAction.Replace
            )
            {
                foreach (T item in e.OldItems)
                    item.PropertyChanged -= ChildPropertyChanged;
            }

            if (e.Action == NotifyCollectionChangedAction.Add
                //||                e.Action == NotifyCollectionChangedAction.Replace
                )
            {
                foreach (T item in e.NewItems)
                    item.PropertyChanged += ChildPropertyChanged;
            }

            if (!eventsFreeze)
            {
                base.OnCollectionChanged(e);
            }

        }

        //  public void 

        protected void OnItemPropertyChanged(ItemPropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(this, e);
        }

        protected void OnItemPropertyChanged(int index, object itemSender, PropertyChangedEventArgs e)
        {
            OnItemPropertyChanged(new ItemPropertyChangedEventArgs(index, itemSender, e));
        }

        protected override void ClearItems()
        {
            foreach (T item in Items)
                item.PropertyChanged -= ChildPropertyChanged;

            base.ClearItems();
        }

        private void ObserveAll()
        {
            foreach (T item in Items)
                item.PropertyChanged += ChildPropertyChanged;
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            T typedSender = (T)sender;
            int i = Items.IndexOf(typedSender);
            
            /*
            if (i < 0)
                throw new ArgumentException("Received property notification from item not in collection");
                */
            OnItemPropertyChanged(i, sender, e);
        }
    }

    /// <summary>
    /// Provides data for the <see cref="FullyObservableCollection{T}.ItemPropertyChanged"/> event.
    /// </summary>
    public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Gets the index in the collection for which the property change has occurred.
        /// </summary>
        /// <value>
        /// Index in parent collection.
        /// </value>
        public int CollectionIndex { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="index">The index in the collection of changed item.</param>
        /// <param name="name">The name of the property that changed.</param>
        public ItemPropertyChangedEventArgs(int index, object itemSender, string name) : base(name)
        {
            CollectionIndex = index;
            ItemSender = itemSender;
        }

        public object ItemSender { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        public ItemPropertyChangedEventArgs(int index, object itemSender, PropertyChangedEventArgs args) : this(index, itemSender, args.PropertyName)
        { }
    }
}
