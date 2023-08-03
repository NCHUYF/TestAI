using EzySlice;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab; // 障碍物预制体
    public Transform startZ; // 开始生成的位置
    public Transform endZ; // 结束生成的位置
    public Transform startX; // X轴开始范围
    public Transform endX; // X轴结束范围
    public Transform ClipMinX; // X轴最小剪切范围
    public Transform ClipMaxX; // X轴最大剪切范围
    public float speed = 1f; // 障碍物移动速度
    public float spawnRate = 5f; // 初始生成频率
    public float difficultyIncreaseTime = 5; // 难度增加的时间间隔
    public float maxSpawnRate = 5f; // 最大生成频率
    public float maxXOffset = 10f; // 最大的X偏移量


    private List<GameObject> obstacles; // 障碍物列表
    private float nextSpawnTime = 0f; // 下次生成的时间
    private float timer = 0f; // 计时器
    private float currentXOffset; // 当前X偏移量

    private void Start()
    {
        obstacles = new List<GameObject>(); // 初始化列表
        Reset();
    }

    void Update()
    {
        // 如果到了下次生成的时间
        if (Time.time > nextSpawnTime)
        {
            // 生成障碍物
            SpawnObstacle();

            // 更新下次生成的时间
            nextSpawnTime = Time.time + spawnRate;
        }

        // 移动障碍物并检查是否达到endZ
        MoveAndCheckObstacles();

        // 难度增加逻辑
        IncreaseDifficulty();
    }

    float RandomSign()
    {
        return (Random.Range(0f, 1f) < 0.5f) ? -1f : 1f;
    }

    void SpawnObstacle()
    {
        // 如果是第一个障碍物，那么不需要偏移量
        float randomX = startZ.transform.localPosition.x;
        if (obstacles.Count > 0)
        {
            randomX = Random.Range(startX.localPosition.x, endX.localPosition.x);
            float lastX = obstacles[obstacles.Count - 1].transform.localPosition.x;
            float offsetX = RandomSign() * currentXOffset;
            randomX = Mathf.Clamp(lastX + offsetX, startX.localPosition.x, endX.localPosition.x); // 限制范围
        }

        Vector3 spawnPosition = new Vector3(randomX, startZ.localPosition.y, startZ.localPosition.z);

        // 创建障碍物并添加到列表
        GameObject obstacle = Instantiate(obstaclePrefab);
        obstacle.transform.SetParent(transform, false);
        obstacle.transform.localPosition = spawnPosition;

        // 裁切场景外的部分
        var subs = obstacle.GetComponentsInChildren<MeshFilter>();
        foreach (var sub in subs)
        {
            SlicedHull hull = sub.gameObject.Slice(ClipMinX.position, Vector3.left);
            if (hull == null) continue;
            GameObject lower = hull.CreateLowerHull(sub.gameObject, sub.GetComponent<MeshRenderer>().material);
            lower.AddComponent<BoxCollider>();
            lower.transform.SetParent(obstacle.transform, false); // 将新物体设置为障碍物的子对象
            Destroy(sub.gameObject);
        }
        foreach (var sub in subs)
        {
            SlicedHull hull = sub.gameObject.Slice(ClipMaxX.position, Vector3.right);
            if (hull == null) continue;
            GameObject lower = hull.CreateLowerHull(sub.gameObject, sub.GetComponent<MeshRenderer>().material);
            lower.AddComponent<BoxCollider>();
            lower.transform.SetParent(obstacle.transform, false); // 将新物体设置为障碍物的子对象
            Destroy(sub.gameObject);
        }
        obstacles.Add(obstacle);
    }

    void MoveAndCheckObstacles()
    {
        // 移动障碍物
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = obstacles[i];
            obstacle.transform.Translate(Vector3.back * speed * Time.deltaTime);

            // 如果达到endZ，销毁障碍物并从列表中移除
            if (obstacle.transform.localPosition.z < endZ.localPosition.z)
            {
                Destroy(obstacle);
                obstacles.RemoveAt(i);
            }
        }
    }

    void IncreaseDifficulty()
    {
        timer += Time.deltaTime;

        if (timer > difficultyIncreaseTime)
        {
            // 难度增加
            spawnRate = Mathf.Max(spawnRate * 0.95f, maxSpawnRate);
            currentXOffset = Mathf.Min(currentXOffset + 0.1f, maxXOffset);

            // 重置计时器
            timer = 0;
        }
    }

    public void Reset()
    {
        // 销毁所有障碍物并清空列表
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        obstacles.Clear();

        timer = 0;
        nextSpawnTime = 0;
        spawnRate = 5f;
        currentXOffset = 0;
    }


}
