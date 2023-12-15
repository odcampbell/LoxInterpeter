using System.Collections.Generic;

namespace LoxApp
{
    abstract class Expr
    {
  public class Binary : Expr {
    public Binary(Expr left, Token @operator, Expr right) {
      this.left = left;
      this.@operator = @operator;
      this.right = right;
    }

    readonly Expr left;
    readonly Token @operator;
    readonly Expr right;
  }
  public class Grouping : Expr {
    public Grouping(Expr expression) {
      this.expression = expression;
    }

    readonly Expr expression;
  }
  public class Literal : Expr {
    public Literal(Object value) {
      this.value = value;
    }

    readonly Object value;
  }
  public class Unary : Expr {
    public Unary(Token @operator, Expr right) {
      this.@operator = @operator;
      this.right = right;
    }

    readonly Token @operator;
    readonly Expr right;
  }
    }
}
