using System;

// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public sealed class NthLastChildSelector : NthChildSelector, IToString
    {
        public NthLastChildSelector(int step, int offset) : base(PseudoSelectorPrefix.PseudoFunctionNthlastchild, step, offset) { }
    }
}