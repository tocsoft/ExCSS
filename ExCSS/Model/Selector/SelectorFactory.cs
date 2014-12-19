using System;
using System.Linq;
using System.Globalization;
using ExCSS.Model;
using ExCSS.Model.TextBlocks;

// ReSharper disable once CheckNamespace
namespace ExCSS
{
    internal sealed class SelectorFactory
    {
        private SelectorOperation _selectorOperation;
        private BaseSelector _currentSelector;
        private SelectorList _aggregateSelectorList;
        private ComplexSelector _complexSelector;
        private bool _hasCombinator;
        private Combinator _combinator;
        private SelectorFactory _nestedSelectorFactory;
        private string _attributeName;
        private string _attributeValue;
        private string _attributeOperator;

        internal SelectorFactory()
        {
            ResetFactory();
        }

        internal BaseSelector GetSelector()
        {
            if (_complexSelector != null)
            {
                _complexSelector.ConcludeSelector(_currentSelector);
                _currentSelector = _complexSelector;
            }

            if (_aggregateSelectorList == null || _aggregateSelectorList.Length == 0)
            {
                return _currentSelector ?? UniveralSelector.Instance;
            }

            if (_currentSelector == null && _aggregateSelectorList.Length == 1)
            {
                return _aggregateSelectorList[0];
            }

            if (_currentSelector == null)
            {
                return _aggregateSelectorList;
            }

            _aggregateSelectorList.AppendSelector(_currentSelector);
            _currentSelector = null;

            return _aggregateSelectorList;
        }

        internal void Apply(Block token)
        {
            switch (_selectorOperation)
            {
                case SelectorOperation.Data:
                    ParseSymbol(token);
                    break;

                case SelectorOperation.Class:
                    PraseClass(token);
                    break;

                case SelectorOperation.Attribute:
                    ParseAttribute(token);
                    break;

                case SelectorOperation.AttributeOperator:
                    ParseAttributeOperator(token);
                    break;

                case SelectorOperation.AttributeValue:
                    ParseAttributeValue(token);
                    break;

                case SelectorOperation.AttributeEnd:
                    ParseAttributeEnd(token);
                    break;

                case SelectorOperation.PseudoClass:
                    ParsePseudoClass(token);
                    break;

                case SelectorOperation.PseudoClassFunction:
                    ParsePseudoClassFunction(token);
                    break;

                case SelectorOperation.PseudoClassFunctionEnd:
                    PrasePseudoClassFunctionEnd(token);
                    break;

                case SelectorOperation.PseudoElement:
                    ParsePseudoElement(token);
                    break;
            }
        }

        internal SelectorFactory ResetFactory()
        {
            _attributeName = null;
            _attributeValue = null;
            _attributeOperator = string.Empty;
            _selectorOperation = SelectorOperation.Data;
            _combinator = Combinator.Descendent;
            _hasCombinator = false;
            _currentSelector = null;
            _aggregateSelectorList = null;
            _complexSelector = null;

            return this;
        }

        private void ParseSymbol(Block token)
        {
            switch (token.GrammarSegment)
            {
                // Attribute [A]
                case GrammarSegment.SquareBraceOpen:
                    _attributeName = null;
                    _attributeValue = null;
                    _attributeOperator = string.Empty;
                    _selectorOperation = SelectorOperation.Attribute;
                    return;

                // Pseudo :P
                case GrammarSegment.Colon:
                    _selectorOperation = SelectorOperation.PseudoClass;
                    return;

                // ID #I
                case GrammarSegment.Hash:
                    Insert(new IdSelector(((SymbolBlock)token).Value));
                    return;

                // Type E
                case GrammarSegment.Ident:
                    //this feels a little dirt
                    InsertElementWithOptionalClasses(((SymbolBlock)token).Value);
                    return;

                // Whitespace
                case GrammarSegment.Whitespace:
                    Insert(Combinator.Descendent);
                    return;

                case GrammarSegment.Delimiter:
                    ParseDelimiter(token);
                    return;

                case GrammarSegment.Comma:
                    InsertCommaDelimited();
                    return;
            }
        }

        private void InsertElementWithOptionalClasses(string value)
        {
            var parts = value.Split('.');
            Insert(new ElementSelector(parts.First()));

            foreach (var c in parts.Skip(1))
            {
                Insert(new ClassSelector(c));
            }
        }

