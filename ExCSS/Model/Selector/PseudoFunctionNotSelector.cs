using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class PseudoNotFunctionSelector : BasePseudoFunctionSelector
    {
        private readonly BaseSelector _selector;

        public BaseSelector Selector { get { return _selector; } }

        public PseudoNotFunctionSelector(BaseSelector selector) : base(PseudoSelectorPrefix.PseudoFunctionNot)
        {
            _selector = selector;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Format(":{1}({0})", _selector.ToString(friendlyFormat), Function);
        }
    }
}
