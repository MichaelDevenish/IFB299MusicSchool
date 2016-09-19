using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApplication1;
using System.Diagnostics;

namespace MusicSchoolUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SaltGeneratorTest()
        {
            PasswordManagment passTest = new PasswordManagment(1);
            Assert.AreEqual("F6SleQLw6d1FJzge", passTest.GenerateSalt());
        }
        [TestMethod]
        public void SaltGeneratorTest2()
        {
            PasswordManagment passTest = new PasswordManagment(2);
            Assert.AreEqual("lPAz6JnRD0l10VNH", passTest.GenerateSalt());
        }
        [TestMethod]
        public void HashGeneratorTest()
        {
            PasswordManagment passTest = new PasswordManagment(1);
            byte[] output = passTest.GenerateHash("password", passTest.GenerateSalt());
            foreach (var item in yourArray)
            {
                Debug.Write(item.ToString());
            }
            Debug.WriteLine(passTest.GenerateHash("password", passTest.GenerateSalt()));
            Assert.IsTrue(compareByteArrays(new byte[] { 1}, passTest.GenerateHash("password", passTest.GenerateSalt())));
        }

        /// <summary>
        /// Compares two byte arrays together and returns true if they match and false if they dont
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        private static bool compareByteArrays(byte[] byte1, byte[] byte2)
        {
            bool hashComparison = true;
            if (byte1.Length != byte2.Length)
                hashComparison = false;
            for (int i = 0; i < byte1.Length; i++)
                if (byte1[i] != byte2[i])
                    hashComparison = false;
            return hashComparison;
        }
    }
}
