using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private GameObject _target;

    private Animator _animator;
    private NavMeshAgent _agent;
    private Enemy _enemy;
    private float _lookRadius = 45f;
    private float _attackRadius = 0.8f;
    private float _lastHit = 0;
    private float _hitCooldown = 2.4f;
    private Vector3 _previousPosition;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        _previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(_target.transform.position, transform.position);
        Vector3 curMove = transform.position - _previousPosition;
        float curSpeed = curMove.magnitude / Time.deltaTime;

        _animator.SetFloat("Speed", curSpeed);

        if (distance <= _attackRadius)
        {
            _agent.SetDestination(transform.position);
            if (Time.time - _lastHit >= _hitCooldown)
            {
                _animator.SetBool("Target", true);
                StartCoroutine(Attack());
                _lastHit = Time.time;
            }
        }
        else if (distance <= _lookRadius)
        {
            _agent.SetDestination(_target.transform.position);
        }


        _previousPosition = transform.position;

        FaceTarget();
    }

    void FaceTarget()
    {
        Vector3 direction = (_target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.2f);
        float distance = Vector3.Distance(_target.transform.position, transform.position);

        if (distance <= (_attackRadius + (_attackRadius/2)))
        {
            var player = _target.GetComponent<Player>();
            player.Damage(_enemy.GetStrength());
        }
        _animator.SetBool("Target", false);
    }

}
