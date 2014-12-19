using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class ElementSelector : BaseSelector
    {
        private readonly string _type;

        public string Element { get { return _type; } }

        public ElementSelector(string type)
        {
            _type = type;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return _type;
        }
    }
}
