using System;

namespace CRMGURUTest
{
    public class SendMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public SendMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}