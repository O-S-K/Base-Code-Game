using UnityEngine;

public class ConsoleToGUI : MonoBehaviour
{
    string myLog = "*begin log";
    string filename = "";
    bool doShow = true;
    int kChars = 1000;

    void OnEnable() => Application.logMessageReceived += Log;
    void OnDisable() => Application.logMessageReceived -= Log; 

    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.Space)) 
        { 
            doShow = !doShow; 
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Application.logMessageReceived += Log;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Application.logMessageReceived -= Log;
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        // for onscreen...
        myLog = myLog + "\n" + logString;
        if (myLog.Length > kChars) 
        { 
            myLog = myLog.Substring(myLog.Length - kChars); 
        }

        // for the file ...
        if (filename == "")
        {
            string d = System.Environment.GetFolderPath(
               System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
            System.IO.Directory.CreateDirectory(d);
            string r = Random.Range(1000, 9999).ToString();
            filename = d + "/log-" + r + ".txt";
        }
        try { System.IO.File.AppendAllText(filename, logString + "\n"); }
        catch { }
    }

    void OnGUI()
    {
        if (!doShow) { return; }
        var screen = new Vector3(Screen.width / 1200F, Screen.height / 800F, 1.0F);
        //GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, screen);
        GUI.TextArea(new Rect(10, 10, 250, 700), myLog);
    }
}