        private void ParseAttribute(Block token)
        {
            if (token.GrammarSegment == GrammarSegment.Whitespace)
            {
                return;
            }

            _selectorOperation = SelectorOperation.AttributeOperator;

            switch (token.GrammarSegment)
            {
                case GrammarSegment.Ident:
                    _attributeName = ((SymbolBlock)token).Value;
                    break;

                case GrammarSegment.String:
                    _attributeName = ((StringBlock)token).Value;
                    break;

                default:
                    _selectorOperation = SelectorOperation.Data;
                    break;
            }
        }

        private void ParseAttributeOperator(Block token)
        {
            if (token.GrammarSegment == GrammarSegment.Whitespace)
            {
                return;
            }

            _selectorOperation = SelectorOperation.AttributeValue;

            if (token.GrammarSegment == GrammarSegment.SquareBracketClose)
            {
                ParseAttributeEnd(token);
            }
            else if (token is MatchBlock || token.GrammarSegment == GrammarSegment.Delimiter)
            {
                _attributeOperator = token.ToString();
            }
            else
            {
                _selectorOperation = SelectorOperation.AttributeEnd;
            }
        }

        private void ParseAttributeValue(Block token)
        {
            if (token.GrammarSegment == GrammarSegment.Whitespace)
            {
                return;
            }

            _selectorOperation = SelectorOperation.AttributeEnd;

            switch (token.GrammarSegment)
            {
                case GrammarSegment.Ident:
                    _attributeValue = ((SymbolBlock)token).Value;
                    break;

                case GrammarSegment.String:
                    _attributeValue = ((StringBlock)token).Value;
                    break;

                case GrammarSegment.Number:
                    _attributeValue = ((NumericBlock)token).Value.ToString(CultureInfo.InvariantCulture);
                    break;

                default:
                    _selectorOperation = SelectorOperation.Data;
                    break;
            }
        }

        private void ParseAttributeEnd(Block token)
        {
            if (token.GrammarSegment == GrammarSegment.Whitespace)
            {
                return;
            }

            _selectorOperation = SelectorOperation.Data;

            if (token.GrammarSegment != GrammarSegment.SquareBracketClose)
            {
                return;
            }

            switch (_attributeOperator)
            {
                case "=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.Match, _attributeValue));
                    break;

                case "~=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.SpaceSeparated, _attributeValue));
                    break;

                case "|=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.DashSeparated, _attributeValue));
                    break;

                case "^=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.StartsWith, _attributeValue));
                    break;

                case "$=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.EndsWith, _attributeValue));
                    break;

                case "*=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.Contains, _attributeValue));
                    break;

                case "!=":
                    Insert(new AttributeSelector(_attributeName, AttributeOperator.NegatedMatch, _attributeValue));
                    break;

                default:
                    Insert(new AttributeSelector(_attributeName));
                    break;
            }
        }

        private void ParsePseudoClass(Block token)
        {
            _selectorOperation = SelectorOperation.Data;

            switch (token.GrammarSegment)
            {
                case GrammarSegment.Colon:
                    _selectorOperation = SelectorOperation.PseudoElement;
                    break;

                case GrammarSegment.Function:
                    _attributeName = ((SymbolBlock)token).Value;
                    _attributeValue = string.Empty;
                    _selectorOperation = SelectorOperation.PseudoClassFunction;

                    if (_nestedSelectorFactory != null)
                    {
                        _nestedSelectorFactory.ResetFactory();
                    }

                    break;

                case GrammarSegment.Ident:
                    var pseudoSelector = GetPseudoSelector(token);

                    if (pseudoSelector != null)
                    {
                        Insert(pseudoSelector);
                    }
                    break;
            }
        }

