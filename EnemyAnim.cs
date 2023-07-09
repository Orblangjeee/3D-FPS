using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//역할 : Enemy가 가진 Animation의 특정 타이밍에 실행되는 Event

public class EnemyAnim : MonoBehaviour
{
    //Enemy의 Attack Animation이 실행될 때 Player(Target)공격하는 함수
    public void AttackTarget()
    {
        print("플레이어 어택!");
        PlayerHealth.Instance.SetHp(-1);
    }    

    //피격 상태가 끝나면 Enemy의 상태를 Idle 상태로 변경한다.
    public void FinishedDamageProcess()
    {
        //3. NavEnemy 컴포넌트를 가진 부모 오브젝트 찾기
        //2. NavEnemy 컴포넌트가 찾아야한다.
        NavEnemy enemy = GetComponent<NavEnemy>();
        //1. Enemy상태 Idle로 변경
        enemy.ChangeState(NavEnemy.State.Idle);
    }
}
