using System.Collections.Generic;

namespace LoxApp
{
    public interface ILoxCallable{

        public int arity();
        public object? call(Interpreter interpreter, List<object> arguments);

        public string toString();

    }

    public class LoxCallable : ILoxCallable{
        public int arity() { return 0; }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
        }

        public string toString(){
            return "<native fn>";
        }


    }

}
