using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using TMPro;

public class FlappyDogAgent : Agent
{

    /// <summary>
    /// 进入新一轮调用
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Reset();

    }

    /// <summary>
    /// 收集观察结果
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(_target.transform.localPosition.x);
        sensor.AddObservation(_target.transform.localPosition.z);
        sensor.AddObservation(_timer);
        sensor.AddObservation(_spawner.diffRate);
        sensor.AddObservation(nextPos.x);
        sensor.AddObservation(nextPos.z);
        sensor.AddObservation(disToNext);
    }

    /// <summary>
    /// 接受动作给予奖励
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 dir = Vector3.zero;
        dir.x = vectorAction[0] - 1;
        dir.z = vectorAction[1] - 1;

        _rigidbody.velocity = dir * _moveSpeed * _spawner.diffRate;

        var disToTarget = Vector3.Distance(transform.localPosition, _target.transform.localPosition);
        // 吃掉目标
        if (disToTarget < 1)
        {
            AddReward(1f);
            ResetTarget();
        }

        nextPos = _spawner.GetNextPosition(transform.localPosition);
        disToNext = Vector3.Distance(nextPos, transform.localPosition);

        // 鼓励找目标、前往下一个位置点
        // 时间奖励
        AddReward(0.001f * (_timer - disToNext - disToTarget));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 出界
        if (collision.gameObject.name.Equals("Wall"))
        {
            AddReward(-1f);
            _plane.GetComponent<Renderer>().material = _matLose;
            Invoke("ChangeMaterial", 2f);
            ResetPosition();
            EndEpisode();
        }

        // 碰到障碍物
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void ChangeMaterial()
    {
        _plane.GetComponent<Renderer>().material = _matWin;
    }

    /// <summary>
    /// 启发训练ai
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        // 拿到移动方向
        actionsOut[0] = Input.GetAxis("Horizontal") + 1;
        actionsOut[1] = Input.GetAxis("Vertical") + 1;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        _timerText.text = Mathf.RoundToInt(_timer).ToString();
        if(_timer <= 0)
        {
            Reset();
        }
    }

    private void Reset()
    {
        transform.eulerAngles = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        ResetPosition();
        ResetTarget();
        _spawner.Reset();
    }

    void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    void ResetTarget()
    {
        _target.transform.localPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
        _timer = 20f;
    }

    public Material _matWin;
    public Material _matLose;
    public Transform _plane;
    public ObstacleSpawner _spawner;
    public Transform _target; // 目标
    public TextMeshPro _timerText;
    private float _timer = 20;
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _moveSpeed = 3;
    private Vector3 nextPos;
    private float disToNext;
}
