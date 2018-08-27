using System;
using System.IO;
using System.Text;

public static class Parser_Test
{
    private class Test_Unit
    {
        public string input;
        public string expected_output; // 直接比较打印的 AST 字符串是否相同。

        public Test_Unit(string input, string expected_output)
        {
            this.input = input;
            this.expected_output = expected_output;
        }
    }

    private static Test_Unit[] test = new Test_Unit[]
    {
        // Integer
        new Test_Unit("0", "0"),
        new Test_Unit("1", "1"),
        new Test_Unit("1234567", "1234567"),

        // Decimal
        new Test_Unit("0.0", "0.0"),
        new Test_Unit("3.1415926535", "3.1415926535"),

        // Boolean
        new Test_Unit("true", "true"),
        new Test_Unit("false", "false"),

        // Bracket
        new Test_Unit("(0)", "0"),
        new Test_Unit("(( (true)) )", "true"),

        // power
        new Test_Unit("1 ^ 2", "(^ 1 2)"),
        new Test_Unit("1 ^ 2 ^ 3 ^ 4 ^ 5", "(^ 1 (^ 2 (^ 3 (^ 4 5))))"),
        new Test_Unit("(1 ^ 2) ^ 3", "(^ (^ 1 2) 3)"),
        new Test_Unit("(1 ^ 2) ^ (3 ^ 4)", "(^ (^ 1 2) (^ 3 4))"),


    };

    public static void run()
    {
        Console.WriteLine("##### Parser 测试 #####");
        foreach (Test_Unit unit in test)
        {
            var tokenizer = new Tokenizer(new StringReader(unit.input));
            var parser = new Parser(tokenizer);

            AST ast = parser.parse();
            string actual_output = ast.ToString();

            if (actual_output == unit.expected_output)
            {
                Console.WriteLine("[ 通过 ] \"{0}\"", unit.input);
            }
            else
            {
                Console.WriteLine("[ 失败 ] \"{0}\"", unit.input);
                Console.WriteLine("期望输出:");
                Console.WriteLine("{0}", unit.expected_output);
                Console.WriteLine("实际输出:");
                Console.WriteLine("{0}", actual_output);
                return;
            }
        }
        Console.WriteLine("Parser 测试全部通过\n");
    }


}
