using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ExCSS.Tests
{
    [TestFixture]
    public class SelectorTypesFixture
    {
        [Test]
        public void Parser_Reads_Mixed_Selectors()
        {
            
            var css = Parser.Parse("button,.button,input[type=button]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as MultipleSelectorList;
            Assert.AreEqual(3, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>().And.Property("Element").EqualTo("button"));
            Assert.That(selector[1], Is.TypeOf<ClassSelector>().And.Property("Class").EqualTo("button"));
            Assert.That(selector[2], Is.TypeOf<AggregateSelectorList>());

            var selector2 = selector[2] as AggregateSelectorList;

            Assert.AreEqual(2, selector2.Length);
            Assert.That(selector2[0], Is.TypeOf<ElementSelector>().And.Property("Element").EqualTo("input"));
            Assert.That(selector2[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("type")
                .And.Property("Operand").EqualTo(AttributeOperator.Match)
                .And.Property("Value").EqualTo("button"));
        }

        [Test]
        public void Parser_Reads_Class_Selectors()
        {
            
            var css = Parser.Parse(".one, .two{}");

            var rules = css.Rules;
            var selector = (rules[0] as StyleRule).Selector as MultipleSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ClassSelector>().And.Property("Class").EqualTo("one"));
            Assert.That(selector[1], Is.TypeOf<ClassSelector>().And.Property("Class").EqualTo("two"));
        }

        [Test]
        public void Parser_Reads_Element_Selectors()
        {
            
            var css = Parser.Parse("E{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ElementSelector>().And.Property("Element").EqualTo("E"));
        }

        [Test]
        public void Parser_Reads_Empty_Attribute_Element_Selectors()
        {
            
            var css = Parser.Parse("E[foo]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>().And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.Unmatched));
        }

        [Test]
        public void Parser_Reads_Quoted_Attribute_Element_Selectors()
        {
            
            var css = Parser.Parse("E[foo=\"bar\"]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.Match)
                .And.Property("Value").EqualTo("bar"));
        }

        [Test]
        public void Parser_Reads_Space_Separated_Attribute()
        {
            
            var css = Parser.Parse("E[foo~=\"bar\"]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.SpaceSeparated)
                .And.Property("Value").EqualTo("bar"));
        }

        [Test]
        public void Parser_Reads_Starts_With_Attribute()
        {
            
            var css = Parser.Parse("E[foo^=\"bar\"]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.StartsWith)
                .And.Property("Value").EqualTo("bar"));
        }

        [Test]
        public void Parser_Reads_Ends_With_Attribute()
        {
            
            var css = Parser.Parse("E[foo$=\"bar\"]{}");

            var rules = css.Rules;
            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.EndsWith)
                .And.Property("Value").EqualTo("bar"));
        }

        [Test]
        public void Parser_Reads_Contains_Attribute()
        {
            
            var css = Parser.Parse("E[foo*=\"bar\"]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.Contains)
                .And.Property("Value").EqualTo("bar"));
        }

        [Test]
        public void Parser_Reads_Dash_Attribute()
        {
            
            var css = Parser.Parse("E[foo|=\"bar\"]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.DashSeparated)
                .And.Property("Value").EqualTo("bar"));
        }

        [Test]
        public void Parser_Reads_Multiple_Attribute()
        {
            
            var css = Parser.Parse("E[foo=\"bar\"][rel=\"important\"]{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(3, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("foo")
                .And.Property("Operand").EqualTo(AttributeOperator.Match)
                .And.Property("Value").EqualTo("bar"));
            Assert.That(selector[2], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("rel")
                .And.Property("Operand").EqualTo(AttributeOperator.Match)
                .And.Property("Value").EqualTo("important"));
        }

        [Test]
        public void Parser_Reads_Pseudo_Selectors()
        {
            
            var css = Parser.Parse("E:pseudo{}");

            var rules = css.Rules;

            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<PseudoClassSelector>()
                .And.Property("Class").EqualTo("pseudo"));
        }

        [Test]
        public void Parser_Reads_Pseudo_Element()
        {
            
            var css = Parser.Parse("E::first-line{}");

            var rules = css.Rules;

            Assert.AreEqual("E::first-line{}", rules[0].ToString());


            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<PseudoElementSelector>()
                .And.Property("Element").EqualTo("first-line"));
        }

        [Test]
        public void Parser_Reads_Class_Attributed_Elements()
        {
            
            var css = Parser.Parse("E.warning{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>().And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<ClassSelector>()
                .And.Property("Class").EqualTo("warning"));
        }

        [Test]
        public void Parser_Reads_Id_Elements()
        {
            
            var css = Parser.Parse("E#id{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>().And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<IdSelector>()
                .And.Property("Id").EqualTo("id"));
        }

        [Test]
        public void Parser_Reads_Descendant_Elements()
        {
            
            var css = Parser.Parse("E F{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = (rules[0] as StyleRule).Selector as ComplexSelector;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector.First().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector.Last().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("F"));


            Assert.That(selector.First().Delimiter, Is.EqualTo(Combinator.Descendent));
            Assert.That(selector.Last().Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Child_Elements()
        {
            
            var css = Parser.Parse("E > F{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = (rules[0] as StyleRule).Selector as ComplexSelector;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector.First().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector.Last().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("F"));

            Assert.That(selector.First().Delimiter, Is.EqualTo(Combinator.Child));
            Assert.That(selector.Last().Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Adjacent_Sibling_Elements()
        {
            
            var css = Parser.Parse("E + F{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = (rules[0] as StyleRule).Selector as ComplexSelector;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector.First().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector.Last().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("F"));


            Assert.That(selector.First().Delimiter, Is.EqualTo(Combinator.AdjacentSibling));
            Assert.That(selector.Last().Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_General_Sibling_Elements()
        {
            
            var css = Parser.Parse("E ~ F{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = (rules[0] as StyleRule).Selector as ComplexSelector;
            Assert.AreEqual(2, selector.Length);
            Assert.That(selector.First().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector.Last().Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("F"));

            Assert.That(selector.First().Delimiter, Is.EqualTo(Combinator.Sibling));
            Assert.That(selector.Last().Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Multiple_Pseudo_Classes()
        {
            
            var css = Parser.Parse("E:focus:hover{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(3, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<PseudoClassSelector>()
                .And.Property("Class").EqualTo("focus"));
            Assert.That(selector[2], Is.TypeOf<PseudoClassSelector>()
                .And.Property("Class").EqualTo("hover"));

        }

        [Test]
        public void Parser_Reads_Element_Class_Pseudo_Classes()
        {
            
            var css = Parser.Parse("E.class:hover{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = (rules[0] as StyleRule).Selector as AggregateSelectorList;
            Assert.AreEqual(3, selector.Length);
            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<ClassSelector>()
                .And.Property("Class").EqualTo("class"));
            Assert.That(selector[2], Is.TypeOf<PseudoClassSelector>()
                .And.Property("Class").EqualTo("hover"));
        }

        [Test]
        public void Parser_Reads_Global_Combinator()
        {
            
            var css = Parser.Parse("E * p{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = ((rules[0] as StyleRule).Selector as ComplexSelector).ToArray();
            Assert.AreEqual(3, selector.Length);
            Assert.That(selector[0].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1].Selector, Is.TypeOf<UniveralSelector>());
            Assert.That(selector[2].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("p"));
        }

        [Test]
        public void Parser_Reads_Global_Attribute()
        {
            
            var css = Parser.Parse("E p *[href]{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = ((rules[0] as StyleRule).Selector as ComplexSelector).ToArray();
            Assert.AreEqual(3, selector.Length);
            Assert.That(selector[0].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("p"));
            Assert.That(selector[2].Selector, Is.TypeOf<AggregateSelectorList>());
            var selector2 = selector[2].Selector as AggregateSelectorList;
            Assert.That(selector2[0], Is.TypeOf<UniveralSelector>());
            Assert.That(selector2[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("href")
                .And.Property("Operand").EqualTo(AttributeOperator.Unmatched));
        }

        [Test]
        public void Parser_Reads_Descendand_And_Child_Combinators()
        {
            
            var css = Parser.Parse("E F>G H{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = ((rules[0] as StyleRule).Selector as ComplexSelector).ToArray();
            Assert.AreEqual(4, selector.Length);

            Assert.That(selector[0].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("F"));
            Assert.That(selector[2].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("G"));
            Assert.That(selector[3].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("H"));

            Assert.That(selector[0].Delimiter, Is.EqualTo(Combinator.Descendent));
            Assert.That(selector[1].Delimiter, Is.EqualTo(Combinator.Child));
            Assert.That(selector[2].Delimiter, Is.EqualTo(Combinator.Descendent
                ));
            Assert.That(selector[3].Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Classed_Element_Combinators()
        {
            
            var css = Parser.Parse("E.warning + h2{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = ((rules[0] as StyleRule).Selector as ComplexSelector).ToArray();
            Assert.AreEqual(2, selector.Length);

            Assert.That(selector[0].Selector, Is.TypeOf<AggregateSelectorList>());

            Assert.That(selector[1].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("h2"));

            var selector2 = selector[0].Selector as AggregateSelectorList;

            Assert.That(selector2[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector2[1], Is.TypeOf<ClassSelector>()
                .And.Property("Class").EqualTo("warning"));

            Assert.That(selector[0].Delimiter, Is.EqualTo(Combinator.AdjacentSibling));
            Assert.That(selector[1].Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Descendand_And_Sibling_Combinators()
        {
            
            var css = Parser.Parse("E F+G{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = ((rules[0] as StyleRule).Selector as ComplexSelector).ToArray();
            Assert.AreEqual(3, selector.Length);

            Assert.That(selector[0].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("F"));
            Assert.That(selector[2].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("G"));

            Assert.That(selector[0].Delimiter, Is.EqualTo(Combinator.Descendent));
            Assert.That(selector[1].Delimiter, Is.EqualTo(Combinator.AdjacentSibling));
            Assert.That(selector[2].Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Attributed_Descendants()
        {
            
            var css = Parser.Parse("E + *[REL=up]{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<ComplexSelector>());
            var selector = ((rules[0] as StyleRule).Selector as ComplexSelector).ToArray();
            Assert.AreEqual(2, selector.Length);

            Assert.That(selector[0].Selector, Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));

            Assert.That(selector[1].Selector, Is.TypeOf<AggregateSelectorList>());

            var selector2 = selector[1].Selector as AggregateSelectorList;

            Assert.That(selector2[0], Is.TypeOf<UniveralSelector>());
            Assert.That(selector2[1], Is.TypeOf<AttributeSelector>()
                .And.Property("Attribute").EqualTo("REL")
                .And.Property("Operand").EqualTo(AttributeOperator.Match)
                .And.Property("Value").EqualTo("up"));

            Assert.That(selector[0].Delimiter, Is.EqualTo(Combinator.AdjacentSibling));
            Assert.That(selector[1].Delimiter, Is.EqualTo(Combinator.Child));
        }

        [Test]
        public void Parser_Reads_Chained_Classes()
        {
            
            var css = Parser.Parse("E.first.second{}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = ((rules[0] as StyleRule).Selector as AggregateSelectorList);
            Assert.AreEqual(3, selector.Length);

            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<ClassSelector>()
                .And.Property("Class").EqualTo("first"));
            Assert.That(selector[2], Is.TypeOf<ClassSelector>()
                .And.Property("Class").EqualTo("second"));
        }


        [Test]
        public void Parser_Reads_Pseudo_Functions()
        {
            
            var css = Parser.Parse("E:nth-child(n){}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = ((rules[0] as StyleRule).Selector as AggregateSelectorList);
            Assert.AreEqual(2, selector.Length);

            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<NthFirstChildSelector>()
                .And.Property("Step").EqualTo(1)
                .And.Property("Offset").EqualTo(0));

        }

        [Test]
        public void Parser_Reads_Pseudo_Functions_With_Negative_Rules()
        {
            
            var css = Parser.Parse("E:nth-last-of-type(-n+2){}");

            var rules = css.Rules;

            Assert.That((rules[0] as StyleRule).Selector, Is.TypeOf<AggregateSelectorList>());
            var selector = ((rules[0] as StyleRule).Selector as AggregateSelectorList);
            Assert.AreEqual(2, selector.Length);

            Assert.That(selector[0], Is.TypeOf<ElementSelector>()
                .And.Property("Element").EqualTo("E"));
            Assert.That(selector[1], Is.TypeOf<NthLastOfTypeSelector>()
                .And.Property("Step").EqualTo(-1)
                .And.Property("Offset").EqualTo(2));
        }

    }
}
