using System.Collections.Generic;

namespace LoxApp
{
    class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly Environment myClosure;

        public  LoxFunction(Stmt.Function declaration, Environment myClosure) {
            this.myClosure = myClosure;
            this.declaration = declaration;
        }

        public new string ToString(){
            return "<fn " + declaration.name.lexeme + ">";
        }

        public int arity(){
            return declaration.@params.Count;
        }

        public object? call(Interpreter interpreter, List<object> arguments){
            Environment environment = new Environment(myClosure);
            // Environment environment = new Environment(interpreter.globals);

            for (int i = 0; i < declaration.@params.Count; i++){
                environment.define(declaration.@params[i].lexeme, arguments[i]);
            }
            try {
            interpreter.executeBlock(declaration.body, environment);
            } 
            catch (Return returnValue) {
                return returnValue.value;
            }
            return null;
        }

    }
}


