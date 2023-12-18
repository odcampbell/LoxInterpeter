using System.Collections.Generic;

namespace LoxApp
{
    class Resolver : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        private readonly Interpreter interpreter;
        Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;

        public Resolver(Interpreter interpreter){
            this.interpreter = interpreter;
        }

        private enum FunctionType {
            NONE,
            FUNCTION
        }

        public void resolve(List<Stmt> statements){ //1

            foreach (Stmt statement in statements){
                resolve(statement);
            }

        }

        private void resolve(Expr expr){

            expr.Accept(this);
        }

        private void beginScope(){
            scopes.Push(new Dictionary<string, bool>());
        }

        private void endScope() {
            scopes.Pop();
        }

        private void declare(Token name){
            if (scopes.Count == 0) return;
            Dictionary<string, bool> scope = scopes.Peek();
             if (scope.ContainsKey(name.lexeme)) {  //unecessary
                Lox.error(name,
                    "WHAT ----     Already a variable with this name in this scope.");
                    
            }

            scope[name.lexeme] = false;
        }

        private void define(Token name){
            if (scopes.Count == 0) return;

            scopes.Peek()[name.lexeme] = true;
        }

        private void resolveLocal(Expr expr, Token name){
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    interpreter.resolve(expr, scopes.Count - 1 - i); //distance
                    return;
                }
            }
        }

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public object? VisitBlockStmt(Stmt.Block stmt){
            beginScope();
            resolve(stmt.statements);
            endScope();
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public object? VisitExpressionStmt(Stmt.Expression stmt) { //V?
            resolve(stmt.expression);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public object? VisitFunctionStmt(Stmt.Function stmt){
            declare(stmt.name);
            define(stmt.name);
            resolveFunction(stmt, FunctionType.FUNCTION);//FIXME??
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
      
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitIfStmt(Stmt.If stmt) {
            resolve(stmt.condition); ///
            resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) resolve(stmt.elseBranch);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitPrintStmt(Stmt.Print stmt) { //
            resolve(stmt.expression);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitReturnStmt(Stmt.Return stmt) { //v

            if (currentFunction == FunctionType.NONE) {
                Lox.error(stmt.keyword, "Can't return from top-level code.");
            }

            if (stmt.value != null) {
                resolve(stmt.value);
            }

            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitVarStmt(Stmt.Var stmt){
            declare(stmt.name);
            if (stmt.initializer != null) //fixme??
            {
                resolve(stmt.initializer);//literal
            }
            define(stmt.name);//end

            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
         public  object? VisitWhileStmt(Stmt.While stmt) {//v
            resolve(stmt.condition);
            resolve(stmt.body);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitVariableExpr(Expr.Variable expr){
            if (scopes.Count > 0 && scopes.Peek().ContainsKey(expr.name.lexeme) //input here??
                && scopes.Peek()[expr.name.lexeme] == false){
                Lox.error(expr.name, "Can't read local variable in its own initializer.");
            }
            resolveLocal(expr, expr.name);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitAssignExpr(Expr.Assign expr){
            resolve(expr.value);
            resolveLocal(expr, expr.name);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
          public  object? VisitBinaryExpr(Expr.Binary expr) {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitCallExpr(Expr.Call expr){
            resolve(expr.callee);
            foreach (Expr argument in expr.arguments){
                resolve(argument);
            }
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitGroupingExpr(Expr.Grouping expr) {
            resolve(expr.expression);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
         public  object? VisitLiteralExpr(Expr.Literal expr) {
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
          public  object? VisitLogicalExpr(Expr.Logical expr) {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public  object? VisitUnaryExpr(Expr.Unary expr) {
            resolve(expr.right);
            return null;
        }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

        private void resolve(Stmt stmt){

            stmt.Accept(this);
        }
       
         private void resolveFunction( Stmt.Function function, FunctionType type) {

            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;

            beginScope();
            foreach (Token param in function.@params)
            {
                declare(@param);
                define(@param);
            }
            resolve(function.body);
            endScope();

            currentFunction = enclosingFunction;
        }

    }
}


