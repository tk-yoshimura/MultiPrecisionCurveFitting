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
            Vector<Pow2.N8> p1 = new(new MultiPrecision<Pow2.N8>[] { 2, -1, 1, 5 }), p2 = new(new MultiPrecision<Pow2.N8>[] { 4, 3, -1 });

            for (int i = 0; i < xs.Length; i++) {
                MultiPrecision<Pow2.N8> x = xs[i];

                ys1[i] = p1[0] + p1[1] * x + p1[2] * x * x + p1[3] * x * x * x;
                ys2[i] = p2[0] * x + p2[1] * x * x + p2[2] * x * x * x;
            }

            PolynomialFitter<Pow2.N8> fitting1 = new(xs, ys1, 3, enable_intercept: true);
            PolynomialFitter<Pow2.N8> fitting2 = new(xs, ys2, 3, enable_intercept: false);

            Assert.IsTrue((fitting1.ExecuteFitting() - p1).Norm < 1e-48);
            Assert.IsTrue((fitting2.ExecuteFitting() - p2).Norm < 1e-48);
        }
    }
}