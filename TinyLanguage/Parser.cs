using System;
using System.Text;
using System.Collections.Generic;

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
        if (has_child() == false) builder.Append(token.value);
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
    private Token next_token;

    public void load(Tokenizer tokenizer)
    {
        if (tokenizer == null) throw new Exception();
        this.tokenizer = tokenizer;
        next_token = tokenizer.read_token();
    }

    public Parser(Tokenizer tokenizer)
    {
        load(tokenizer);
    }

    private Token read_token()
    {
        var result = next_token;
        next_token = tokenizer.read_token();
        return result;
    }

    public AST parse()
    {
        return parse_expression();
    }

    private AST parse_expression()
    {
        return parse_multiply_divide();
    }

    // sub: parse_positive_negative
    private AST parse_multiply_divide()
    {
        AST left_operand = parse_positive_negative();

        while (next_token.is_operator() &&
              (next_token.value == "*" || next_token.value == "/"))
        {
            AST @operator = new AST(read_token());
            AST right_operand = parse_positive_negative();

            @operator.child = left_operand;
            left_operand.next = right_operand;
            left_operand = @operator;
        }

        return left_operand;
    }

    // sub: parse_power
    private AST parse_positive_negative()
    {
        AST @operator;
        if (next_token.is_operator())
        {         
            if (next_token.value == "+")
            {
                var token = read_token();
                token.value = "positive";
                @operator = new AST(token);
            }
            else if (next_token.value == "-")
            {
                var token = read_token();
                token.value = "negative";
                @operator = new AST(token);
            }
            else @operator = null;
        }
        else @operator = null;

        AST operand = parse_power();

        if (@operator != null)
        {
            @operator.child = operand;
            return @operator;
        }
        else return operand;
    }

    // sub: parse_atom
    private AST parse_power() // 解析 expr1 (^ expr2)*
    {
        // 因为 ^ 是右结合的，所以从右向左处理操作数。
        var operands = new Stack<AST>(); // 保存待处理的操作数。
        var operators = new Stack<AST>(); // 保存待处理的 “^” Token。

        operands.Push(parse_atom()); // 读取 expr1

        while (next_token.is_operator() && next_token.value == "^")
        {
            operators.Push(new AST(read_token())); // 读取 ^
            operands.Push(parse_atom()); // 读取 expr2
        }

        AST right_operand = operands.Pop(); // left ^ right 的 right
                                            
        while (operands.Count > 0)
        {
            AST left_operand = operands.Pop();
            AST power_operator = operators.Pop();

            power_operator.child = left_operand;
            left_operand.next = right_operand;

            right_operand = power_operator; // a^b^c 中 b ^ c 整体相当于 a ^ x 的 x 。        
        }

        return right_operand;
    }

    private AST parse_atom()
    {
        switch (next_token.type)
        {
            case Token_Type.Integer:
            case Token_Type.Decimal:
            case Token_Type.Boolean:
            return new AST(read_token());

            case Token_Type.Bracket:
            {
                var left_bracket = read_token();
                if (left_bracket.value == "(")
                {
                    AST sub_expression = parse_expression();
                    var right_bracket = read_token();
                    if (right_bracket.is_bracket() && right_bracket.value == ")")
                        return sub_expression;
                    else throw new Exception();
                }
                else throw new Exception();
            }

            default:
            throw new Exception();
        }
    }



}
