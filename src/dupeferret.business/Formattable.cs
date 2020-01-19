namespace dupeferret.business
{
    public class Formattable
    {
        private string _message;
        public Formattable(string message)
        {
            _message = message;
        }

        public string Format(object obj)
        {
            return string.Format(_message, obj);
        }
    }
}
