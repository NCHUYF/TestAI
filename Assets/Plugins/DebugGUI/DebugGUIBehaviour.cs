using System.Collections.Generic;
using UnityEngine;

// DebugGUIBehaviour类
public class DebugGUIBehaviour : MonoBehaviour
{
    public Dictionary<string, IDebugItem> DebugItems = new Dictionary<string, IDebugItem>();
    private Vector2 scrollPosition; // 添加一个变量来存储滚动位置

    private void OnGUI()
    {
        foreach (var item in DebugItems.Values)
        {
            item.Refresh();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition); // 开始ScrollView
        foreach (var item in DebugItems.Values)
        {
            DisplayDebugItem(item);
        }
        GUILayout.EndScrollView(); // 结束ScrollView
    }

    private void DisplayDebugItem(IDebugItem item)
    {
        var keyStyle = new GUIStyle
        {
            normal = { textColor = item.Color },
            fontSize = (int)(item.FontSize * 1.2f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft  // 设置左对齐
        };
        var valueStyle = new GUIStyle
        {
            normal = { textColor = item.Color },
            fontSize = item.FontSize,
            alignment = TextAnchor.MiddleCenter
        };

        item.Display(keyStyle, valueStyle);
    }
}
