using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApplication1;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Documents;

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
        #region Messages Tests

        [TestMethod]
        public void MessageFormatTest1Teacher()
        {

            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("test1", new DateTime(2016, 10, 8, 14, 30, 0), "i am testing", 3);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 4);
            conversation1.AddMessage("test3", new DateTime(2016, 10, 8, 15, 40, 0), "cool", 3);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("test1\nBy you on 08/10/2016 14:30 PM\n\ni am testing\r\n" +
                "test2\nBy john doe on 08/10/2016 12:32 PM\n\ni am replying to the test\r\n" +
                "test3\nBy you on 08/10/2016 15:40 PM\n\ncool\r\n", textRange.Text);
        }

        [TestMethod]
        public void MessageFormatTest1Student()
        {

            Messages conversation1 = new Messages(2, 3, 4, "john doe");
            conversation1.AddMessage("test1", new DateTime(2016, 10, 8, 14, 30, 0), "i am testing", 3);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 4);
            conversation1.AddMessage("test3", new DateTime(2016, 10, 8, 15, 40, 0), "cool", 3);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("test1\nBy john doe on 08/10/2016 14:30 PM\n\ni am testing\r\n" +
                "test2\nBy you on 08/10/2016 12:32 PM\n\ni am replying to the test\r\n" +
                "test3\nBy john doe on 08/10/2016 15:40 PM\n\ncool\r\n", textRange.Text);
        }

        [TestMethod]
        public void MessageFormatTest2Teacher()
        {

            Messages conversation1 = new Messages(2, 7, 6, "no name");
            conversation1.AddMessage("test1", new DateTime(2016, 10, 8, 11, 30, 0), "i am testing test two", 7);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test two", 6);
            conversation1.AddMessage("test3", new DateTime(2016, 10, 8, 15, 40, 0), "recived", 7);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("test1\nBy no name on 08/10/2016 11:30 AM\n\ni am testing test two\r\n" +
                "test2\nBy you on 08/10/2016 12:32 PM\n\ni am replying to the test two\r\n" +
                "test3\nBy no name on 08/10/2016 15:40 PM\n\nrecived\r\n", textRange.Text);
        }
        [TestMethod]
        public void MessageFormatTest2Student()
        {

            Messages conversation1 = new Messages(2, 7, 6, "no name");
            conversation1.AddMessage("test1", new DateTime(2016, 10, 8, 11, 30, 0), "i am testing test two", 7);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test two", 6);
            conversation1.AddMessage("test3", new DateTime(2016, 10, 8, 15, 40, 0), "recived", 7);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("test1\nBy no name on 08/10/2016 11:30 AM\n\ni am testing test two\r\n" +
                "test2\nBy you on 08/10/2016 12:32 PM\n\ni am replying to the test two\r\n" +
                "test3\nBy no name on 08/10/2016 15:40 PM\n\nrecived\r\n", textRange.Text);
        }

        [TestMethod]
        public void MessageFormatTestWeirdOrder()
        {

            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("test1", new DateTime(2016, 10, 8, 14, 30, 0), "i am testing", 3);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 3);
            conversation1.AddMessage("test3", new DateTime(2016, 10, 8, 15, 40, 0), "cool", 4);
            conversation1.AddMessage("asdf", new DateTime(2016, 10, 8, 15, 40, 0), "cool", 4);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("test1\nBy you on 08/10/2016 14:30 PM\n\ni am testing\r\n" +
                "test2\nBy you on 08/10/2016 12:32 PM\n\ni am replying to the test\r\n" +
                "test3\nBy john doe on 08/10/2016 15:40 PM\n\ncool\r\n" +
                "asdf\nBy john doe on 08/10/2016 15:40 PM\n\ncool\r\n", textRange.Text);
        }
        [TestMethod]
        public void MessageFormatTestBadOrder()
        {

            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("test1", new DateTime(2016, 10, 8, 14, 30, 0), "i am testing", 3);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 1);
            conversation1.AddMessage("test3", new DateTime(2016, 10, 8, 15, 40, 0), "cool", 4);
            conversation1.AddMessage("asdf", new DateTime(2016, 10, 8, 15, 40, 0), "cool", 4);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("test1\nBy you on 08/10/2016 14:30 PM\n\ni am testing\r\n" +
                "test3\nBy john doe on 08/10/2016 15:40 PM\n\ncool\r\n" +
                "asdf\nBy john doe on 08/10/2016 15:40 PM\n\ncool\r\n", textRange.Text);
        }
        [TestMethod]
        public void MessageFormatTestNone()
        {

            Messages conversation1 = new Messages(1, 3, 4, "john doe");


            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("", textRange.Text);
        }
        [TestMethod]
        public void MessageFormatTestOneYou()
        {

            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 3);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("hey\nBy you on 08/10/2016 14:30 PM\n\nThis is only a test\r\n", textRange.Text);
        }
        [TestMethod]
        public void MessageFormatTestOneThem()
        {

            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);

            RichTextBox text = new RichTextBox();
            text = conversation1.formatMessage(text);

            TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
            Assert.AreEqual("hey\nBy john doe on 08/10/2016 14:30 PM\n\nThis is only a test\r\n", textRange.Text);
        }
        [TestMethod]
        public void MessageUserTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            Assert.AreEqual("john doe", conversation1.User);
        }
        [TestMethod]
        public void MessageTeacherIDTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            Assert.AreEqual(3, conversation1.TeacherID);
        }
        [TestMethod]
        public void MessageStudentIDTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            Assert.AreEqual(4, conversation1.StudentID);
        }
        [TestMethod]
        public void MessageUserTypeTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            Assert.AreEqual(1, conversation1.UserType);
        }
        [TestMethod]
        public void MessageOneMessageTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);
            Assert.AreEqual("hey", conversation1.RecentMessage);
        }
        [TestMethod]
        public void MessageTwoMessageYouTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 3);
            string m = conversation1.RecentMessage;
            Assert.AreEqual("hey", conversation1.RecentMessage);
        }
        [TestMethod]
        public void MessageTwoMessageThemTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 4);
            Assert.AreEqual("test2", conversation1.RecentMessage);
        }
        [TestMethod]
        public void MessageOneMessageYouTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 3);
            Assert.AreEqual("hey", conversation1.RecentMessage);
        }
        [TestMethod]
        public void MessageTwoMessageYouYouTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 3);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 3);
            Assert.AreEqual("test2", conversation1.RecentMessage);
        }
        [TestMethod]
        public void MessageOneDateTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);
            Assert.AreEqual("08/10/2016 14:30 PM", conversation1.Date);
        }
        [TestMethod]
        public void MessageTwoDateYouTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 3);
            string m = conversation1.RecentMessage;
            Assert.AreEqual("08/10/2016 14:30 PM", conversation1.Date);
        }
        [TestMethod]
        public void MessageTwoDateThemTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 4);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 4);
            Assert.AreEqual("08/10/2016 12:32 PM", conversation1.Date);
        }
        [TestMethod]
        public void MessageOneDateYouTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 3);
            Assert.AreEqual("08/10/2016 14:30 PM", conversation1.Date);
        }
        [TestMethod]
        public void MessageTwoDateYouYouTest()
        {
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("hey", new DateTime(2016, 10, 8, 14, 30, 0), "This is only a test", 3);
            conversation1.AddMessage("test2", new DateTime(2016, 10, 8, 12, 32, 0), "i am replying to the test", 3);
            Assert.AreEqual("08/10/2016 12:32 PM", conversation1.Date);
        }
        #endregion 
    }
}
