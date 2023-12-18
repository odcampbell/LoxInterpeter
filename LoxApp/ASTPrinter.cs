using System;
using System.Text;

namespace LoxApp
{
    class AstPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr){
            return expr.Accept(this);
        }
                                // VisitBinaryExpr
        public string VisitBinaryExpr(Expr.Binary expr){
            return Parenthesize(expr.@operator.lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr){
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr){
            if (expr.value == null) return "nil";
        #pragma warning disable CS8603 // Possible null reference return.
            return expr.value.ToString();
        #pragma warning restore CS8603 // Possible null reference return.
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.@operator.lexeme, expr.right);
        }

        private string Parenthesize(string name, params Expr[] exprs){
            StringBuilder builder = new StringBuilder();
            builder.Append("(").Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");
            return builder.ToString();
        }

        //was bullied into making these by ASTPrinter, would not settle for any other forms
        //so im not sure if these will have an acceptable or desired effect
        public string VisitAssignExpr(Expr.Assign expr) 
        {
            return Parenthesize(expr.@name.lexeme, expr.value);
            // throw new NotImplementedException();

        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return Parenthesize(expr.@name.lexeme, expr);
            // throw new NotImplementedException();

        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            return Parenthesize(expr.@operator.lexeme, expr.right);
        }

        public string VisitCallExpr(Expr.Call expr)
        {
            return Parenthesize("fn2k",expr.callee);
            // throw new NotImplementedException();
        }

        public string VisitGetExpr(Expr.Get expr)
        {
            return Parenthesize(expr.@name.lexeme, expr);

        }

        public string VisitSetExpr(Expr.Set expr)
        {
            return Parenthesize(expr.@name.lexeme, expr);

        }

        public string VisitThisExpr(Expr.This expr)
        {
            return Parenthesize(expr.keyword.lexeme, expr);

        }

        //    public static void Main(string[] args)
        //     {
        //         Expr expression = new Expr.Binary(
        //             new Expr.Unary(
        //                 new Token(TokenType.MINUS, "-", null, 1),
        //                 new Expr.Literal(123)),
        //             new Token(TokenType.STAR, "*", null, 1),
        //             new Expr.Grouping(
        //                 new Expr.Literal(45.67)));
        //         Console.WriteLine(new AstPrinter().Print(expression));
        //     }

    }
}