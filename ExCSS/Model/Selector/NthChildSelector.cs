
// ReSharper disable once CheckNamespace
namespace ExCSS
{
    public abstract class NthChildSelector : BasePseudoSelector, IToString
    {
        private int _step;
        private int _offset;

        public int Step { get { return _step; } }

        public int Offset { get { return _offset; } }

        public NthChildSelector(string functionName) : base(functionName)
        {
        }

        public NthChildSelector(string functionName, int step, int offset) : base(functionName)
        {
            _step = step;
            _offset = offset;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var format = _offset < 0
                ? "({0}n{1})"
                : "({0}n+{1})";

            return base.ToString(friendlyFormat, indentation) + string.Format(format, _step, _offset);
        }
    }
}