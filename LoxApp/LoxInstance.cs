
namespace LoxApp
{
    public class LoxInstance
    {
        private LoxClass klass;
        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();
        public LoxInstance(LoxClass klass){
            this.klass = klass;
        }

        public object get(Token name){
            if (fields.ContainsKey(name.lexeme)){
                return fields[name.lexeme];
            }

            LoxFunction? method = klass.findMethod(name.lexeme);
            if (method != null) return method.bind(this);
            
            throw new RuntimeError(name, "Undefined property '" + name.lexeme + "'.");
        }

        
        public void set(Token name, object value){
            fields[name.lexeme] = value;
        }

        public override string ToString(){
            return klass.name + " instance";
        }
    }
}


