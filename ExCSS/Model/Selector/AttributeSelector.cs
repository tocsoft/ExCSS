using System;
using ExCSS.Model;

namespace ExCSS
{
    public sealed class AttributeSelector : BaseSelector
    {
        private readonly string _attribute;
        private readonly AttributeOperator _operand;
        private readonly string _value;
        private readonly string _valueAsString;

        public string Attribute { get { return _attribute; } }

        public AttributeOperator Operand { get { return _operand; } }

        public string Value { get { return _value; } }

        public AttributeSelector(string attribute) : this(attribute, AttributeOperator.Unmatched, null)
        {
            _attribute = attribute;
        }

        public AttributeSelector(string attribute, AttributeOperator operand, string value)
        {
            if (operand != AttributeOperator.Unmatched)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "value must be set if matching");
                }
            }
            _attribute = attribute;
            _operand = operand;
            _value = value;
            _valueAsString = GetValueAsString(value);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            string formatString = (string.IsNullOrWhiteSpace(_value) ? "[{0}]" : "[{0}=\"{1}\"]");
            switch (_operand)
            {
                case AttributeOperator.Unmatched:
                    formatString = "[{0}]";
                    break;
                case AttributeOperator.Match:
                    formatString = "[{0}=\"{1}\"]";
                    break;
                case AttributeOperator.NegatedMatch:
                    formatString = "[{0}!=\"{1}\"]";
                    break;
                case AttributeOperator.SpaceSeparated:
                    formatString = "[{0}~=\"{1}\"]";
                    break;
                case AttributeOperator.StartsWith:
                    formatString = "[{0}^=\"{1}\"]";
                    break;
                case AttributeOperator.EndsWith:
                    formatString = "[{0}$=\"{1}\"]";
                    break;
                case AttributeOperator.Contains:
                    formatString = "[{0}*=\"{1}\"]";
                    break;
                case AttributeOperator.DashSeparated:
                    formatString = "[{0}|=\"{1}\"]";
                    break;
            }

            return string.Format(formatString, _attribute, _valueAsString);
        }



        public static string GetValueAsString(string value)
        {
            if (value == null)
            {
                return null;
            }

            var containsSpace = false;

            for (var i = 0; i < value.Length; i++)
            {
                if (!value[i].IsSpaceCharacter())
                {
                    continue;
                }
                containsSpace = true;
                break;
            }

            if (!containsSpace)
            {
                return value;
            }

            if (value.IndexOf(Specification.SingleQuote) != -1)
            {
                return '"' + value + '"';
            }

            return "'" + value + "'";
        }
    }

}