        private void ParsePseudoElement(Block token)
        {
            if (token.GrammarSegment != GrammarSegment.Ident)
            {
                return;
            }
            var data = ((SymbolBlock)token).Value;

            switch (data)
            {
                case PseudoSelectorPrefix.PseudoElementBefore:
                    Insert(new PseudoElementSelector(PseudoSelectorPrefix.PseudoElementBefore));
                    break;

                case PseudoSelectorPrefix.PseudoElementAfter:
                    Insert(new PseudoElementSelector(PseudoSelectorPrefix.PseudoElementAfter));
                    break;

                case PseudoSelectorPrefix.PseudoElementSelection:
                    Insert(new PseudoElementSelector(PseudoSelectorPrefix.PseudoElementSelection));
                    break;

                case PseudoSelectorPrefix.PseudoElementFirstline:
                    Insert(new PseudoElementSelector(PseudoSelectorPrefix.PseudoElementFirstline));
                    break;

                case PseudoSelectorPrefix.PseudoElementFirstletter:
                    Insert(new PseudoElementSelector(PseudoSelectorPrefix.PseudoElementFirstletter));
                    break;

                default:
                    Insert(new PseudoElementSelector(data));
                    break;
            }
        }

        private void PraseClass(Block token)
        {
            _selectorOperation = SelectorOperation.Data;

            if (token.GrammarSegment == GrammarSegment.Ident)
            {
                Insert(new ClassSelector(((SymbolBlock)token).Value));
            }
        }

        private void ParsePseudoClassFunction(Block token)
        {
            if (token.GrammarSegment == GrammarSegment.Whitespace)
            {
                return;
            }

            switch (_attributeName)
            {
                case PseudoSelectorPrefix.PseudoFunctionNthchild:
                case PseudoSelectorPrefix.PseudoFunctionNthlastchild:
                case PseudoSelectorPrefix.PseudoFunctionNthOfType:
                case PseudoSelectorPrefix.PseudoFunctionNthLastOfType:
                    {
                        switch (token.GrammarSegment)
                        {
                            case GrammarSegment.Ident:
                            case GrammarSegment.Number:
                            case GrammarSegment.Dimension:
                                _attributeValue += token.ToString();
                                return;

                            case GrammarSegment.Delimiter:
                                var chr = ((DelimiterBlock)token).Value;

                                if (chr == Specification.PlusSign || chr == Specification.MinusSign)
                                {
                                    _attributeValue += chr;
                                    return;
                                }

                                break;
                        }

                        break;
                    }
                case PseudoSelectorPrefix.PseudoFunctionNot:
                    {
                        if (_nestedSelectorFactory == null)
                        {
                            _nestedSelectorFactory = new SelectorFactory();
                        }

                        if (token.GrammarSegment != GrammarSegment.ParenClose || _nestedSelectorFactory._selectorOperation != SelectorOperation.Data)
                        {
                            _nestedSelectorFactory.Apply(token);
                            return;
                        }

                        break;
                    }
                case PseudoSelectorPrefix.PseudoFunctionDir:
                    {
                        if (token.GrammarSegment == GrammarSegment.Ident)
                        {
                            _attributeValue = ((SymbolBlock)token).Value;
                        }

                        _selectorOperation = SelectorOperation.PseudoClassFunctionEnd;
                        return;
                    }
                case PseudoSelectorPrefix.PseudoFunctionLang:
                    {
                        if (token.GrammarSegment == GrammarSegment.Ident)
                        {
                            _attributeValue = ((SymbolBlock)token).Value;
                        }

                        _selectorOperation = SelectorOperation.PseudoClassFunctionEnd;
                        return;
                    }
                case PseudoSelectorPrefix.PseudoFunctionContains:
                    {
                        switch (token.GrammarSegment)
                        {
                            case GrammarSegment.String:
                                _attributeValue = ((StringBlock)token).Value;
                                break;

                            case GrammarSegment.Ident:
                                _attributeValue = ((SymbolBlock)token).Value;
                                break;
                        }

                        _selectorOperation = SelectorOperation.PseudoClassFunctionEnd;
                        return;
                    }
            }

            PrasePseudoClassFunctionEnd(token);
        }

        private void PrasePseudoClassFunctionEnd(Block token)
        {
            _selectorOperation = SelectorOperation.Data;

            if (token.GrammarSegment != GrammarSegment.ParenClose)
            {
                return;
            }

            switch (_attributeName)
            {
                case PseudoSelectorPrefix.PseudoFunctionNthchild:
                case PseudoSelectorPrefix.PseudoFunctionNthlastchild:
                case PseudoSelectorPrefix.PseudoFunctionNthOfType:
                case PseudoSelectorPrefix.PseudoFunctionNthLastOfType:
                    Insert(GetNthChildSelector());
                    break;
                case PseudoSelectorPrefix.PseudoFunctionNot:
                    Insert(new PseudoNotFunctionSelector(_nestedSelectorFactory.GetSelector()));
                    break;

                case PseudoSelectorPrefix.PseudoFunctionDir:
                    Insert(new PseudoFunctionSelector(PseudoSelectorPrefix.PseudoFunctionDir, _attributeValue));
                    break;
                case PseudoSelectorPrefix.PseudoFunctionLang:
                    Insert(new PseudoFunctionSelector(PseudoSelectorPrefix.PseudoFunctionLang, _attributeValue));
                    break;
                case PseudoSelectorPrefix.PseudoFunctionContains:
                    Insert(new PseudoFunctionSelector(PseudoSelectorPrefix.PseudoFunctionContains, _attributeValue));
                    break;
            }
        }

