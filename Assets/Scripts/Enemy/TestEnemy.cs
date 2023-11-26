using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private void OnDrawGizmos()
    {
        if (MapGenerator.Instance == null) return;
        List<Vector2Int> path = MapGenerator.Instance.FindPath(transform.position, target.transform.position);

        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y));
            }
        }
    }
    /*
    private BehaviourTree bt;
    [SerializeField] private Rigidbody2D rigidbody;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        bt = new BehaviourTree(SetBT());
    }

    private INode SetBT()
    {
        return new SelectorNode(
            new List<INode>()
            {
                new SelectorNode(new List<INode>()
                {
                    CheckMove(),
                    Move(),
                }
                ),
                new SelectorNode(new List<INode>()
                {
                    Detect(),
                    Attack(),
                }
                ),
            }
            );
    }

    private INode CheckMove()
    {
        path.canMove = true;

    }

    private INode Detect()
    {

    }
    private INode Attack()
    {
    }

    private INode Move()
    {
    }*/
}
