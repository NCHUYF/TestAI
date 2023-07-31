using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Dog : Agent
{

    /// <summary>
    /// 进入新一轮调用
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Reset();
    }
    int nOnEpisodeBeginTimes = 0;

    /// <summary>
    /// 收集观察结果
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(_target.transform.position);
        sensor.AddObservation(_rigidbody.velocity.x);
        sensor.AddObservation(_rigidbody.velocity.z);
    }

    /// <summary>
    /// 接受动作给予奖励
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 dir = Vector3.zero;
        dir.x = vectorAction[0];
        dir.z = vectorAction[1];
        _rigidbody.AddForce(dir * _moveSpeed);

        // 碰到目标
        if (Vector3.Distance(transform.position, _target.transform.position) < 1f)
        {
            SetReward(1f);
            EndEpisode();
        }

        // 出界
        if (transform.position.y < -0.5f)
        {
            ResetPosition();
            EndEpisode();
        }

    }

    /// <summary>
    /// 是否手动操作ai
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        // 拿到移动方向
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _timer -= Time.fixedDeltaTime;
        //DebugGUI.AddDebugItem("注意",$"距离下一轮剩余{_timer.ToString("f1")}秒");
    }

    private void Reset()
    {
        _timer = MaxStep / 60f;
        ResetTarget();
    }

    void ResetTarget()
    {
        _target.transform.position = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }

    void ResetPosition()
    {
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public Transform _target; // 目标

    private Rigidbody _rigidbody;
    private float _moveSpeed = 10;
    private float _timer = 0;
}
