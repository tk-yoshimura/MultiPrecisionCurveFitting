using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class LinearFitterTests {
        [TestMethod()]
        public void ExecuteFittingTest() {
            MultiPrecision<Pow2.N8>[] xs = { 2, 3 }, ys = { 1, 8 };

            LinearFitter<Pow2.N8> fitting1 = new(xs, ys, enable_intercept: true);
            LinearFitter<Pow2.N8> fitting2 = new(xs, ys, enable_intercept: false);

            Assert.AreEqual(new Vector<Pow2.N8>(new MultiPrecision<Pow2.N8>[] { -13, 7 }), fitting1.ExecuteFitting());
            Assert.AreEqual(new Vector<Pow2.N8>(new MultiPrecision<Pow2.N8>[] { 2d }), fitting2.ExecuteFitting());
        }
    }
}