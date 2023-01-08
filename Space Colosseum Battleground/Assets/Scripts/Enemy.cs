using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _originalHealDrop;
    private Animator _animator;
    private int _health;
    public int GetHealth()
    {
        return _health;
    }
    private int _maxHealth;
    public void SetMaxHealth(int newMaxHealth)
    {
        _maxHealth = newMaxHealth;
        _health = newMaxHealth;
    }
    public int GetMaxHealth()
    {
        return _maxHealth;
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
        _animator = GetComponent<Animator>();
    }

    public void Damage(int damage)
    {
        _health -= damage;
/*        Debug.Log(damage);
        Debug.Log("Enemy touch !");
        Debug.Log(_health);
        Debug.Log(_maxHealth);
        Debug.Log(_strength);
*/
        if (_health <= 0)
        {
            Game.Instance.UpdateScore(_maxHealth,_strength);
            int dropItemChance = Random.Range(0, 25);
            if (dropItemChance == 0)
            {
                var heal = Instantiate(_originalHealDrop, transform.position, transform.rotation);
                heal.SetActive(true);
                var healData = heal.GetComponent<CrateItem>();
                healData.heal = 15;
                heal.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            StartCoroutine(DeleteEnemy());
        }

    }

    IEnumerator DeleteEnemy()
    {
        _animator.SetBool("Dead", true);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void AutoKill()
    {
        Destroy(gameObject);
    }
}
