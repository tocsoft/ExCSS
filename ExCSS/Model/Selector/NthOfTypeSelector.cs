
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public sealed class NthOfTypeSelector : NthChildSelector, IToString
    {
        private int offset;
        private int step;

        public NthOfTypeSelector(int step, int offset) : base(PseudoSelectorPrefix.PseudoFunctionNthOfType, step, offset) { }

    }
}