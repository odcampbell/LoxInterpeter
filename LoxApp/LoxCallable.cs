using System.Collections.Generic;

namespace LoxApp
{
    public interface ILoxCallable{

        public int arity();
        public object? call(Interpreter interpreter, List<object> arguments);
    }

    public class LoxCallable : ILoxCallable{
        public int arity() { return 0; }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
        }
    }

}
