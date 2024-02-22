using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    public static class CurveFittingUtils {
        public static IEnumerable<(int m, int n)> EnumeratePadeDegree(int coef_counts, int degree_delta) {
            ArgumentOutOfRangeException.ThrowIfLessThan(coef_counts, 4);
            ArgumentOutOfRangeException.ThrowIfNegative(degree_delta);

            int d = (coef_counts + 1) / 2;

            if (coef_counts % 2 == 0 && d > 0) {
                yield return (d, d);

                for (int k = 1; k <= degree_delta / 2 && k + 1 < d; k++) {
                    yield return (d + k, d - k);
                    yield return (d - k, d + k);
                }
            }
            else if (coef_counts % 2 == 1 && d > 1 && degree_delta > 0) {
                yield return (d, d - 1);
                yield return (d - 1, d);

                for (int k = 1; k < (degree_delta + 1) / 2 && k + 2 < d; k++) {
                    yield return (d + k, d - 1 - k);
                    yield return (d - 1 - k, d + k);
                }
            }
        }

        public static bool HasLossDigitsPolynomialCoef<N>(Vector<N> coefs, MultiPrecision<N> xmin, MultiPrecision<N> xmax) where N : struct, IConstant {
            if (MultiPrecision<N>.IsZero(coefs[0])) {
                if (coefs.Dim > 1) {
                    return HasLossDigitsPolynomialCoef(coefs[1..], xmin, xmax);
                }

                return false;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan(xmin, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(xmax, 0);

            MultiPrecision<N> xnmin = 1, xnmax = 1;

            for (int k = 1; k < coefs.Dim; k++) {
                xnmin *= xmin;
                xnmax *= xmax;

                MultiPrecision<N> cnmin = coefs[k] * xnmin;
                MultiPrecision<N> cnmax = coefs[k] * xnmax;

                if (coefs[0].Sign != cnmin.Sign && (cnmin / coefs[0]).Exponent >= -2) {
                    return true;
                }

                if (coefs[0].Sign != cnmax.Sign && (cnmax / coefs[0]).Exponent >= -2) {
                    return true;
                }
            }

            MultiPrecision<N> ymin = Vector<N>.Polynomial(xmin, coefs);
            MultiPrecision<N> ymax = Vector<N>.Polynomial(xmax, coefs);

            if (coefs[0].Sign != ymin.Sign || (ymin / coefs[0]).Exponent <= -2) {
                return true;
            }

            if (coefs[0].Sign != ymax.Sign || (ymax / coefs[0]).Exponent <= -2) {
                return true;
            }

            return false;
        }

        public static MultiPrecision<N> RelativeError<N>(MultiPrecision<N> expected, MultiPrecision<N> actual) where N : struct, IConstant {
            if (MultiPrecision<N>.IsZero(expected)) {
                return MultiPrecision<N>.IsZero(actual) ? MultiPrecision<N>.Zero : MultiPrecision<N>.PositiveInfinity;
            }
            else {
                MultiPrecision<N> error = MultiPrecision<N>.Abs((expected - actual) / expected);

                return error;
            }
        }

        public static MultiPrecision<N> AbsoluteError<N>(MultiPrecision<N> expected, MultiPrecision<N> actual) where N : struct, IConstant {
            MultiPrecision<N> error = MultiPrecision<N>.Abs(expected - actual);

            return error;
        }

        public static MultiPrecision<N> MaxRelativeError<N>(Vector<N> expected, Vector<N> actual) where N : struct, IConstant {
            return Vector<N>.Func(RelativeError, expected, actual).Select(item => item.val).Max();
        }

        public static MultiPrecision<N> MaxAbsoluteError<N>(Vector<N> expected, Vector<N> actual) where N : struct, IConstant {
            return Vector<N>.Func(AbsoluteError, expected, actual).Select(item => item.val).Max();
        }

        public static IEnumerable<(MultiPrecision<N> numer, MultiPrecision<N> denom)> EnumeratePadeCoef<N>(Vector<N> param, int m, int n) where N : struct, IConstant {
            if (param.Dim != checked(m + n)) {
                throw new ArgumentException("invalid param dims", nameof(param));
            }

            if (m >= n) {
                for (int i = 0; i < n; i++) {
                    yield return (param[..m][i], param[m..][i]);
                }
                for (int i = 0; i < m - n; i++) {
                    yield return (param[..m][n + i], 0);
                }
            }
            else {
                for (int i = 0; i < m; i++) {
                    yield return (param[..m][i], param[m..][i]);
                }
                for (int i = 0; i < n - m; i++) {
                    yield return (0, param[m..][m + i]);
                }
            }
        }
    }
}
