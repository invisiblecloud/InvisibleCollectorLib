namespace InvoiceCaptureLib.Connection
{
    internal class IcResponseJsonError
    {
        private string code;
        private string message;

        public string Code
        {
            get => code;

            set => code = value;
        }

        public string Message
        {
            get => message;

            set => message = value;
        }
    }
}