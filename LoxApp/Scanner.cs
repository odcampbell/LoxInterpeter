using System.Collections.Generic;
using static LoxApp.TokenType;

namespace LoxApp
{
    class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source){
            this.source = source;
        }

    static Scanner() {
            keywords.Add("and",    AND);
            keywords.Add("class",  CLASS);
            keywords.Add("else",   ELSE);
            keywords.Add("false",  FALSE);
            keywords.Add("for",    FOR);
            keywords.Add("fun",    FUN);
            keywords.Add("if",     IF);
            keywords.Add("nil",    NIL);
            keywords.Add("or",     OR);
            keywords.Add("print",  PRINT);
            keywords.Add("return", RETURN);
            keywords.Add("super",  SUPER);
            keywords.Add("this",   THIS);
            keywords.Add("true",   TRUE);
            keywords.Add("var",    VAR);
            keywords.Add("while",  WHILE);
        }

        public List<Token> ScanTokens(){
            while (!isAtEnd()){
                start = current;
                scanToken();
            }
            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }  

        private void scanToken(){
            char c = advance();
            switch (c)
            {
                case '(':
                    addToken(LEFT_PAREN); break;
                case ')':
                    addToken(RIGHT_PAREN); break;
                case '{':
                    addToken(LEFT_BRACE); break;
                case '}':
                    addToken(RIGHT_BRACE); break;
                case ',':
                    addToken(COMMA); break;
                case '.':
                    addToken(DOT); break;
                case '-':
                    addToken(MINUS); break;
                case '+':
                    addToken(PLUS); break;
                case ';':
                    addToken(SEMICOLON); break;
                case '*':
                    addToken(STAR); break;
                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                case '/':
                    if (match('/')){
                        while (peek() != '\n' && !isAtEnd()){
                            advance();
                        }
                    }
                    else{
                        addToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;

                case '"':
                    String();break;

                default:
                    if (isDigit(c)) {
                        number();
                    }
                    else if (isAlpha(c)) {
                        identifier();
                    } 
                    else {
                        Lox.error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void identifier() {
            while (isAlphaNumeric(peek())) advance();

            string text = source.Substring(start, current - start);
            TokenType type = keywords.ContainsKey(text) ? keywords[text] : TokenType.IDENTIFIER;
            addToken(type);
        }
        private void number() {
            while (isDigit(peek())) advance();
            
            if (peek() == '.' && char.IsDigit(peekNext())) {
                advance();
                while (char.IsDigit(peek())) advance();
            }
            addToken(NUMBER, double.Parse(source.Substring(start, current - start)));
        }
        private void String(){
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }
            if (isAtEnd())
            {
                Lox.error(line, "Unterminated string.");
                return;
            }
            
            advance();
            
            string value = source.Substring(start + 1, current - 1);
            addToken(STRING, value);
        }

        private bool match(char expected){

            if (isAtEnd()) return false;
            if (source[current] != expected) return false;
            current++;
            return true;
        }

        private char peek(){
            if (isAtEnd()) return '\0';
            return source[current];
        }

        private char peekNext(){
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool isAlpha(char c){
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool isAlphaNumeric(char c){
            return isAlpha(c) || isDigit(c);
        }

        private bool isDigit(char c){
            return c >= '0' && c <= '9';
        }

        private bool isAtEnd(){
            return current >= source.Length;
        } 
        private char advance(){
            return source[current++];
        }

        private void addToken(TokenType type){
            addToken(type, null);
        }

        private void addToken(TokenType type, object? literal){
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
      
    }
}


