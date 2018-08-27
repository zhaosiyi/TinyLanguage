using System;
using System.Text;

public class AST
{
    public Token token;
    public AST child, next;

    public AST(Token token, AST child = null, AST next = null)
    {
        this.token = token;
        this.child = child;
        this.next = next;
    }

    public Token_Type type()
    {
        return token.type;
    }

    public bool has_child()
    {
        return child != null;
    }

    public bool has_next()
    {
        return next != null;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        if (has_child()) builder.Append(child.ToString());
        else
        {
            builder.Append('(');
            builder.Append(token.value);
            builder.Append(' ');
            builder.Append(child.ToString());
            builder.Append(')');
        }
        if (has_next())
        {
            builder.Append(' ');
            builder.Append(next.ToString());
        }

        return builder.ToString();
    }
}


public class Parser
{
    private Tokenizer tokenizer;
}
