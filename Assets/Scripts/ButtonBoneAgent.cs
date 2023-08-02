using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class ButtonBoneAgent : Agent
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
        sensor.AddObservation(_isTrigger);
        sensor.AddObservation(transform.localPosition);
        if (_target.gameObject.activeSelf)
            sensor.AddObservation(_target.transform.localPosition);
        else
            sensor.AddObservation(Vector3.zero);
        sensor.AddObservation(_button.transform.localPosition);
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

        float distanceToButton = Vector3.Distance(transform.localPosition, _button.localPosition);
        float distanceToTarget = Vector3.Distance(transform.localPosition, _target.transform.localPosition);

        // 接近按钮的奖励
        if (!_isTrigger)
        {
            AddReward(-distanceToButton / MaxStep);
        }

        // 按下按钮
        if (distanceToButton < 1f && Mathf.RoundToInt(vectorAction[2]) == 1)
        {
            AddReward(1f);
            _isTrigger = true;
            _button.GetComponentInChildren<Renderer>().material = _matWin;
            _target.gameObject.SetActive(true);
        }

        // 接近骨头的奖励
        if (_isTrigger)
        {
            AddReward(-distanceToTarget / MaxStep);
        }

        // 时间惩罚
        AddReward(-0.1f / MaxStep);
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

    private void OnTriggerEnter(Collider other)
    {
        // 碰到目标
        if(other.transform == _target)
        {
            AddReward(1f);
            _plane.GetComponent<Renderer>().material = _matWin;
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

    private void Reset()
    {
        ResetTarget();

        _isTrigger = false;
        _button.GetComponentInChildren<Renderer>().material = _matLose;
        _target.gameObject.SetActive(false);
    }

    void ResetTarget()
    {
        _button.transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        _target.transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }

    void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
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
