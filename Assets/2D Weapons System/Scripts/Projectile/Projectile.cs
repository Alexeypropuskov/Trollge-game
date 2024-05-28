using UnityEngine;


namespace WeaponSystem {



[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{

    public float speed;
    public bool UnlimitedLifeTime;
    public float lifetime;
    public bool DestroyOnDistenceTravled;
    public float MaxDistence;
    public float GravityModifier;
    public GameObject OnDestroyEffect;
    public Transform Target;
    public float FollowStrength;
    public enum ProjectileType { Ballistic, Direct, Follow }
    public ProjectileType type;
    private Rigidbody2D _rigidbody2D;
    private Vector3 startpos;
    public int Damege, DamegeToStructure;


    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (type == ProjectileType.Ballistic)
        {

            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

            _rigidbody2D.gravityScale = GravityModifier;
        }
        else if (type == ProjectileType.Direct)
        {

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        }
        else if (type == ProjectileType.Follow)
        {

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        }
        _rigidbody2D.velocity = transform.right * speed;

        if (!UnlimitedLifeTime)
        {
            Invoke("DestroyBullet", lifetime);
        }

        if (DestroyOnDistenceTravled)
        {
            startpos = transform.position;
        }

    }
    void Update()
    {

        if (type == ProjectileType.Follow && Target != null)
            _rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity, (Target.transform.position - transform.position).normalized * speed, Time.deltaTime * FollowStrength);


        Vector3 Direction = _rigidbody2D.velocity;
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);



        if (DestroyOnDistenceTravled)

        {
            if (Vector3.Distance(startpos, transform.position) > MaxDistence)
            {
                DestroyBullet();

            }

        }
    }

    private void DestroyBullet()
    {
        if (OnDestroyEffect != null)
            Instantiate(OnDestroyEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Hundle collisions

    }
}
}
