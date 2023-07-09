using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;

//역할 : FSM을 통해 각 상태관리
//Idle, Move, Attack, Damaged, Die & Animator

//1. Idle : 가만히 있을 때 (탐지) - 탐지 범위(Raycast 충돌범위)
//2. Move : 이동할 때 (탐지, 공격 사거리) - 공격 범위 (Raycast 충돌범위)
//3. Attack : 공격할 때 (공격사거리) - player에게 Damage를 입힌다.
//4. Damaged + DamageProcess : (피격 시 한 번 실행) - HP, MaxHp
//5. Die : 죽었을 때 실행 - DestroyTime

public class NavEnemy : MonoBehaviour, IDamage
{
    public enum State { Idle, Move, Attack, Damaged, Die} //FSM
    public State state = State.Idle;
    private Animator anim; //Animator
    private NavMeshAgent agent; //NavMeshAgent
    private Transform target; //쫓아다닐 타깃

    public float detectRange = 10f; //탐지 범위(충돌범위 Raycast)
    public float attackRange = 4f; //공격 범위(충돌범위 Raycast)
    public int maxHp = 10;
    private int currentHp;
    public float destroyTime = 4f;

    private float currentTime;
    private float attackTime =1.5f;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle: Idle(); break;
            case State.Move: Move(); break;
            case State.Attack: Attack(); break;
            case State.Damaged: Damaged(); break;
            case State.Die: Die(); break;
        }
    }

    //Idle : 탐지영역으로 Player 탐색
    public void Idle()
    {
        bool isDetect = IsHitTarget(detectRange);
        //Player가 탐색범위 안으로 들어오면 Idle - > Move 상태로 전환
        if (isDetect)
        {
            ChangeState(State.Move);
        }
    }

    //영역 탐지 : 탐지되면 True, 안되면 False
    private bool IsHitTarget(float range) //OnDraw 탐지/공격 영역 등 Gizmo로 표시
    {
        //Player에만 충돌하는 탐색 영역을 구 형태로 만듦
        int layer = 1 << LayerMask.NameToLayer("Player");
        //만일 부딪혔다면?
        Collider[] hits = Physics.OverlapSphere(transform.position, range, layer);
       
        if (hits.Length > 0) //A. 부딪힌 경우 : True
        {
            return true;
        }
        else  //안 부딪힌 경우 : False
        {
            return false;
        }
    }
    private void OnAnimatorMove()
    {
        
       // state = State.Move;
        
    }
    //Move : Player 탐지 + 공격 범위 내 Player 탐색
    private void Move()
    {
        //NavMeshAgent 목적지를 알려준다. (이동 & 회전)
        agent.SetDestination(target.position);
        // A. Player가 탐색범위를 벗었나는지 확인
        bool isDetect = IsHitTarget(detectRange);
        // player가 공격범위 안으로 들어왔는지 확인
        bool isAttack = IsHitTarget(attackRange);

        // 벗어났으면 Move - > Idle

        // 들어왔으면 Move - > Attack
        if (isAttack)
        {
            ChangeState(State.Attack);
        }
        else if(isAttack == false && isDetect == false)
        {
            ChangeState(State.Idle);
        }
    
    }
    //Attack : Player가 도망가는지 탐색
    private void Attack() 
    {
        //공격범위에 있는지 확인한다.
        bool isAttack = IsHitTarget(attackRange);

        /* Attack anim + damage
        //AttackTime마다 Player에게 Damage를 입힌다
        //목표 : Attack 애니메이션 끝날 때마다 Console 창에 "Attack!"
        //1. 시간이 경과한다
        currentTime += Time.deltaTime;
        //2. 경과한 시간이 일정시간(attackTime) 보다 커지면
        if (currentTime > attackTime)
        {
            Debug.Log("Attack!");
            currentTime = 0f;
        }
        //3. 경과시간 Reset
        */
        //만약 공격범위를 벗어나면
        if (isAttack == false)
        {//Attack -> Move
            ChangeState(State.Move);
        }
        
    }

    //Damaged
    private void Damaged()
    {

    }
    //DamageProcess : Enemy HP 관리 및 죽음
    public void DamageProcess()
    {
        if (state == State.Die) { return; }
        currentHp--;  //1. HP를 -1씩 감소
        anim.SetTrigger("Damaged");
        state = State.Damaged;
            if (currentHp < 1) //2. 만일 HP가 0이 되면
        {
            ChangeState(State.Die);         //3. Die 상태로 변환

        }
        
    }
    //Coroutine : DamageProcess

    //Die : 죽었으면 흙으로 돌아감
    private void Die()
    {

    }

    public void ChangeState(State nextState) //상태 변경
    {
        switch (nextState)
        {
            case State.Idle: 
                anim.SetTrigger("Idle");
                agent.enabled = false; //움직임 Stop
                break;
            case State.Move: 
                anim.SetTrigger("Move");
                agent.enabled = true;
                break;
            //case State.Damaged: anim.SetTrigger("Damaged"); break;
            case State.Attack: 
                anim.SetTrigger("Attack");
                agent.enabled = false;
                break;
            case State.Die:
                anim.SetTrigger("Die");
                agent.enabled = false;
                GetComponent<CharacterController>().enabled = false;//충돌 off
                Destroy(gameObject, destroyTime);//DestroyTime 지나면 파괴
                break;
           
        }
        state = nextState;
    }

    //Gizmo 그리기
    private void OnDrawGizmosSelected()
    {
        //A. 탐지 범위 그려준다. (녹색)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        //B. 공격 범위 그려준다. (적색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