        private void InsertCommaDelimited()
        {
            if (_currentSelector == null)
            {
                return;
            }

            if (_aggregateSelectorList == null)
            {
                _aggregateSelectorList = new MultipleSelectorList();
            }

            if (_complexSelector != null)
            {
                _complexSelector.ConcludeSelector(_currentSelector);
                _aggregateSelectorList.AppendSelector(_complexSelector);
                _complexSelector = null;
            }
            else
            {
                _aggregateSelectorList.AppendSelector(_currentSelector);
            }

            _currentSelector = null;
        }

        private void Insert(BaseSelector selector)
        {
            if (_currentSelector != null)
            {
                if (!_hasCombinator)
                {
                    var compound = _currentSelector as AggregateSelectorList;

                    if (compound == null)
                    {
                        compound = new AggregateSelectorList();
                        compound.AppendSelector(_currentSelector);
                    }

                    compound.AppendSelector(selector);
                    _currentSelector = compound;
                }
                else
                {
                    if (_complexSelector == null)
                    {
                        _complexSelector = new ComplexSelector();
                    }

                    _complexSelector.AppendSelector(_currentSelector, _combinator);
                    _combinator = Combinator.Descendent;
                    _hasCombinator = false;
                    _currentSelector = selector;
                }
            }
            else
            {
                if (_currentSelector == null && _complexSelector == null && _combinator == Combinator.Namespace)
                {
                    _complexSelector = new ComplexSelector();
                    _complexSelector.AppendSelector(EmptySelector.Instance, _combinator);
                    _currentSelector = selector;
                }
                else
                {
                    _combinator = Combinator.Descendent;
                    _hasCombinator = false;
                    _currentSelector = selector;
                }
            }
        }

        private void Insert(Combinator combinator)
        {
            _hasCombinator = true;

            if (combinator != Combinator.Descendent)
            {
                _combinator = combinator;
            }
        }

        private void ParseDelimiter(Block token)
        {
            switch (((DelimiterBlock)token).Value)
            {
                case Specification.Comma:
                    InsertCommaDelimited();
                    return;

                case Specification.GreaterThan:
                    Insert(Combinator.Child);
                    return;

                case Specification.PlusSign:
                    Insert(Combinator.AdjacentSibling);
                    return;

                case Specification.Tilde:
                    Insert(Combinator.Sibling);
                    return;

                case Specification.Asterisk:
                    Insert(UniveralSelector.Instance);
                    return;

                case Specification.Period:
                    _selectorOperation = SelectorOperation.Class;
                    return;

                case Specification.Pipe:
                    Insert(Combinator.Namespace);
                    return;
            }
        }


        private BaseSelector GetNthChildSelector()
        {
            int offset = 0;
            int step = 0;

            if (_attributeValue.Equals(PseudoSelectorPrefix.NthChildOdd, StringComparison.OrdinalIgnoreCase))
            {
                return NthChildOddSelector.Instance;
            }
            else if (_attributeValue.Equals(PseudoSelectorPrefix.NthChildEven, StringComparison.OrdinalIgnoreCase))
            {
                return NthChildEvenSelector.Instance;
            }
            else if (!int.TryParse(_attributeValue, out offset))
            {
                var index = _attributeValue.IndexOf(PseudoSelectorPrefix.NthChildN, StringComparison.OrdinalIgnoreCase);

                if (_attributeValue.Length <= 0 || index == -1)
                {
                    return GetNthChildSelector(0, 0);
                }

                var first = _attributeValue.Substring(0, index).Replace(" ", "");

                var second = "";

                if (_attributeValue.Length > index + 1)
                {
                    second = _attributeValue.Substring(index + 1).Replace(" ", "");
                }

                if (first == string.Empty || (first.Length == 1 && first[0] == Specification.PlusSign))
                {
                    step = 1;
                }
                else if (first.Length == 1 && first[0] == Specification.MinusSign)
                {
                    step = -1;
                }
                else
                {
                    int _step;
                    if (int.TryParse(first, out _step))
                    {
                        step = _step;
                    }
                }

                if (second == string.Empty)
                {
                    offset = 0;
                }
                else
                {
                    int _offset;
                    if (int.TryParse(second, out _offset))
                    {
                        offset = _offset;
                    }
                }
            }

            return GetNthChildSelector(step, offset);
        }

