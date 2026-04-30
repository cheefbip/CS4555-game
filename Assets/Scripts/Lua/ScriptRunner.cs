using System;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class ScriptRunner : MonoBehaviour
{
    private readonly Script script = new();

    private readonly List<DynValue> collisionSubscribers = new();

    public static ScriptRunner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Script.DefaultOptions.DebugPrint = s => Debug.Log(s);

        UserData.RegisterType<LuaVector3>();
        UserData.RegisterType<LuaGameObject>();

        script.Globals["print"] = (Action<string>)(s => Debug.Log(s));

        script.Globals["onCollision"] = (Action<DynValue>)SubscribeCollision;

        RegisterVector3();

        script.DoString(@"
            onCollision(function(groupId, obj)
                print('Hit group: ' .. groupId)
                print('Hit object: ' .. obj.name)

                local p = obj.position
                print('Position: ' .. p.x .. ', ' .. p.y .. ', ' .. p.z)
            end)
        ");
    }

    private void RegisterVector3()
    {
        var vector3Table = new Table(script);

        vector3Table["new"] = (Func<float, float, float, LuaVector3>)((x, y, z) =>
            new LuaVector3(x, y, z)
        );

        script.Globals["Vector3"] = vector3Table;
    }

    private void SubscribeCollision(DynValue callback)
    {
        if (callback.Type != DataType.Function)
        {
            Debug.LogWarning("onCollision expects a function.");
            return;
        }

        collisionSubscribers.Add(callback);
    }

    public void FireCollision(int groupId, GameObject obj)
    {
        LuaGameObject luaObj = new LuaGameObject(obj);

        foreach (DynValue callback in collisionSubscribers)
        {
            script.Call(callback, groupId, luaObj);
        }
    }

    public Script GetScript()
    {
        return script;
    }
}