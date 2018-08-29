using System;
using System.Collections.Generic;
using System.IO;

enum Value_Type : Byte
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

    public Typed_Value(Value_Type t, dynamic v)
    {
        this.type = t;
        this.value = v;
    }
}

public class Interpreter
{
    private Stack<Typed_Value> eval_stack; // evaluation stack 求值栈

    public void evaluate()
    {

    }
}
