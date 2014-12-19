// ReSharper disable once CheckNamespace
namespace ExCSS
{
    internal sealed class LastChildSelector : BasePseudoSelector, IToString
    {
        LastChildSelector() : base(PseudoSelectorPrefix.PseudoLastchild)
        { }

        static LastChildSelector _instance;

        public static LastChildSelector Instance
        {
            get { return _instance ?? (_instance = new LastChildSelector()); }
        }
    }
}