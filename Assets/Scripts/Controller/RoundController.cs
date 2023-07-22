using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    private RoundModel _roundModel;
    private TreeModel _treeModel;
    [Header("Dependencies")]
    [SerializeField]
    private TreeController _treeController;
    [SerializeField]
    private PlayerController _playerController1;
    [SerializeField]
    private PlayerController _playerController2;

    public void Init(RoundModel roundModel, TreeModel treeModel)
    {
        _roundModel = roundModel;
        _treeModel = treeModel;
        _treeModel.OnTreeDestroyed += OnStageOver;

        StartNewRound();
    }

    // TODO: RoundView에서 증강 선택이 완료되거나, 시간이 만료되면 StartNewRound 호출
    public void StartNewRound()
    {
        var roundIdx = _roundModel.StageIndex;

        _treeModel.Reset(_roundModel.TreeHealthByStage[roundIdx]);
        _playerController1.Reset();
        _playerController2.Reset();
    }

    private void OnStageOver(int attackerId)
    {
        _roundModel.WinnerList.Add(attackerId switch
        {
            0 => 0,
            1 => 2,
            2 => 1,
            _ => throw new Exception()
        });
        _roundModel.StageIndex++;
    }
}
