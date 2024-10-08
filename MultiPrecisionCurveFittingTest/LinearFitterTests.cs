﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class LinearFitterTests {
        [TestMethod()]
        public void FitTest() {
            MultiPrecision<Pow2.N8>[] xs = [2, 3], ys = [1, 8];

            LinearFitter<Pow2.N8> fitter1 = new(xs, ys);
            LinearFitter<Pow2.N8> fitter2 = new(xs, ys, intercept: 0);
            LinearFitter<Pow2.N8> fitter3 = new(xs, ys, intercept: -13);

            Assert.AreEqual(new Vector<Pow2.N8>(-13, 7), fitter1.Fit());
            Assert.AreEqual(new Vector<Pow2.N8>(0, 2), fitter2.Fit());
            Assert.AreEqual(new Vector<Pow2.N8>(-13, 7), fitter3.Fit());

            Assert.AreEqual(new Vector<Pow2.N8>(0, 0), fitter1.Error(fitter1.Fit()));
            Assert.AreEqual(new Vector<Pow2.N8>(0, 0), fitter3.Error(fitter3.Fit()));
        }

        [TestMethod()]
        public void ExecuteWeightedFittingTest() {
            MultiPrecision<Pow2.N8>[] xs = [2, 3, 4], ys = [1, 8, 1e+8], ws = [0.5, 0.5, 0];

            LinearFitter<Pow2.N8> fitter1 = new(xs, ys);
            LinearFitter<Pow2.N8> fitter2 = new(xs, ys, intercept: 0);
            LinearFitter<Pow2.N8> fitter3 = new(xs, ys, intercept: -13);

            Assert.AreEqual(new Vector<Pow2.N8>(-13, 7), fitter1.Fit(ws));
            Assert.AreEqual(new Vector<Pow2.N8>(0, 2), fitter2.Fit(ws));
            Assert.AreEqual(new Vector<Pow2.N8>(-13, 7), fitter3.Fit(ws));
        }
    }
}