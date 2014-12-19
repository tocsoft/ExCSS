
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public sealed class NthLastOfTypeSelector : NthChildSelector, IToString
    {
        public NthLastOfTypeSelector(int step, int offset) : base(PseudoSelectorPrefix.PseudoFunctionNthLastOfType, step, offset) { }
    }
}