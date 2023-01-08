using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _aimCamera;
    [SerializeField] private Image _aimCrossHair;

    [SerializeField] private float _normalCameraSensibility = 1f;
    [SerializeField] private float _aimCameraSensibility = 0.5f;
/*    [SerializeField] private float _speedMultiplicator = 1f;
    [SerializeField] private float _jumpMultiplicator = 1f;
*/    [SerializeField] private LayerMask _aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform _debugTransform;
    [SerializeField] private Transform _originalLaserTransform;
    [SerializeField] private Transform _shootPointTransform;

    [SerializeField] private GameObject _powerFlower;
    private Animator _powerFlowerAnimator;
    private Animator _thirdPersonAnimator;

    private StarterAssetsInputs _starterAssetsInputs;
    private ThirdPersonController _thirdPersonController;
    private Player _player;
    private float _lastShootTime;
    private float _lastRollTime;
    private float _lastInputTime;
    private float _shootCoolDown = 0.5f;
    private float _rollCoolDown = 1f;
    private float _inputCoolDown = 0.5f;
    private Vector3 latestMouseWorldPosition = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _thirdPersonAnimator = GetComponent<Animator>();
        _powerFlowerAnimator = _powerFlower.GetComponent<Animator>();
        _player = GetComponent<Player>();
        _lastShootTime = 0f;
        _lastRollTime = 0f;
        _lastInputTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimColliderLayerMask))
        {
            if (!raycastHit.collider.name.Contains("LaserBullet"))
            {
                _debugTransform.position = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
                latestMouseWorldPosition = mouseWorldPosition;
            }
            else
            {
                mouseWorldPosition = latestMouseWorldPosition;
            }

        }
        else
        {
            mouseWorldPosition = latestMouseWorldPosition;
        }

        if (!_player.isDead && _starterAssetsInputs.roll && _thirdPersonController.Grounded && _player.GetRoll() && Time.time - _lastRollTime >= _rollCoolDown)
        {
            _thirdPersonAnimator.SetBool("Roll",true);
            _player.isRoll = true;
            _lastRollTime = Time.time;
            StartCoroutine(ResetRollAnim());
        }
        if (!_player.isDead && _starterAssetsInputs.aim)
        {
            _aimCamera.gameObject.SetActive(true);
            _aimCrossHair.gameObject.SetActive(true);
            _thirdPersonController.SetCameraSensitivity(_aimCameraSensibility);
            _thirdPersonController.SetRotateOnMove(false);
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward,aimDirection,Time.deltaTime * 20f);
            _powerFlower.transform.forward = Vector3.Lerp(transform.forward,aimDirection,Time.deltaTime * 20f);
        }
        else
        {
            _aimCamera.gameObject.SetActive(false);
            _aimCrossHair.gameObject.SetActive(false);
            _thirdPersonController.SetCameraSensitivity(_normalCameraSensibility);
            _thirdPersonController.SetRotateOnMove(true);
        }

        if (!_player.isDead && _starterAssetsInputs.shoot && _starterAssetsInputs.aim && Time.time - _lastShootTime >= _shootCoolDown)
        {
            _powerFlowerAnimator.SetLayerWeight(1, 0);
            _powerFlowerAnimator.SetTrigger("Shoot");
            Vector3 aimDir;
            aimDir = (mouseWorldPosition - _shootPointTransform.position).normalized;
            Transform laser = Instantiate(_originalLaserTransform, _shootPointTransform.position, Quaternion.LookRotation(aimDir, Vector3.up));
            laser.gameObject.SetActive(true);
            _starterAssetsInputs.shoot = false;
            _lastShootTime = Time.time;
            StartCoroutine(LaunchShootAnim());
        }

        if (_player.isDead && _starterAssetsInputs.restart && Time.time - _lastInputTime >= _inputCoolDown)
        {
            _player.isDead = false;
            _lastInputTime = Time.time;
            Game.Instance.Setup();
        }

        if (_player.isDead && _starterAssetsInputs.quit && Time.time - _lastInputTime >= _inputCoolDown)
        {
            Debug.Log("QUIT!");
            _lastInputTime = Time.time;
            Application.Quit();
        }

        /*        _thirdPersonController.SetSpeedModificator(_speedMultiplicator);
                _thirdPersonController.SetJumpModificator(_jumpMultiplicator);
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CrateItem>(out CrateItem crateItem))
        {
            Debug.Log(crateItem.heal);
            _player.Heal(crateItem.heal, crateItem);
        }
    }

    IEnumerator LaunchShootAnim()
    {
        yield return new WaitForSeconds(0.4f);
        _powerFlowerAnimator.SetLayerWeight(1, 1);
        _powerFlowerAnimator.ResetTrigger("Shoot");
    }

    IEnumerator ResetRollAnim()
    {
        yield return new WaitForSeconds(1f);
        _thirdPersonAnimator.SetBool("Roll", false);
        _player.isRoll = false;
    }



}
