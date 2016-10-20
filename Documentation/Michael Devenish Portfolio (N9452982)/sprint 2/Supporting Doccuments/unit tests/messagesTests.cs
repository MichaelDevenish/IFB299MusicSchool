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
 