using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    #region Members

    [Header("Slot Numbers")] [SerializeField]
    private TextMeshProUGUI[] slotNumberTexts;

    [Header("Cash Texts")] [SerializeField]
    private TextMeshProUGUI cashAmountText;

    [SerializeField] private TextMeshProUGUI amountLost;
    [SerializeField] private TextMeshProUGUI amountGained;

    [Header("Particles")] [SerializeField] private GameObject bonusParticles;

    private Random[] _randoms = new[] {new Random(), new Random(), new Random()};
    private bool _isSlotPlaying = true;
    private int _wagerAmount;
    private int _totalNumbers;

    #endregion

    #region Monobehaviours

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (_isSlotPlaying)
        {
            for (var i = 0; i < 3; i++)
            {
                slotNumberTexts[i].text = _randoms[i].Next(0, 9).ToString();
            }
        }
    }

    #endregion

    #region Methods

    private void Initialize()
    {
        SetWager(100);
        amountLost.gameObject.SetActive(false);
        amountGained.gameObject.SetActive(false);
        bonusParticles.SetActive(false);
    }

    private void CheckWinningNumbers(int updatedCashValue)
    {
        if (_totalNumbers == 21)
        {
            var addedValue = _totalNumbers * _wagerAmount;
            StartCoroutine(DisableProfitsOrGains(amountGained, addedValue));
            updatedCashValue += addedValue;
            cashAmountText.text = updatedCashValue.ToString();
        }
        else if (_totalNumbers == 7)
        {
            var addedValue = _totalNumbers * 10 * _wagerAmount;
            StartCoroutine(DisableProfitsOrGains(amountGained, addedValue));
            updatedCashValue += addedValue;
            cashAmountText.text = updatedCashValue.ToString();
        }
        else if (_totalNumbers == 11)
        {
            var addedValue = _totalNumbers * _wagerAmount;
            StartCoroutine(DisableProfitsOrGains(amountGained, addedValue));
            updatedCashValue += addedValue;
            cashAmountText.text = updatedCashValue.ToString();
        }
        else if (_totalNumbers == 1)
        {
            var addedValue = _totalNumbers * 100 * _wagerAmount;
            StartCoroutine(DisableProfitsOrGains(amountGained, addedValue));
            updatedCashValue += addedValue;
            cashAmountText.text = updatedCashValue.ToString();
        }
    }

    #endregion

    #region Coroutines

    IEnumerator DisableProfitsOrGains(TextMeshProUGUI textMeshPro, int gainOrLoss)
    {
        textMeshPro.gameObject.SetActive(true);

        textMeshPro.text = $"{gainOrLoss}";

        yield return new WaitForSeconds(.5f);

        textMeshPro.gameObject.SetActive(false);
    }

    IEnumerator RestartSlots()
    {
        yield return new WaitForSeconds(2f);
        _isSlotPlaying = true;
    }

    IEnumerator DisableBonusEffect(GameObject bonusVFX)
    {
        bonusVFX.SetActive(true);
        yield return new WaitForSeconds(2f);
        bonusVFX.SetActive(false);
    }

    #endregion

    #region Public Methods

    public void SetWager(int wagerAmount)
    {
        _wagerAmount = wagerAmount;
    }

    public void StopSlots()
    {
        var updatedCashValue = Convert.ToInt32(cashAmountText.text);
        var currentAmountCheck = updatedCashValue - _wagerAmount;

        if (_isSlotPlaying && currentAmountCheck >= 0)
        {
            _isSlotPlaying = false;
            _totalNumbers = 0;

            StartCoroutine(DisableProfitsOrGains(amountLost, -_wagerAmount));

            updatedCashValue -= _wagerAmount;
            cashAmountText.text = updatedCashValue.ToString();

            var allSlotNumbers = new List<int>();
            foreach (var slotNumbers in slotNumberTexts)
            {
                _totalNumbers += Convert.ToInt32(slotNumbers.text);
                allSlotNumbers.Add(Convert.ToInt32(slotNumbers.text));
            }

            //if slots are all same numbers
            if (allSlotNumbers[0] == allSlotNumbers[1] && allSlotNumbers[1] == allSlotNumbers[2])
            {
                var bonus = _totalNumbers * 100 * _wagerAmount;
                
                StartCoroutine(DisableProfitsOrGains(amountGained, bonus));
                StartCoroutine(DisableBonusEffect(bonusParticles));
                
                updatedCashValue += bonus;
                cashAmountText.text = updatedCashValue.ToString();
            }

            CheckWinningNumbers(updatedCashValue);

            StartCoroutine(RestartSlots());
        }
    }

    #endregion
}