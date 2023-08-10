using EzySlice;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Obstacle 
{
    public GameObject gameObject;
    public Obstacle nextObstacle;
}

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab; // 障碍物预制体
    public Transform startZ; // 开始生成的位置
    public Transform endZ; // 结束生成的位置
    public Transform startX; // X轴开始范围
    public Transform endX; // X轴结束范围
    public Transform ClipMinX; // X轴最小剪切范围
    public Transform ClipMaxX; // X轴最大剪切范围
    public TextMeshPro _scoreText; // 分数
    private float spawnRate = 10f; // 初始生成频率
    private float difficultyIncreaseTime = 1; // 难度增加的时间间隔
    private float minXOffset = 0f; // 最的X偏移量
    private float maxXOffset = 10f; // 最大的X偏移量

    private float startSpeed = 0.5f; // 障碍物移动速度
    private float speed; // 障碍物移动速度
    private float curSpawnRate; // 生成频率
    private List<Obstacle> obstacles; // 障碍物列表
    private float nextSpawnTime = 0f; // 下次生成的时间
    private float timer = 0f; // 计时器
    private float score = 0f; // 分数
    private float currentXOffset; // 当前X偏移量
    private float maxScore = 0f;
    [HideInInspector]
    public float diffRate = 1f; // 困难指数
    [Range(1f, 5f)]
    public float startDiffRate = 1f;
    public readonly float minDiffRate = 1f;
    public readonly float maxDiffRate = 5f;
    private void Start()
    {
        obstacles = new List<Obstacle>(); // 初始化列表
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
            nextSpawnTime = Time.time + curSpawnRate;
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
            float lastX = obstacles[obstacles.Count - 1].gameObject.transform.localPosition.x;
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
            lower.tag = sub.tag;
            lower.transform.SetParent(obstacle.transform, false); // 将新物体设置为障碍物的子对象
            Destroy(sub.gameObject);
        }
        foreach (var sub in subs)
        {
            SlicedHull hull = sub.gameObject.Slice(ClipMaxX.position, Vector3.right);
            if (hull == null) continue;
            GameObject lower = hull.CreateLowerHull(sub.gameObject, sub.GetComponent<MeshRenderer>().material);
            lower.AddComponent<BoxCollider>();
            lower.tag = sub.tag;
            lower.transform.SetParent(obstacle.transform, false); // 将新物体设置为障碍物的子对象
            Destroy(sub.gameObject);
        }
        Obstacle ob = new Obstacle { nextObstacle = null, gameObject = obstacle };
        if(obstacles.Count > 0)
        {
            obstacles[obstacles.Count - 1].nextObstacle = ob;
        }
        obstacles.Add(ob);
    }

    void MoveAndCheckObstacles()
    {
        // 移动障碍物
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = obstacles[i].gameObject;
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
        score += Time.deltaTime;
        maxScore = Mathf.Max(maxScore, score);
        _scoreText.text = $"{Mathf.RoundToInt(score)}/{Mathf.RoundToInt(maxScore)}";

        if (timer > difficultyIncreaseTime)
        {
            // 难度增加
            diffRate = Mathf.Clamp(diffRate * 1.05f, minDiffRate, maxDiffRate);
            UpdateData();

            // 重置计时器
            timer = 0;
        }
    }

    public void Reset()
    {
        // 销毁所有障碍物并清空列表
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }
        obstacles.Clear();

        timer = 0;
        nextSpawnTime = 0;
        currentXOffset = minXOffset;
        score = 0;
        diffRate = startDiffRate;
        UpdateData();
    }

    void UpdateData()
    {
        speed = startSpeed * diffRate;
        curSpawnRate = spawnRate / diffRate;
        currentXOffset = MapValue(diffRate, minDiffRate, maxDiffRate, minXOffset, maxXOffset);
    }

    private float MapValue(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
    }

    /// <summary>
    /// 根据player的位置获取下一个障碍物的位置，障碍物是沿着z轴从大向小方向移动的
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    public Vector3 GetNextPosition(Vector3 playerPosition)
    {
        float farthestZ = float.PositiveInfinity; // 初始化为正无穷，确保任何障碍物的Z值都小于它
        Vector3 nextPosition = Vector3.zero; // 下一个障碍物的位置

        // 遍历障碍物列表
        foreach (Obstacle obstacle in obstacles)
        {
            Vector3 obstaclePosition = obstacle.gameObject.transform.localPosition;

            // 如果障碍物位于玩家前方并且比当前找到的最远障碍物还要近
            if (obstaclePosition.z > playerPosition.z && obstaclePosition.z < farthestZ)
            {
                farthestZ = obstaclePosition.z;
                nextPosition = obstaclePosition; // 更新下一个障碍物的位置
            }
        }

        // 在Scene视图中绘制nextPosition的位置
        //Debug.DrawRay(nextPosition, Vector3.up * 5f, Color.red, 1f);
        return nextPosition;
    }

}
