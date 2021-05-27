using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClassLibrary {
    [Serializable]
    public class V2DataOnGrid : V2Data {
        public Grid1D[] Param_grid;
        public Complex[,] Values_field;
        private static readonly Random Rand = new Random(42);

        public V2DataOnGrid(double freq_field,
                            string description,
                            double[] step_grid,
                            int[] num_nodes_grid) : base(freq_field, description) {
            Param_grid = new Grid1D[] {
                new Grid1D(step_grid[0], num_nodes_grid[0]),
                new Grid1D(step_grid[1], num_nodes_grid[1])
            };
            Values_field = new Complex[num_nodes_grid[0], num_nodes_grid[1]];
        }

        public static implicit operator V2DataCollection(V2DataOnGrid v2_data_on_grid) {
            V2DataCollection v2_data_collection = new V2DataCollection(v2_data_on_grid.Freq_field,
                                                                       v2_data_on_grid.Description);

            for (int i = 0; i < v2_data_on_grid.Param_grid[0].Num_nodes_grid; ++i) {
                for (int j = 0; j < v2_data_on_grid.Param_grid[1].Num_nodes_grid; ++j) {
                    DataItem val_field = new DataItem(v2_data_on_grid.Values_field[i, j],
                                                      new Vector2((float)(v2_data_on_grid.Param_grid[0].Step_grid * i),
                                                                  (float)(v2_data_on_grid.Param_grid[1].Step_grid * j)));

                    v2_data_collection.Values_field.Add(val_field);
                }
            }

            return v2_data_collection;
        }

        public (Complex, Complex) MinMaxValues {
            get
            {
                Complex min = Values_field[0, 0];
                Complex max = Values_field[0, 0];

                for (int i = 0; i < Param_grid[0].Num_nodes_grid; ++i) {
                    for (int j = 0; j < Param_grid[1].Num_nodes_grid; ++j) {
                        if (Complex.Abs(min) > Complex.Abs(Values_field[i, j])) {
                            min = Values_field[i, j];
                        }

                        if (Complex.Abs(max) < Complex.Abs(Values_field[i, j])) {
                            max = Values_field[i, j];
                        }
                    }
                }

                return (min, max);
            }
        }

        public void InitRandom(double minValue, double maxValue) {
            Values_field = new Complex[Param_grid[0].Num_nodes_grid, Param_grid[1].Num_nodes_grid];
            for (int i = 0; i < Param_grid[0].Num_nodes_grid; ++i) {
                for (int j = 0; j < Param_grid[1].Num_nodes_grid; ++j) {
                    Values_field[i, j] = new Complex(Rand.NextDouble() * (maxValue - minValue),
                                                     Rand.NextDouble() * (maxValue - minValue));
                }
            }
        }

        public override Complex[] NearAverage(float eps) {
            double real_mean_val = 0.0;
            foreach (Complex val in Values_field) {
                real_mean_val += val.Real;
            }
            real_mean_val /= Values_field.Length;

            return (from Complex val in Values_field
                    where Math.Abs(val.Real - real_mean_val) < eps
                    select val).ToArray();
        }

        public override string ToString() => $"Type: V2DataOnGrid\n{base.ToString()}\nParams grid:\n{Param_grid[0]}{Param_grid[1]}\n";

        public override string ToLongString() {
            string str = ToString();
            for (int i = 0; i < Param_grid[0].Num_nodes_grid; ++i) {
                for (int j = 0; j < Param_grid[1].Num_nodes_grid; ++j) {
                    str += ($"Value field: {Values_field[i, j]} X: {Param_grid[0].Step_grid * i} Y: {Param_grid[1].Step_grid * j}\n");
                }
            }
            return str;
        }

        public override string ToLongString(string format) {
            string str = ToString();
            for (int i = 0; i < Param_grid[0].Num_nodes_grid; ++i) {
                for (int j = 0; j < Param_grid[1].Num_nodes_grid; ++j) {
                    str += ($"Value field: {Values_field[i, j].ToString(format)} X: {String.Format(format, Param_grid[0].Step_grid * i)} " +
                        $"Y: {String.Format(format, Param_grid[0].Step_grid * j)}\n");
                }
            }
            return str;
        }

        public override IEnumerator<DataItem> GetEnumerator() {
            for (int i = 0; i < Param_grid[0].Num_nodes_grid; ++i) {
                for (int j = 0; j < Param_grid[1].Num_nodes_grid; ++j) {
                    yield return new DataItem(Values_field[i, j], new Vector2((float)Param_grid[0].Step_grid * i, (float)Param_grid[1].Step_grid * j));
                }
            }
        }
    }
}
