using System.Collections.Generic;

namespace LoxApp
{
    public class Environment
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        public Environment? enclosing;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Environment(){
            enclosing = null;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Environment(Environment enclosing){
            this.enclosing = enclosing;
        }

        public void define(string name, object value){
            values[name] = value;
        }

        public object get(Token name){
            if (values.ContainsKey(name.lexeme))
            {
                return values[name.lexeme];
            }
            if (enclosing != null) return enclosing.get(name);
            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }

        public void assign(Token name, object value){
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            if (enclosing != null) {
                enclosing.assign(name, value);
                return;
            }
            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }


    }
}


