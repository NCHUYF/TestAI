using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class ButtonBoneAgent : Agent
{

    /// <summary>
    /// 进入新一轮调用
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        ResetButton();

        transform.eulerAngles = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// 收集观察结果
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        var targetDir = (_target.transform.localPosition - transform.localPosition).normalized;
        var canPress = CanPress();

        sensor.AddObservation(_isTrigger);
        sensor.AddObservation(canPress);
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(targetDir.x);
        sensor.AddObservation(targetDir.z);
        sensor.AddObservation(_button.transform.localPosition.x);
        sensor.AddObservation(_button.transform.localPosition.z);
    }

    private bool CanPress()
    {
        return Vector3.Distance(transform.localPosition, _button.localPosition) < 1f && !_isTrigger;
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

        // 按下按钮
        if (CanPress() && Mathf.RoundToInt(vectorAction[2]) == 1)
        {
            AddReward(1f);
            _isTrigger = true;
            _button.GetComponentInChildren<Renderer>().material = _matWin;
            _target.gameObject.SetActive(true);
        }

        // 吃到目标
        var disToTarget = Vector3.Distance(transform.localPosition, _target.transform.localPosition);
        if (_isTrigger && disToTarget < 1f)
        {
            AddReward(1f);
            _plane.GetComponent<Renderer>().material = _matWin;
            ResetButton();
        }

        // 鼓励去找目标
        if(_isTrigger)
        {
            AddReward(-0.001f * disToTarget);
        }

        // 时间惩罚
        AddReward(-0.01f / MaxStep);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // 出界
        if (collision.gameObject.name.Equals("Wall"))
        {
            AddReward(-1f);
            _plane.GetComponent<Renderer>().material = _matLose;
            ResetPosition();
            EndEpisode();
        }
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

        actionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void ResetButton()
    {
        _button.transform.localPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
        _target.transform.localPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
        _isTrigger = false;
        _target.gameObject.SetActive(false);
        _button.GetComponentInChildren<Renderer>().material = _matLose;
    }

    void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    public Transform _target; // 目标
    public Transform _button; // 按钮
    public bool _isTrigger; // 是否触发了按钮
    public Material _matWin;
    public Material _matLose;
    public Transform _plane;

    private Rigidbody _rigidbody;
    [SerializeField]
    private float _moveSpeed = 3;
}
