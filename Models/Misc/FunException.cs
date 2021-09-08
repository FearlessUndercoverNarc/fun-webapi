using System;

namespace Models.Misc
{
    public class FunException : Exception
    {
        public FunException(string message) : base(message)
        {
        }
    }
}