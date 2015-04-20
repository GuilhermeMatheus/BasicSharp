using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.ILEmitter.Extensions
{
    public static class TacUnitExtensions
    {
        public static string GetFirstLabelValue(this IEnumerable<TacUnit> source)
        {
            if (source == null || !source.Any())
                return null;

            var first = source.First();
            var padLength = 7 - first.LabelPrefix.Length;

            return first.LabelPrefix + first.LabelIndex.ToString().PadLeft(padLength, '0');
        }

        public static Tuple<string, int> GetNextLabel(this IEnumerable<TacUnit> source)
        {
            if (source == null || !source.Any())
                return null;

            var last = source.Last();

            return new Tuple<string, int>(last.LabelPrefix, last.LabelIndex + 1);
        }
    }
}
