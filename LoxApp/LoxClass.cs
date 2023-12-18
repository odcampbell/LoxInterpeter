namespace LoxApp
{
    public class LoxClass : ILoxCallable
    {
        public readonly string name;

      private readonly Dictionary<string, LoxFunction> methods;
    
        public LoxClass(string name, Dictionary<string, LoxFunction> methods){
            this.name = name;
            this.methods = methods;
        }

        public LoxFunction? findMethod(string name){
            if (methods.ContainsKey(name)){
                return methods[name];
            }
            return null;
        }

        public override string ToString(){
            return name;
        }

        public object call(Interpreter interpreter, List<object> arguments){
            LoxInstance instance = new LoxInstance(this);

            LoxFunction? initializer = findMethod("init");
            if (initializer != null) {
            initializer.bind(instance).call(interpreter, arguments);
            }

            return instance;
        }

        public int arity(){
            LoxFunction? initializer = findMethod("init");
            if (initializer == null) return 0;
            return initializer.arity();
        }



    }
}


