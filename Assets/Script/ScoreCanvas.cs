using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI comboNum;
    [SerializeField]
    private TextMeshProUGUI highestComboNum;
    [SerializeField]
    private TextMeshProUGUI hitNum;
    [SerializeField]
    private TextMeshProUGUI missNum;

    private MusicManager manager;

    private void Start()
    {
        manager = MusicManager.instance;
    }

    void Update()
    {
        comboNum.text = manager.combo.ToString();
        highestComboNum.text = manager.highestCombo.ToString();
        hitNum.text = manager.hitCount.ToString();
        missNum.text = manager.missCount.ToString();
    }
}