        private BaseSelector GetNthChildSelector(int step, int offset)
        {
            switch (_attributeName)
            {
                case PseudoSelectorPrefix.PseudoFunctionNthchild:
                    return new NthFirstChildSelector(step, offset);

                case PseudoSelectorPrefix.PseudoFunctionNthlastchild:
                    return new NthLastChildSelector(step, offset);

                case PseudoSelectorPrefix.PseudoFunctionNthOfType:
                    return new NthOfTypeSelector(step, offset);

                case PseudoSelectorPrefix.PseudoFunctionNthLastOfType:
                    return new NthLastOfTypeSelector(step, offset);
            }
            return new NthFirstChildSelector(step, offset);
        }

        private static BaseSelector GetPseudoSelector(Block token)
        {
            switch (((SymbolBlock)token).Value)
            {
                case PseudoSelectorPrefix.PseudoRoot:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoRoot);

                case PseudoSelectorPrefix.PseudoFirstOfType:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoFirstOfType);

                case PseudoSelectorPrefix.PseudoLastoftype:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoLastoftype);

                case PseudoSelectorPrefix.PseudoOnlychild:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoOnlychild);

                case PseudoSelectorPrefix.PseudoOnlyOfType:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoOnlyOfType);

                case PseudoSelectorPrefix.PseudoFirstchild:
                    return FirstChildSelector.Instance;

                case PseudoSelectorPrefix.PseudoLastchild:
                    return LastChildSelector.Instance;

                case PseudoSelectorPrefix.PseudoEmpty:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoEmpty);

                case PseudoSelectorPrefix.PseudoLink:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoLink);

                case PseudoSelectorPrefix.PseudoVisited:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoVisited);

                case PseudoSelectorPrefix.PseudoActive:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoActive);

                case PseudoSelectorPrefix.PseudoHover:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoHover);

                case PseudoSelectorPrefix.PseudoFocus:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoFocus);

                case PseudoSelectorPrefix.PseudoTarget:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoTarget);

                case PseudoSelectorPrefix.PseudoEnabled:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoEnabled);

                case PseudoSelectorPrefix.PseudoDisabled:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoDisabled);

                case PseudoSelectorPrefix.PseudoDefault:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoDefault);

                case PseudoSelectorPrefix.PseudoChecked:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoChecked);

                case PseudoSelectorPrefix.PseudoIndeterminate:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoIndeterminate);

                case PseudoSelectorPrefix.PseudoUnchecked:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoUnchecked);

                case PseudoSelectorPrefix.PseudoValid:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoValid);

                case PseudoSelectorPrefix.PseudoInvalid:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoInvalid);

                case PseudoSelectorPrefix.PseudoRequired:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoRequired);

                case PseudoSelectorPrefix.PseudoReadonly:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoReadonly);

                case PseudoSelectorPrefix.PseudoReadwrite:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoReadwrite);

                case PseudoSelectorPrefix.PseudoInrange:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoInrange);

                case PseudoSelectorPrefix.PseudoOutofrange:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoOutofrange);

                case PseudoSelectorPrefix.PseudoOptional:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoOptional);

                case PseudoSelectorPrefix.PseudoElementBefore:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoElementBefore);

                case PseudoSelectorPrefix.PseudoElementAfter:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoElementAfter);

                case PseudoSelectorPrefix.PseudoElementFirstline:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoElementFirstline);

                case PseudoSelectorPrefix.PseudoElementFirstletter:
                    return new PseudoClassSelector(PseudoSelectorPrefix.PseudoElementFirstletter);

                default:
                    return new PseudoClassSelector(token.ToString());
            }
        }
    }
}
