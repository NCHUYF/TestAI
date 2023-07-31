using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugGUITest : MonoBehaviour
{
    private class TestObject
    {
        public int IntegerValue = 123;
        public float FloatValue = 456.789f;
        public string StringValue = "Hello, world!";
        public Color ColorValue = Color.green;
        public Vector3 Vector3Value = new Vector3(1, 2, 3);
        public Dictionary<string, int> DictionaryValue = new Dictionary<string, int> { ["one"] = 1, ["two"] = 2 };
        public List<int> ListValue = new List<int> { 1, 2, 3, 4, 5 };
    }

    private TestObject testObject = new TestObject();
    private float timeSinceStartup;
    private string testString = "Initial String";

    private void Start()
    {
        DebugGUI.AddDebugTotalObject("testObject", testObject);
        DebugGUI.AddDebugItem("timeSinceStartup", () => timeSinceStartup.ToString());
        DebugGUI.AddDebugItem("testString-1", testString);
        testString = "modify string";
        DebugGUI.AddDebugItem("testString-2", testString);
    }

    private void Update()
    {
        timeSinceStartup = Time.timeSinceLevelLoad;

        // 更新测试对象的值
        if (testObject != null)
        {
            testObject.IntegerValue = (int)(timeSinceStartup * 100);
            testObject.FloatValue = timeSinceStartup;
            testObject.StringValue = $"Time: {timeSinceStartup}";
            testObject.ColorValue = new Color(timeSinceStartup % 1, (timeSinceStartup * 0.5f) % 1, (timeSinceStartup * 0.2f) % 1);
            testObject.Vector3Value = new Vector3(timeSinceStartup, timeSinceStartup * 2, timeSinceStartup * 3);
            testObject.DictionaryValue["one"] = (int)timeSinceStartup;
            testObject.ListValue = Enumerable.Range(0, (int)timeSinceStartup % 100).ToList();
        }

        // 更新测试字符串的值
        testString = $"Time: {timeSinceStartup}";

        // 模拟对象销毁
        if (timeSinceStartup > 10) // 在10秒后销毁测试对象
        {
            testObject = null;
        }
    }
}