using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class LuaGameObject
{
    private readonly GameObject gameObject;

    internal LuaGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public string name
    {
        get => gameObject.name;
    }

    public LuaVector3 position
    {
        get => new LuaVector3(gameObject.transform.position);
        set => gameObject.transform.position = value.ToUnityVector3();
    }
}