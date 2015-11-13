namespace System
{
    /// <summary>
    /// SQL Syntax Error Exception
    /// </summary>
    public class SQLSyntaxErrorException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SQLSyntaxErrorException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SQLSyntaxErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}