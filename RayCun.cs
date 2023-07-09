using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할 : 보이지 않는 총알을 발사하고 싶다
// 보이지 않는 총알 : Ray(방향과 거리를 가진 선) , RayCast(선을 발사), RayCasHit(발사한 선이 부딪힌 곳의 정보)
// 총알이 부딪혔을 때 효과, 총구,총알 발사 위치(총구)

// [문제] 화면의 가운데 조준점과 총구에서 발사되는 총알 위치가 맞지 않음
// [해결] 총구의 위치에서 카메라가 바라보는 방향으로  Ray를 발사
// + 보이지 않는 총알이 특정 물체와 충돌되지 않도록 만들고 싶다. (layer)

// 역할 : Mode를 구분해서 단발/연사 모드
// Mode (pistol, rifle) , 다음 발사 시간, 발사 쿨타임

//역할 : Enemy를 쏘면 Enemy에게 Damage를 입힌다
// - 총의 데미지, 
public class RayCun : MonoBehaviour
{
    public ParticleSystem impactEffect; //효과
    public Transform firePos; //총구
    public AudioSource impactaudio;

    public Camera mainCam; //최적화

    public enum Mode { Pistol, Rifle }
    public Mode mode = Mode.Pistol;
    private float nextTimeToFire; // 다음 발사 시간
    public float fireRate = 10f; //연사율
    private Vector3 randomMousePos; //반동 구현

   

    
    void Start()
    {
        impactaudio = GetComponent<AudioSource>();
        mainCam = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // A. 단발 (Pistol 모드)
        if (mode == Mode.Pistol)
        {
            //fire1 버튼 누르면 발사
            if (Input.GetButtonDown("Fire1")) { Shoot(); }
        }

        // B. 연사 (Rifle 모드)
        if (mode == Mode.Rifle)
        {
            //Fire1 버튼을 누르고 있는 동안 총 발사
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                //다음 발사 시간 계산 = 현재 발사 시간 기준 + 발사쿨타임(연사율)
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        
    }

    //Ray 보이지 않는 총알 발사
    void Shoot()
    {
        // A. 단발 모드 시, Ray의 위치는 마우스 커서 위치
        if (mode == Mode.Pistol)
        {
            randomMousePos = Input.mousePosition;
        }
        // B. 연사 모드 시, Ray의 위치는 Random 마우스 커서 위치
        if (mode == Mode.Rifle)
        {
            //반지름이 R인 원을 만들어서 화면 가운데 기준 랜덤 사격 포인트 위치 생성
            randomMousePos = Input.mousePosition + Random.insideUnitSphere * 30f; //radius
            //z 값은 2차원 평면 Screen에서 필요 없기 때문에 0 으로 Reset
            randomMousePos.z = 0f;
        }
        

        // 1. Ray 생성 : 위치(firePos), 방향(firePos의 Z축, Camera가 바라보는 정중앙의 방향) , 거리
        //Vector3 direction = mainCam.ScreenPointToRay(Input.mousePosition).direction;
        Ray ray = mainCam.ScreenPointToRay(randomMousePos);
        //Ray ray = new Ray(firePos.position, direction);
        // 2. RaycastHit 생성 : 부딪힌 곳 정보
        RaycastHit hitInfo;
        // 3. Raycast ... (1)번에서 생성한 Ray를 발사
        int layer = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Weapon");
        // 4. 부딪혔는지 안 부딪혔는지 충돌체크
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~layer)) //(선, 부딪힐정보저장, 거리, layer 이진수 반전);
        {
            //부딪힌 물체가 enemy라면 데미지를 입힌다 -> Tag
            /*if (hitInfo.collider.tag == "Enemy")
            {
                print("EnemyHp--");
            }*/
            if (hitInfo.transform.CompareTag("Enemy"))
            {
                IDamage enemy = hitInfo.transform.GetComponent<IDamage>(); //0. IDamage 인터페이스가 달려있다면
                if (enemy != null) {
                    enemy.DamageProcess(); //Damage 입힌다
                 }

               /* 풀이
                Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.DamageProcess();
                }
                NavEnemy navEnemy = hitInfo.transform.GetComponent<NavEnemy>();
                if (navEnemy != null)
                {
                    navEnemy.DamageProcess();
                }
                */
            }

            // 5. ImpactEffect를 부딪힌 곳에 위치
            impactEffect.transform.position = hitInfo.point;
            // 5-1. 부딪힌 곳에서 수직으로 Effect 의 방향을 회전시켜준다. 
            impactEffect.transform.forward = hitInfo.normal;
            // 6. ImpactEffect 효과 재생
            impactEffect.Stop();
            impactEffect.Play();

           
        }
        impactaudio.Stop();
        impactaudio.Play();
    }

    private void OnDrawGizmos() //레이저사이트 만들기
    {
        //1. Gizmo용 Ray를 생성한다.
        //Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray (mainCam.transform.position, mainCam.transform.forward);
        //2. Gizmo용 RayCastHit을 생성한다.
        RaycastHit hitInfo;
        // + 충돌 제외할 Layer 설정 (Player, Weapon)
        int layer = 1 << LayerMask.NameToLayer("Player") | 1<< LayerMask.NameToLayer("Weapon");
        //3. Gizmo용 Ray를 발사한다.
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) //(ray, 정보그릇, 길이)
        {
            
            Gizmos.color = Color.red; 
            Gizmos.DrawLine(ray.origin, hitInfo.point); // (ray 시작점, 끝점)

        } else
        {
            Gizmos.color = Color.white;
            // Ray의 시작점 위치 : ray.origin 
            Vector3 startPos = ray.origin;
            //Ray의 끝점 위치 : 시작점 + ray의 방향 * ray를 그릴 거리 (p=p0+vt)
            Vector3 endPos = startPos + ray.direction * 100f;
            //b-1. Gizmo용 Ray의 원래 길이(100)만큼 그려준다.
            Gizmos.DrawLine(startPos, endPos);
        }
        //3-a. Gizmo용 Ray가 어딘가에 부딪힌 경우
        // a-1. 부딪힌 위치까지만 Gizmo용 Ray를 그려준다.
        //3-b. Gizmo용 Ray가 부딪히지 않은 경우
        //b-1. Gizmo용 Ray의 원래 길이 (Mathf.Infinity) 만큼 그려준다.
    }
}
