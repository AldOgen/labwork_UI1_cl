using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClassLibrary {
    public class V2MainCollection : IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged {
        #region Attr
        List<V2Data> ListData = new List<V2Data>();

        public int Count { get; set; } = 0;

        public bool IsSave = true;

        public double Averege => ListData.Count() > 0 ? (from data in ListData
                                                         from val in data
                                                         select Complex.Abs(val.Value_field)).Average() : 0.0;


        public IEnumerable<DataItem> MaxDeviation => (from data in ListData
                                                      from val in data
                                                      orderby Math.Abs(Complex.Abs(val.Value_field) - Averege)
                                                      group val by val.Value_field into gr
                                                      select gr).Last();

        public IEnumerable<Vector2> DuplicateMeasurement => from data in ListData
                                                            from val in data
                                                            orderby val
                                                            group val.Coord_field by val.Coord_field into gr
                                                            where gr.Count() > 1
                                                            select gr.First();
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Methods
        public void Add(V2Data item) {
            ListData.Add(item);
            ++Count;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Averege"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ListData));
        }

        public bool Remove(string id, double w) {
            int removedCount = ListData.RemoveAll(elem => elem.Description == id &&
                                                  elem.Freq_field == w);

            Count -= removedCount;

            if (removedCount > 0) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Averege"));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            return removedCount > 0;
        }

        public void AddDefaults() {
            for (int i = 0; i < 2; ++i) {
                V2DataOnGrid v2_data_on_grid = new V2DataOnGrid(0.0, "Default info", new double[] { 0.01, 0.01 }, new int[] { 3, 3 });
                v2_data_on_grid.InitRandom(-10.0f, 10.0f);
                ListData.Add(v2_data_on_grid);
                V2DataCollection v2_data_collection = new V2DataCollection(0.0, "Default info");
                v2_data_collection.InitRandom(3, 10.0f, 10.0f, -10.0f, 10.0f);
                ListData.Add(v2_data_collection);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Averege"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ListData));
        }

        public override string ToString() {
            string str = "";

            foreach (V2Data dataElem in ListData) {
                str += $"{dataElem}\n\n";
            }

            return str;
        }

        public void Save(string filename) {
            FileStream fileStream = null;
            try {
                fileStream = File.Create(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, ListData);
            } catch (Exception ex) {
                Console.WriteLine($"{GetType()}.Save(): {ex.Message}");
            } finally {
                fileStream?.Close();
            }
        }

        public void Load(string filename) {
            FileStream fileStream = null;
            try {
                fileStream = File.OpenRead(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                ListData = binaryFormatter.Deserialize(fileStream) as List<V2Data>;
            } catch (Exception ex) {
                Console.WriteLine($"{GetType()}.Load(): {ex.Message}");
            } finally {
                fileStream?.Close();
            }
        }

        public void UpdateData() {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Averege"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)ListData).GetEnumerator();
        }
    }
}
