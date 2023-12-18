using System.Collections.Generic;

namespace LoxApp
{
    public class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly Environment myClosure;
        

        private readonly bool isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment myClosure, bool isInitializer) {
            this.isInitializer = isInitializer;
            this.myClosure = myClosure;
            this.declaration = declaration;
        }

        public LoxFunction bind(LoxInstance instance) {
            Environment environment = new Environment(myClosure);
            environment.define("this", instance);
            return new LoxFunction(declaration, environment,
                           isInitializer);
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
                if (isInitializer) return myClosure.getAt(0, "this");
                return returnValue.value;
            }
            if (isInitializer) return myClosure.getAt(0, "this");
            return null;
        }

    }
}


