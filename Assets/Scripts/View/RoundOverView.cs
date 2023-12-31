using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoundOverView : MonoBehaviour
{
    private RoundModel _roundModel;

    [field: SerializeField]
    private SelectCardView _selectCardView;
    [field: SerializeField]
    private StartRoundView _startRoundView;

    [field: SerializeField]
    private Image[] _1PCounts;
    [field: SerializeField]
    private Image[] _1PMasks;

    [field: SerializeField]
    private Image[] _2PCounts;
    [field: SerializeField]
    private Image[] _2PMasks;

    private int _1PCount = 0;
    private int _2PCount = 0;

    [field: SerializeField]
    private Vector3 _pointSeq_TO = new Vector3(0.05f, 0.05f, 0.05f);
    [field: SerializeField]
    private float _pointSeq_Duration = 0.2f;
    [field: SerializeField]
    private int _pointSeq_Vibrato = 0;

    private Sequence _pointSeq;

    private Sequence _roundSeq;

    public void Init(RoundModel roundModel)
    {
        _roundModel = roundModel;

        _roundModel.OnStageChanged += RoundOverUI;
    }

    private void RoundOverUI(int stage, int winner)
    {
        Image count = null;
        Image mask = null;

        if (winner == 1)
        {
            count = _1PCounts[_1PCount];
            mask = _1PMasks[_1PCount];
            _1PCount++;
        }
        else if (winner == 2)
        {
            count = _2PCounts[_2PCount];
            mask = _2PMasks[_2PCount];
            _2PCount++;
        }

        if (winner == 1 || winner == 2)
            PointUITween(stage - 1, count, mask);

        if (_1PCount == 3 || _2PCount == 3) return;
        StartCoroutine(GotoNextStage(stage - 1));
    }

    private void PointUITween(int stage, Image count, Image mask)
    {
        _pointSeq = DOTween.Sequence().Pause().SetUpdate(true)
        .Append(count.transform.DOScale(0, 0.2f))
        .Append(mask.DOFade(1, 0))
        .Join(count.DOFade(0, 0))
        .Join(count.transform.DOScale(1, 0))
        .Join(mask.transform.DOPunchScale(_pointSeq_TO, _pointSeq_Duration, _pointSeq_Vibrato).SetEase(Ease.OutQuad));

        _pointSeq.Restart();
    }

    private IEnumerator GotoNextStage(int stage)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(stage);
        if (stage == 2 || stage == 4)
        {
            _selectCardView.gameObject.SetActive(true);
            _selectCardView.CardSelectUITween(stage switch
            {
                2 => KnowHowManager.Inst.GetRandomEpicKnowHow(),
                4 => KnowHowManager.Inst.GetRandomEpicKnowHow()
            });
        }
        else
        {
            _startRoundView.StartCountDown(stage);
        }
    }
}
