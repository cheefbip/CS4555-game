using UnityEngine;
using MoonSharp.Interpreter;
using System;

public class ScriptRunner : MonoBehaviour
{
    private readonly Script script = new();
    public static ScriptRunner Instance { get; private set; }

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Script.DefaultOptions.DebugPrint = s => Debug.Log(s);
        script.Globals["print"] = (Action<DynValue>)CustomPrint;

        UserData.RegisterType<Vector3>();
        script.Globals["Vector3"] = (Func<float, float, float, Vector3>)((x, y, z) => new Vector3(x, y, z));
    }

    void Start()
    {
        script.DoString("print('Hello from Lua!')");
    }

    private void CustomPrint(DynValue value)
    {
        Debug.Log(value.ToPrintString());
    }

    public Script GetScript()
    {
        return Instance.script;
    }
}