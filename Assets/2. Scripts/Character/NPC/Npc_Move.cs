using System.Collections;
using UnityEngine;

public enum PatrolDirection
{
    Idle,
    Left,
    Right,
    Up,
    Down
}


public class Npc_Move : MonoBehaviour
{
    public float MoveSpeed = 1.0f;
    public float PatrolTime = 5.0f;
    public Animator Animator;

    public PatrolDirection PatrolDirection;

    private Rigidbody2D _rigidbody;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        _rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();

        StartCoroutine(Move_Npc_Patrol());
        if (PatrolDirection == PatrolDirection.Left || PatrolDirection == PatrolDirection.Right)
        {
            Animator.SetBool("RightandLeft", true);
        }
        else if(PatrolDirection != PatrolDirection.Idle)
        {
            Animator.SetBool("UpandDown", true);
        }
    }

    private IEnumerator Move_Npc_Patrol()
    {
        while (true)
        {
            float _changeTime = 0f;

            Animator.SetBool($"{PatrolDirection}", true);

            while (_changeTime < PatrolTime)
            {
                _rigidbody.velocity = PatrolDirection switch
                {
                    PatrolDirection.Left => new Vector2(-MoveSpeed, 0),
                    PatrolDirection.Right => new Vector2(MoveSpeed, 0),
                    PatrolDirection.Up => new Vector2(0, MoveSpeed),
                    PatrolDirection.Down => new Vector2(0, -MoveSpeed),
                    _ => Vector2.up,
                };

                _changeTime += Time.deltaTime;

                yield return null;
            }

            _rigidbody.velocity = Vector2.zero;
            Animator.SetBool($"{PatrolDirection}", false);

            PatrolDirection = Get_Opposit_Direction(PatrolDirection);

            yield return new WaitForSeconds(1f);
        }
    }

    private PatrolDirection Get_Opposit_Direction(PatrolDirection direction)
    {
        return direction switch
        {
            PatrolDirection.Left => PatrolDirection.Right,
            PatrolDirection.Right => PatrolDirection.Left,
            PatrolDirection.Up => PatrolDirection.Down,
            PatrolDirection.Down => PatrolDirection.Up,
            _ => PatrolDirection.Left,
        };
    }
}
