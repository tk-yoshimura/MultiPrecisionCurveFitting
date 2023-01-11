using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    public class PadeFitter<N> : Fitter<N> where N : struct, IConstant {
        /// <summary>y切片</summary>
        public MultiPrecision<N> Intercept { get; private set; }

        /// <summary>分子係数</summary>
        public int Numer { get; private set; }

        /// <summary>分母係数</summary>
        public int Denom { get; private set; }

        /// <summary>コンストラクタ</summary>
        /// <param name="intercept">切片</param>
        /// <param name="numer">分子係数</param>
        /// <param name="denom">分母係数</param>
        public PadeFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, MultiPrecision<N> intercept, int numer, int denom)
            : base(xs, ys,
                  (numer >= 2 && denom >= 2) ? (numer + denom) : throw new ArgumentOutOfRangeException($"{nameof(numer)},{nameof(denom)}")) {

            this.Intercept = intercept;
            this.Numer = numer;
            this.Denom = denom;
        }

        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters) {
            (MultiPrecision<N> numer, MultiPrecision<N> denom) = Fraction(x, parameters, Numer);

            MultiPrecision<N> y = numer / denom;

            return y;
        }

        protected static MultiPrecision<N> Polynomial(MultiPrecision<N> x, MultiPrecision<N>[] coefs) {
            MultiPrecision<N> y = coefs[^1];

            for (int i = coefs.Length - 2; i >= 0; i--) {
                y = x * y + coefs[i];
            }

            return y;
        }

        public static (MultiPrecision<N> numer, MultiPrecision<N> denom) Fraction(MultiPrecision<N> x, Vector<N> parameters, int numer) {
            MultiPrecision<N> n = Polynomial(x, ((MultiPrecision<N>[])parameters)[..numer]);
            MultiPrecision<N> d = Polynomial(x, ((MultiPrecision<N>[])parameters)[numer..]);

            return (n, d);
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting(double lambda_init = 0.5, double lambda_decay = 0.975, int iter = 256) {
            Vector<N> poly = new PolynomialFitter<N>(
                X, Y.Select((v) => v - Intercept).ToArray(),
                Numer + Denom - 2, enable_intercept: false).ExecuteFitting();
            poly = (new MultiPrecision<N>[] { Intercept }).Concat((MultiPrecision<N>[])poly).ToArray();

            (MultiPrecision<N>[] ms, MultiPrecision<N>[] ns) = PadeSolver<N>.Solve(poly, Numer - 1, Denom - 1);
            (ms, ns) = (ms[1..].ToArray(), ns[1..].ToArray());

            MultiPrecision<N> fitting_func(MultiPrecision<N> x, Vector<N> parameters) {
                (MultiPrecision<N> numer, MultiPrecision<N> denom) = Fraction(x, parameters, Numer - 1);

                numer = x * numer + Intercept;
                denom = x * denom + 1;

                return numer / denom;
            }

            Vector<N> fitting_diff_func(MultiPrecision<N> x, Vector<N> parameters) {
                (MultiPrecision<N> numer, MultiPrecision<N> denom) = Fraction(x, parameters, Numer - 1);

                numer = x * numer + Intercept;
                denom = x * denom + 1;

                MultiPrecision<N>[] gms = new MultiPrecision<N>[Numer - 1], gns = new MultiPrecision<N>[Denom - 1];
                gms[0] = x / denom;
                gns[0] = -x * numer / (denom * denom);

                for (int i = 1; i < gms.Length; i++) {
                    gms[i] = x * gms[i - 1];
                }
                for (int i = 1; i < gns.Length; i++) {
                    gns[i] = x * gns[i - 1];
                }

                return gms.Concat(gns).ToArray();
            }

            LevenbergMarquardtFitter<N> fitter = new(X, Y, new FittingFunction<N>(Numer + Denom - 2, fitting_func, fitting_diff_func));
            MultiPrecision<N>[] parameters = fitter.ExecuteFitting(ms.Concat(ns).ToArray(), lambda_init, lambda_decay, iter);

            parameters = (new MultiPrecision<N>[] { Intercept })
                         .Concat(parameters[..ms.Length])
                         .Concat(new MultiPrecision<N>[] { 1 })
                         .Concat(parameters[ms.Length..]).ToArray();

            return parameters;
        }
    }
}
