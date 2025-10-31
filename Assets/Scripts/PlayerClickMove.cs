using UnityEngine;
using UnityEngine.AI;

public class PlayerClickMove : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    [SerializeField] private Camera sceneCamera;

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(sceneCamera.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                m_Agent.isStopped = false;
                m_Agent.SetDestination(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Agent.isStopped = true;
        }
        
        
    }
}
