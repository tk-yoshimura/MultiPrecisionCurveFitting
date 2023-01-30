using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class SumTableTests {
        [TestMethod()]
        public void IndexerTest() {
            (MultiPrecision<Pow2.N4> x, MultiPrecision<Pow2.N4> y)[] vs = new (MultiPrecision<Pow2.N4>, MultiPrecision<Pow2.N4>)[] {
                (2, 11), (3, 13), (5, 17), (7, 19)
            };
            MultiPrecision<Pow2.N4> s(int xn, int yn) {
                return vs.Select(v => MultiPrecision<Pow2.N4>.Pow(v.x, xn) * MultiPrecision<Pow2.N4>.Pow(v.y, yn)).Sum();
            };

            SumTable<Pow2.N4> table = new(vs.Select(v => v.x).ToArray(), vs.Select(v => v.y).ToArray());

            for (int i = 0; i <= 16; i++) {
                for (int j = 0; j <= i; j++) {
                    Assert.AreEqual(s(i, j), table[i, j]);
                    Assert.AreEqual(s(j, i), table[j, i]);
                }
            }
        }

        [TestMethod()]
        public void ReverseIndexerTest() {
            (MultiPrecision<Pow2.N4> x, MultiPrecision<Pow2.N4> y)[] vs = new (MultiPrecision<Pow2.N4>, MultiPrecision<Pow2.N4>)[] {
                (2, 11), (3, 13), (5, 17), (7, 19)
            };
            MultiPrecision<Pow2.N4> s(int xn, int yn) {
                return vs.Select(v => MultiPrecision<Pow2.N4>.Pow(v.x, xn) * MultiPrecision<Pow2.N4>.Pow(v.y, yn)).Sum();
            };

            SumTable<Pow2.N4> table = new(vs.Select(v => v.x).ToArray(), vs.Select(v => v.y).ToArray());

            for (int i = 16; i >= 0; i--) {
                for (int j = i; j >= 0; j--) {
                    Assert.AreEqual(s(i, j), table[i, j]);
                    Assert.AreEqual(s(j, i), table[j, i]);
                }
            }
        }
    }
}