
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public class NthChildEvenSelector : NthChildSelector
    {
        static NthChildEvenSelector _instance;

        public static NthChildEvenSelector Instance
        {
            get { return _instance ?? (_instance = new NthChildEvenSelector()); }
        }

        private NthChildEvenSelector() : base(PseudoSelectorPrefix.PseudoFunctionNthchild, 2, 0)
        {
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Format(":{1}({0})", PseudoSelectorPrefix.NthChildEven, this.PseudoSelector);
        }
    }
}