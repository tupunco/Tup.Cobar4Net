namespace System
{
    /// <summary>
    ///     Sql Syntax Error Exception
    /// </summary>
    public class SqlSyntaxErrorException : Exception
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public SqlSyntaxErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SqlSyntaxErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public SqlSyntaxErrorException(Exception innerException)
            : base("Exception", innerException)
        {
        }
    }
}