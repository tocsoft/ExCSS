﻿using System;
using NUnit.Framework;

namespace ExCSS.Tests
{
    [TestFixture]
    public class PropertyFixture
    {
        [Test]
        public void Parser_Finds_Multiple_Properties()
        {
            
            var css = Parser.Parse(".class{border: solid 1px red;height: 5px} .other{margin: 0;}");

            Assert.AreEqual(2, css.Rules.Count);
            Assert.AreEqual(2, css.StyleRules[0].Declarations.Count);
        }

        [Test]
        public void Force_Long_Html_Color()
        {
            var color = HtmlColor.FromHex("FF00FF");
            var colorString = color.ToString(true, false);

            Assert.AreEqual(colorString.Length, 7);
        }

        [Test]
        public void Converts_Rgb_To_Hex()
        {
            
            var css = Parser.Parse(".class{color:rgb(10,255,34);");
            Assert.AreEqual(".class{color:#0AFF22;}", css.ToString());
        }


        [Test]
        public void Converts_Rgba_With_Opacity_To_Hex()
        {
            
            var css = Parser.Parse(".class{color:rgba(10, 255, 34, 0.4);");
            Assert.AreEqual(".class{color:rgba(10,255,34,0.4);}", css.ToString());
        }

        [Test]
        public void Converts_Rgba_To_Hex()
        {
            
            var css = Parser.Parse(".class{color:rgba(255, 0, 255, 1);");
            Assert.AreEqual(".class{color:#F0F;}", css.ToString());
        }

        [Test]
        public void Converts_Hsl_To_Hex()
        {
            
            var css = Parser.Parse(".class{color:hsl(0, 100, 50);");
            Assert.AreEqual(".class{color:#F00;}", css.ToString());
        }

        [Test]
        public void Converts_Hsl_To_Hex_Percentage()
        {
            
            var css = Parser.Parse(".class{color:hsl(0,100%,50%);");
            Assert.AreEqual(".class{color:#F00;}", css.ToString());
        }

        [Test]
        public void PrimitiveString_DoubleQuoteAndEscapeControlCharacters()
        {
            
            var css = Parser.Parse(@".body:after{content:""\A\273C\A"";}");
            Assert.AreEqual(@".body:after{content:""\A✼\A"";}", css.ToString());
        }

        [Test]
        public void PrimitiveString_SingleQuoteStringsWithoutControlCharacters()
        {
            
            var css = Parser.Parse(@".body:after{content:'boo';}");
            Assert.AreEqual(@".body:after{content:'boo';}", css.ToString());
        }

        [Test]
        public void PrimitiveString_EmptyValue()
        {
            
            var css = Parser.Parse(@".body:after{content:'';}");
            Assert.AreEqual(@".body:after{content:'';}", css.ToString());
        }

    }
}
