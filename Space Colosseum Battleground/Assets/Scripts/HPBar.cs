using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Slider _slider;
    private int _health;
    private int _maxHealth;
    private TextMeshProUGUI _hpPercent;

    [SerializeField] private Player _player;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _hpPercent = GetComponentInChildren<TextMeshProUGUI>();
        _slider = GetComponent<Slider>();
        _maxHealth = _health = _player.GetMaxHealth();
        _slider.maxValue = 100;
        _slider.value = 100;
    }

    public void SetHPBarHealth(int hp)
    {
        _health = hp;
        _maxHealth = _player.GetMaxHealth();
        float percent = GetPercent();
        _slider.value = percent;
        _hpPercent.text = _health.ToString() + " / " + _maxHealth.ToString() + " (" + percent.ToString() + "%)";
    }

    private float GetPercent()
    {
        float percent = ((float)_health / (float)_maxHealth) * 100;
        return (float)Math.Ceiling(percent);
    }

}
