using System;
using System.Runtime.CompilerServices;

namespace LoxApp
{
    class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        // System.Type t = typeof(void); 
        private Environment environment = new Environment();

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

            return null;
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
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return obj.ToString();
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
            return null;
        }

        public object VisitExpressionStmt(Stmt.Expression stmt){
            evaluate(stmt.expression);
            return null;
        }

        public void VisitIfStmt(Stmt.If stmt){
            if (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                execute(stmt.elseBranch);
            }
        }

        public object VisitPrintStmt(Stmt.Print stmt){
            object value = evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitVarStmt(Stmt.Var stmt){
            object value = null;
            if (stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }
            environment.define(stmt.name.lexeme, value);
            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt){ //void
            while (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.body);
            }
            return null;
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

            return null;
        }

        private void checkNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(@operator, "Operands must be numbers.");
        }

        object Stmt.Visitor<object>.VisitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }
    }
}

