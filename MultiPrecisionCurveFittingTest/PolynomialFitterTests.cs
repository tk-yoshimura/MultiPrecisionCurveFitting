﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PolynomialFitterTests {
        [TestMethod()]
        public void ExecuteFittingTest() {
            Vector<Pow2.N8> p1 = new(2, -1, 1, 5), p2 = new(1, 4, 3, -1);
            MultiPrecision<Pow2.N8>[] xs = { 1, 3, 4, 7, 8, 9, 13, 15, 20 };
            MultiPrecision<Pow2.N8>[] ys1 = Vector<Pow2.N8>.Polynomial(xs, p1), ys2 = Vector<Pow2.N8>.Polynomial(xs, p2);

            PolynomialFitter<Pow2.N8> fitter1 = new(xs, ys1, 3);
            PolynomialFitter<Pow2.N8> fitter2 = new(xs, ys2, 3, intercept: 1);

            Assert.IsTrue((fitter1.ExecuteFitting() - p1).Norm < 1e-48);
            Assert.IsTrue((fitter2.ExecuteFitting() - p2).Norm < 1e-48);

            Assert.IsTrue(fitter1.Error(fitter1.ExecuteFitting()).Norm < 1e-48);
            Assert.IsTrue(fitter2.Error(fitter2.ExecuteFitting()).Norm < 1e-48);
        }

        [TestMethod()]
        public void ExecuteWeightedFittingTest() {
            Vector<Pow2.N8> p1 = new(2, -1, 1, 5), p2 = new(1, 4, 3, -1);
            MultiPrecision<Pow2.N8>[] xs = { 1, 3, 4, 7, 8, 9, 13, 15, 20 };
            MultiPrecision<Pow2.N8>[] ys1 = Vector<Pow2.N8>.Polynomial(xs, p1), ys2 = Vector<Pow2.N8>.Polynomial(xs, p2);
            MultiPrecision<Pow2.N8>[] ws = { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0 };

            ys1[^1] = ys2[^1] = 1e+8;

            PolynomialFitter<Pow2.N8> fitter1 = new(xs, ys1, 3);
            PolynomialFitter<Pow2.N8> fitter2 = new(xs, ys2, 3, intercept: 1);

            Assert.IsTrue((fitter1.ExecuteFitting(ws) - p1).Norm < 1e-48);
            Assert.IsTrue((fitter2.ExecuteFitting(ws) - p2).Norm < 1e-48);
        }
    }
}