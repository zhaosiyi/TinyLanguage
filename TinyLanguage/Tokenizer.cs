using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum Token_Type : byte
{
    Terminator = 0, // EOF
    Null, Integer, Decimal, Boolean, String,// value
    Keyword,
    Name,
    Bracket,
    Operator, // + - * / ^ mod and or not & | ~ is in > < == >= <= !=
    Assignment, // =
    Arrow, // =>
    Colon, // :
    Semicolon, // ;
    Comma, // ,
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

    public bool is_bracket()
    {
        return type == Token_Type.Bracket;
    }

    public bool is_operator()
    {
        return type == Token_Type.Operator;
    }
}

public class Tokenizer
{
    private TextReader reader;
    private int next_char, next_next; // 仅在解析数字时使用 next_next
    private uint line ,column;
    private Token_Type token_type; // 当前正在解析的 token 的类型
    private StringBuilder builder;

    private static HashSet<string> keywords = new HashSet<string>()
    {
        "define", "let",
        "if", "else",
        "while", "for", "break", "continue",
        "function", "return",
    };

    private static HashSet<string> operators = new HashSet<string>()
    {
        "and", "or", "not",
        "mod",
        "in", "is"
    };

    public Tokenizer(TextReader input)
    {
        builder = new StringBuilder();
        load(input);
    }

    public void load(TextReader input)
    {
        reader = input;
        next_char = reader.Read();
        next_next = reader.Read();
        line = column = 1;
        builder.Clear();
    }

    public Token read_token()
    {
        skip_space_and_comment();

        var result = new Token(line, column);

        begin_parse();

        result.type = token_type;
        result.value = builder.ToString();

        builder.Clear();
        return result;
    }

    private void begin_parse()
    {
        if (is_letter(next_char))
        {
            parse_letter(); // name, keyword, boolean(true, false), null, operator(and, or, not...)
        }
        else if (is_digit(next_char))
        {
            parse_number(); // integer or decimal
        }
        else switch (next_char)
        {
            // 括号
            case '(': case ')': case '[': case ']': case '{': case '}':
            {
                append_char_to_builder();
                token_type = Token_Type.Bracket;
            }
            break;

            case '+': case '-': // 加减号或正负号
            {
                append_char_to_builder();
                token_type = Token_Type.Operator;
            }
            break;

            case '*': case '/': case '^': case '.':
            case '&': case '|': case '~':
            {
                append_char_to_builder();
                token_type = Token_Type.Operator;
            }
            break;

            case '>':
            case '<':
            {
                append_char_to_builder(); // >, <
                if (next_char == '=')
                {
                    append_char_to_builder(); // >=, <=
                }
                token_type = Token_Type.Operator;
            }
            break;

            case '=':
            {
                append_char_to_builder();
                if (next_char == '>') // =>
                {
                    append_char_to_builder();
                    token_type = Token_Type.Arrow;
                }
                else if (next_char == '=') // ==
                {
                    append_char_to_builder();
                    token_type = Token_Type.Operator;
                }
                else token_type = Token_Type.Assignment; // =
            }
            break;

            case '!':
            {
                append_char_to_builder();
                if (next_char == '=') // !=
                {
                    append_char_to_builder();
                    token_type = Token_Type.Operator;
                }
                else throw new Exception();
            }
            break;

            case ':':
            {
                append_char_to_builder();
                token_type = Token_Type.Colon;
            }
            break;

            case ';':
            {
                append_char_to_builder();
                token_type = Token_Type.Semicolon;
            }
            break;

            case ',':
            {
                append_char_to_builder();
                token_type = Token_Type.Comma;
            }
            break;

            case -1: token_type = Token_Type.Terminator; break;

            default: throw new Exception();
        }
    }

    private void skip_space_and_comment()
    {
        while (is_space(next_char)) read_char();
    }

    private bool is_space(int c)
    {
        return c == ' ' || c == '\n' || c == '\r' || c == '\t';
    }

    private void parse_letter()
    {
        do { append_char_to_builder(); }
        while (is_letter(next_char));

        string s = builder.ToString();
        builder.Clear();

        token_type = keywords.Contains(s)? Token_Type.Keyword
        : operators.Contains(s)?           Token_Type.Operator
        : (s == "true" || s == "false")?   Token_Type.Boolean
        : (s == "null")?                   Token_Type.Null
        : Token_Type.Name;

        builder.Append(s);
    }

    private void parse_number()
    {
        do { append_char_to_builder(); }
        while (is_digit(next_char));

        if (next_char == '.' && is_digit(next_next))
        {
            do { append_char_to_builder(); }
            while (is_digit(next_char));
            token_type = Token_Type.Decimal;
        }
        else token_type = Token_Type.Integer;
    }

    private void append_char_to_builder()
    {
        builder.Append((char)read_char());
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

    private bool is_digit(int c)
    {
        return c >= '0' && c <= '9';
    }

    // 判断 c 是否是组成标识符的字母
    private bool is_letter(int c)
    {
        return (c >= 'a' && c <= 'z')
            || (c >= 'A' && c <= 'Z')
            || c == '_';
    }
}

