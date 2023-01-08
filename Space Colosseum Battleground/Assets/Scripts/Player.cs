using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private HPBar _hpbar;

    public bool isDead;
    public bool isRoll;
    private int _health;
    public void SetHealth(int newHealth)
    {
        _health = newHealth;
    }

    public int GetHealth()
    {
        return _health; 
    }
    private int _maxHealth;
    public void SetMaxHealth(int newMaxHealth)
    {
        _maxHealth = newMaxHealth;
    }
    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    private bool _roll;
    public void SetRoll(bool newRoll)
    {
        _roll = newRoll;
    }
    public bool GetRoll()
    {
        return _roll;
    }

    private int _strength;
    public void SetStrength(int newStrength)
    {
        _strength = newStrength;
    }
    public int GetStrength()
    {
        return _strength;
    }
    private void Awake()
    {
/*        _maxHealth = 150;
        _strength = 10;
*/      
        _health = _maxHealth;
        _roll = false;
        isDead = false;
        isRoll = false;
    }

    public void Damage(int damage)
    {
        //Debug.Log(damage);
        if (!isRoll)
        {
            _health -= damage;
            _hpbar.SetHPBarHealth(_health);
            if (_health <= 0)
            {
                isDead = true;
            }
        }
    }

    public void Heal(int heal, CrateItem crateItem)
    {
        if (_health < _maxHealth)
        {
            _health += heal;
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }
            _hpbar.SetHPBarHealth(_health);
            crateItem.DeleteCrateItem();
        }
    }
}
