using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private PlayerController _playerCntrl;
    [SerializeField] private const float _smoothSpeed = 0.5f;
    [SerializeField] private const float _turnSpeed = 720.0f;
    [SerializeField] private Vector3 _delta;
	[SerializeField] private Animator _anim;
	[SerializeField] private const float _hitTimer = 1.55f;
	[SerializeField] private float _lastHit = 0.0f;
	[SerializeField] private int _hitCount = 0;
	[SerializeField] private const int _deathCount = 3;

	[SerializeField] private bool _dead = false;
	[SerializeField] private bool _hurt = false;
	[SerializeField] private bool _aware = true;
	[SerializeField] private bool _attackClose = false;
	[SerializeField] private bool _detected = false;

	void Start()
	{
		_anim = GetComponent<Animator>();
	}

	private void OnTriggerEnter (Collider col)
	{
		if (!_dead && col.gameObject.tag == "player_weapon" )
		{
			_anim.Play("GetHit");
			_lastHit = 0.0f;
			_hurt = true;
			_hitCount++;
		}
	}

	void Update()
	{
		if(!_dead)
		{
			if(_hitCount >= _deathCount)
			{
				gameObject.tag = "Untagged";
				_dead = true;
				_anim.Play("Die");
				_playerCntrl.xp += 15.0f;	
			}

			if(_hurt)
			{
				_lastHit += Time.deltaTime;
				if(_lastHit >= _hitTimer)
				{
					_lastHit = 0.0f;
					_hurt = false;
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if(!_dead)
		{
			_delta = _target.position - transform.position;

			float distance = Vector3.Magnitude(_delta);
			_anim.SetFloat("Distance", distance);

			if(distance <= 10.0f)
			{
				_detected = true;
			}
			else
			{
				_detected = false;
			}
			_anim.SetBool("Detected", _detected);

			if( distance <= 1.0f)
			{
				_attackClose = true;
			}
			else
			{
				_attackClose = false;
			}

			if(!_hurt && _aware && !_attackClose && _detected)
			{
				var rot = Quaternion.LookRotation(_delta, Vector3.up);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
				transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

				transform.position += _delta * _smoothSpeed * Time.deltaTime;
			}
		}
	}
}
