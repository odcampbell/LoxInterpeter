using System;

namespace LoxApp
{
    class Interpreter : Expr.Visitor<object>
    {
        public object VisitLiteralExpr(Expr.Literal expr){
            return expr.value;
        }

        public void interpret(Expr expression){
            try
            {
                object value = evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
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




    }
}

