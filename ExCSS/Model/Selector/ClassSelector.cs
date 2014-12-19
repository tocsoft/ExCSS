using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class ClassSelector : BaseSelector
    {
        private readonly string _class;

        public string Class { get { return _class; } }

        public ClassSelector(string className)
        {
            _class = className;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return "." + _class;
        }
    }
}
