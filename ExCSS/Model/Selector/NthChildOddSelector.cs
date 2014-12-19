
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public class NthChildOddSelector : NthChildSelector, IToString
    {
        static NthChildOddSelector _instance;

        public static NthChildOddSelector Instance
        {
            get { return _instance ?? (_instance = new NthChildOddSelector()); }
        }

        private NthChildOddSelector() : base(PseudoSelectorPrefix.PseudoFunctionNthchild, 2, 1)
        {
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Format(":{1}({0})", PseudoSelectorPrefix.NthChildOdd, this.PseudoSelector);
        }
    }
}