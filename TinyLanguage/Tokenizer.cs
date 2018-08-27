using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum Token_Type : byte
{
    Terminator = 0,
    Null,
    Integer,
    Decimal,
    Boolean,
    Keyword,
    Name,
    Bracket,
    Operator,
}

public class Token
{
    public uint line, column;
    public Token_Type type;
    public string value;

    public Token(uint line, uint column)
    {
        this.line = line;
        this.column = column;
    }

    public Token(uint line, uint column, Token_Type type, string value)
    {
        this.line = line;
        this.column = column;
        this.type = type;
        this.value = value;
    }
}

public class Tokenizer
{
    private TextReader reader;
    private int next_char, next_next;
    private uint line ,column;

    private static HashSet<string> keywords = new HashSet<string>()
    {
        "define", "let",
        "if", "else",
    };

    private static HashSet<string> operators = new HashSet<string>()
    {
        "and", "or", "not",
        "mod"
    };

    public void load(TextReader input)
    {
        if (input == null) throw new Exception();
        reader = input;
        next_char = reader.Read();
        next_next = reader.Read();
        line = column = 1;
    }

    public Tokenizer(TextReader input)
    {
        load(input);
    }

    private int read_char()
    {
        int result = next_char;
        next_char = next_next;
        next_next = reader.Read();

        if (result == '\n')
        {
            ++line; column = 1;
        }
        else if (result != -1)
        {
            ++column;
        }

        return result;
    }

    private bool is_space(int c)
    {
        return c == ' ' || c == '\n' || c == '\r' || c == '\t';
    }

    private void skip_space()
    {
        while (is_space(next_char)) read_char();
    }

    private bool is_digit(int c)
    {
        return c >= '0' && c <= '9';
    }

    // 判断 c 是否是组成标识符的字母
    private bool is_identifier_letter(int c)
    {
        return (c >= 'a' && c <= 'z')
            || (c >= 'A' && c <= 'Z')
            || c == '_';
    }

    public Token read_token()
    {
        skip_space();
        switch (next_char)
        {
            case '(': case ')':
            case '[': case ']':
            case '{': case '}':
            return new Token(line, column, Token_Type.Bracket, ((char)read_char()).ToString());

            case '+': case '-':
            case '*': case '/':
            case '^': case '.':
            return new Token(line, column, Token_Type.Operator, ((char)read_char()).ToString());
            
            case -1: return new Token(line, column, Token_Type.Terminator, null);

            default:
            var token = new Token(line, column);
            var builder = new StringBuilder();

            if (is_identifier_letter(next_char))
            {
                do { builder.Append((char)read_char()); }
                while (is_identifier_letter(next_char));

                string result = builder.ToString();
                if (keywords.Contains(result))
                {
                    token.type = Token_Type.Keyword;
                }
                else if (operators.Contains(result))
                {
                    token.type = Token_Type.Operator;
                }
                else if (result == "true" || result == "false")
                {
                    token.type = Token_Type.Boolean;
                }
                else if (result == "null")
                {
                    token.type = Token_Type.Null;
                }
                else
                {
                    token.type = Token_Type.Name;
                }

                token.value = result;
            }
            else if (is_digit(next_char))
            {
                do { builder.Append((char)read_char()); }
                while (is_digit(next_char));

                if (next_char == '.' && is_digit(next_next))
                {
                    do { builder.Append((char)read_char()); }
                    while (is_digit(next_char));
                    token.type = Token_Type.Decimal;
                }
                else token.type = Token_Type.Integer;

                token.value = builder.ToString();
            }
            else throw new Exception();      

            return token;
        }
    }
}

