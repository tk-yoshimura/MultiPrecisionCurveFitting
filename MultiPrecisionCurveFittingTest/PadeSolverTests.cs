using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PadeSolverTests {
        [TestMethod()]
        public void ExpTest() {
            MultiPrecision<Pow2.N8>[] cs = MultiPrecision<Pow2.N8>.TaylorSequence.Take(8).ToArray();

            {
                (MultiPrecision<Pow2.N8>[] ms, MultiPrecision<Pow2.N8>[] ns) = PadeSolver<Pow2.N8>.Solve(cs[..6], 3, 2);

                Assert.AreEqual(1, ms[0]);
                Assert.AreEqual(0.6, (double)ms[1], 1e-12);
                Assert.AreEqual(0.15, (double)ms[2], 1e-12);
                Assert.AreEqual(1d / 60, (double)ms[3], 1e-12);

                Assert.AreEqual(1, ns[0]);
                Assert.AreEqual(-0.4, (double)ns[1], 1e-12);
                Assert.AreEqual(0.05, (double)ns[2], 1e-12);
            }

            {
                (MultiPrecision<Pow2.N8>[] ms, MultiPrecision<Pow2.N8>[] ns) = PadeSolver<Pow2.N8>.Solve(cs[..6], 2, 3);

                Assert.AreEqual(1, ms[0]);
                Assert.AreEqual(0.4, (double)ms[1], 1e-12);
                Assert.AreEqual(0.05, (double)ms[2], 1e-12);

                Assert.AreEqual(1, ms[0]);
                Assert.AreEqual(-0.6, (double)ns[1], 1e-12);
                Assert.AreEqual(0.15, (double)ns[2], 1e-12);
                Assert.AreEqual(-1d / 60, (double)ns[3], 1e-12);
            }

            {
                (MultiPrecision<Pow2.N8>[] ms, MultiPrecision<Pow2.N8>[] ns) = PadeSolver<Pow2.N8>.Solve(cs[..7], 3, 3);

                Assert.AreEqual(1, ms[0]);
                Assert.AreEqual(0.5, (double)ms[1], 1e-12);
                Assert.AreEqual(0.1, (double)ms[2], 1e-12);
                Assert.AreEqual(1d / 120, (double)ms[3], 1e-12);

                Assert.AreEqual(1, ns[0]);
                Assert.AreEqual(-0.5, (double)ns[1], 1e-12);
                Assert.AreEqual(0.1, (double)ns[2], 1e-12);
                Assert.AreEqual(-1d / 120, (double)ns[3], 1e-12);
            }
        }

        [TestMethod()]
        public void LogTest() {
            MultiPrecision<Pow2.N8>[] cs = new MultiPrecision<Pow2.N8>[]{
                0, 1, MultiPrecision<Pow2.N8>.Div(-1, 2), MultiPrecision<Pow2.N8>.Div(1, 3), MultiPrecision<Pow2.N8>.Div(-1, 4),
                MultiPrecision<Pow2.N8>.Div(1, 5), MultiPrecision<Pow2.N8>.Div(-1, 6), MultiPrecision<Pow2.N8>.Div(1, 7), MultiPrecision<Pow2.N8>.Div(-1, 8),
            };

            {
                (MultiPrecision<Pow2.N8>[] ms, MultiPrecision<Pow2.N8>[] ns) = PadeSolver<Pow2.N8>.Solve(cs[..6], 3, 2);

                Assert.AreEqual(0, ms[0]);
                Assert.AreEqual(1, (double)ms[1], 1e-12);
                Assert.AreEqual(7d / 10, (double)ms[2], 1e-12);
                Assert.AreEqual(1d / 30, (double)ms[3], 1e-12);

                Assert.AreEqual(1, ns[0]);
                Assert.AreEqual(6d / 5, (double)ns[1], 1e-12);
                Assert.AreEqual(3d / 10, (double)ns[2], 1e-12);
            }

            {
                (MultiPrecision<Pow2.N8>[] ms, MultiPrecision<Pow2.N8>[] ns) = PadeSolver<Pow2.N8>.Solve(cs[..7], 3, 3);

                Assert.AreEqual(0, ms[0]);
                Assert.AreEqual(1, (double)ms[1], 1e-12);
                Assert.AreEqual(1, (double)ms[2], 1e-12);
                Assert.AreEqual(11d / 60, (double)ms[3], 1e-12);

                Assert.AreEqual(1, ns[0]);
                Assert.AreEqual(3d / 2, (double)ns[1], 1e-12);
                Assert.AreEqual(3d / 5, (double)ns[2], 1e-12);
                Assert.AreEqual(1d / 20, (double)ns[3], 1e-12);
            }
        }
    }
}