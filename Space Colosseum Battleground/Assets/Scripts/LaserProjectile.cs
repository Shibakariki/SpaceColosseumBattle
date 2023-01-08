using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    private Rigidbody _laserBody;

    [SerializeField] private GameObject _explosion;
    [SerializeField] private Player _player;

    private void Awake()
    {
        _laserBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float laserSpeed = 20f;
        _laserBody.velocity = transform.forward * laserSpeed;
        StartCoroutine(DeleteLaser());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "PlayerArmature")
        {
            var explode = Instantiate(_explosion, transform.position, transform.rotation);
            explode.SetActive(true);
            //Debug.Log("Collider : "+other.name);

            StartCoroutine(DeleteLaserWithExplode(explode));
            if (other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.Damage(_player.GetStrength() + Random.Range(0, 3));
            }
        }
    }

    IEnumerator DeleteLaserWithExplode(GameObject explode)
    {
        var bxc = gameObject.GetComponent<BoxCollider>();
        bxc.enabled = false;
        yield return new WaitForSeconds(0.3f);
        Destroy(explode.gameObject);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
    IEnumerator DeleteLaser()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
