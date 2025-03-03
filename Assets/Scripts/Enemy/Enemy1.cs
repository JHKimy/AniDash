using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : MonoBehaviour
{
    public Transform _target;
    Rigidbody _rigidbody;
    BoxCollider _boxCollider;
    Material _material;
    NavMeshAgent _navMeshAgent;

    public int maxHealth;
    public int curHealth;

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _material = GetComponent<Material>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _navMeshAgent.SetDestination(_target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Box")
        {
            curHealth -= 5;
        }
    }
    void Start()
    {
        
    }


}
