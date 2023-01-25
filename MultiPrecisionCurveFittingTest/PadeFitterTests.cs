﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PadeFitterTests {
        [TestMethod()]
        public void ExecuteFittingWithInterceptTest() {
            MultiPrecision<Pow2.N8>[] xs = (new MultiPrecision<Pow2.N8>[1024]).Select((_, i) => MultiPrecision<Pow2.N8>.Div(i, 1024)).ToArray();
            MultiPrecision<Pow2.N8>[] ys = xs.Select(v => MultiPrecision<Pow2.N8>.Cos(v) - 0.25).ToArray();

            PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

            Vector<Pow2.N8> parameters = fitter.ExecuteFitting();

            Console.WriteLine($"Numer : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[..fitter.Numer]}");
            Console.WriteLine($"Denom : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[fitter.Numer..]}");

            Assert.AreEqual(0.75, fitter.FittingValue(0, parameters));

            for (int i = 0; i < xs.Length; i++) {
                Assert.IsTrue(MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.FittingValue(xs[i], parameters)) < 1e-5,
                    $"\nexpected : {ys[i]}\n actual  : {fitter.FittingValue(xs[i], parameters)}"
                );
            }
        }

        [TestMethod()]
        public void ExecuteFittingWithoutInterceptTest() {
            MultiPrecision<Pow2.N8>[] xs = (new MultiPrecision<Pow2.N8>[1024]).Select((_, i) => MultiPrecision<Pow2.N8>.Div(i, 1024)).ToArray();
            MultiPrecision<Pow2.N8>[] ys = xs.Select(v => MultiPrecision<Pow2.N8>.Cos(v) - 0.25).ToArray();

            PadeFitter<Pow2.N8> fitter = new(xs, ys, numer: 4, denom: 3);

            Vector<Pow2.N8> parameters = fitter.ExecuteFitting();

            Console.WriteLine($"Numer : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[..fitter.Numer]}");
            Console.WriteLine($"Denom : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[fitter.Numer..]}");

            for (int i = 0; i < xs.Length; i++) {
                Assert.IsTrue(MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.FittingValue(xs[i], parameters)) < 1e-5,
                    $"\nexpected : {ys[i]}\n actual  : {fitter.FittingValue(xs[i], parameters)}"
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

            Assert.AreEqual(m, m.Transpose);

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