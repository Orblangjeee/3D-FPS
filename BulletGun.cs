using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할 : 내가 원하는 때에, 총이 향하는 방향으로 총알을 발사합니다. 
//  - 총알 공장 : bullet Factory
//  - 총알 발사 위치 : Fire Pos
//  - 총알 발사 방향 : Fire Pos(총구)가 향하는 방향
//  - 사용자의 입력 : 마우스 왼쪽 버튼 클릭

// 역할 : 총알을 ObjectPooling을 통해 탄창으로 만들어 재사용한다.
//  - 총알 공장 : bullet Factory
//  - 탄창 
//      a. 배열   : 탄창의 Original 초기값(Backup)
//      b. 리스트 : 진짜진짜진짜 탄창.
//  - 탄창의 크기 : 한 번에 만들 총알의 개수

// 역할 : ObjectPool을 사용해 만든 재사용 총알이 파괴되는 경우,
//        다시 내 탄창 안에 집어넣는다.

// 역할 : 총 발사 시, Effect들을 사용한다.
//  - 특수효과 a.총알이 어딘가에 부딪혔을 때 특수효과 재생
//  - 특수효과 b.총알을 발사할 때 특수효과 재생
//  - 총알 발사 시, 효과음 재생

// 역할 : Mode (단발, 연사 모드)
//  - Mode --- 단발, 연사모드
//  - 연사(Easy)
//   - 일정시간 : 1발을 발사할 때 걸리는 시간(sec)
//   - 경과시간 : 1발 발사 시 일정시간에 도달하는 시간
//  - 사용자의 입력 : 눌렀을 때, 누르고 있는 동안
public class BulletGun : MonoBehaviour
{
    //  - 총알 공장 : bullet Factory
    public GameObject bulletFactory;
    //  - 총알 발사 위치 : Fire Pos
    public Transform firePos;
    //  - 탄창 a.배열   : 탄창의 Original 초기값 -->> 
    private GameObject[] bulletPool;
    //  - 탄창 b.리스트 : 진짜진짜진짜 탄창. --> 
    private List<GameObject> bulletPoolList = new List<GameObject>();
    //  - 탄창의 크기 : 한 번에 만들 총알의 개수
    public int bulletSize = 30;

    // ----- 총알의 Effect ------
    // 총알이 부딪혔을 때 Effect
    public GameObject bulletImpactEffect;
    // 총알을 발사할 때 Effect 
    public ParticleSystem muzzleFlashEffect;
    // 총알을 발사할 때 효과음 사운드
    public AudioSource audios;

    // ----- 연사 모드 -----
    public enum Mode
    {
        Pistol, // 단발(권총)
        Rifle   // 연사(머)
    }
    public Mode mode = Mode.Pistol;
    //   - 일정시간 : 1발을 발사할 때 걸리는 시간(sec)
    public float nextTimeToFire = 1f;
    //   - 경과시간 : 1발 발사 시 일정시간에 도달하는 시간
    private float currentTime;

    

    private void Start()
    {
        //AudioSource 컴포넌트 가져온다 (=초기화)
        AudioSource audio = GetComponent<AudioSource>();
        // Object Pool 은 맨 처음 한 번만 만들면 된다.
        BulletObjectPool();
    }

    // Update is called once per frame
    void Update()
    {
        // A. 만일 GunMode가 단발(Pistol) 모드라면?
        if (mode == Mode.Pistol)
        {
            // 만일, 총알이 남아있는 경우에만 사용자의 입력 : "Fire1" 버튼을 클릭했을 때 -> 발사
            if (bulletPoolList.Count > 0 && Input.GetButtonDown("Fire1"))
            {
                Fire();
                // 발사 시, Effect 재생
                PlayMuzzleFlash();
            }
        }
        // B. 만일 GunMode가 연사(Rifle) 모드라면?
        if (mode == Mode.Rifle)
        {
            // 총알이 남아있는 경우에만, Fire1버튼을 누르면 일정시간마다 총알을 발사하고 싶다.
            if (bulletPoolList.Count > 0)
            {
                // Fire1 버튼을 눌렀을 때 -> 총알 발사
                if (Input.GetButtonDown("Fire1"))
                {
                    Fire();
                    PlayMuzzleFlash();
                }
                // Fire1 버튼을 누르는 동안 -> nextTimeToFire 시간에 맞추어 연사
                else if (Input.GetButton("Fire1"))
                {
                    currentTime += Time.deltaTime;

                    if (currentTime > nextTimeToFire)
                    {
                        Fire();
                        PlayMuzzleFlash();
                        currentTime = 0f;
                    }
                   

                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    currentTime = 0f;
                }
                // Fire1 버튼을 뗐을 때 -> 경과시간 Reset


                //  B-1. 시간이 흐른다(경과한다.)
               
            }
        }
    }

