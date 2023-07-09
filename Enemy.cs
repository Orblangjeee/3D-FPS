using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//���� : Player(Target)�� �Ѿư��� �����Ѵ�.
// ����, �̵�, ����, �ǰ�, ����
//CharacterController, target
//Animator�� ���� �� state�� �´� Animation�� �����Ѵ�.

//FSM ���¸� ����
// 1. ���(Idle): Player�� ���� ���� �ȿ� ������ ã�� ���
//- Ž������, Target���� �Ÿ� 
// 2. �̵�(Move): Player�� ������ �ٰ����� ���
// - �̵��ӷ�, �̵�����
// 3. ����(Attack): player���� �� ������ �ٰ����� �����ϴ� ���
// - ���ݹ���, Target���� �Ÿ� Distance
// 4. �ǰ�(Damage): Player���� ���� ���ϴ� ���
// - Enemy �ִ� HP, ���� HP, Hp 0�� �Ǹ� ����
// �ǰݽ� �����ð� ���� ������ ����
// ���� �ð�/ ����ð�

// 5. ����(Death) : player���� ���� ���ϴٰ� ���� 
// [����] Target�� ���� ���ƴٴϴ� ����
// [�ذ�1] �߷� ���ӵ�(-9.81)�� �Ʒ� �������� �����־� ���ư��� ���ϰ� ��
// [�ذ�2] ���� ����ִ� ������ �߷°��ӵ� Reset
public class Enemy : MonoBehaviour, IDamage
{
    private CharacterController cc;
    private Animator anim;
    public Transform target;            //Player

    [Header("Idle Info")]               //Header, ����
    public float detectRange = 10f;     //Ž������
    public float distance = 0f;         //Target���� �Ÿ�

    [Header("Move Info")]
    public float moveSpeed = 2f;
    public float rotSpeed = 100f; //ȸ�� �ӷ�
    public float yvelocity; //ĳ���Ͱ� �޴� �߷� ������
    private float gravity = -9.81f; //�߷°��ӵ�

    [Header("Attack Info")]
    public float attackRange = 4f;
    public float stunTime =3f;
    public float currentTime;

    [Header("Damaged Info")]
    public float currentHP;
    public int maxHp = 10;

    [Header ("Die Info")]
    private float destroyTime = 5f;
    public enum State //��� ȣ��
    {
        Idle, Move, Attack, Damaged, Die

    }
    public State state = State.Idle;
    
    void Start()
    {
        //CharacterController ������Ʈ�� ������ �ʱ�ȭ
        cc= GetComponent<CharacterController>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //"Player" tag�� ���� Gameobject �˻��ؼ� Target ���� �ʱ�ȭ
        currentHP = maxHp;
        anim = this.GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        #region Switch Ǯ�� (if��)
        if (state == State.Idle) //��� :player�� ã�� ���
        {
            Idle();
        }
        else if (state == State.Move) // �̵� : Player���� ������ �ٰ����� ���
        {
            Move();
        }
        else if (state == State.Attack) //���� : Player ���� �� ������ �ٰ����� �����ϴ� ���
        {
            Attack();
        }
        else if (state == State.Damaged) //�ǰ� : player���� ���� ���ϴ� ���
        {
            Damaged();
        }
        else if (state == State.Die) //���� : player���� ���� ���ϴٰ� ����
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

        //3. player�� �� ������ �Ÿ��� �˾ƾ� �Ѵ�.
        distance = Vector3.Distance(target.position, transform.position);
        //float localDistance = (target.position - transform.position).magnitude;
        //float localDistance = (target.position - transform.position).sqrmagnitude;

    }

