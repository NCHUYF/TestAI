using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

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

        _rigidbody.velocity = dir * _moveSpeed;

        // 时间奖励
        AddReward(0.02f);
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

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.1f);
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

    private void Reset()
    {
        transform.eulerAngles = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        ResetPosition();
        _spawner.Reset();
    }

    void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    public Material _matWin;
    public Material _matLose;
    public Transform _plane;
    public ObstacleSpawner _spawner;
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _moveSpeed = 3;
}
