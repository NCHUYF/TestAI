using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// DebugGUI静态类
public static class DebugGUI
{
    private static DebugGUIBehaviour debugGuiBehaviour;

    private static DebugGUIBehaviour GetDebugGUIBehaviour()
    {
        if (debugGuiBehaviour == null)
        {
            var gameObject = new GameObject("DebugGUI") { hideFlags = HideFlags.HideAndDontSave };
            debugGuiBehaviour = gameObject.AddComponent<DebugGUIBehaviour>();
        }

        return debugGuiBehaviour;
    }

    public static void AddDebugItem(string keyName, string valInfo, Color color = default)
    {
        var item = new DebugItemBase(keyName, keyName, () => valInfo, color);
        GetDebugGUIBehaviour().DebugItems[keyName] = item;
    }

    public static void AddDebugItem(string keyName, Func<string> valGetter, Color color = default)
    {
        var item = new DebugItemBase(keyName, keyName,valGetter, color);
        GetDebugGUIBehaviour().DebugItems[keyName] = item;
    }

    public static void AddDebugTotalObject(string keyName, object obj, Color color = default)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var item = new DebugItemObject(keyName, obj, color);
        GetDebugGUIBehaviour().DebugItems[keyName] = item;
    }
}




// 示例：
//// 添加一个固定的字符串调试条目
//DebugGUI.AddDebugItem("Version", "1.0.0");

//// 添加一个获取字符串的函数调试条目，显示红色文本
//DebugGUI.AddDebugItem("Status", () => StatusManager.GetCurrentStatus().ToString(), Color.red);

//// 添加一个获取字符串的函数调试条目，显示默认颜色的文本
//DebugGUI.AddDebugItem("Status Elapsed Time", () => StatusManager.GetStatusElapsedTime().ToString());

//// 监控 Example 对象的所有字段和属性的变化
// DebugGUI.AddDebugTotalObject("example", new Example());


