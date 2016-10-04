using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApplication1;
using System.Diagnostics;
using System.Windows.Controls;

namespace MusicSchoolUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        TextBox textTest = new TextBox();
        Label labeltest = new Label();

        #region hashing Tests
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
            byte[] check = new byte[] { 14, 82, 174, 234, 23, 248, 28, 253, 32, 73, 199,
                                        47, 66, 175, 223, 12, 119, 0, 97, 248, 37, 8, 218,
                                        228, 150, 127, 191,140, 16, 35, 86, 180, 172, 4, 236,
                                        109, 252, 72, 20, 73, 202, 217, 49,68, 3, 102, 37, 234,
                                        164, 200, 174, 203, 154, 95, 184, 146, 244, 207, 162, 254,
                                        22, 38, 195, 119 };
            PasswordManagment passTest = new PasswordManagment(1);
            Assert.IsTrue(HelperFunctions.compareByteArrays(check, passTest.GenerateHash("password", passTest.GenerateSalt())));
        }

        [TestMethod]
        public void HashGeneratorTest2()
        {
            byte[] check = new byte[] { 42, 44, 179, 218, 239, 37, 35, 196, 190, 57, 25, 77, 156,
                                        229, 211, 75, 18, 200, 7, 101, 104, 213, 72, 197, 38, 102,
                                        25, 158, 205, 60, 208, 227, 87, 119, 34, 113, 176, 247,
                                        157, 203, 107, 65, 100, 130, 174, 164, 139, 202, 177, 45,
                                        185, 106, 123, 81, 26, 152, 171, 184, 197, 184, 86, 224,
                                        98, 106 };
            PasswordManagment passTest = new PasswordManagment(2);
            Assert.IsTrue(HelperFunctions.compareByteArrays(check, passTest.GenerateHash("password", passTest.GenerateSalt())));
        }

        [TestMethod]
        public void HashGeneratorTestAgain()
        {
            PasswordManagment passTest = new PasswordManagment(2);
            Assert.IsFalse(HelperFunctions.compareByteArrays(passTest.GenerateHash("password", passTest.GenerateSalt()),
                passTest.GenerateHash("password", passTest.GenerateSalt())));
        }
        #endregion
        #region Get9To5TimeFrom16Int Tests

        [TestMethod]
        public void TestTimeConverter1()
        {
            Assert.AreEqual("12:00pm", HelperFunctions.Get9to5TimeFrom16Int(6));
        }
        [TestMethod]
        public void TestTimeConverter2()
        {
            Assert.AreEqual("12:30pm", HelperFunctions.Get9to5TimeFrom16Int(7));
        }
        [TestMethod]
        public void TestTimeConverterLowerBounds()
        {
            Assert.AreEqual("9:00am", HelperFunctions.Get9to5TimeFrom16Int(0));
        }
        [TestMethod]
        public void TestTimeConverterUpperBounds()
        {
            Assert.AreEqual("5:00pm", HelperFunctions.Get9to5TimeFrom16Int(16));
        }
        [TestMethod]
        public void TestTimeConverterTooLow()
        {
            Assert.AreEqual(null, HelperFunctions.Get9to5TimeFrom16Int(-1));
        }
        [TestMethod]
        public void TestTimeConverterTooHigh()
        {
            Assert.AreEqual(null, HelperFunctions.Get9to5TimeFrom16Int(17));
        }
        #endregion
        #region CompareByteArrays Tests
        [TestMethod]
        public void byteArrayComparisonTestSame()
        {
            Assert.IsTrue(HelperFunctions.compareByteArrays(new byte[] { 1, 5 }, new byte[] { 1, 5 }));
        }
        [TestMethod]
        public void byteArrayComparisonTestShorterLeft()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { 1 }, new byte[] { 1, 5 }));
        }
        [TestMethod]
        public void byteArrayComparisonTestShorterRight()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { 1, 5 }, new byte[] { 1 }));
        }
        [TestMethod]
        public void byteArrayComparisonTestDifferentClose()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { 1, 6 }, new byte[] { 1, 5 }));
        }
        [TestMethod]
        public void byteArrayComparisonTestDifferentFar()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { 7, 3 }, new byte[] { 12, 2 }));
        }
        [TestMethod]
        public void byteArrayComparisonNull()
        {
            Assert.IsTrue(HelperFunctions.compareByteArrays(null, null));
        }
        [TestMethod]
        public void byteArrayComparisonNullLeft()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(null, new byte[] { 7, 3 }));
        }
        [TestMethod]
        public void byteArrayComparisonNullRight()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { 7, 3 }, null));
        }
        [TestMethod]
        public void byteArrayComparisonEmpty()
        {
            Assert.IsTrue(HelperFunctions.compareByteArrays(new byte[] { }, new byte[] { }));
        }
        [TestMethod]
        public void byteArrayComparisonTestEmptyLeft()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { }, new byte[] { 1, 5 }));
        }
        [TestMethod]
        public void byteArrayComparisonTestEmptyRight()
        {
            Assert.IsFalse(HelperFunctions.compareByteArrays(new byte[] { 1, 5 }, new byte[] { }));
        }
        #endregion
        #region Database Tests
        [TestMethod]
        public void TestUSerCheck()
        {
            DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
            Assert.IsTrue(db.CheckUsername("admin"));
        }
        [TestMethod]
        public void TestUSerCheckFalse()
        {
            DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
            Assert.IsFalse(db.CheckUsername("asdfwefawgrewagasrghdtrhdsargsarhgasrhdthgyadsgehgehgdetrghegerg"));
        }
        #endregion
        #region Login Tests
        #region Email
        [TestMethod]
        public void EmailCheck()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "Test";
            Assert.IsTrue(HelperFunctions.emailCheck(false, labeltest, textTest));
        }
        [TestMethod]
        public void EmailCheckReal()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "test@test.com";
            Assert.IsFalse(HelperFunctions.emailCheck(false, labeltest, textTest));
        }
        [TestMethod]
        public void EmailCheckReal2()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "st@tt.co";
            Assert.IsFalse(HelperFunctions.emailCheck(false, labeltest, textTest));
        }
        [TestMethod]
        public void EmailCheck2()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "s@t.o";
            Assert.IsFalse(HelperFunctions.emailCheck(false, labeltest, textTest));
        }
        [TestMethod]
        public void EmailCheckNull()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "";
            Assert.IsFalse(HelperFunctions.emailCheck(false, labeltest, textTest));
        }
        [TestMethod]
        public void EmailCheckLabel()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "Test";
            HelperFunctions.emailCheck(false, labeltest, textTest);
            Assert.AreEqual(System.Windows.Visibility.Visible, labeltest.Visibility);
        }
        [TestMethod]
        public void EmailCheckRealLabel()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "test@test.com";
            HelperFunctions.emailCheck(false, labeltest, textTest);
            Assert.AreEqual(System.Windows.Visibility.Hidden, labeltest.Visibility);
        }
        [TestMethod]
        public void EmailCheckReal2Label()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "st@tt.co";
            HelperFunctions.emailCheck(false, labeltest, textTest);
            Assert.AreEqual(System.Windows.Visibility.Hidden, labeltest.Visibility);
        }
        [TestMethod]
        public void EmailCheck2Label()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "s@t.o";
            HelperFunctions.emailCheck(false, labeltest, textTest);
            Assert.AreEqual(System.Windows.Visibility.Hidden, labeltest.Visibility);
        }
        [TestMethod]
        public void EmailCheckNullLabel()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "";
            HelperFunctions.emailCheck(false, labeltest, textTest);
            Assert.AreEqual(System.Windows.Visibility.Hidden, labeltest.Visibility);
        }
        #endregion
        #region NullError Tests
        [TestMethod]
        public void ErrorCheckNull()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            Assert.IsTrue(HelperFunctions.checkError(false, labeltest, ""));
        }
        [TestMethod]
        public void ErrorCheckNotNull()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            Assert.IsFalse(HelperFunctions.checkError(false, labeltest, "Test"));
        }
        [TestMethod]
        public void ErrorCheckLabel()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "";
            HelperFunctions.checkError(false, labeltest, "");
            Assert.AreEqual(System.Windows.Visibility.Visible, labeltest.Visibility);
        }
        [TestMethod]
        public void ErrorCheckNotLabel()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            textTest.Text = "Test";
            HelperFunctions.checkError(false, labeltest, "Test");
            Assert.AreEqual(System.Windows.Visibility.Hidden, labeltest.Visibility);
        }
        #endregion
        #region ShowError Tests
        [TestMethod]
        public void ErrorShowTest()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            HelperFunctions.ShowError(labeltest);
            Assert.AreEqual(System.Windows.Visibility.Visible, labeltest.Visibility);
        }
        [TestMethod]
        public void ErrorShowTestControl()
        {
            labeltest.Visibility = System.Windows.Visibility.Hidden;
            Assert.AreEqual(System.Windows.Visibility.Hidden, labeltest.Visibility);
        }
        #endregion
        #endregion
    }
}
