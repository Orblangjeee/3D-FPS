using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

//���� : ������� �Է¿� ���� NavMesh������ �̵��Ѵ�.
// ������(Ŭ��), ���� ������? (����� Ű�Է�)

public class NavPlayer : MonoBehaviour
{
    private NavMeshAgent agent; //������Ʈ
    public Transform target; //������

    
    void Start()
    {
        //NavMeshAgent ������Ʈ�� �����´�(�ʱ�ȭ)
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //1. Ray�� ����ϴ�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //2. Ray�� �浹�� ����
            RaycastHit hitInfo;
            //3. Ray�� ���µ� �浹���� ���
            if (Physics.Raycast(ray, out hitInfo))
            {
                //�ش� ���(target)�� �̵�
                agent.SetDestination(hitInfo.point);
                //� ������ ���콺�� �����ߴ���
                target.position = hitInfo.point;
            }
            

           
        }
    }
}
