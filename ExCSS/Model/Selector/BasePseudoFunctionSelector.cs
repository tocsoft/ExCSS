using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public abstract class BasePseudoFunctionSelector : BasePseudoSelector
    {
        private readonly string _value;

        public string Function { get { return PseudoSelector; } }

        public BasePseudoFunctionSelector(string pseudoFunction) : base(pseudoFunction)
        {
        }
    }
}
