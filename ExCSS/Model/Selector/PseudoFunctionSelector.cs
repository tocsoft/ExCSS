using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class PseudoFunctionSelector : BasePseudoFunctionSelector
    {
        private readonly string _value;

        public string Value { get { return _value; } }

        public PseudoFunctionSelector(string function, string value) : base(function)
        {
            _value = value;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Format(":{1}({0})", _value, Function);
        }
    }
}
