using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//역할 : Player(Target)을 쫓아가서 공격한다.
// 정지, 이동, 공격, 피격, 죽음
//CharacterController, target
//Animator를 통해 각 state에 맞는 Animation을 관리한다.

//FSM 상태를 구분
// 1. 대기(Idle): Player가 일정 범위 안에 들어오면 찾는 기능
//- 탐색범위, Target과의 거리 
// 2. 이동(Move): Player에 가까이 다가가는 기능
// - 이동속력, 이동방향
// 3. 공격(Attack): player에게 더 가까이 다가가면 공격하는 기능
// - 공격범위, Target과의 거리 Distance
// 4. 피격(Damage): Player에게 공격 당하는 기능
// - Enemy 최대 HP, 현재 HP, Hp 0이 되면 죽음
// 피격시 일정시간 동안 움직임 정지
// 일정 시간/ 경과시간

// 5. 죽음(Death) : player에게 공격 당하다가 죽음 
// [문제] Target을 따라서 날아다니는 문제
// [해결1] 중력 가속도(-9.81)를 아래 방향으로 눌러주어 날아가지 못하게 함
// [해결2] 땅에 닿아있는 동안은 중력가속도 Reset
public class Enemy : MonoBehaviour, IDamage
{
    private CharacterController cc;
    private Animator anim;
    public Transform target;            //Player

    [Header("Idle Info")]               //Header, 제목
    public float detectRange = 10f;     //탐색범위
    public float distance = 0f;         //Target과의 거리

    [Header("Move Info")]
    public float moveSpeed = 2f;
    public float rotSpeed = 100f; //회전 속력
    public float yvelocity; //캐릭터가 받는 중력 누적값
    private float gravity = -9.81f; //중력가속도

    [Header("Attack Info")]
    public float attackRange = 4f;
    public float stunTime =3f;
    public float currentTime;

    [Header("Damaged Info")]
    public float currentHP;
    public int maxHp = 10;

    [Header ("Die Info")]
    private float destroyTime = 5f;
    public enum State //계속 호출
    {
        Idle, Move, Attack, Damaged, Die

    }
    public State state = State.Idle;
    
    void Start()
    {
        //CharacterController 컴포넌트를 가져와 초기화
        cc= GetComponent<CharacterController>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //"Player" tag를 가진 Gameobject 검색해서 Target 변수 초기화
        currentHP = maxHp;
        anim = this.GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        #region Switch 풀이 (if문)
        if (state == State.Idle) //대기 :player를 찾는 기능
        {
            Idle();
        }
        else if (state == State.Move) // 이동 : Player에게 가까이 다가가는 기능
        {
            Move();
        }
        else if (state == State.Attack) //공격 : Player 에게 더 가까이 다가가면 공격하는 기능
        {
            Attack();
        }
        else if (state == State.Damaged) //피격 : player에게 공격 당하는 기능
        {
            Damaged();
        }
        else if (state == State.Die) //죽음 : player에게 공격 당하다가 죽음
        {
            Die();
        }
        #endregion 
        switch (state) 
        {
            case State.Idle: Idle(); break;
            case State.Move: Move(); break;
            case State.Attack: Attack(); break;
            case State.Damaged: Damaged(); break;
            case State.Die: Die(); break;
        }

        //3. player와 나 사이의 거리를 알아야 한다.
        distance = Vector3.Distance(target.position, transform.position);
        //float localDistance = (target.position - transform.position).magnitude;
        //float localDistance = (target.position - transform.position).sqrmagnitude;

    }

