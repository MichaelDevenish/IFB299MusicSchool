using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfApplication1
{
    /// <summary>
    /// A class that acts as a grouping of a chat log between two people and contains functions
    /// that can format the log to be viewable in a richtextbox
    /// </summary>
    public class Messages
    {
        #region globals
        private int currentUserType;
        private int teacherID;
        private int studentID;
        private List<Message> messages;
        private string otherPartyName;

        public const int ADMIN = 0;
        public const int TEACHER = 1;
        public const int STUDENT = 2;
        #endregion
        #region properties
        public string User { get { return otherPartyName; } }
        public int TeacherID { get { return teacherID; } }
        public int StudentID { get { return studentID; } }
        public int UserType { get { return currentUserType; } }

        /// <summary>
        /// gets the title of the most recent message sent to you
        /// </summary>
        public string RecentMessage
        {
            get
            {
                for (int i = messages.Count - 1; i >= 0; i--)
                    if (messages[i].Sender == false)
                        return messages[i].Title;
                return messages[messages.Count - 1].Title;
            }
        }
        /// <summary>
        /// Gets the most recent date of a message that was sent to you
        /// </summary>
        public string Date
        {
            get
            {
                for (int i = messages.Count - 1; i >= 0; i--)
                    if (messages[i].Sender == false)
                        return messages[i].Date.ToString("dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                return messages[messages.Count - 1].Date.ToString("dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="currentUserType">0 = admin, 1 = teacher, 2 = student</param>
        /// <param name="teacherID">the id of the teacher</param>
        /// <param name="studentID">the id of the student</param>
        /// <param name="otherPartyName">the name of the other person</param>
        public Messages(int currentUserType, int teacherID, int studentID, string otherPartyName)
        {
            this.currentUserType = currentUserType;
            this.teacherID = teacherID;
            this.studentID = studentID;
            this.otherPartyName = otherPartyName;
            messages = new List<Message>();
        }

        /// <summary>
        /// creates a Message object and adds it to an internal message list
        /// </summary>
        /// <param name="title">the title of the message</param>
        /// <param name="date">the date the message was sent</param>
        /// <param name="message">the content of the message</param>
        /// <param name="sender">the id of the sender</param>
        public void AddMessage(string title, DateTime date, string message, int sender)
        {
            if (sender == teacherID || sender == studentID)
            {
                bool sent = false;
                sent = ((sender == teacherID && (currentUserType == 1 || currentUserType == 0)) || (sender == studentID && currentUserType == 2));
                messages.Add(new Message(title, date, message, sent));
            }
        }

        /// <summary>
        /// Formats the conversation to be readable in a RichTextBox
        /// </summary>
        /// <param name="textbox">the richtextbox to be cleared and formatted</param>
        /// <returns></returns>
        public RichTextBox formatMessage(RichTextBox textbox)
        {
            textbox.Document.Blocks.Clear();

            foreach (Message message in messages)
            {
                string user = "";
                if (message.Sender) user = "you";
                else user = otherPartyName;

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Bold(new Run(message.Title + "\nBy " + user + " on " + message.Date.ToString("dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture) + "\n\n")));
                paragraph.Inlines.Add(new Run(message.Contents));
                textbox.Document.Blocks.Add(paragraph);
                textbox.Focus();
                textbox.ScrollToEnd();

            }
            return textbox;

        }

    }
    /// <summary>
    /// An individual message
    /// </summary>
    public class Message
    {
        private string title;
        private DateTime date;
        private string message;
        private bool sender;//false if not current user

        public string Title { get { return title; } }
        public DateTime Date { get { return date; } }
        public bool Sender { get { return sender; } }
        public string Contents { get { return message; } }


        public Message(string title, DateTime date, string message, bool sender)
        {
            this.title = title;
            this.date = date;
            this.message = message;
            this.sender = sender;
        }
    }
}
