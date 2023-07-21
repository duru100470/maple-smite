using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField]
    private bool _isHeadingRight;
    private bool _isAxeCooldown = false;
    private bool _isThrowCooldown = false;
    private PlayerState _curState;

    private PlayerModel _playerModel;
    [Header("Dependencies")]
    [SerializeField]
    private KeyInputSender _keyInputSender;
    [SerializeField]
    private TreeController _treeController;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _stoneObject;



    public void Init(PlayerModel playerModel)
    {
        _playerModel = playerModel;
        _curState = PlayerState.Idle;
        _keyInputSender.OnKeyPressed += PressKey;
    }

    private void PressKey(KeyType keyType)
    {
        switch (keyType)
        {
            case KeyType.Axe:
                if (_curState != PlayerState.Idle || _isAxeCooldown) break;
                StartCoroutine(DoAxing());
                StartCoroutine(SetAxeCooldown());
                break;
            case KeyType.Throw:
                if (_curState != PlayerState.Idle || _isThrowCooldown) break;
                StartCoroutine(DoThrowing());
                StartCoroutine(SetThrowCooldown());
                break;
            case KeyType.Jump:
                if (_curState != PlayerState.Idle) break;
                StartCoroutine(DoJump());
                break;
        }
    }

    private IEnumerator DoAxing()
    {
        _treeController.GetDamage(_playerModel.AxeDamage);
        _curState = PlayerState.Act;
        yield return new WaitForSeconds(_playerModel.AxeMotionTime);
        _curState = PlayerState.Idle;
    }

    private IEnumerator SetAxeCooldown()
    {
        _isAxeCooldown = true;
        yield return new WaitForSeconds(_playerModel.AxeCooldown);
        _isAxeCooldown = false;
    }

    private IEnumerator DoThrowing()
    {
        var go = Instantiate(_stoneObject, transform.position, Quaternion.identity);
        go.GetComponent<ProjectileController>().Init(_isHeadingRight, _playerModel.ThrowStunDuration, gameObject);

        _curState = PlayerState.Act;
        yield return new WaitForSeconds(_playerModel.ThrowMotionTime);
        _curState = PlayerState.Idle;
    }

    private IEnumerator SetThrowCooldown()
    {
        _isThrowCooldown = true;
        yield return new WaitForSeconds(_playerModel.ThrowCooldown);
        _isThrowCooldown = false;
    }

    private IEnumerator DoJump()
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(transform.position.y + _playerModel.JumpHeight, _playerModel.JumpTime / 2).SetEase(Ease.OutQuad));
        seq.Append(transform.DOLocalMoveY(transform.position.y, _playerModel.JumpTime / 2).SetEase(Ease.InQuad));

        _curState = PlayerState.Act;
        yield return new WaitForSeconds(_playerModel.JumpTime);
        _curState = PlayerState.Idle;
    }

    public void GetStunned(float duration)
    {
        StopCoroutine(DoAxing());
        StopCoroutine(DoThrowing());
        StopCoroutine(DoJump());
        StartCoroutine(GetStunnedCoroutine(duration));
    }

    private IEnumerator GetStunnedCoroutine(float duration)
    {
        _curState = PlayerState.Stun;
        yield return new WaitForSeconds(duration);
        _curState = PlayerState.Idle;
    }
}

public enum PlayerState
{
    Idle,
    Act,
    Stun
}