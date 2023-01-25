using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    internal class SumTable<N> where N : struct, IConstant {
        private readonly List<Vector<N>> xs = new(), ys = new();
        private readonly Vector<N>? w;
        private readonly Dictionary<(int xn, int yn), MultiPrecision<N>> table;

        public SumTable(Vector<N> x, Vector<N> y, Vector<N>? w = null) {
            if (x.Dim != y.Dim) {
                throw new ArgumentException("mismatch dim", $"{nameof(x)},{nameof(y)}");
            }
            if (w is not null && x.Dim != w.Dim) {
                throw new ArgumentException("mismatch dim", nameof(w));
            }

            this.xs.Add(x);
            this.ys.Add(y);
            this.w = w;
            this.table = new() {
                { (0, 0), w is null ? x.Dim : ((MultiPrecision<N>[])w).Sum() },
            };
        }

        public MultiPrecision<N> this[int xn, int yn] {
            get {
                if (xn < 0 || yn < 0) {
                    throw new ArgumentOutOfRangeException($"{nameof(xn)},{nameof(yn)}");
                }

                if (xs.Count < xn) {
                    for (int i = xs.Count; i < xn; i++) {
                        xs.Add(xs[0] * xs[^1]);
                    }
                }

                if (ys.Count < yn) {
                    for (int i = ys.Count; i < yn; i++) {
                        ys.Add(ys[0] * ys[^1]);
                    }
                }

                if (!table.ContainsKey((xn, yn))) {
                    if (xn > 0 && yn > 0) {
                        Vector<N> x = xs[xn - 1], y = ys[yn - 1];

                        MultiPrecision<N> s = w is null ? ((MultiPrecision<N>[])(x * y)).Sum() : ((MultiPrecision<N>[])(x * y * w)).Sum();

                        table.Add((xn, yn), s);
                    }
                    else if (xn > 0) {
                        Vector<N> x = xs[xn - 1];

                        MultiPrecision<N> s = w is null ? ((MultiPrecision<N>[])x).Sum() : ((MultiPrecision<N>[])(x * w)).Sum();

                        table.Add((xn, yn), s);
                    }
                    else {
                        Vector<N> y = ys[yn - 1];

                        MultiPrecision<N> s = w is null ? ((MultiPrecision<N>[])y).Sum() : ((MultiPrecision<N>[])(y * w)).Sum();

                        table.Add((xn, yn), s);
                    }
                }

                return table[(xn, yn)];
            }
        }
    }
}
