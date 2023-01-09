using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>フィッティング関数</summary>
    public class FittingFunction<N> where N : struct, IConstant {
        readonly Func<MultiPrecision<N>, Vector<N>, MultiPrecision<N>> f;
        readonly Func<MultiPrecision<N>, Vector<N>, Vector<N>> df;

        /// <summary>コンストラクタ</summary>
        public FittingFunction(int parameters, Func<MultiPrecision<N>, Vector<N>, MultiPrecision<N>> f, Func<MultiPrecision<N>, Vector<N>, Vector<N>> df) {
            this.Parameters = parameters;
            this.f = f;
            this.df = df;
        }

        /// <summary>パラメータ数</summary>
        public int Parameters {
            get; private set;
        }

        /// <summary>関数値</summary>
        public MultiPrecision<N> F(MultiPrecision<N> x, Vector<N> v) {
            return f(x, v);
        }

        /// <summary>関数勾配</summary>
        public Vector<N> DiffF(MultiPrecision<N> x, Vector<N> v) {
            return df(x, v);
        }
    }
}