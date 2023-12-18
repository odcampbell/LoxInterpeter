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
            // System.Console.WriteLine("NAME: " + name + "   VALUE" + value); //seems fine

            values[name] = value;
        }

        public Environment ancestor(int distance)
        {
            Environment? environment = this;
            for (int i = 0; i < distance; i++)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                environment = environment.enclosing;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
#pragma warning disable CS8603 // Possible null reference return.
            return environment;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public object? getAt(int distance, string name){
            if(ancestor(distance).values.ContainsKey(name)){
                return ancestor(distance).values[name];
            }
            return null; //not too happy about this one but what can ya do
        }
        
        public void assignAt(int distance, Token name, object value){
            ancestor(distance).values[name.lexeme] = value;
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


