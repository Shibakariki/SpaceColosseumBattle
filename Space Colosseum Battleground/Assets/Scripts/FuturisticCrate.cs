using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuturisticCrate : MonoBehaviour, IDamageable
{
    private int health;

    [SerializeField] private GameObject _explosion;
    [SerializeField] GameObject _originalCrateItem;
    private void Awake()
    {
        health = 25;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Explode();
        }

    }

    private void Explode()
    {
        var newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3);

        var newCrateItem = Instantiate(_originalCrateItem, newPos, transform.rotation);
        var healData = newCrateItem.GetComponent<CrateItem>();
        healData.heal = 30;

        var explode = Instantiate(_explosion, newPos, transform.rotation);
        explode.SetActive(true);

        StartCoroutine(DeleteObject(explode, newCrateItem));
    }


    IEnumerator DeleteObject(GameObject explode, GameObject newCrateItem)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(explode.gameObject);
        newCrateItem.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

}
