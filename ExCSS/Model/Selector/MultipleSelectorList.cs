using System.Text;

// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public class MultipleSelectorList : SelectorList, IToString
    {
        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var builder = new StringBuilder();

            if (Selectors.Count <= 0)
            {
                return builder.ToString();
            }

            builder.Append(Selectors[0]);

            for (var i = 1; i < Selectors.Count; i++)
            {
                builder.Append(',').Append(Selectors[i]);
            }

            return builder.ToString();
        }
    }
}
