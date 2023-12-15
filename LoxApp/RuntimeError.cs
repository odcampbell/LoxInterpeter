using System;

namespace LoxApp
{
    public class RuntimeError : SystemException
    {
        public Token token { get; }
        
        public RuntimeError(Token token, string message) : base(message)
        {
            this.token = token;
        }
    }
}


