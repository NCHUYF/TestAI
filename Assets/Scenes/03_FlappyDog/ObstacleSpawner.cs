using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab; // 障碍物预制体
    public Transform startZ; // 开始生成的位置
    public Transform endZ; // 结束生成的位置
    public Transform startX; // X轴开始范围
    public Transform endX; // X轴结束范围
    public float speed = 5f; // 障碍物移动速度
    public float spawnRate = 2f; // 初始生成频率
    public float difficultyIncreaseTime = 30f; // 难度增加的时间间隔
    public float maxSpawnRate = 0.5f; // 最大生成频率
    public float maxXOffset = 5f; // 最大的X偏移量

    private List<GameObject> obstacles; // 障碍物列表
    private float nextSpawnTime = 0f; // 下次生成的时间
    private float timer = 0f; // 计时器
    private float currentXOffset; // 当前X偏移量

    private void Start()
    {
        obstacles = new List<GameObject>(); // 初始化列表
        for(int i = 0; i < 10; ++i)
        {
            DebugGUI.AddDebugTotalObject(gameObject.name + i, this, Random.ColorHSV());
        }
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

    void SpawnObstacle()
    {
        // 计算障碍物的位置和大小
        float randomX = Random.Range(startX.position.x, endX.position.x);
        Vector3 spawnPosition = new Vector3(randomX + Random.Range(-currentXOffset, currentXOffset), 0.5f, startZ.position.z);
        Vector3 spawnScale = new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5));

        // 创建障碍物并添加到列表
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacle.transform.localScale = spawnScale;
        obstacles.Add(obstacle);
    }

    void MoveAndCheckObstacles()
    {
        // 移动障碍物
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = obstacles[i];
            obstacle.transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // 如果达到endZ，销毁障碍物并从列表中移除
            if (obstacle.transform.position.z > endZ.position.z)
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
            spawnRate = Mathf.Max(spawnRate * 0.9f, maxSpawnRate);
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
        spawnRate = 2f;
        currentXOffset = 0;
    }
}
