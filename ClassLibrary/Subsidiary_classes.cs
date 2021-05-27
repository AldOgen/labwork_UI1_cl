using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;

namespace ClassLibrary {
    [Serializable]
    public struct DataItem : IComparable<DataItem>, ISerializable {
        public Complex Value_field { get; set; }
        public Vector2 Coord_field { get; set; }

        public DataItem(Complex value_field, Vector2 coord_field) {
            Value_field = value_field;
            Coord_field = coord_field;
        }

        public DataItem(SerializationInfo info, StreamingContext context) {
            Value_field = (Complex)info.GetValue("Value_field", typeof(Complex));
            float x = info.GetSingle("Coord_field_X");
            float y = info.GetSingle("Coord_field_Y");
            Coord_field = new Vector2(x, y);
        }

        public override string ToString() => $"Value of the electromagnetic field: {Value_field}\n" +
            $"Point coordinates: {Coord_field}\n";

        public string ToString(string format) => $"Value of the electromagnetic field: {Value_field.ToString(format)}\n" +
            $"Point coordinates: {Coord_field.ToString(format)}\n";

        public int CompareTo(DataItem other) {
            if (Coord_field.X == other.Coord_field.X) {
                return Math.Sign(Coord_field.Y - other.Coord_field.Y);
            } else {
                return Math.Sign(Coord_field.X - other.Coord_field.X);
            }

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Value_field", Value_field);
            info.AddValue("Coord_field_X", Coord_field.X);
            info.AddValue("Coord_field_Y", Coord_field.Y);
        }


    }


    [Serializable]
    public struct Grid1D {
        public double Step_grid { get; set; }
        public int Num_nodes_grid { get; set; }

        public Grid1D(double step_grid, int num_nodes_grid) {
            Step_grid = step_grid;
            Num_nodes_grid = num_nodes_grid;
        }

        public override string ToString() => $"Grid step: {Step_grid}\nNumber of grid nodes: {Num_nodes_grid}\n";

        public string ToString(string format) => $"Grid step: {String.Format(format, Step_grid)}\nNumber of grid nodes: {Num_nodes_grid}\n";
    }


    [Serializable]
    public abstract class V2Data : IEnumerable<DataItem> {
        public double Freq_field { get; set; }
        public string Description { get; set; }

        public V2Data(double freq_field, string description) {
            Freq_field = freq_field;
            Description = description;
        }

        public abstract Complex[] NearAverage(float eps);

        public abstract string ToLongString();

        public abstract string ToLongString(string format);

        public override string ToString() => $"Frequency field: {Freq_field}\n{Description}\n";
        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
