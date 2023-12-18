namespace LoxApp
{
    public class LoxClass : ILoxCallable
    {
        public readonly string name;

        public LoxClass(string name){
            this.name = name;
        }

        public override string ToString(){
            return name;
        }

        public object call(Interpreter interpreter, List<object> arguments){
            LoxInstance instance = new LoxInstance(this);
            return instance;
        }

        public int arity(){
            return 0;
        }



    }
}


