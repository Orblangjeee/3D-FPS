using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� : Enemy�� ���� Animation�� Ư�� Ÿ�ֿ̹� ����Ǵ� Event

public class EnemyAnim : MonoBehaviour
{
    //Enemy�� Attack Animation�� ����� �� Player(Target)�����ϴ� �Լ�
    public void AttackTarget()
    {
        print("�÷��̾� ����!");
        PlayerHealth.Instance.SetHp(-1);
    }    

    //�ǰ� ���°� ������ Enemy�� ���¸� Idle ���·� �����Ѵ�.
    public void FinishedDamageProcess()
    {
        //3. NavEnemy ������Ʈ�� ���� �θ� ������Ʈ ã��
        //2. NavEnemy ������Ʈ�� ã�ƾ��Ѵ�.
        NavEnemy enemy = GetComponent<NavEnemy>();
        //1. Enemy���� Idle�� ����
        enemy.ChangeState(NavEnemy.State.Idle);
    }
}
