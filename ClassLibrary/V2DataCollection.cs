using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;

namespace ClassLibrary {
    [Serializable]
    public class V2DataCollection : V2Data, INotifyCollectionChanged {
        #region Attr
        public List<DataItem> Values_field { get; set; }
        private static readonly Random Rand = new Random(42);
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Methods
        public V2DataCollection(double freq_field, string description) : base(freq_field, description) {
            Values_field = new List<DataItem>();
        }

        public V2DataCollection(string filename) : base(0.0, "") {
            Values_field = new List<DataItem>();
            try {
                using StreamReader input = new StreamReader(filename, System.Text.Encoding.Default);
                string raw_line;
                var parser_base = input.ReadLine().Split(';');
                Freq_field = double.Parse(parser_base[0]);
                Description = parser_base[1];
                while ((raw_line = input.ReadLine()) != null) {
                    var parser = raw_line.Split(';').Select(col => col.TrimStart(' ').TrimEnd(' ')).Select(col => col.Split(' '))
                        .Select(elem => new DataItem(new Complex(double.Parse(elem[0]), double.Parse(elem[1])),
                                                     new Vector2(float.Parse(elem[2]), float.Parse(elem[3]))));
                    foreach (var ValueTuple in parser) {
                        Values_field.Add(ValueTuple);
                    }
                }

            } catch (Exception exp) {
                Console.WriteLine(exp.Message);
            }
        }

        public void Add(DataItem item) {
            Values_field.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Values_field));
        }

        public void InitRandom(int nItems,
                               float xmax,
                               float ymax,
                               double minValue,
                               double maxValue) {
            for (int i = 0; i < nItems; ++i) {
                DataItem val_field = new DataItem(new Complex(Rand.NextDouble() * (maxValue - minValue),
                                                              Rand.NextDouble() * (maxValue - minValue)),
                                                   new Vector2((float)(Rand.NextDouble() * xmax),
                                                               (float)(Rand.NextDouble() * ymax)));
                Values_field.Add(val_field);
            }
        }

        public override Complex[] NearAverage(float eps) {
            double real_mean_val = 0.0;
            foreach (DataItem val in Values_field) {
                real_mean_val += val.Value_field.Real;
            }
            real_mean_val /= Values_field.Count;

            return (from val in Values_field
                    where Math.Abs(val.Value_field.Real - real_mean_val) < eps
                    select val.Value_field).ToArray();
        }

        public override string ToString() => $"Type: V2DataCollection\n{base.ToString()}\nValues count: {Values_field.Count}\n";

        public override string ToLongString() {
            string str = ToString();
            foreach (DataItem val in Values_field) {
                str += val.ToString();
            }
            return str;
        }

        public override string ToLongString(string format) {
            string str = ToString();
            foreach (DataItem val in Values_field) {
                str += val.ToString(format);
            }
            return str;
        }

        public bool CheckNewData(Vector2 new_coord_field) => !Values_field.Exists(dataitem => dataitem.Coord_field == new_coord_field);
        #endregion

        public override IEnumerator<DataItem> GetEnumerator() {
            return Values_field.GetEnumerator();
        }
    }
}
