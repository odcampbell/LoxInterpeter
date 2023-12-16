using System;
using System.Runtime.CompilerServices;

namespace LoxApp
{
    public class GlobalsLite : Environment {
        public override string ToString() { return "<native fn>";}
    }

    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        // System.Type t = typeof(void); 
        public readonly Environment globals = new Environment();
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
            return environment.get(expr.name);
        }


        private void checkNumberOperand(Token @operator, object operand)
        {
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
                    text = text.Substring(0, text.Length - 2);
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

        public void executeBlock(List<Stmt> statements, Environment environment){
            Environment previous = this.environment;
            try
            {
                this.environment = environment;
                foreach (Stmt statement in statements)
                {
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

        public object VisitExpressionStmt(Stmt.Expression stmt){
            evaluate(stmt.expression);
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitFunctionStmt(Stmt.Function stmt){
            LoxFunction function = new LoxFunction(stmt);
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
            environment.assign(expr.name, value);
            return value;
        }

        public object VisitBinaryExpr(Expr.Binary expr){
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

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
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
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

        private void checkNumberOperands(Token @operator, object left, object right){
            if (left is double && right is double) return;
            throw new RuntimeError(@operator, "Operands must be numbers.");
        }

    }
}

