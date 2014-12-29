﻿using System;
using NUnit.Framework;

namespace ExCSS.Tests
{
    [TestFixture]
    public class AtRuleFixture
    {
        #region Charset
        [Test]
        public void Parser_Reads_Character_Sets_Symbols()
        {
            var css = Parser.Parse("@charset 'utf-8';");

            var charset = css.CharsetDirectives;

            Assert.AreEqual("@charset 'utf-8';", charset[0].ToString());
        }

        [Test]
        public void Parser_Reads_Character_Sets_Strings()
        {
            var css = Parser.Parse("@charset 'utf-8';");
            var charset = css.CharsetDirectives;

            Assert.AreEqual("@charset 'utf-8';", charset[0].ToString());

            css = Parser.Parse("@charset \"utf-8\";");
            charset = css.CharsetDirectives;

            Assert.AreEqual("@charset 'utf-8';", charset[0].ToString());
        }
        #endregion

        #region Imports
        [Test]
        public void Parser_Reads_Imports_Double_Quoted()
        {
            var css = Parser.Parse("@import \"style.css\";");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css);", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_URL_Double_Quoted()
        {
            var css = Parser.Parse("@import url(\"style.css\");");

            var imports = css.ImportDirectives;


            Assert.AreEqual("@import url(style.css);", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_With_Single_Media()
        {
            var css = Parser.Parse("@import url(\"style.css\") print;");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css) print;", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_With_Multiple_Media()
        {
            var css = Parser.Parse("@import url(\"style.css\") projection, tv;");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css) projection, tv;", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_With_Constraints()
        {
            var css = Parser.Parse("@import url('style.css') handheld and (max-width: 400px);");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css) handheld and (max-width: 400px);", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_With_Plain_And_Quoted_Meida()
        {
            var css = Parser.Parse("@import url(style.css) screen \"Plain style\";");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css) screen 'Plain style';", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_With_Quoted_Media_And_Delimiters()
        {
            var css = Parser.Parse("@import url(style.css) \"Four-columns and dark\";");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css) 'Four-columns and dark';", imports[0].ToString());
        }

        [Test]
        public void Parser_Reads_Imports_With_Quoted_URL_And_Media()
        {
            var css = Parser.Parse("@import \"style.css\" \"Style Sheet\";");

            var imports = css.ImportDirectives;

            Assert.AreEqual("@import url(style.css) 'Style Sheet';", imports[0].ToString());
        }
        #endregion

        #region Fontface
        [Test]
        public void Parser_Reads_Fontface()
        {
            var css = Parser.Parse(
@"@font-face
{
    font-family: testFont;
    src: url(SomeFont.ttf),
         url(SomeFont_Italic.tff); 
}");

            var fontfaces = css.FontFaceDirectives;

            Assert.AreEqual("@font-face{font-family:testFont;src:url(SomeFont.ttf),url(SomeFont_Italic.tff);}", fontfaces[0].ToString());
        }

        #endregion

        #region Keyframes
        [Test]
        public void Parser_Reads_Keyframes()
        {
            var css = Parser.Parse("@keyframes test-keyframes{from {top:0px;} to {top:200px;}}");

            var keyframes = css.KeyframeDirectives;

            Assert.AreEqual(@"@keyframes test-keyframes{from{top:0px;}to{top:200px;}}", keyframes[0].ToString());
        }

        [Test]
        public void KeyFrames_ToString_Test()
        {
            Assert.AreEqual("@keyframes ixp-bounce{0%,100%{transform:translateY(0);}}",
                Parser.Parse("@keyframes ixp-bounce { 0%, 100% { transform: translateY(0);}}").ToString());
        }

        [Test]
        public void KeyFrames_ToString_With_Vendor_Prefix_Test()
        {
            Assert.AreEqual("@-moz-keyframes ixp-bounce{0%,100%{transform:translateY(0);}}",
                Parser.Parse("@-moz-keyframes ixp-bounce { 0%, 100% { transform: translateY(0);}}").ToString());
        }

        [Test]
        public void KeyFrames_Multi_Test()
        {
            var css = @"
            @-moz-keyframes ixp-tada {
                0% {transform: scale(111);}
                10%,20% {transform: scale(222) rotate(-3deg);}   
                100% {transform: scale(333) rotate(0);}
            }
            @keyframes ixp-tada {
                0% {transform: scale(666);}
                100% {transform: scale(1) rotate(777);}
            }
            ";

            var result = Parser.Parse(css).ToString();
            Console.WriteLine(result);
            Assert.That(result.Contains("@keyframes ixp-tada"));
            Assert.AreEqual(@"@-moz-keyframes ixp-tada{0%{transform:scale(111);}10%,20%{transform:scale(222) rotate(-3deg);}100%{transform:scale(333) rotate(0);}}@keyframes ixp-tada{0%{transform:scale(666);}100%{transform:scale(1) rotate(777);}}", result);
        }

        [Test]
        public void KeyFrames_Browser_Prefix_Works_Test()
        {
            StyleSheet result = null;
            Assert.DoesNotThrow(() =>
            {
                result = Parser.Parse(@"@-webkit-keyframes ixp-bounce { 100% { -webkit-transform: translateY(0); } }");
            });
            Assert.That(result.ToString(), Is.StringContaining("transform"));

        }

        #endregion

        #region Media
        [Test]
        public void Parser_Reads_Media_Queries()
        {
            var css = Parser.Parse("@media print {body { font-size: 12pt; } h1 { font-size: 24pt; }}");

            var media = css.MediaDirectives;

            Assert.AreEqual("@media print {body{font-size:12pt;}h1{font-size:24pt;}}", media[0].ToString());
        }

        #endregion

        #region Page
        [Test]
        public void Parser_Reads_Page_Directives()
        {
            var css = Parser.Parse("@page {size: auto;margin: 10%;}");

            var pages = css.PageDirectives;

            Assert.AreEqual("@page {size:auto;margin:10%;}", pages[0].ToString());
        }

        [Test]
        public void Parser_Reads_Page_Directives_With_Pseudo()
        {
            var css = Parser.Parse("@page :left {size: auto;margin: 10%;}");

            var pages = css.PageDirectives;

            Assert.AreEqual("@page :left{size:auto;margin:10%;}", pages[0].ToString());
        }

        #endregion

        #region Supports
        [Test]
        public void Parser_Reads_Supports_Directives()
        {
            var css = Parser.Parse("@supports (animation-name: test){h2 {top: 0px;float: none;}}");

            var supports = css.SupportsDirectives;

            Assert.AreEqual("@supports (animation-name: test){h2{top:0px;float:none;}}", supports[0].ToString());
        }
        #endregion

        #region Namespace
        [Test]
        public void Parser_Reads_Namespace_Directives_With_Prefix()
        {
            var css = Parser.Parse("@namespace toto \"http://toto.example.org\";");

            var namespaces = css.NamespaceDirectives;

            Assert.AreEqual("@namespace toto 'http://toto.example.org';", namespaces[0].ToString());
        }

        [Test]
        public void Parser_Reads_Namespace_Directives()
        {
            var css = Parser.Parse("@namespace \"http://toto.example.org\";");

            var namespaces = css.NamespaceDirectives;

            Assert.AreEqual("@namespace 'http://toto.example.org';", namespaces[0].ToString());
        }
        #endregion
    }
}
