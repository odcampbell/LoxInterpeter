using System.Collections.Generic;

namespace LoxApp
{
    class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function declaration;

        public LoxFunction(Stmt.Function declaration){
            this.declaration = declaration;
        }

        public override string ToString(){
            return "<fn " + declaration.name.lexeme + ">";
        }

        public int arity(){
            return declaration.@params.Count;
        }

        public object? call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(interpreter.globals);
            for (int i = 0; i < declaration.@params.Count; i++)
            {
                environment.define(declaration.@params[i].lexeme, arguments[i]);
            }
            interpreter.executeBlock(declaration.body, environment);
            return null;
        }

    }
}


