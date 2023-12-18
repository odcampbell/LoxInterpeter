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
#pragma warning disable CS8604 // Possible null reference argument.
                statements.Add(declaration());
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return statements;
        }

        private Expr expression() {
            return assignment();
        }

        private Stmt? declaration(){
            try{
                if (match(FUN)) return function("function");
                if (match(VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError){
                Synchronize();
                return null;
            }
        }


        private Stmt statement(){
            if (match(FOR)) return forStatement();
            if (match(IF)) return ifStatement();
            if (match(PRINT)) return printStatement();
            if (match(RETURN)) return returnStatement();
            if (match(WHILE)) return whileStatement();
            if (match(LEFT_BRACE)) return new Stmt.Block(block());
            return expressionStatement();
        }

        private Stmt forStatement(){//FIXME
            consume(LEFT_PAREN, "Expect '(' after 'for'.");
            Stmt? initializer;
            if (match(SEMICOLON)){
                initializer = null;
            }
            else if (match(VAR)){
                initializer = varDeclaration();
            }
            else{
                initializer = expressionStatement();
            }

            Expr? condition = null;
            if (!check(SEMICOLON)) {
                condition = expression();
            }
            consume(SEMICOLON, "Expect ';' after loop condition.");

               Expr? increment = null;
                if (!check(RIGHT_PAREN)) {
                    increment = expression();
                }
                consume(RIGHT_PAREN, "Expect ')' after for clauses.");

            Stmt body = statement();

            if (increment != null){
                body = new Stmt.Block(
                    new List<Stmt> 
                    { 
                        body, 
                        new Stmt.Expression(increment) 
                    });
            }

            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null){
                body = new Stmt.Block(new List<Stmt> { initializer, body });
            }
            return body;
        }

        private Stmt ifStatement(){
            consume(LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = expression();
            consume(RIGHT_PAREN, "Expect ')' after if condition.");
            Stmt thenBranch = statement();
            Stmt? elseBranch = null;
            if (match(ELSE)){
                elseBranch = statement();
            }
#pragma warning disable CS8604 // Possible null reference argument.
            return new Stmt.If(condition, thenBranch, elseBranch);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private Stmt printStatement(){
            Expr value = expression();
            consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt returnStatement(){
            Token? keyword = previous();
            Expr? value = null;
            if (!check(SEMICOLON)){
                value = expression();
            }
            consume(SEMICOLON, "Expect ';' after return value.");
#pragma warning disable CS8604 // Possible null reference argument.
            return new Stmt.Return(keyword, value);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private Stmt varDeclaration(){
            Token name = consume(IDENTIFIER, "Expect variable name.");
            Expr? initializer = null;
            if (match(EQUAL)){
                initializer = expression();
            }
            consume(SEMICOLON, "Expect ';' after variable declaration.");
#pragma warning disable CS8604 // Possible null reference argument.
            return new Stmt.Var(name, initializer);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private Stmt whileStatement(){
            consume(LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = expression();
            consume(RIGHT_PAREN, "Expect ')' after condition.");
            Stmt body = statement();
            return new Stmt.While(condition, body);
        }


        private Stmt expressionStatement(){
            Expr expr = expression();
            consume(SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt.Function function(string kind){
            Token name = consume(IDENTIFIER, "Expect " + kind + " name.");
            consume(LEFT_PAREN, "Expect '(' after " + kind + " name.");

            List<Token> parameters = new List<Token>();

            if (!check(RIGHT_PAREN)) {
                do {
                    if (parameters.Count >= 255) {
                        error(peek(), "Can't have more than 255 parameters.");
                    }
                    parameters.Add(
                        consume(IDENTIFIER, "Expect parameter name."));
                } while (match(COMMA));
            }
            
            consume(RIGHT_PAREN, "Expect ')' after parameters.");

            consume(LEFT_BRACE, "Expect '{' before " + kind + " body.");
            List<Stmt> body = block();
            return new Stmt.Function(name, parameters, body);
        }


        private List<Stmt> block(){
            List<Stmt> statements = new List<Stmt>();
            while (!check(RIGHT_BRACE) && !isAtEnd()){
#pragma warning disable CS8604 // Possible null reference argument.
                statements.Add(declaration());
#pragma warning restore CS8604 // Possible null reference argument.
            }
            consume(RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }


        private Expr assignment(){
            Expr expr = or();
            if (match(EQUAL)){
                Token? equals = previous();
                Expr value = assignment();
                if (expr is Expr.Variable variable)
                {
                    Token name = variable.name;
                    return new Expr.Assign(name, value);
                }
#pragma warning disable CS8604 // Possible null reference argument.
                error(equals, "Invalid assignment target.");
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }

        private Expr or(){
            Expr expr = and();
            while (match(OR)){
                Token? @operator = previous();
                Expr right = and();
#pragma warning disable CS8604 // Possible null reference argument.
                expr = new Expr.Logical(expr, @operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }

        private Expr and(){
            Expr expr = equality();
            while (match(AND)){
                Token? @operator = previous();
                Expr right = equality();
#pragma warning disable CS8604 // Possible null reference argument.
                expr = new Expr.Logical(expr, @operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return expr;
        }


        private Expr equality(){
            Expr expr = comparison();
            while (match(BANG_EQUAL, EQUAL_EQUAL)){
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
            while (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL)){
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
            while (match(MINUS) || match(PLUS)){
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
            while (match(SLASH, STAR)){
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
            if (match(BANG, MINUS)){
                Token? @operator = previous();
                Expr right = unary();
#pragma warning disable CS8604 // Possible null reference argument.
                    return new Expr.Unary(@operator, right);
#pragma warning restore CS8604 // Possible null reference argument.
                }
            return call();
        }

        private Expr finishCall(Expr callee){
            List<Expr> arguments = new List<Expr>();
            if (!check(RIGHT_PAREN)){
                do
                {
                     if (arguments.Count() >= 255) {
                        error(peek(), "Can't have more than 255 arguments.");
                    }
                    arguments.Add(expression());
                } while (match(COMMA));
            }
            Token paren = consume(RIGHT_PAREN, "Expect ')' after arguments.");
            return new Expr.Call(callee, paren, arguments);
        }

        private Expr call(){
            Expr expr = Primary();
            while (true){
                if (match(LEFT_PAREN)){
                    expr = finishCall(expr);
                }
                else{
                    break;
                }
            }
            return expr;
        }

        private Expr Primary(){
            if (match(FALSE)) return new Expr.Literal(false);
            if (match(TRUE)) return new Expr.Literal(true);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            if (match(NIL)) return new Expr.Literal(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            if (match(NUMBER, STRING)){
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    return new Expr.Literal(previous().literal);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
            }

            if (match(IDENTIFIER)){
#pragma warning disable CS8604 // Possible null reference argument.
                return new Expr.Variable(previous());
#pragma warning restore CS8604 // Possible null reference argument.
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
            Lox.error(token, message);
            return new ParseError();
        }

        private void Synchronize(){
            advance();
            while (!isAtEnd())
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (previous().type == SEMICOLON)
                {
                    return;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                switch (peek().type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }
                advance();
            }
        }




    }
}


