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

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Parenthesize(expr.@name.lexeme, expr.value);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return Parenthesize(expr.@name.lexeme, expr);

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