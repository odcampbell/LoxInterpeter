
namespace LoxApp
{
    class LoxInstance
    {
        private LoxClass klass;
        public LoxInstance(LoxClass klass){
            this.klass = klass;
        }
        public override string ToString(){
            return klass.name + " instance";
        }
    }
}


