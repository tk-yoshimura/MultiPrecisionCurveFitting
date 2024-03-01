using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class CurveFittingUtilsTests {
        [TestMethod()]
        public void EnumeratePadeDegreeTest() {
            for (int coef_counts = 4; coef_counts <= 16; coef_counts++) {
                for (int degree_delta = 0; degree_delta <= 8; degree_delta++) {
                    Console.WriteLine($"{nameof(coef_counts)} = {coef_counts}");
                    Console.WriteLine($"{nameof(degree_delta)} = {degree_delta}");

                    foreach ((int m, int n) in CurveFittingUtils.EnumeratePadeDegree(coef_counts, degree_delta)) {
                        Console.WriteLine($"{m},{n}");

                        Assert.AreEqual(coef_counts, m + n);
                        Assert.IsTrue(Math.Abs(m - n) <= degree_delta);
                        Assert.IsTrue(m > 1);
                        Assert.IsTrue(n > 1);
                    }
                }
            }
        }

        [TestMethod()]
        public void HasLossDigitsPolynomialCoefTest() {
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(Vector<Pow2.N8>.Zero(8), -0.25, 0.25));

            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1), 0, 0.26));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1), -0.26, 0));

            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(0, 1, -1), 0, 0.26));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(0, 1, -1), -0.26, 0));

            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1), 0, 0.24));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1), -0.24, 0));

            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(0, 1, -1), 0, 0.24));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(0, 1, -1), -0.24, 0));

            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 0, -3), 0, 0.3));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 0, -2), 0, 0.3));

            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 0, -3), -0.3, 0));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 0, -2), -0.3, 0));

            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1 / 16d, -1 / 32d, -1 / 64d), 0, 2));
            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1.1 / 8d, -1.1 / 16d, -1.1 / 32d), 0, 2));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1 / 16d, -1 / 32d, -1 / 64d, -0.9 / 128d), 0, 2));
            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1 / 16d, -1 / 32d, -1 / 64d, -1.1 / 128d), 0, 2));

            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1 / 16d, 1 / 32d, 1 / 64d), 0, 2));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1.1 / 8d, 1.1 / 16d, 1.1 / 32d), 0, 2));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1 / 16d, 1 / 32d, 1 / 64d, 0.9 / 128d), 0, 2));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1 / 16d, 1 / 32d, 1 / 64d, 1.1 / 128d), 0, 2));

            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1 / 16d, 1 / 32d, -1 / 64d), -2, 0));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1.1 / 8d, 1.1 / 16d, -1.1 / 32d), -2, 0));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1 / 16d, 1 / 32d, -1 / 64d, 0.9 / 128d), -2, 0));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, -1 / 16d, 1 / 32d, -1 / 64d, 1.1 / 128d), -2, 0));

            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1 / 16d, -1 / 32d, 1 / 64d), -2, 0));
            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1.1 / 8d, -1.1 / 16d, 1.1 / 32d), -2, 0));
            Assert.IsFalse(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1 / 16d, -1 / 32d, 1 / 64d, -0.9 / 128d), -2, 0));
            Assert.IsTrue(CurveFittingUtils.HasLossDigitsPolynomialCoef(new Vector<Pow2.N8>(1, 1 / 16d, -1 / 32d, 1 / 64d, -1.1 / 128d), -2, 0));
        }

        [TestMethod()]
        public void RelativeErrorTest() {
            Assert.AreEqual(0, CurveFittingUtils.RelativeError<Pow2.N8>(0, 0));

            Assert.IsTrue(MultiPrecision<Pow2.N8>.IsPositiveInfinity(CurveFittingUtils.RelativeError<Pow2.N8>(0, 1)));

            Assert.AreEqual(1, CurveFittingUtils.RelativeError<Pow2.N8>(1, 0));

            Assert.AreEqual(0.5, CurveFittingUtils.RelativeError<Pow2.N8>(1, 1.5));
            Assert.AreEqual(0.25, CurveFittingUtils.RelativeError<Pow2.N8>(2, 1.5));

            Assert.AreEqual(0.5, CurveFittingUtils.RelativeError<Pow2.N8>(-1, -1.5));
            Assert.AreEqual(0.25, CurveFittingUtils.RelativeError<Pow2.N8>(-2, -1.5));
        }

        [TestMethod()]
        public void AbsoluteErrorTest() {
            Assert.AreEqual(0, CurveFittingUtils.AbsoluteError<Pow2.N8>(0, 0));

            Assert.AreEqual(1, CurveFittingUtils.AbsoluteError<Pow2.N8>(0, 1));

            Assert.AreEqual(1, CurveFittingUtils.AbsoluteError<Pow2.N8>(1, 0));

            Assert.AreEqual(0.5, CurveFittingUtils.AbsoluteError<Pow2.N8>(1, 1.5));
            Assert.AreEqual(0.5, CurveFittingUtils.AbsoluteError<Pow2.N8>(2, 1.5));

            Assert.AreEqual(0.5, CurveFittingUtils.AbsoluteError<Pow2.N8>(-1, -1.5));
            Assert.AreEqual(0.5, CurveFittingUtils.AbsoluteError<Pow2.N8>(-2, -1.5));
        }

        [TestMethod()]
        public void MaxRelativeErrorTest() {
            Assert.AreEqual(1, CurveFittingUtils.MaxRelativeError(new Vector<Pow2.N8>(2, 3, 4), new Vector<Pow2.N8>(4, 3, 2)));
        }

        [TestMethod()]
        public void MaxAbsoluteErrorTest() {
            Assert.AreEqual(2, CurveFittingUtils.MaxAbsoluteError(new Vector<Pow2.N8>(2, 3, 4), new Vector<Pow2.N8>(4, 3, 2)));
        }

        [TestMethod()]
        public void EnumeratePadeCoefTest() {
            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 4), (2, 5), (3, 6) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5, 6), 3, 3).ToArray()
            );

            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 5), (2, 6), (3, 0), (4, 0) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5, 6), 4, 2).ToArray()
            );

            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 3), (2, 4), (0, 5), (0, 6) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5, 6), 2, 4).ToArray()
            );

            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 6), (2, 0), (3, 0), (4, 0), (5, 0) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5, 6), 5, 1).ToArray()
            );

            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 2), (0, 3), (0, 4), (0, 5), (0, 6) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5, 6), 1, 5).ToArray()
            );

            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 5), (2, 0), (3, 0), (4, 0) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5), 4, 1).ToArray()
            );

            CollectionAssert.AreEqual(
                new (MultiPrecision<Pow2.N8>, MultiPrecision<Pow2.N8>)[] { (1, 2), (0, 3), (0, 4), (0, 5) },
                CurveFittingUtils.EnumeratePadeCoef(new Vector<Pow2.N8>(1, 2, 3, 4, 5), 1, 4).ToArray()
            );
        }

        [TestMethod()]
        public void StandardizeExponentTest() {
            Vector<Pow2.N8> v = new double[] { 0, 0.125, 0.25, -0.125 };

            (long exp_scale, Vector<Pow2.N8> u) = CurveFittingUtils.StandardizeExponent(v);

            Assert.AreEqual(-2, exp_scale);
            Assert.AreEqual(new Vector<Pow2.N8>(0, 0.5, 1, -0.5), u);

            Assert.ThrowsException<ArgumentException>(() => {
                _ = CurveFittingUtils.StandardizeExponent<Pow2.N8>(new double[] { 0, 0, 0 });
            });

            Assert.ThrowsException<ArgumentException>(() => {
                _ = CurveFittingUtils.StandardizeExponent<Pow2.N8>(new double[] { 1, 1, double.PositiveInfinity });
            });

            Assert.ThrowsException<ArgumentException>(() => {
                _ = CurveFittingUtils.StandardizeExponent<Pow2.N8>(new double[] { 1, 1, double.NaN });
            });
        }
    }
}