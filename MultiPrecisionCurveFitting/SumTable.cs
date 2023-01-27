using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    internal class SumTable<N> where N : struct, IConstant {
        private readonly List<Vector<N>> xs = new(), ys = new();
        private readonly Dictionary<(int xn, int yn), MultiPrecision<N>> table;

        private Vector<N>? w = null;

        public SumTable(Vector<N> x, Vector<N> y) {
            if (x.Dim != y.Dim) {
                throw new ArgumentException("Illegal length.", $"{nameof(x)},{nameof(y)}");
            }

            this.xs.Add(x);
            this.ys.Add(y);
            this.table = new() {
                { (0, 0), x.Dim },
            };
        }

        public MultiPrecision<N> this[int xn, int yn] {
            get {
                if (xn < 0 || yn < 0) {
                    throw new ArgumentOutOfRangeException($"{nameof(xn)},{nameof(yn)}");
                }

                for (int i = xs.Count; i < xn; i++) {
                    xs.Add(xs[0] * xs[^1]);
                }

                for (int i = ys.Count; i < yn; i++) {
                    ys.Add(ys[0] * ys[^1]);
                }

                if (!table.ContainsKey((xn, yn))) {
                    if (xn > 0 && yn > 0) {
                        Vector<N> x = xs[xn - 1], y = ys[yn - 1];

                        MultiPrecision<N> s = w is null ? (x * y).Sum : (x * y * w).Sum;

                        table.Add((xn, yn), s);
                    }
                    else if (xn > 0) {
                        Vector<N> x = xs[xn - 1];

                        MultiPrecision<N> s = w is null ? x.Sum : (x * w).Sum;

                        table.Add((xn, yn), s);
                    }
                    else {
                        Vector<N> y = ys[yn - 1];

                        MultiPrecision<N> s = w is null ? y.Sum : (y * w).Sum;

                        table.Add((xn, yn), s);
                    }
                }

                return table[(xn, yn)];
            }
        }

        public Vector<N>? W {
            get => w;
            set {
                if (value is not null && xs[0].Dim != value.Dim) {
                    throw new ArgumentException("Illegal length.", nameof(w));
                }

                this.w = value;
                this.table[(0, 0)] = w is null ? xs[0].Dim : w.Sum;
            }
        }
    }
}
