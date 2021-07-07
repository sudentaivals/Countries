using System;
using System.Collections.Generic;
using System.Text;

namespace CRMGURUTest
{
    public class ErrorOccurredEventArgs : EventArgs
    {
        public string ErrorMessage { get; }
        public ErrorOccurredEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
