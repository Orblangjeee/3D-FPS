using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할 : 총알을 앞으로 날아가게 하고 싶다.
//  + Rigidbody를 통해 총알의 움직임을 제어하고 싶다.
//  - 방향 : 총알이 날아가는 방향, 총알의 Z축 방향
//  - 속력 : 총알이 날아가는 속력
//  - Rigidbody Component 

// 역할a : 총알이 어딘가에 부딪히면, 총알을 제거한다.
//  - 부딪힘 체크가 필요
// 역할b : 어디에도 부딪히지 않은 경우가 생기기 때문에,
//         일정시간 이후에, 총알을 제거하게 만든다.
//  - 일정시간 (lifeTime)
//  - 경과시간 (currentTime)

public class Bullet : MonoBehaviour
{
    // 속력 : 총알이 날아가는 속력
    public float moveSpeed = 10f;
    // Rigidbody Component 
    private Rigidbody rb;
    // 일정시간 (lifeTime)
    public float lifeTime = 10f;
    // 경과시간 (currentTime)
    private float currentTime;

    // 총알이 부딪혔을 때 Effect -- Factory
    public GameObject bulletImpactEffect;

    // Start is called before the first frame update
    private void Awake()
    {
        // Rigidbody 컴포넌트를 가져와서 초기화(init)
        rb = GetComponent<Rigidbody>();
    }
    // 총알이 활성화 될 때마다(=재사용) 방향 재설정
    void OnEnable()
    {
        // Rigidbody를 통해 총알을 앞으로 날린다.
        // -> AddForce : 가만히 있다가, 움직이게 만들 친구들에게 사용
        // rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        // -> velocity : 이미 움직이고 있는 친구들, 움직일 친구들에게 사용
        // + 문제 : deltaTime의 값이 계속 변동되므로, 총알의 날아가는 속도가 일정X
        //   해결 -> bullet은 Rigidbody의 영향을 받으므로 deltaTime을 삭제
        rb.velocity = transform.forward * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // 일정시간 이후에, 총알을 제거한다.
        // 1. 시간이 흐른다. (시간이 경과)
        currentTime += Time.deltaTime;
        // 2. 만일 경과한 시간이 일정시간을 넘어선 경우
        if (currentTime > lifeTime)
        {
            // 3. 총알(=나 자신)을 탄창에 다시 넣는다.
            FindObjectOfType<BulletGun>().DeactiveBullet(gameObject);
        }
    }

    private void OnDisable()
    {
        // LifeTime 경과시간 초기화
        currentTime = 0f;
    }

    // 총알이 어딘가에 부딪히면,
    private void OnCollisionEnter(Collision collision)
    {
        // + 총알의 위치에 부딪힌 특수효과를 재생한다.
        FindObjectOfType<BulletGun>().PlayImpactEffect(transform.position);
        // 3. 총알(=나 자신)을 탄창에 다시 넣는다.
        FindObjectOfType<BulletGun>().DeactiveBullet(gameObject);
    }
}
