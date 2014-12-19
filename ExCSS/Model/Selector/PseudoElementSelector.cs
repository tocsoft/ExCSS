using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class PseudoElementSelector : BasePseudoSelector
    {

        public string Element { get { return PseudoSelector; } }

        public PseudoElementSelector(string pseudoElement) : base(pseudoElement)
        {
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return "::" + Element;
        }
    }
}
