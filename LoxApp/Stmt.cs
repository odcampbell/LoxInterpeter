using System.Collections.Generic;

namespace LoxApp
{
    abstract public class Stmt
    {
  public interface Visitor<R> {
    public R VisitExpressionStmt(Expression stmt);
    public R VisitPrintStmt(Print stmt);
  }
  public class Expression : Stmt {
    public Expression(Expr expression) {
      this.expression = expression;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitExpressionStmt(this);
    }

    public readonly Expr expression;
  }
  public class Print : Stmt {
    public Print(Expr expression) {
      this.expression = expression;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitPrintStmt(this);
    }

    public readonly Expr expression;
  }

  public abstract R Accept<R>(Visitor<R> visitor);
    }
}