    // Bullet 오브젝트 풀을 만든다.
    private void BulletObjectPool()
    {
        // 1. 오브젝트 풀의 크기만큼 배열의 방을 잡아놓습니다. = 탄창 안의 총알 개수
        bulletPool = new GameObject[bulletSize];
        // 2. [반복]총알 개수 만큼 총알을 생성해서, 오브젝트 풀에 넣는다.
        //  - 생성할 총알 개수 : bulletSize
        for (int i = 0; i < bulletSize; i++)
        {
            // 2-1. 총알을 생성한다.
            GameObject bullet = Instantiate(bulletFactory);
            // 2-2. 총알을 방(=Original 탄창)에 넣는다.
            bulletPool[i] = bullet;
            // 2-3. 방에 넣은 총알을 비활성화 상태로 만든다.
            bulletPool[i].gameObject.SetActive(false);
            // 2-4. 총알을 진짜진짜진짜 탄창에 넣는다.
            bulletPoolList.Add(bulletPool[i]);

            // + 총알이 돌아가야할 탄창도 함께 알려준다.
            bulletPool[i].GetComponent<Bullet>().InitMyGun(this); //BulletGun 펌웨어업데이트
        }
    }


    // 총알 발사 ---> 탄창에 있는 총알을 사용해 발사한다.
    private void Fire()
    {
        // 1. 진짜진짜진짜 탄창의 첫 번째 총알을 가져옵니다.
        GameObject bullet = bulletPoolList[0];
        // 2. 총알을 총구에 위치시킵니다.
        bullet.transform.position = firePos.position;
        // 3. 총알의 방향을 총구의 방향과 일치시킵니다.
        bullet.transform.forward = firePos.forward;
        // 4. 총알을 활성화 상태로 만든다.
        bullet.SetActive(true);
        // 5. 탄창에서 사용한 (첫 번째)총알을 제거한다.
        bulletPoolList.RemoveAt(0);
        // 6. 총알 발사 사운드 재생
        audios.Stop();
        audios.Play();

    }

    // 총알 발사 ---> 총알을 직접 생성해서 발사한다.
    private void LegacyFire()
    {
        // 2. 총알을 생성한다.
        GameObject bullet = Instantiate(bulletFactory);
        // 3. 총알을 총구에 위치시킨다.
        bullet.transform.position = firePos.position;
        // 4. 총알의 방향(Z축)을 총구의 방향(Z축)과 일치시킨다.
        //  - 총알의 z축 방향 : Bullet의 transform.forward
        //  - 총구의 z축 방향 : firePos의 .forward
        bullet.transform.forward = firePos.forward;
    }

    // 탄창에 어떤? 총알을 다시 추가한다.
    // - 전달 받을 매개체 : Bullet (부딪히거나, lifetime이 다 된 총알)
    public void DeactiveBullet(GameObject bullet)
    {
        // 1. 받아온 총알을 비활성화 상태로 변경한다.
        bullet.SetActive(false);
        // 2. 비활성화 상태인 총알을 탄창에 다시 넣는다.
        bulletPoolList.Add(bullet);
    }

    // 총알이 부딪혔을 때의 Effect 재생한다.
    // - 총알이 부딪힌 위치
    // - Effect (부딪혔을 때 재생될 특수효과)
    public void PlayImpactEffect(Vector3 impactPos)
    {
        // 1. 파티클을 총알이 부딪힌 곳에 위치시킨다.
        bulletImpactEffect.transform.position = impactPos;
        // 2. 파티클을 재생 (Stop & Play)
        ParticleSystem ps = bulletImpactEffect.GetComponent<ParticleSystem>();
        ps.Stop();
        ps.Play();
    }

    // 총알을 발사할 때 Effect를 재생한다.
    // - 총알 발사 Effect
    public void PlayMuzzleFlash()
    {
        // 총알 발사 Effect를 재생 (Stop & Play)
        muzzleFlashEffect.Stop();
        muzzleFlashEffect.Play();
    }
}
