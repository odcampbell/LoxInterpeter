using System;
using System.Runtime.CompilerServices;

namespace LoxApp
{
    public class GlobalsLite : Environment {
        public new string ToString() { return "<native fn>";}
    }

    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        // System.Type t = typeof(void); 
        public readonly Environment globals = new Environment();
        private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        private Environment environment;

        public Interpreter(){
            environment = globals;
            globals.define("clock", new LoxCallable());
        }

        public object VisitLiteralExpr(Expr.Literal expr){
            return expr.value;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            object left = evaluate(expr.left);
            if (expr.@operator.type == TokenType.OR)
            {
                if (isTruthy(left)) return left;
            }
            else
            {
                if (!isTruthy(left)) return left;
            }
            return evaluate(expr.right);
        }

        public object VisitSetExpr(Expr.Set expr){
            object obj = evaluate(expr.objt);
            if (!(obj is LoxInstance))
            {
                throw new RuntimeError(expr.name, "Only instances have fields.");
            }
            object value = evaluate(expr.value);
            ((LoxInstance)obj).set(expr.name, value);
            return value;
        }

        public object VisitThisExpr(Expr.This expr){
            return lookUpVariable(expr.keyword, expr);
        }
        public void interpret(List<Stmt> statements){
            try
            {
                foreach (Stmt statement in statements){
                    execute(statement);
                }
            }
            catch (RuntimeError error){
                Lox.runtimeError(error);
            }
        }

        public object VisitUnaryExpr(Expr.Unary expr){
            object right = evaluate(expr.right);
            switch (expr.@operator.type)
            {
                case TokenType.BANG:
                    return !isTruthy(right);
                case TokenType.MINUS:
                    checkNumberOperand(expr.@operator, right);
                    return -(double)right;
            }

      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }

        
        public object VisitVariableExpr(Expr.Variable expr){
            return lookUpVariable(expr.name, expr);
        }

        private object lookUpVariable(Token name, Expr expr){
            int? distance=null;
            if(locals.ContainsKey(expr)){ //if expr found
                distance = locals[expr]; //fingers crossed
            }

            // System.Console.WriteLine("NAME: " + name.lexeme + "   DISTANCE" + distance); //n
            if (distance != null)
            {
      #pragma warning disable CS8603 // Possible null reference return.
            // System.Console.WriteLine("NAME: " + name.lexeme + "   DISTANCE" + distance); //n

                return environment.getAt((int)distance, name.lexeme);
      #pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                return globals.get(name);
            }
        }

