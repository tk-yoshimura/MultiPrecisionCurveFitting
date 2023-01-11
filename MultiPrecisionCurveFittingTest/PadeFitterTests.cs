using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PadeFitterTests {
        [TestMethod()]
        public void ExecuteFittingTest() {
            MultiPrecision<Pow2.N8>[] xs = (new MultiPrecision<Pow2.N8>[1024]).Select((_, i) => MultiPrecision<Pow2.N8>.Div(i, 1024)).ToArray();
            MultiPrecision<Pow2.N8>[] ys = xs.Select(v => MultiPrecision<Pow2.N8>.Cos(v)).ToArray();

            PadeFitter<Pow2.N8> fitting = new(xs, ys, 1, 4, 3);
            fitting.ExecuteFitting();
        }
    }
}