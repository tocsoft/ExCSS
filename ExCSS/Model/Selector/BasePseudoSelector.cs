using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public abstract class BasePseudoSelector : BaseSelector
    {
        private readonly string _pseudoSelector;

        protected string PseudoSelector { get { return _pseudoSelector; } }

        public BasePseudoSelector(string pseudoSelector)
        {
            _pseudoSelector = pseudoSelector;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return ":" + _pseudoSelector;
        }
    }
}
