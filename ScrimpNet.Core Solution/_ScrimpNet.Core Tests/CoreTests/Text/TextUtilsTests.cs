using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrimpNet.Cryptography;
using ScrimpNet.Text;

namespace CoreTests.Text
{
    /// <summary>
    /// Summary description for TextUtilsTests
    /// </summary>
    [TestClass]
    public class TextUtilsTests
    {
        public TextUtilsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            string s= CryptoUtils.Hash.ComputeSha1("this is a hash functasfklajs;fajkf;asfj as;flajf;lj as;lkjasf  as;fljasdf asd;ljasf asjklasf asflkajsf as;lkjaf asd;fkljasf  asfkljaf asflkjaf aslfja;AS;J", CryptoUtils.StringFormat.Base64);
        }

        [TestMethod]
        public void Count()
        {
            int result = TextUtils.Count("bad candy bad", "bad");
            Assert.AreEqual(2, result);

            result = TextUtils.Count("bad candy bad", "a");
            Assert.AreEqual(3, result);

            result = TextUtils.Count("bad candy bad", "z");
            Assert.AreEqual(0, result);
        }
    }
}
