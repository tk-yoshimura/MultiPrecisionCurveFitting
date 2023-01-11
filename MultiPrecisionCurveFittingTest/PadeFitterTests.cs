using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class PadeFitterTests {
        [TestMethod()]
        public void ExecuteFittingTest() {
            MultiPrecision<Pow2.N8>[] xs = (new MultiPrecision<Pow2.N8>[1024]).Select((_, i) => MultiPrecision<Pow2.N8>.Div(i, 1024)).ToArray();
            MultiPrecision<Pow2.N8>[] ys = xs.Select(v => MultiPrecision<Pow2.N8>.Cos(v) - 0.25).ToArray();

            PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

            bool callback(Vector<Pow2.N8> parameters) {
                MultiPrecision<Pow2.N8> norm = fitter.Error(parameters).Norm;

                Console.WriteLine(parameters);
                Console.WriteLine(norm);

                bool is_continue = norm > 1e-3;

                return is_continue;
            };

            Vector<Pow2.N8> parameters = fitter.ExecuteFitting(iter: 256, iter_callback: callback);

            Console.WriteLine($"Numer : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[..fitter.Numer]}");
            Console.WriteLine($"Denom : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[fitter.Numer..]}");
        }
    }
}