    private void Idle()
    {
       
        anim.SetTrigger("Idle");
        StopCoroutine(Stun());
        //2. Player가 탐지 범위 안으로 들어와야한다.
        if (distance < detectRange)
        {
            //1. Player 찾아서 쫓아가게 만들고 싶다. (이동상태)
            state = State.Move;
            anim.SetTrigger("Move");
        }
    }

   

   
    private void Move()
    {
        
        //1. Target의 방향을 구한다. -> 방향 구한 후 정규화!
        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        Vector3 rotDirection = direction;

        //2. Target의 방향을 바라보면서 이동하기 (회전)
        rotDirection.y = 0f;
        //transform.forward = rotDirection; // transform.forward = target.position - transform.position; --> Vector를 구해서 내 z축 위치를 방향과 일치시킨다.
        //transform.LookAt(target);  //Unity 제공 함수 : target을 바라보도록 한다.
        //3. target을 향해 서서히 회전시킨다.
        Vector3 dir = target.position - transform.position;
        Vector3 detailDir = new Vector3(3, 4, -3) - new Vector3(2, 1, 0);

        float x = target.position.x - transform.position.x;
        float y = 0; //target.position.y - transform.position.y;
        float z = target.position.z - transform.position.z;
        detailDir = new Vector3(x, y, z);

        Quaternion rotation = Quaternion.LookRotation(rotDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);

        // 중력(F) 적용 : 중력가속도만큼 아래로 짓누르는 힘
        if (cc.isGrounded) { yvelocity = 0f; }//중력 누적값 Reset
        // 1. F(중력) = m(질량=1)*a(중력가속도=gravity) F=ma
        // 2. V(속도) = Vo(중력누적값) + a(중력가속도) * t V=V0+at
        yvelocity += gravity * Time.deltaTime;
        // 중력을 enemy의 방향에 적용
        direction.y = yvelocity;

        //2. Target의 방향으로 일정한 속도로 이동
        cc.Move(direction * moveSpeed * Time.deltaTime);

        //이동하다 Target이 내 공격범위에 들어오면 공격
        if (distance < attackRange) { state = State.Attack; anim.SetTrigger("Attack"); }
        //이동 중, Target이 탐지범위 바깥으로 나가면 정지
        if (distance > detectRange * 1.5f) { state = State.Idle; anim.SetTrigger("Idle"); }

    }
    private void Attack()
    {
       
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        // [문제] Attack 상태에서는 Player를 공격하지만 회전하지 않는다.
        // [해결] Attack 상태에서도 Player를 공격하면서 회전할 수 있도록 한다.
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);

        if (distance > attackRange) { state = State.Move; anim.SetTrigger("Move"); }

    }
    private void Damaged()
    {
        //애니메이션 상태를 Damage -> Idle

        //[문제] : Damaged 애니메이션이 끝나기 전에, code 상에서 Idle 상태로 이동 -> 애니메이션 & 움직임 싱크가 안맞음
        //[해결] : Damaged 애니메이션이 끝난 후(일정시간)에 Enemy가 움직이도록 코드 수정

       



    }

    //피격 (Damaged) 당했을 때 외부에서 호출할 함수
    public void DamageProcess()
    {
       
        if (state == State.Die)
        {
            return;
        }
        if (currentHP < 1)
        {
            currentHP = 0;
            state = State.Die;
            anim.SetTrigger("Die");
            print("Die");
        }
        else if (currentHP > 0) { }
        {
           
            anim.SetTrigger("Damaged");
            currentHP--;
            StartCoroutine(Stun());
            #region 코루틴 풀이
            //1. 시간이 경과한다
            /*currentTime += Time.deltaTime;
            //2. 경과한 시간이 일정시간을 넘으면
            if (currentTime > stunTime)
            {
                //Animation : Damaged -> Idle
                state = State.Idle;
                currentTime = 0; //4. 경과시간을 Reset
            }*/
            #endregion
            //state = State.Damaged;
            Debug.Log(currentHP);
        }
    }
    public void Die() 
    {
        //아래로 서서히 일정한 속도로 이동
        //p = p0 +vt
        // 방향 아래 속력 0.3f
        
        transform.position = transform.position + Vector3.down * 0.3f * Time.deltaTime;
        Destroy(gameObject, destroyTime);
    }

    //Coroutine(코루틴) 
    private IEnumerator Stun() //type이 다르기 때문에 return 함수 필요!
    {
        //0. Enemy 상태를 Damage 상태로 전환
        state = State.Damaged;
        //1. 일정시간 동안 대기
        yield return new WaitForSeconds(stunTime);
        //2. Enemy 상태를 Idle 상태로 전환
        state = State.Idle;

    }


    //Gizmo를 활용하여 탐지범위/공격범위 시각화
    private void OnDrawGizmos()
    {
        //1. 탐지범위를 그려준다. (노란색) / 색상 지정, Enemy(나 자신)를 기준으로 Sphere(구)를 하나 그린다.
        Gizmos.color = Color.yellow;
        if (state == State.Move || state == State.Attack) { Gizmos.DrawWireSphere(transform.position, detectRange *1.5f); }
        else {Gizmos.DrawWireSphere(transform.position, detectRange);}
        //2. 공격범위를 그려준다. (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
    }
}
