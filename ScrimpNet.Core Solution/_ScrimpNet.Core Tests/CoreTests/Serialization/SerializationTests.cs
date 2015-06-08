using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using ScrimpNet;

namespace CoreTests.Serialization
{
    [DataContract]
    public class TestContract
    {
        public TestContract() { }

        [DataMember]
        public string TestStringProperty { get; set; }
    }
    /// <summary>
    /// Summary description for SerializationTests
    /// </summary>
    [TestClass]
    public class SerializationTests
    {
        public SerializationTests()
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
        public void Serialization_DataContract_HappyPath()
        {
            TestContract dc = new TestContract();
            dc.TestStringProperty="TestString";
            string s = Serialize.To.DataContract(dc);
            dc = null;
            dc = Serialize.From.DataContract<TestContract>(s);
            Assert.IsNotNull(dc);
            Assert.AreEqual("TestString", dc.TestStringProperty);
        }
    }
}
