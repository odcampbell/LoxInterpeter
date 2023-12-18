using System.Collections.Generic;

namespace LoxApp
{
    abstract public class Expr
    {
  public interface Visitor<R> {
    public R VisitAssignExpr(Assign expr);
    public R VisitBinaryExpr(Binary expr);
    public R VisitCallExpr(Call expr);
    public R VisitGetExpr(Get expr);
    public R VisitGroupingExpr(Grouping expr);
    public R VisitLiteralExpr(Literal expr);
    public R VisitLogicalExpr(Logical expr);
    public R VisitSetExpr(Set expr);
    public R VisitThisExpr(This expr);
    public R VisitUnaryExpr(Unary expr);
    public R VisitVariableExpr(Variable expr);
  }
  public class Assign : Expr {
    public Assign(Token name, Expr value) {
      this.name = name;
      this.value = value;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitAssignExpr(this);
    }

    public readonly Token name;
    public readonly Expr value;
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

    public readonly Expr left;
    public readonly Token @operator;
    public readonly Expr right;
  }
  public class Call : Expr {
    public Call(Expr callee, Token @paren, List<Expr> arguments) {
      this.callee = callee;
      this.@paren = @paren;
      this.arguments = arguments;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitCallExpr(this);
    }

    public readonly Expr callee;
    public readonly Token @paren;
    public readonly List<Expr> arguments;
  }
  public class Get : Expr {
    public Get(Expr objt, Token @name) {
      this.objt = objt;
      this.@name = @name;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitGetExpr(this);
    }

    public readonly Expr objt;
    public readonly Token @name;
  }
  public class Grouping : Expr {
    public Grouping(Expr expression) {
      this.expression = expression;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitGroupingExpr(this);
    }

    public readonly Expr expression;
  }
  public class Literal : Expr {
    public Literal(Object value) {
      this.value = value;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitLiteralExpr(this);
    }

    public readonly Object value;
  }
  public class Logical : Expr {
    public Logical(Expr left, Token @operator, Expr right) {
      this.left = left;
      this.@operator = @operator;
      this.right = right;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitLogicalExpr(this);
    }

    public readonly Expr left;
    public readonly Token @operator;
    public readonly Expr right;
  }
  public class Set : Expr {
    public Set(Expr objt, Token @name, Expr value) {
      this.objt = objt;
      this.@name = @name;
      this.value = value;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitSetExpr(this);
    }

    public readonly Expr objt;
    public readonly Token @name;
    public readonly Expr value;
  }
  public class This : Expr {
    public This(Token @keyword) {
      this.@keyword = @keyword;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitThisExpr(this);
    }

    public readonly Token @keyword;
  }
  public class Unary : Expr {
    public Unary(Token @operator, Expr right) {
      this.@operator = @operator;
      this.right = right;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitUnaryExpr(this);
    }

    public readonly Token @operator;
    public readonly Expr right;
  }
  public class Variable : Expr {
    public Variable(Token name) {
      this.name = name;
    }

    public override R Accept<R>(Visitor<R> visitor) {
        return visitor.VisitVariableExpr(this);
    }

    public readonly Token name;
  }

  public abstract R Accept<R>(Visitor<R> visitor);
    }
}
