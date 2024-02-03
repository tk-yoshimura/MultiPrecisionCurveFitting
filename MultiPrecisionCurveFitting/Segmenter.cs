using MultiPrecision;

namespace MultiPrecisionCurveFitting {
    public class Segmenter<N> where N: struct, IConstant {
        private readonly List<(MultiPrecision<N> min, MultiPrecision<N> max, bool is_success)> approximated_ranges = [];
        private readonly List<(MultiPrecision<N> min, MultiPrecision<N> max, MultiPrecision<N> range_limit)> uncompleted_ranges;

        private readonly Func<MultiPrecision<N>, MultiPrecision<N>, bool> approximation_func;

        public Segmenter(
            List<(MultiPrecision<N> min, MultiPrecision<N> max, MultiPrecision<N> range_limit)> ranges,
            Func<MultiPrecision<N>, MultiPrecision<N>, bool> approximation_func) {

            if (ranges.Count < 1 || ranges.Any(item => !(item.min <= item.max) || !(item.range_limit > 0))) {
                throw new ArgumentException("invalid ranges", nameof(ranges));
            }

            this.uncompleted_ranges = new List<(MultiPrecision<N> min, MultiPrecision<N> max, MultiPrecision<N> range_limit)>(ranges);

            this.approximation_func = approximation_func;
        }

        public void Execute() {
            while (uncompleted_ranges.Count > 0) {
                (MultiPrecision<N> min, MultiPrecision<N> max, MultiPrecision<N> range_limit) = uncompleted_ranges.First();
                uncompleted_ranges.RemoveAt(0);

                if (max - min < range_limit) {
                    approximated_ranges.Add((min, max, is_success: false));
                    continue;
                }

                if (approximation_func(min, max)) { 
                    approximated_ranges.Add((min, max, is_success: true));
                    continue;
                }

                MultiPrecision<N> mid = (min + max) / 2;

                uncompleted_ranges.InsertRange(
                    index: 0, [(min, mid, range_limit), (mid, max, range_limit)]
                );
            }
        }

        public IEnumerable<(MultiPrecision<N> min, MultiPrecision<N> max, bool is_success)> ApproximatedRanges => approximated_ranges;
    }
}
