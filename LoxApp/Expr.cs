using System.Collections.Generic;

namespace LoxApp
{
    abstract class Expr
    {
  public interface Visitor<R> {
    R VisitBinaryExpr(Binary expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitUnaryExpr(Unary expr);
  }
  public class Binary : Expr {
    public Binary(Expr left, Token @operator, Expr right) {
      this.left = left;
      this.@operator = @operator;
      this.right = right;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitBinaryExpr(this);
    }

    readonly Expr left;
    readonly Token @operator;
    readonly Expr right;
  }
  public class Grouping : Expr {
    public Grouping(Expr expression) {
      this.expression = expression;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitGroupingExpr(this);
    }

    readonly Expr expression;
  }
  public class Literal : Expr {
    public Literal(Object value) {
      this.value = value;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitLiteralExpr(this);
    }

    readonly Object value;
  }
  public class Unary : Expr {
    public Unary(Token @operator, Expr right) {
      this.@operator = @operator;
      this.right = right;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitUnaryExpr(this);
    }

    readonly Token @operator;
    readonly Expr right;
  }

  public abstract R Accept<R>(Visitor<R> visitor);
    }
}
