using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipRandomaser : MonoBehaviour
{
    public GameObject[] TipsCommon, TipsRare, TipsLegend;

    private enum Rareness { Common = 1, Rare, Legend }

    private Rareness _rareness;
    private GameObject _generatedTip, _shownTip;

    private void OnEnable()
    {
        _rareness = RandomRareness();
        switch (_rareness)
        {
            case Rareness.Common:
                _generatedTip = TipsCommon[Random.Range(0, TipsCommon.Length)];
                break;
            case Rareness.Rare:
                _generatedTip = TipsRare[Random.Range(0, TipsRare.Length)];
                break;
            case Rareness.Legend:
                _generatedTip = TipsLegend[Random.Range(0, TipsLegend.Length)];
                break;
            default:
                break;
        }
        
        _shownTip = Instantiate(_generatedTip, transform);
    }

    private void OnDisable()
    {
        Destroy(_shownTip);
    }

    private Rareness RandomRareness()
    {
        float r = Random.value;

        if (r <= 0.5f)
            return Rareness.Common;
        else if (r > 0.5f && r <= 0.85f)
            return Rareness.Rare;
        else if (r > 0.85f)
            return Rareness.Legend;

        return Rareness.Common;
    }
}
