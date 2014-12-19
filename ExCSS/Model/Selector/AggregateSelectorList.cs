using System;
using System.Text;

// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public class AggregateSelectorList : SelectorList
    {
        public AggregateSelectorList()
        {
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var builder = new StringBuilder();

            foreach (var selector in Selectors)
            {
                builder.Append(selector.ToString(friendlyFormat, indentation + 1));
            }

            return builder.ToString();
        }
    }
}
