using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class PseudoClassSelector : BasePseudoSelector
    {
        private readonly string _class;

        public string Class { get { return PseudoSelector; } }

        public PseudoClassSelector(string pseudoClass) : base(pseudoClass)
        {
        }
    }
}
