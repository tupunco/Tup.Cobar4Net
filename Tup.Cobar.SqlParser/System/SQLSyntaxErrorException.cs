using System;

namespace System
{
    public class SQLSyntaxErrorException : Exception
    {
        public SQLSyntaxErrorException(string err) : base(err)
        {
        }
    }
}
