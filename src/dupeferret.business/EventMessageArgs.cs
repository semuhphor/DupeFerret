using System;

namespace dupeferret.business
{
    public class EventMessageArgs : EventArgs
    {
        public EventMessageArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}