using System;
using UnityEngine;

[Serializable]
public class PlayerAnimationData 
{
    [SerializeField] private string _ghostParameterName ="@Ghost";
    [SerializeField] private string _moveSpeedParameterName = "MoveSpeed";
    [SerializeField] private string _lastMoveXParameterName = "LastMoveX";
    [SerializeField] private string _lastMoveYParameterName = "LastMoveY";
    [SerializeField] private string _moveXParameterName = "MoveX";
    [SerializeField] private string _moveYParameterName = "MoveY";
    [SerializeField] private string _Die = "Die";

    public int GhostParameterHash { get; private set; }
    public int MoveSpeedParameterHash { get; private set; }
    public int LastMoveXParameterHash { get; private set; }
    public int LastMoveYParameterHash { get; private set; }
    public int MoveXParameterHash { get; private set; }
    public int MoveYParameterHash { get; private set; }
    public int Die { get; private set; }
    //
    /// <summary>
    /// 모든 애니메이션 해쉬값 Init 여러번 setfloat 같은 메서드로 불러오면 많은 가비지 생성하므로 만든 메서드
    /// </summary>
    public void Initailize()
    {
        GhostParameterHash = Animator.StringToHash(_ghostParameterName);

        MoveSpeedParameterHash = Animator.StringToHash(_moveSpeedParameterName);

        LastMoveXParameterHash = Animator.StringToHash(_lastMoveXParameterName);

        LastMoveYParameterHash = Animator.StringToHash(_lastMoveYParameterName);

        MoveXParameterHash = Animator.StringToHash(_moveXParameterName);

        MoveYParameterHash = Animator.StringToHash(_moveYParameterName);

        Die = Animator.StringToHash(_Die);
    }
}
