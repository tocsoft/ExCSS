using System;
using ExCSS.Model;
// ReSharper disable once CheckNamespace

namespace ExCSS
{
    public sealed class IdSelector : BaseSelector
    {
        private readonly string _id;

        public string Id { get { return _id; } }

        public IdSelector(string id)
        {
            _id = id;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return "#" + _id;
        }
    }
}
