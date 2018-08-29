using System;
using System.Collections.Generic;
using System.IO;

public enum Value_Type : Byte
{
    Integer,
    Decimal,
    Boolean,
    String,

}

public class Typed_Value
{
    public Value_Type type;
    public dynamic value;

    public Typed_Value(Value_Type type, dynamic value)
    {
        this.type = type;
        this.value = value;
    }
}

public class Interpreter
{
    // private Stack<Typed_Value> eval_stack; // evaluation stack 求值栈

    public void evaluate(AST ast) // 开始对 AST 求值
    {

    }
}
