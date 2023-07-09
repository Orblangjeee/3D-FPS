using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

//역할 : 사용자의 입력에 의해 NavMesh영역을 이동한다.
// 목적지(클릭), 언제 가야해? (사용자 키입력)

public class NavPlayer : MonoBehaviour
{
    private NavMeshAgent agent; //컴포넌트
    public Transform target; //목적지

    
    void Start()
    {
        //NavMeshAgent 컴포넌트를 가져온다(초기화)
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //1. Ray를 만듭니다
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //2. Ray가 충돌한 정보
            RaycastHit hitInfo;
            //3. Ray를 쐈는데 충돌했을 경우
            if (Physics.Raycast(ray, out hitInfo))
            {
                //해당 장소(target)로 이동
                agent.SetDestination(hitInfo.point);
                //어떤 지점을 마우스로 선택했는지
                target.position = hitInfo.point;
            }
            

           
        }
    }
}