        private void checkNumberOperand(Token @operator, object operand){
            if (operand is double)
            {
                return;
            }
            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        private bool isTruthy(object obj){
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool isEqual(object a, object b){
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        private string Stringify(object obj){
            if (obj == null) return "nil";

            if (obj is double){
                string? text = obj.ToString();
      #pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);//FIXME
                }
      #pragma warning restore CS8602 // Dereference of a possibly null reference.
                return text;
            }
      #pragma warning disable CS8603 // Possible null reference return.
            return obj.ToString();
      #pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitGroupingExpr(Expr.Grouping expr){
            return evaluate(expr.expression);
        }

        private object evaluate(Expr expr){
            return expr.Accept(this);
        }

        private void execute(Stmt stmt){
            stmt.Accept(this);
        }

        public void resolve(Expr expr, int depth) {
            // locals[expr] = depth;//
            locals.Add(expr,depth);
        }

        public void executeBlock(List<Stmt> statements, Environment environment){
            Environment previous = this.environment;
            try
            {
                this.environment = environment;
                foreach (Stmt statement in statements){
                    execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }


        public object VisitBlockStmt(Stmt.Block stmt){

            executeBlock(stmt.statements, new Environment(environment));
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }

      #pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public object? VisitClassStmt(Stmt.Class stmt){
      #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            environment.define(stmt.name.lexeme, null);
            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            foreach (Stmt.Function method in stmt.methods){
                LoxFunction function = new LoxFunction(method, environment,
                    method.name.lexeme.Equals("init"));

                methods.Add(method.name.lexeme, function);
            }
            LoxClass klass = new LoxClass(stmt.name.lexeme, methods);

      #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            // LoxClass klass = new LoxClass(stmt.name.lexeme);
            environment.assign(stmt.name, klass);
            return null;
        }
      #pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

        public object VisitExpressionStmt(Stmt.Expression stmt){
            evaluate(stmt.expression);
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitFunctionStmt(Stmt.Function stmt){
           LoxFunction function = new LoxFunction(stmt, environment,false);
            environment.define(stmt.name.lexeme, function);
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }


        public object VisitIfStmt(Stmt.If stmt){
            if (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                execute(stmt.elseBranch);
            }
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitPrintStmt(Stmt.Print stmt){
            object value = evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitReturnStmt(Stmt.Return stmt){
            object? value = null;
            if (stmt.value != null) value = evaluate(stmt.value);
      #pragma warning disable CS8604 // Possible null reference argument.
            throw new Return(value);
      #pragma warning restore CS8604 // Possible null reference argument.
        }


        public object VisitVarStmt(Stmt.Var stmt){
            object? value = null;
            if (stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }
      #pragma warning disable CS8604 // Possible null reference argument.

            environment.define(stmt.name.lexeme, value);
      #pragma warning restore CS8604 // Possible null reference argument.
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitWhileStmt(Stmt.While stmt){ //void
            while (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.body);
            }
      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }


        public  object VisitAssignExpr(Expr.Assign expr){
            object value = evaluate(expr.value);

            int? distance=null;
            if(locals.ContainsKey(expr)){ //if expr found
                distance = locals[expr]; //fingers crossed
            }
      #pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (distance != null) {
                environment.assignAt((int)distance, expr.name, value);
            } 
      #pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            else {
                globals.assign(expr.name, value);
            }
            return value;
        }

        public object VisitBinaryExpr(Expr.Binary expr){
            object left = evaluate(expr.left);//evals n just fine 
            object right = evaluate(expr.right); // evals 1 literal expr val
            // Console.WriteLine ("POST EVALS: L: "+left+" R: "+right);

            switch (expr.@operator.type){
                case TokenType.BANG_EQUAL: return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL: return isEqual(left, right);
                case TokenType.GREATER:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left > (double)right;

                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left >= (double)right;

                case TokenType.LESS:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left < (double)right;

                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.MINUS:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left - (double)right;

                case TokenType.PLUS:
                    if (left is double && right is double){
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string){
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.@operator, "Operands must be two numbers or two strings.");

                case TokenType.SLASH:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left / (double)right;

                case TokenType.STAR:
                    checkNumberOperands(expr.@operator, left, right);
                    return (double)left * (double)right;
            }

      #pragma warning disable CS8603 // Possible null reference return.
            return null;
      #pragma warning restore CS8603 // Possible null reference return.
        }


        public object VisitCallExpr(Expr.Call expr){
            object callee = evaluate(expr.callee);

            List<object> arguments = new List<object>();

            foreach (Expr argument in expr.arguments){
                arguments.Add(evaluate(argument));
            }

            if (!(callee is ILoxCallable)){
                throw new RuntimeError(expr.paren, "Can only call functions and classes.: "+callee);
            }

            ILoxCallable function = (ILoxCallable)callee;//issuehere
            
            if (arguments.Count != function.arity()){ //as expected..
                throw new RuntimeError(expr.paren, "Expected " +
                    function.arity() + " arguments but got " +
                    arguments.Count + ".");
            }
      #pragma warning disable CS8603 // Possible null reference return.
            return function.call(this, arguments);
      #pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitGetExpr(Expr.Get expr){
            object obj = evaluate(expr.objt);
            if (obj is LoxInstance){
                return ((LoxInstance)obj).get(expr.name);
            }
            throw new RuntimeError(expr.name, "Only instances have properties.");
        }

        private void checkNumberOperands(Token @operator, object left, object right){
            if (left is double && right is double) return;
            Console.WriteLine("Woops.. Type L "+left+": "+left.GetType().Name+ "  Type R "+right+": "+ right.GetType().Name);
            throw new RuntimeError(@operator, "Operands must be numbers.");
        }

    }
}

