namespace LostAndFoundWebUi.Models
{
    public class OperationStatus
    {
        public bool Status { get; set; }
        public bool Exception { get; set; }
        public int RecordsAffected { get; set; }
        public string Message { get; set; }
        public Object OperationId { get; set; }
        public Object ReturnObject { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionInnerMessage { get; set; }
        public string ExceptionInnerStackTrace { get; set; }

        public static OperationStatus CreateFromException(string message, Exception ex)
        {
            var opStatus = new OperationStatus
            {
                Status = false,
                Exception = true,
                Message = message,
                OperationId = null
            };

            if (ex == null) return opStatus;

            opStatus.ExceptionMessage = ex.Message;
            opStatus.ExceptionStackTrace = ex.StackTrace;
            opStatus.ExceptionInnerMessage = ex.InnerException?.Message;
            opStatus.ExceptionInnerStackTrace = ex.InnerException?.StackTrace;
            return opStatus;
        }
    }
}
