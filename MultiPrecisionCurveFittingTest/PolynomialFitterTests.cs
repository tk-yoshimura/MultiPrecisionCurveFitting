using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PolynomialFitterTests {
        [TestMethod()]
        public void ExecuteFittingTest() {
            MultiPrecision<Pow2.N8>[] xs = { 1, 3, 4, 7, 8, 9, 13, 15, 20 };
            MultiPrecision<Pow2.N8>[] ys1 = new MultiPrecision<Pow2.N8>[xs.Length], ys2 = new MultiPrecision<Pow2.N8>[xs.Length];
            Vector<Pow2.N8> p1 = new(2, -1, 1, 5), p2 = new(4, 3, -1);

            for (int i = 0; i < xs.Length; i++) {
                MultiPrecision<Pow2.N8> x = xs[i];

                ys1[i] = p1[0] + p1[1] * x + p1[2] * x * x + p1[3] * x * x * x;
                ys2[i] = p2[0] * x + p2[1] * x * x + p2[2] * x * x * x;
            }

            PolynomialFitter<Pow2.N8> fitter1 = new(xs, ys1, 3, enable_intercept: true);
            PolynomialFitter<Pow2.N8> fitter2 = new(xs, ys2, 3, enable_intercept: false);

            Assert.IsTrue((fitter1.ExecuteFitting() - p1).Norm < 1e-48);
            Assert.IsTrue((fitter2.ExecuteFitting() - p2).Norm < 1e-48);
        }

        [TestMethod()]
        public void ExecuteWeightedFittingTest() {
            MultiPrecision<Pow2.N8>[] xs = { 1, 3, 4, 7, 8, 9, 13, 15, 20 };
            MultiPrecision<Pow2.N8>[] ys1 = new MultiPrecision<Pow2.N8>[xs.Length], ys2 = new MultiPrecision<Pow2.N8>[xs.Length];
            MultiPrecision<Pow2.N8>[] ws = { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0 };
            Vector<Pow2.N8> p1 = new(2, -1, 1, 5), p2 = new(4, 3, -1);

            for (int i = 0; i < xs.Length; i++) {
                MultiPrecision<Pow2.N8> x = xs[i];

                ys1[i] = p1[0] + p1[1] * x + p1[2] * x * x + p1[3] * x * x * x;
                ys2[i] = p2[0] * x + p2[1] * x * x + p2[2] * x * x * x;
            }

            ys1[^1] = ys2[^1] = 1e+8;

            PolynomialFitter<Pow2.N8> fitter1 = new(xs, ys1, 3, enable_intercept: true);
            PolynomialFitter<Pow2.N8> fitter2 = new(xs, ys2, 3, enable_intercept: false);

            Assert.IsTrue((fitter1.ExecuteFitting(ws) - p1).Norm < 1e-48);
            Assert.IsTrue((fitter2.ExecuteFitting(ws) - p2).Norm < 1e-48);
        }
    }
}