// ReSharper disable once CheckNamespace
namespace ExCSS
{
    internal sealed class FirstChildSelector : BasePseudoSelector, IToString
    {
        FirstChildSelector() : base(PseudoSelectorPrefix.PseudoFirstchild)
        { }

        static FirstChildSelector _instance;

        public static FirstChildSelector Instance
        {
            get { return _instance ?? (_instance = new FirstChildSelector()); }
        }
    }
}