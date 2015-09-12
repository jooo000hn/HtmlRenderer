﻿using System;
using System.Linq;
using CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlParserSharp.Tests
{
    [TestClass]
    public class Basic
    {
        static CQ Dom;

        /// <summary>
        /// This method ensures that the huge DOM gets parsed correctly by checking a few key selectors. 
        /// </summary>
        
        [ TestMethod]
        public void DomParsingTestWithNthChild()
        {

            // these values have been verified in Chrome with jQuery 1.7.2

            Assert.AreEqual(2704, Dom["div span:first-child"].Length);
            Assert.AreEqual(2517, Dom["div span:only-child"].Length);
            Assert.AreEqual(2, Dom["[type]"].Length);
            Assert.AreEqual(505, Dom["div:nth-child(2n+1)"].Length);
            Assert.AreEqual(13, Dom["div:nth-child(3)"].Length);
            Assert.AreEqual(534, Dom["div:nth-last-child(2n+1)"].Length);
            Assert.AreEqual(7, Dom["div:nth-last-child(3)"].Length);
            Assert.AreEqual(2605, Dom["div span:last-child"].Length);
        
        }

        [ TestMethod]
        public void AutoGeneratedTags()
        {

            // these values have been verified in Chrome with jQuery 1.7.2

            Assert.AreEqual(110, Dom["tbody"].Length);

        }


        [ClassInitialize]
        public static void ReadLargeDoc(TestContext context)
        {
            // CsQuery (version 1.3.0 and above) uses this code.

            Dom = CQ.Create(
                CsQuery.Utility.Support.GetFile("HtmlParserSharp.Tests\\Resources\\html standard.htm")
            );
        }

        
    }
}