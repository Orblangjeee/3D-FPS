using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;

//���� : FSM�� ���� �� ���°���
//Idle, Move, Attack, Damaged, Die & Animator

//1. Idle : ������ ���� �� (Ž��) - Ž�� ����(Raycast �浹����)
//2. Move : �̵��� �� (Ž��, ���� ��Ÿ�) - ���� ���� (Raycast �浹����)
//3. Attack : ������ �� (���ݻ�Ÿ�) - player���� Damage�� ������.
//4. Damaged + DamageProcess : (�ǰ� �� �� �� ����) - HP, MaxHp
//5. Die : �׾��� �� ���� - DestroyTime

public class NavEnemy : MonoBehaviour, IDamage
{
    public enum State { Idle, Move, Attack, Damaged, Die} //FSM
    public State state = State.Idle;
    private Animator anim; //Animator
    private NavMeshAgent agent; //NavMeshAgent
    private Transform target; //�Ѿƴٴ� Ÿ��

    public float detectRange = 10f; //Ž�� ����(�浹���� Raycast)
    public float attackRange = 4f; //���� ����(�浹���� Raycast)
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

    //Idle : Ž���������� Player Ž��
    public void Idle()
    {
        bool isDetect = IsHitTarget(detectRange);
        //Player�� Ž������ ������ ������ Idle - > Move ���·� ��ȯ
        if (isDetect)
        {
            ChangeState(State.Move);
        }
    }

    //���� Ž�� : Ž���Ǹ� True, �ȵǸ� False
    private bool IsHitTarget(float range) //OnDraw Ž��/���� ���� �� Gizmo�� ǥ��
    {
        //Player���� �浹�ϴ� Ž�� ������ �� ���·� ����
        int layer = 1 << LayerMask.NameToLayer("Player");
        //���� �ε����ٸ�?
        Collider[] hits = Physics.OverlapSphere(transform.position, range, layer);
       
        if (hits.Length > 0) //A. �ε��� ��� : True
        {
            return true;
        }
        else  //�� �ε��� ��� : False
        {
            return false;
        }
    }
    private void OnAnimatorMove()
    {
        
       // state = State.Move;
        
    }
    //Move : Player Ž�� + ���� ���� �� Player Ž��
    private void Move()
    {
        //NavMeshAgent �������� �˷��ش�. (�̵� & ȸ��)
        agent.SetDestination(target.position);
        // A. Player�� Ž�������� ���������� Ȯ��
        bool isDetect = IsHitTarget(detectRange);
        // player�� ���ݹ��� ������ ���Դ��� Ȯ��
        bool isAttack = IsHitTarget(attackRange);

        // ������� Move - > Idle

        // �������� Move - > Attack
        if (isAttack)
        {
            ChangeState(State.Attack);
        }
        else if(isAttack == false && isDetect == false)
        {
            ChangeState(State.Idle);
        }
    
    }
    //Attack : Player�� ���������� Ž��
    private void Attack() 
    {
        //���ݹ����� �ִ��� Ȯ���Ѵ�.
        bool isAttack = IsHitTarget(attackRange);

        /* Attack anim + damage
        //AttackTime���� Player���� Damage�� ������
        //��ǥ : Attack �ִϸ��̼� ���� ������ Console â�� "Attack!"
        //1. �ð��� ����Ѵ�
        currentTime += Time.deltaTime;
        //2. ����� �ð��� �����ð�(attackTime) ���� Ŀ����
        if (currentTime > attackTime)
        {
            Debug.Log("Attack!");
            currentTime = 0f;
        }
        //3. ����ð� Reset
        */
        //���� ���ݹ����� �����
        if (isAttack == false)
        {//Attack -> Move
            ChangeState(State.Move);
        }
        
    }

    //Damaged
    private void Damaged()
    {

    }
    //DamageProcess : Enemy HP ���� �� ����
    public void DamageProcess()
    {
        if (state == State.Die) { return; }
        currentHp--;  //1. HP�� -1�� ����
        anim.SetTrigger("Damaged");
        state = State.Damaged;
            if (currentHp < 1) //2. ���� HP�� 0�� �Ǹ�
        {
            ChangeState(State.Die);         //3. Die ���·� ��ȯ

        }
        
    }
    //Coroutine : DamageProcess

    //Die : �׾����� ������ ���ư�
    private void Die()
    {

    }

    public void ChangeState(State nextState) //���� ����
    {
        switch (nextState)
        {
            case State.Idle: 
                anim.SetTrigger("Idle");
                agent.enabled = false; //������ Stop
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
                GetComponent<CharacterController>().enabled = false;//�浹 off
                Destroy(gameObject, destroyTime);//DestroyTime ������ �ı�
                break;
           
        }
        state = nextState;
    }

    //Gizmo �׸���
    private void OnDrawGizmosSelected()
    {
        //A. Ž�� ���� �׷��ش�. (���)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        //B. ���� ���� �׷��ش�. (����)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
