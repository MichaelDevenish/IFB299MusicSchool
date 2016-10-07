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
    class Messages
    {
        private int currentUserType;
        private int teacherID;
        private int studentID;
        private List<Message> messages;
        private string otherPartyName;

        public string User { get { return otherPartyName; } }
        public int TeacherID { get { return teacherID; } }
        public int StudentID { get { return studentID; } }

        public string RecentMessage
        {
            get
            {
                for (int i = messages.Count - 1; i >= 0; i--)
                    if (messages[i].Sender != false)
                        return messages[i].Title;
                return messages[messages.Count - 1].Title;
            }
        }
        public string Date
        {
            get
            {
                for (int i = messages.Count - 1; i >= 0; i--)
                    if (messages[i].Sender != false)
                        return messages[i].Date.ToString("dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                return messages[messages.Count - 1].Date.ToString("dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
            }
        }

        public Messages(int currentUserType, int teacherID, int studentID, string otherPartyName)
        {
            this.currentUserType = currentUserType;
            this.teacherID = teacherID;
            this.studentID = studentID;
            this.otherPartyName = otherPartyName;
            messages = new List<Message>();
        }

        public void AddMessage(string title, DateTime date, string message, int sender)
        {
            bool sent = false;
            sent = ((sender == teacherID && (currentUserType == 1 || currentUserType == 0)) || (sender == studentID && currentUserType == 2));
            messages.Add(new Message(title, date, message, sent));
        }

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
    class Message
    {
        private string title;
        private DateTime date;
        private string message;
        private bool sender;//false if current user

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
