using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private Rigidbody _rb;
	[SerializeField] private const float _speed = 5.5f;
	[SerializeField] private const float _turnSpeed = 1480.0f;
	[SerializeField] private Animator _anim;
	[SerializeField] private Vector3 _destination;
	[SerializeField] private GameObject _weapon;
	[SerializeField] private Collider _weaponCollider;

	[SerializeField] private bool _attacking = false;
	[SerializeField] private float _lastAttack = 0.0f;
	[SerializeField] private const float _attackTime = 0.33f;

	[SerializeField] private Slider _healthBar;
	[SerializeField] private Slider _xpBar;
	[SerializeField] private float _health = 100.0f;
	[SerializeField] private const float _maxHealth = 100.0f;
	[SerializeField] private const float _regenRate = 2.5f;

	[SerializeField] public float xp = 0.0f;

	[SerializeField] private bool _hurt = false;
	[SerializeField] private bool _dizzy = false;
	[SerializeField] private bool _dead = false;

	[SerializeField] private int _hurtCount = 0;
	[SerializeField] private const int _hurtThresh = 3;

	[SerializeField] private float _lastHurt = 0.0f;
	[SerializeField] private const float _hurtTime = 1.4f;
	[SerializeField] private const float _dizzyTime = 2.8f;


	void Start()
	{
		_anim = GetComponent<Animator>();
		_weapon = GameObject.Find("Weapon");
		_weaponCollider = _weapon.GetComponent<Collider>();
		_weaponCollider.enabled = false;
	}

	void Update()
	{
		if(!_dead)
		{
			_health = Mathf.Min(_regenRate * Time.deltaTime + _health, 100.0f);

			if(!_hurt)
				_destination = Vector3.Normalize(_target.position - transform.position);

			if(Vector3.Magnitude(_target.position - transform.position) <= 1.0f && !_attacking)
			{
				_anim.Play("Attack");
				_attacking = true;
			}

			if(_attacking)
			{
				_lastAttack += Time.deltaTime;
				if(_lastAttack >= 0.2f)
				{
					_weaponCollider.enabled = true;
				}
			}

			if(_lastAttack >= _attackTime)
			{
				_attacking = false;
				_lastAttack = 0.0f;
				_weaponCollider.enabled = false;
			}

			if(_hurt)
			{
				_lastHurt += Time.deltaTime;
				if(_lastHurt >= (_dizzy ? _dizzyTime : _hurtTime))
				{
					_hurt = false;
					_dizzy = false;
					_lastHurt = 0.0f;
				}
			}

			Look();
			_anim.SetFloat("Speed", _destination.magnitude);
		}
	}

	void FixedUpdate()
	{
		if(!_hurt && !_dizzy && !_dead)
			Move();
	}

	void Look()
	{
		if (_destination != Vector3.zero && !_attacking)
		{
			var relative = (transform.position + _destination.ToIso()) - transform.position;
			var rot = Quaternion.LookRotation(relative, Vector3.up);

			transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
		}
	}

	void Move()
	{
		_rb.MovePosition(transform.position + transform.forward * Mathf.Lerp(0, 1, Mathf.Pow(_destination.magnitude, 4)) * _speed * Time.deltaTime);
	}

	void OnCollisionEnter(Collision col)
	{
		if(!_dead)
		{
			if(col.gameObject.tag == "NULL")
			{
				_hurtCount++;
				_hurt = true;

				if(_health <= 0.0f)
				{
					_dead = true;
					_anim.Play("Die");
				}
				else if(_hurtCount >= 3)
				{
					_hurtCount = 0;
					_dizzy = true;
					_anim.Play("Dizzy");
				}
				else
					_anim.Play("GetHit");

				_health -= 20.0f;
			}
		}
	}
}
