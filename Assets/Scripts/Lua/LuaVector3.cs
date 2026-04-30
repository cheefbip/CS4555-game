using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class LuaVector3
{
    private Vector3 value;

    public float x
    {
        get => value.x;
        set => this.value.x = value;
    }

    public float y
    {
        get => value.y;
        set => this.value.y = value;
    }

    public float z
    {
        get => value.z;
        set => this.value.z = value;
    }

    public LuaVector3(float x, float y, float z)
    {
        value = new Vector3(x, y, z);
    }

    public LuaVector3(Vector3 value)
    {
        this.value = value;
    }

    public float magnitude => value.magnitude;

    public LuaVector3 normalized => new LuaVector3(value.normalized);

    public float distanceTo(LuaVector3 other)
    {
        return Vector3.Distance(value, other.value);
    }

    public static LuaVector3 operator +(LuaVector3 a, LuaVector3 b)
    {
        return new LuaVector3(a.value + b.value);
    }

    public static LuaVector3 operator -(LuaVector3 a, LuaVector3 b)
    {
        return new LuaVector3(a.value - b.value);
    }

    public static LuaVector3 operator *(LuaVector3 a, float scalar)
    {
        return new LuaVector3(a.value * scalar);
    }

    public static LuaVector3 operator *(float scalar, LuaVector3 a)
    {
        return new LuaVector3(a.value * scalar);
    }

    public static LuaVector3 operator /(LuaVector3 a, float scalar)
    {
        return new LuaVector3(a.value / scalar);
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public Vector3 ToUnityVector3()
    {
        return value;
    }
}