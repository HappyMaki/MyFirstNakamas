#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class RunPlayer
{
    [MenuItem("Run/Start Server %&1")]
    public static void StartServer()
    {
        string server_bat = @"/Server/start.bat";
        RunFile(server_bat);
    }

    [MenuItem("Run/Stop Server %&2")]
    public static void StopServer()
    {
        string server_stop_bat = @"/Server/stop.bat";
        RunFile(server_stop_bat);
    }


    public static void RunFile(string path)
    {
        Process.Start(Directory.GetCurrentDirectory() + path);
    }

}
#endif

