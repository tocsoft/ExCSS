using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class UnknownSelector : BaseSelector
    {
        private readonly string _code;

        public UnknownSelector(string selectorText)
        {
            _code = selectorText;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return _code;
        }
    }
}
