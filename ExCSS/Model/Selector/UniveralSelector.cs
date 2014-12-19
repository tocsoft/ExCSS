using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class UniveralSelector : BaseSelector
    {
        static UniveralSelector _instance;

        public static UniveralSelector Instance
        {
            get { return _instance ?? (_instance = new UniveralSelector()); }
        }

        private UniveralSelector()
        {
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return "*";
        }
    }
}
