using System;
using System.IO;

public static class Tokenizer_Test
{
    private class Test_Unit
    {
        public string input;
        public Token expected_output;

        public Test_Unit(string input, Token expected_output)
        {
            this.input = input;
            this.expected_output = expected_output;
        }
    }

    private static Test_Unit[] test = new Test_Unit[]
    {
        // Terminator, Space
        new Test_Unit("", new Token(1, 1, Token_Type.Terminator, null)),
        new Test_Unit(" \t\n", new Token(2, 1, Token_Type.Terminator, null)),

        // Bracket
        new Test_Unit("(", new Token(1, 1, Token_Type.Bracket, "(")),
        new Test_Unit(")", new Token(1, 1, Token_Type.Bracket, ")")),
        new Test_Unit("[", new Token(1, 1, Token_Type.Bracket, "[")),
        new Test_Unit("]", new Token(1, 1, Token_Type.Bracket, "]")),
        new Test_Unit("{", new Token(1, 1, Token_Type.Bracket, "{")),
        new Test_Unit("}", new Token(1, 1, Token_Type.Bracket, "}")),

        // Operator
        new Test_Unit("+", new Token(1, 1, Token_Type.Operator, "+")),
        new Test_Unit("-", new Token(1, 1, Token_Type.Operator, "-")),
        new Test_Unit("*", new Token(1, 1, Token_Type.Operator, "*")),
        new Test_Unit("/", new Token(1, 1, Token_Type.Operator, "/")),
        new Test_Unit("^", new Token(1, 1, Token_Type.Operator, "^")),
        new Test_Unit(".", new Token(1, 1, Token_Type.Operator, ".")),
        new Test_Unit("and", new Token(1, 1, Token_Type.Operator, "and")),
        new Test_Unit("or", new Token(1, 1, Token_Type.Operator, "or")),
        new Test_Unit("not", new Token(1, 1, Token_Type.Operator, "not")),
        new Test_Unit("mod", new Token(1, 1, Token_Type.Operator, "mod")),

        // Keyword
        new Test_Unit("define", new Token(1, 1, Token_Type.Keyword, "define")),
        new Test_Unit("let", new Token(1, 1, Token_Type.Keyword, "let")),

        new Test_Unit("if", new Token(1, 1, Token_Type.Keyword, "if")),
        new Test_Unit("else", new Token(1, 1, Token_Type.Keyword, "else")),

        // Null
        new Test_Unit("null", new Token(1, 1, Token_Type.Null, "null")),

        // Boolean
        new Test_Unit("true", new Token(1, 1, Token_Type.Boolean, "true")),
        new Test_Unit("false", new Token(1, 1, Token_Type.Boolean, "false")),

        // Integer, Decimal
        new Test_Unit("0", new Token(1, 1, Token_Type.Integer, "0")),
        new Test_Unit("1", new Token(1, 1, Token_Type.Integer, "1")),
        new Test_Unit("2548705244", new Token(1, 1, Token_Type.Integer, "2548705244")),

        new Test_Unit("0.0", new Token(1, 1, Token_Type.Decimal, "0.0")),
        new Test_Unit("0.00001", new Token(1, 1, Token_Type.Decimal, "0.00001")),
        new Test_Unit("3.1415926535", new Token(1, 1, Token_Type.Decimal, "3.1415926535")),


    };

    private static void print_token(Token token)
    {
        Console.WriteLine("Line: {0}, Column = {1}", token.line, token.column);
        Console.WriteLine("Type: {0}", token.type);
        Console.WriteLine("Value: {0}", token.value?? "null");
    }

    private static bool equal(Token t1, Token t2)
    {
        return t1.line == t2.line
            && t1.column == t2.column
            && t1.type == t2.type
            && t1.value == t2.value;
    }

    public static void run()
    {
        foreach (Test_Unit unit in test)
        {
            var tokenizer = new Tokenizer(new StringReader(unit.input));
            var token = tokenizer.read_token();
            if (equal(token, unit.expected_output))
            {
                Console.WriteLine("[ PASSED ] \"{0}\"", unit.input);
            }
            else
            {
                Console.WriteLine("[ FAILED ] \"{0}\"", unit.input);
                Console.WriteLine("Expected Output:");
                print_token(unit.expected_output);
                Console.WriteLine("Actual Output:");
                print_token(token);
                return;
            }
        }
    }
}
