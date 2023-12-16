using System.Collections.Generic;
using static LoxApp.TokenType;
// using static LoxApp.Lox;


namespace LoxApp
{

    public class Parser{
        private class ParseError : SystemException {}
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens){
            this.tokens = tokens;
        }

       public List<Stmt> parse(){
            List<Stmt> statements = new List<Stmt>();
            while (!isAtEnd())
            {
                statements.Add(declaration());
            }
            return statements;
        }

        private Expr expression() {
            return assignment();
        }

        private Stmt declaration(){
            try
            {
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError error)
            {
                Synchronize();
                return null;
            }
        }


        private Stmt statement(){
            if (match(PRINT)) return printStatement();
            if (match(LEFT_BRACE)) return new Stmt.Block(block());
            return expressionStatement();
        }

        private Stmt printStatement(){
            Expr value = expression();
            consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }


        private Stmt varDeclaration(){
            Token name = consume(IDENTIFIER, "Expect variable name.");
            Expr initializer = null;
            if (match(EQUAL))
            {
                initializer = expression();
            }
            consume(SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt expressionStatement(){
            Expr expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private List<Stmt> block(){
            List<Stmt> statements = new List<Stmt>();
            while (!check(RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }
            consume(RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }


        private Expr assignment(){
            Expr expr = equality();
            if (match(TokenType.EQUAL))
            {
                Token equals = previous();
                Expr value = assignment();
                if (expr is Expr.Variable variable)
                {
                    Token name = variable.name;
                    return new Expr.Assign(name, value);
                }
                error(equals, "Invalid assignment target.");
            }
            return expr;
        }

        private Expr equality(){
            Expr expr = comparison();
            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Token operatorToken = previous();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Expr right = comparison();
#pragma warning disable CS8604 // Possible null reference argument.
                expr = new Expr.Binary(expr, operatorToken, right);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }

        private Expr comparison(){
            Expr expr = term();
            while (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Token @operator = previous();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Expr right = term();
#pragma warning disable CS8604 // Possible null reference argument.
                expr = new Expr.Binary(expr, @operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }        

        private Expr term(){
            Expr expr = factor();
            while (match(MINUS) || match(PLUS))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Token @operator = previous();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Expr right = factor();
#pragma warning disable CS8604 // Possible null reference argument.
                expr = new Expr.Binary(expr, @operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }


        private Expr factor(){
            Expr expr = unary();
            while (match(SLASH, STAR))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Token @operator = previous();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Expr right = unary();
#pragma warning disable CS8604 // Possible null reference argument.
                expr = new Expr.Binary(expr, @operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }

        private Expr unary(){
            if (match(BANG, MINUS))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Token @operator = previous();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Expr right = unary();
#pragma warning disable CS8604 // Possible null reference argument.
                    return new Expr.Unary(@operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
                }
            return Primary();
        }

        private Expr Primary()
        {
            if (match(FALSE)) return new Expr.Literal(false);
            if (match(TRUE)) return new Expr.Literal(true);
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            if (match(NIL)) return new Expr.Literal(null);
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            if (match(NUMBER, STRING)){
            #pragma warning disable CS8604 // Dereference of a possibly null reference.
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
                    return new Expr.Literal(previous().literal);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            #pragma warning restore CS8604 // Dereference of a possibly null reference.
                }

            if (match(IDENTIFIER)){
                return new Expr.Variable(previous());
            }

            
            if (match(LEFT_PAREN))
            {
                Expr expr = expression();
                consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw new Exception($"Error: {peek()}, Expect expression.");

        }

        private bool match(params TokenType[] types){
            foreach (TokenType type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }
            return false;
        }

        private bool check(TokenType type){
            if (isAtEnd()) return false;
            return peek().type == type;
        }


        private Token ? advance(){
            if (!isAtEnd()) current++;
            return previous();
        }


        private bool isAtEnd(){
            return peek().type == EOF;
        }

        private Token peek(){
            return tokens[current];
        }

        private Token ? previous(){
            return tokens[current - 1];
        }

        private Token consume(TokenType type, string message){
#pragma warning disable CS8603 // Possible null reference return.
            if (check(type)) return advance();
#pragma warning restore CS8603 // Possible null reference return.
            throw error(peek(), message);
        }


        private ParseError error(Token token, string message) //reprot?
        {
            report(token, message);
            return new ParseError();
        }

        static void report(Token token, string message){
            if (token.type == TokenType.EOF)
            {
                Lox.report(token.line, " at end", message);
            }
            else
            {
                Lox.report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        private void Synchronize()
        {
            advance();
            while (!isAtEnd())
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (previous().type == TokenType.SEMICOLON)
                {
                    return;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                switch (peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                advance();
            }
        }




    }
}


