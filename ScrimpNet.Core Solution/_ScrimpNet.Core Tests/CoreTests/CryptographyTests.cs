using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrimpNet.Cryptography;
using System.Security.Cryptography;
using ScrimpNet;
using ScrimpNet.Text;

namespace CoreTests
{
    /// <summary>
    /// Summary description for CryptographyTests
    /// </summary>
    [TestClass]
    public class CryptographyTests
    {
        public CryptographyTests()
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
        public void EncryptByteArray_HappyPath()
        {
            string pwd = Guid.NewGuid().ToString();
            Rfc2898DeriveBytes byteFactory = new Rfc2898DeriveBytes(pwd, 50);
        
            byte[] keyBytes = byteFactory.GetBytes(32);
            byte[] ivBytes = byteFactory.GetBytes(16);

            byte[] plainText = Transform.StringToBytes("hello world");

            string result = CryptoUtils.Encrypt("hello world");
        }

        [TestMethod]
        public void KeyFile_Serialization()
        {
            string s = CryptoUtils.GenerateInternalKeys_Base64Unencrypted(new TimeSpan(365*2,0,0,0,0));
        }
    }
}