    private void Idle()
    {
       
        anim.SetTrigger("Idle");
        StopCoroutine(Stun());
        //2. Player�� Ž�� ���� ������ ���;��Ѵ�.
        if (distance < detectRange)
        {
            //1. Player ã�Ƽ� �Ѿư��� ����� �ʹ�. (�̵�����)
            state = State.Move;
            anim.SetTrigger("Move");
        }
    }

   

   
    private void Move()
    {
        
        //1. Target�� ������ ���Ѵ�. -> ���� ���� �� ����ȭ!
        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        Vector3 rotDirection = direction;

        //2. Target�� ������ �ٶ󺸸鼭 �̵��ϱ� (ȸ��)
        rotDirection.y = 0f;
        //transform.forward = rotDirection; // transform.forward = target.position - transform.position; --> Vector�� ���ؼ� �� z�� ��ġ�� ����� ��ġ��Ų��.
        //transform.LookAt(target);  //Unity ���� �Լ� : target�� �ٶ󺸵��� �Ѵ�.
        //3. target�� ���� ������ ȸ����Ų��.
        Vector3 dir = target.position - transform.position;
        Vector3 detailDir = new Vector3(3, 4, -3) - new Vector3(2, 1, 0);

        float x = target.position.x - transform.position.x;
        float y = 0; //target.position.y - transform.position.y;
        float z = target.position.z - transform.position.z;
        detailDir = new Vector3(x, y, z);

        Quaternion rotation = Quaternion.LookRotation(rotDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);

        // �߷�(F) ���� : �߷°��ӵ���ŭ �Ʒ��� �������� ��
        if (cc.isGrounded) { yvelocity = 0f; }//�߷� ������ Reset
        // 1. F(�߷�) = m(����=1)*a(�߷°��ӵ�=gravity) F=ma
        // 2. V(�ӵ�) = Vo(�߷´�����) + a(�߷°��ӵ�) * t V=V0+at
        yvelocity += gravity * Time.deltaTime;
        // �߷��� enemy�� ���⿡ ����
        direction.y = yvelocity;

        //2. Target�� �������� ������ �ӵ��� �̵�
        cc.Move(direction * moveSpeed * Time.deltaTime);

        //�̵��ϴ� Target�� �� ���ݹ����� ������ ����
        if (distance < attackRange) { state = State.Attack; anim.SetTrigger("Attack"); }
        //�̵� ��, Target�� Ž������ �ٱ����� ������ ����
        if (distance > detectRange * 1.5f) { state = State.Idle; anim.SetTrigger("Idle"); }

    }
    private void Attack()
    {
       
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        // [����] Attack ���¿����� Player�� ���������� ȸ������ �ʴ´�.
        // [�ذ�] Attack ���¿����� Player�� �����ϸ鼭 ȸ���� �� �ֵ��� �Ѵ�.
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);

        if (distance > attackRange) { state = State.Move; anim.SetTrigger("Move"); }

    }
    private void Damaged()
    {
        //�ִϸ��̼� ���¸� Damage -> Idle

        //[����] : Damaged �ִϸ��̼��� ������ ����, code �󿡼� Idle ���·� �̵� -> �ִϸ��̼� & ������ ��ũ�� �ȸ���
        //[�ذ�] : Damaged �ִϸ��̼��� ���� ��(�����ð�)�� Enemy�� �����̵��� �ڵ� ����

       



    }

    //�ǰ� (Damaged) ������ �� �ܺο��� ȣ���� �Լ�
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
            #region �ڷ�ƾ Ǯ��
            //1. �ð��� ����Ѵ�
            /*currentTime += Time.deltaTime;
            //2. ����� �ð��� �����ð��� ������
            if (currentTime > stunTime)
            {
                //Animation : Damaged -> Idle
                state = State.Idle;
                currentTime = 0; //4. ����ð��� Reset
            }*/
            #endregion
            //state = State.Damaged;
            Debug.Log(currentHP);
        }
    }
    public void Die() 
    {
        //�Ʒ��� ������ ������ �ӵ��� �̵�
        //p = p0 +vt
        // ���� �Ʒ� �ӷ� 0.3f
        
        transform.position = transform.position + Vector3.down * 0.3f * Time.deltaTime;
        Destroy(gameObject, destroyTime);
    }

    //Coroutine(�ڷ�ƾ) 
    private IEnumerator Stun() //type�� �ٸ��� ������ return �Լ� �ʿ�!
    {
        //0. Enemy ���¸� Damage ���·� ��ȯ
        state = State.Damaged;
        //1. �����ð� ���� ���
        yield return new WaitForSeconds(stunTime);
        //2. Enemy ���¸� Idle ���·� ��ȯ
        state = State.Idle;

    }


    //Gizmo�� Ȱ���Ͽ� Ž������/���ݹ��� �ð�ȭ
    private void OnDrawGizmos()
    {
        //1. Ž�������� �׷��ش�. (�����) / ���� ����, Enemy(�� �ڽ�)�� �������� Sphere(��)�� �ϳ� �׸���.
        Gizmos.color = Color.yellow;
        if (state == State.Move || state == State.Attack) { Gizmos.DrawWireSphere(transform.position, detectRange *1.5f); }
        else {Gizmos.DrawWireSphere(transform.position, detectRange);}
        //2. ���ݹ����� �׷��ش�. (������)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
    }
}
