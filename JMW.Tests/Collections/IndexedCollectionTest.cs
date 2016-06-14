﻿using JMW.Types.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace JMW.Types.Functional.Tests
{
    [TestFixture]
    public class IndexedCollectionTest
    {
        [Test]
        public void Test1()
        {
            var idx = new IndexedCollection<Foo>();
            var f1 = new Foo() { Bar = "jason", BarCollection = new List<string>() { "one", "two" }, Baz = "me", BazCollection = new List<int>() { 1, 2 } };
            var f2 = new Foo() { Bar = "wall", BarCollection = new List<string>() { "three", "two" }, Baz = "me", BazCollection = new List<int>() { 3, 4 } };
            idx.Add(f1);
            idx.Add(f2);

            Assert.That(idx[0].Bar == "jason");
            Assert.That(idx[1].Bar == "wall");

            idx[0].Bar = "jeremy";

            Assert.That(idx[0].Bar == "jeremy");
            idx[0].Bar = "jeremy";
            Assert.That(idx[0].Bar == "jeremy");

            idx[0] = f1;

            var arr = new Foo[10];
            idx.CopyTo(arr, 1);
            Assert.That(arr[1] == idx[0]);
            Assert.That(arr[2] == idx[1]);

            Assert.That(idx.Contains(f1));

            f1.Baz = "my";
            f1.BazCollection = new List<int>() { 1, 2, 5 };
            f1.BarCollection = new List<string>() { "1", "2" };
            f1.BarCollection = new List<string>() { "one", "two" };

            Assert.That(idx.Count == 2);

            Assert.That(idx.Equals(idx));

            Assert.That(idx.First() == f1);
            Assert.That(idx.IndexOf(f2) == 1);

            var indices = idx.GetUniqueIndexCollection(p => p.Bar);
            var indices2 = idx.GetIndexCollection(p => p.BarCollection);

            indices.Do(if_success: d => Assert.That(d.Count == 2), if_exception: null);
            indices2.Do(if_success: d => Assert.That(d.Count == 3), if_exception: null);

            // make sure the indices can be modified without modifying the original.
            indices.Do(if_success: d => d.Remove("jeremy"), if_exception: null);
            idx.GetUniqueIndexCollection(p => p.Bar).Do(if_success: d => Assert.That(d.Count == 2), if_exception: null);

            Assert.That(idx.First() == f1);
            Assert.That(idx.First(d => d.Bar == "jeremy") == f1);
            idx.GetByUniqueIndex(p => p.Bar, "jeremy").Do(if_success: d=> Assert.That(d== f1),if_exception: null);
            idx.GetByIndex(p => p.BarCollection, "two").Do(
                if_success: d =>
                Assert.That(d.Count == 2), 
                if_exception: null);

            idx.GetByUniqueIndex(p => p.BarCollection, "jeremy").Do(if_success: null, if_exception: d=>Assert.That(d.GetType() == typeof(ArgumentException)));
            idx.GetByIndex(p => p.Bar, "two").Do(if_success: null, if_exception: d => Assert.That(d.GetType() == typeof(ArgumentException)));


            var en = idx.GetEnumerator();
            while (en.MoveNext())
            {
                Assert.That(typeof(Foo) == en.Current.GetType());
            }
            
            idx.Remove(f1);
            idx.GetUniqueIndexCollection(p => p.Bar).Do(if_success: d => Assert.That(d.Count == 1), if_exception: null);
            idx.GetIndexCollection(p => p.BarCollection).Do(if_success: d => Assert.That(d.Count == 2), if_exception: null);
            idx.Add(f1);

            idx.Remove(p => p.Bar == "jeremy");
            idx.GetUniqueIndexCollection(p => p.Bar).Do(if_success: d => Assert.That(d.Count == 1), if_exception: null);
            idx.GetIndexCollection(p => p.BarCollection).Do(if_success: d => Assert.That(d.Count == 2), if_exception: null);
            idx.Add(f1);

            idx.RemoveAt(0);
            idx.GetUniqueIndexCollection(p => p.Bar).Do(if_success: d => Assert.That(d.Count == 1), if_exception: null);
            idx.GetIndexCollection(p => p.BarCollection).Do(if_success: d => Assert.That(d.Count == 2), if_exception: null);
            idx.Insert(0, f1);

            try
            {
                idx.Add(f1);
            }
            catch (Exception ex)
            {
                Assert.That(ex.GetType() == typeof(OperationCanceledException));
            }
            idx.RefreshIndices();
            Assert.That(idx.TryAdd(f1) == false);
            idx.RefreshIndices();
            try
            {
                f1.BazCollection = new List<int>() { 1, 2, 5, 5 };
            }
            catch (Exception ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
            try
            {
                var f3 = new Foo() { Bar = "wall", BarCollection = new List<string>() { "three", "two" }, Baz = "me", BazCollection = new List<int>() { 6, 7, 7 } };
                idx.Add(f3);
            }
            catch
            (Exception ex)
            {
                Assert.That(ex.GetType() == typeof(OperationCanceledException));
            }
            f1.BarCollection = new List<string>() { "1", "2", "3" };

            f1.PropertyChanged += F1_PropertyChanged;
            f1.PropertyChanging += F1_PropertyChanging;
            f1.Bar = "blah";
            f1.PropertyChanged -= F1_PropertyChanged;
            f1.PropertyChanging -= F1_PropertyChanging;

            idx.Clear();
            Assert.That(idx.Count == 0);
            idx.GetUniqueIndexCollection(p => p.Bar).Do(if_success: d => Assert.That(d.Count == 0), if_exception: null);

            idx.CollectionChanged += Idx_CollectionChanged;
            idx.Add(f1);
            idx.CollectionChanged -= Idx_CollectionChanged;
            Assert.That(idx.IsReadOnly == false);

            idx.ForEach(item => Assert.That(item.GetType() == typeof(Foo)));

            f1.PropertyChanged += F1_PropertyChanged;
            f1.PropertyChanging += F1_PropertyChanging;
            f1.IndexedPropertyChanged += F1_IndexedPropertyChanged;
            f1.ClearEvents();
            Assert.That(f1.IndexedPropertyChangedEventCount == 0);

            f1.PropertyChanged += F1_PropertyChanged;
            f1.ClearPropertyChanged();
            Assert.That(f1.PropertyChangedEventCount == 0);

            f1.PropertyChanging += F1_PropertyChanging;
            f1.ClearPropertyChanging();
            Assert.That(f1.PropertyChangingEventCount == 0);

            f1.IndexedPropertyChanged += F1_IndexedPropertyChanged;
            f1.ClearIndexedPropertyChanged();
            Assert.That(f1.IndexedPropertyChangedEventCount == 0);
        }

        private void F1_IndexedPropertyChanged(object sender, IndexedPropertyChangedEventArgs e)
        {
            
        }

        private void Idx_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Assert.That(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add);
        }

        private void F1_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            Assert.That(e.PropertyName == "Bar");
        }

        private void F1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Assert.That(e.PropertyName == "Bar");
        }

        private class Foo : IndexedClass
        {
            private string _Bar = "";
            [Indexed(IsUnique = true)]
            public string Bar
            {
                get
                {
                    return _Bar;
                }

                set
                {
                    Set(ref _Bar, value);
                }
            }

            private List<string> _BarCollection = new List<string>();
            [Indexed]
            public List<string> BarCollection
            {
                get
                {
                    return _BarCollection;
                }

                set
                {
                    Set(ref _BarCollection, value);
                }
            }

            private string _Baz = "";
            [Indexed]
            public string Baz
            {
                get
                {
                    return _Baz;
                }

                set
                {
                    Set(ref _Baz, value);
                }
            }

            private List<int> _BazCollection = new List<int>();
            [Indexed(IsUnique = true)]
            public List<int> BazCollection
            {
                get
                {
                    return _BazCollection;
                }

                set
                {
                    Set(ref _BazCollection, value);
                }
            }
        }
    }
}