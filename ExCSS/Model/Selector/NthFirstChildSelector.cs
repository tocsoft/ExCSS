
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public sealed class NthFirstChildSelector : NthChildSelector, IToString
    {
        public NthFirstChildSelector(int step, int offset) : base(PseudoSelectorPrefix.PseudoFunctionNthchild, step, offset) { }
    }
}