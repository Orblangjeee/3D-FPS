using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� : �Ѿ��� ������ ���ư��� �ϰ� �ʹ�
//- ���� : �Ѿ��� ���ư��� ����, �Ѿ��� z�� ����
//- �ӷ� : �Ѿ��� ���ư��� �ӷ�


public class Bullet : MonoBehaviour
{
    public float moveSpeed = 1f;
    
   
    void Start()
    {
        
    }

 
    void Update()
    {//p = p0 + v(����*�ӷ�)t
        transform.position +=  (transform.forward * moveSpeed) * Time.deltaTime;
    }
}
