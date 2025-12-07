using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PadeFitterTests {
        [TestMethod()]
        public void FitWithInterceptTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);

            PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

            Vector<Pow2.N8> parameters = fitter.Fit();

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            Assert.AreEqual(0.75, fitter.Regress(0, parameters));

            for (int i = 0; i < xs.Length; i++) {
                Assert.IsLessThan(1e-5,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }

            Assert.IsLessThan(1e-4, fitter.Error(parameters).Norm);
        }

        [TestMethod()]
        public void FitWithoutInterceptTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);

            PadeFitter<Pow2.N8> fitter = new(xs, ys, numer: 4, denom: 3);

            Vector<Pow2.N8> parameters = fitter.Fit();

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            for (int i = 0; i < xs.Length; i++) {
                Assert.IsLessThan(1e-5,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }

            Assert.IsLessThan(1e-4, fitter.Error(parameters).Norm);
        }

        [TestMethod()]
        public void ExecuteWeightedFittingWithInterceptTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);
            MultiPrecision<Pow2.N8>[] ws = Vector<Pow2.N8>.Fill(xs.Length, value: 0.5);

            ys[256] = 1e+8;
            ws[256] = 0;

            PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

            Vector<Pow2.N8> parameters = fitter.Fit(ws);

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            Assert.AreEqual(0.75, fitter.Regress(0, parameters));

            for (int i = 0; i < xs.Length; i++) {
                if (i == 256) {
                    continue;
                }

                Assert.IsLessThan(1e-5,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void ExecuteWeightedFittingWithoutInterceptTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);
            MultiPrecision<Pow2.N8>[] ws = Vector<Pow2.N8>.Fill(xs.Length, value: 0.5);

            ys[256] = 1e+8;
            ws[256] = 0;

            PadeFitter<Pow2.N8> fitter = new(xs, ys, numer: 4, denom: 3);

            Vector<Pow2.N8> parameters = fitter.Fit(ws);

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            for (int i = 0; i < xs.Length; i++) {
                if (i == 256) {
                    continue;
                }

                Assert.IsLessThan(1e-5,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void FitWithInterceptWithCostTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);

            PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

            Assert.IsGreaterThan(fitter.Fit(norm_cost: 1e-8).Norm, fitter.Fit().Norm);
            Assert.IsGreaterThan(fitter.Fit(norm_cost: 1e-4).Norm, fitter.Fit(norm_cost: 1e-8).Norm);
            Assert.IsGreaterThan(fitter.Fit(norm_cost: 1e-2).Norm, fitter.Fit(norm_cost: 1e-4).Norm);

            Vector<Pow2.N8> parameters = fitter.Fit(norm_cost: 1e-8);

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            Assert.AreEqual(0.75, fitter.Regress(0, parameters));

            for (int i = 0; i < xs.Length; i++) {
                Assert.IsLessThan(1e-4,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void FitWithoutInterceptWithCostTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);

            PadeFitter<Pow2.N8> fitter = new(xs, ys, numer: 4, denom: 3);

            Assert.IsGreaterThan(fitter.Fit(norm_cost: 1e-8).Norm, fitter.Fit().Norm);
            Assert.IsGreaterThan(fitter.Fit(norm_cost: 1e-4).Norm, fitter.Fit(norm_cost: 1e-8).Norm);
            Assert.IsGreaterThan(fitter.Fit(norm_cost: 1e-2).Norm, fitter.Fit(norm_cost: 1e-4).Norm);

            Vector<Pow2.N8> parameters = fitter.Fit(norm_cost: 1e-8);

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            for (int i = 0; i < xs.Length; i++) {
                Assert.IsLessThan(1e-4,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void ExecuteWeightedFittingWithInterceptWithCostTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);
            MultiPrecision<Pow2.N8>[] ws = Vector<Pow2.N8>.Fill(xs.Length, value: 0.5);

            ys[256] = 1e+8;
            ws[256] = 0;

            PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

            Assert.IsGreaterThan(fitter.Fit(ws, norm_cost: 1e-8).Norm, fitter.Fit(ws).Norm);
            Assert.IsGreaterThan(fitter.Fit(ws, norm_cost: 1e-4).Norm, fitter.Fit(ws, norm_cost: 1e-8).Norm);
            Assert.IsGreaterThan(fitter.Fit(ws, norm_cost: 1e-2).Norm, fitter.Fit(ws, norm_cost: 1e-4).Norm);

            Vector<Pow2.N8> parameters = fitter.Fit(ws, norm_cost: 1e-8);

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            Assert.AreEqual(0.75, fitter.Regress(0, parameters));

            for (int i = 0; i < xs.Length; i++) {
                if (i == 256) {
                    continue;
                }

                Assert.IsLessThan(1e-4,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void ExecuteWeightedFittingWithoutInterceptWithCostTest() {
            MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);
            MultiPrecision<Pow2.N8>[] ws = Vector<Pow2.N8>.Fill(xs.Length, value: 0.5);

            ys[256] = 1e+8;
            ws[256] = 0;

            PadeFitter<Pow2.N8> fitter = new(xs, ys, numer: 4, denom: 3);

            Assert.IsGreaterThan(fitter.Fit(ws, norm_cost: 1e-8).Norm, fitter.Fit(ws).Norm);
            Assert.IsGreaterThan(fitter.Fit(ws, norm_cost: 1e-4).Norm, fitter.Fit(ws, norm_cost: 1e-8).Norm);
            Assert.IsGreaterThan(fitter.Fit(ws, norm_cost: 1e-2).Norm, fitter.Fit(ws, norm_cost: 1e-4).Norm);

            Vector<Pow2.N8> parameters = fitter.Fit(ws, norm_cost: 1e-8);

            Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
            Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

            for (int i = 0; i < xs.Length; i++) {
                if (i == 256) {
                    continue;
                }

                Assert.IsLessThan(1e-4,
MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)), $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void GenerateTableTest() {
            (MultiPrecision<Pow2.N4> x, MultiPrecision<Pow2.N4> y)[] vs = new (MultiPrecision<Pow2.N4>, MultiPrecision<Pow2.N4>)[] {
                (2, 11), (3, 13), (5, 17), (7, 19)
            };

            MultiPrecision<Pow2.N4> s(int xn, int yn) {
                return vs.Select(v => MultiPrecision<Pow2.N4>.Pow(v.x, xn) * MultiPrecision<Pow2.N4>.Pow(v.y, yn)).Sum();
            };

            SumTable<Pow2.N4> table = new(vs.Select(v => v.x).ToArray(), vs.Select(v => v.y).ToArray());

            (Matrix<Pow2.N4> m, Vector<Pow2.N4> v) = PadeFitter<Pow2.N4>.GenerateTable(table, 5, 4);

            Assert.AreEqual(m, m.T);

            Assert.AreEqual(s(0, 0), m[0, 0]);
            Assert.AreEqual(s(1, 0), m[1, 0]);
            Assert.AreEqual(s(2, 0), m[2, 0]);
            Assert.AreEqual(s(3, 0), m[3, 0]);
            Assert.AreEqual(s(4, 0), m[4, 0]);
            Assert.AreEqual(-s(1, 1), m[5, 0]);
            Assert.AreEqual(-s(2, 1), m[6, 0]);
            Assert.AreEqual(-s(3, 1), m[7, 0]);

            Assert.AreEqual(s(2, 0), m[1, 1]);
            Assert.AreEqual(s(3, 0), m[2, 1]);
            Assert.AreEqual(s(4, 0), m[3, 1]);
            Assert.AreEqual(s(5, 0), m[4, 1]);
            Assert.AreEqual(-s(2, 1), m[5, 1]);
            Assert.AreEqual(-s(3, 1), m[6, 1]);
            Assert.AreEqual(-s(4, 1), m[7, 1]);

            Assert.AreEqual(s(4, 0), m[2, 2]);
            Assert.AreEqual(s(5, 0), m[3, 2]);
            Assert.AreEqual(s(6, 0), m[4, 2]);
            Assert.AreEqual(-s(3, 1), m[5, 2]);
            Assert.AreEqual(-s(4, 1), m[6, 2]);
            Assert.AreEqual(-s(5, 1), m[7, 2]);

            Assert.AreEqual(s(6, 0), m[3, 3]);
            Assert.AreEqual(s(7, 0), m[4, 3]);
            Assert.AreEqual(-s(4, 1), m[5, 3]);
            Assert.AreEqual(-s(5, 1), m[6, 3]);
            Assert.AreEqual(-s(6, 1), m[7, 3]);

            Assert.AreEqual(s(8, 0), m[4, 4]);
            Assert.AreEqual(-s(5, 1), m[5, 4]);
            Assert.AreEqual(-s(6, 1), m[6, 4]);
            Assert.AreEqual(-s(7, 1), m[7, 4]);

            Assert.AreEqual(s(2, 2), m[5, 5]);
            Assert.AreEqual(s(3, 2), m[6, 5]);
            Assert.AreEqual(s(4, 2), m[7, 5]);

            Assert.AreEqual(s(4, 2), m[6, 6]);
            Assert.AreEqual(s(5, 2), m[7, 6]);

            Assert.AreEqual(s(6, 2), m[7, 7]);

            Assert.AreEqual(s(0, 1), v[0]);
            Assert.AreEqual(s(1, 1), v[1]);
            Assert.AreEqual(s(2, 1), v[2]);
            Assert.AreEqual(s(3, 1), v[3]);
            Assert.AreEqual(s(4, 1), v[4]);
            Assert.AreEqual(-s(1, 2), v[5]);
            Assert.AreEqual(-s(2, 2), v[6]);
            Assert.AreEqual(-s(3, 2), v[7]);
        }
    }
}