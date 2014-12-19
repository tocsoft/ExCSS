using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class EmptySelector : BaseSelector
    {
        static EmptySelector _instance;

        public static EmptySelector Instance
        {
            get { return _instance ?? (_instance = new EmptySelector()); }
        }

        private EmptySelector()
        {
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return "";
        }
    }
}
