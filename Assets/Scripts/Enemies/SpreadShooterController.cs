using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShooterController : MonoBehaviour, IDamageable
{
    [SerializeField]
    GameObject projectile;
    [SerializeField]
    Transform firePoint;
    Vector3 firePointOffset;

    private float bulletSpeed = 8.0f;
    private float moveSpeed = 1.5f;

    public GameObject destroyParticle;

    Timer betweenSeriesTimer;

    public bool spawnRight = true;
    bool isActive = false;
    CameraScroll cameraScript;

    private int hp = 7; //was 10 if need to switch back
    Timer damagedTimer;
    SpriteRenderer spriteRenderer;

    public AudioClip enemyDieSfx;
    AudioManager audioManager;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        cameraScript = Camera.main.GetComponent<CameraScroll>();
        betweenSeriesTimer = new Timer(2.0f, 1.0f);
        damagedTimer = new Timer(0.1f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

        Vector3 tempPos = firePoint.localPosition;
        if (!spawnRight) tempPos.x = -tempPos.x;
        firePointOffset = tempPos;
    }
    private void Update()
    {
        if (!spawnRight) anim.SetBool("to_right", true);
        else anim.SetBool("to_right", false);

        if (!isActive)
        {
            if (!CameraScroll.IsSpriteOffScreen(gameObject, 2.0f))
            {
                Spawn();
            } else
            {
                return; // off screen
            }
        }

        if (CameraScroll.IsSpriteOffScreen(gameObject, 2.0f)) isActive = false;

        betweenSeriesTimer.Tick();
        damagedTimer.Tick();

        if (betweenSeriesTimer.ResetTimer())
        {
            Shoot(transform.position + firePointOffset, (spawnRight ? -1 : 1) * Vector2.right);
        }

        if (damagedTimer.IsReady())
        {
            spriteRenderer.material.SetColor("_Color", Color.black);
        }
        else
        {
            spriteRenderer.material.SetColor("_Color", Color.Lerp(Color.grey, Color.white, Random.value));
        }

        Move();
    }

    void Shoot(Vector2 position, Vector2 direction)
    {
        if (direction.sqrMagnitude != 0.0f) direction.Normalize();
        else direction = Vector2.right;

        for (int i = 0; i < 3; ++i)
        {

            GameObject projectileObj = Instantiate(projectile, position, Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, direction) + 20 * (i - 1), Vector3.forward));
            projectileObj.GetComponent<Rigidbody2D>().velocity = Quaternion.AngleAxis(20 * (i - 1), Vector3.forward) * direction * bulletSpeed;
        }
    }

    void Spawn()
    {
        isActive = true;
        if (!spawnRight)
        {
            Rect screenRect = CameraScroll.GetScreenRect();
            var sprite = GetComponent<Renderer>();
            Vector2 center = sprite.bounds.center;
            Vector2 extends = sprite.bounds.extents;

            Vector3 oldPos = transform.position;
            oldPos.x = screenRect.x - extends.x * 1.0f;
            transform.position = oldPos;
        }
    }

    void Move()
    {
        transform.Translate(moveSpeed * Time.deltaTime * (spawnRight ? -1 : 1) * Vector2.right);
    }

    public void InflictDamage(int dmg)
    {
        if (!isActive) return;
        hp -= dmg;
        damagedTimer.ResetTimer();
        if (hp <= 0)
        {
            Instantiate(destroyParticle, transform.position, Quaternion.identity);
            audioManager.PlayOneShot(enemyDieSfx);
            Destroy(gameObject);
        }
    }
}