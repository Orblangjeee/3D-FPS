using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//역할 : 총알이 앞으로 날아가게 하고 싶다
//- 방향 : 총알이 날아가는 방향, 총알의 z축 방향
//- 속력 : 총알이 날아가는 속력


public class Bullet : MonoBehaviour
{
    public float moveSpeed = 1f;
    
   
    void Start()
    {
        
    }

 
    void Update()
    {//p = p0 + v(방향*속력)t
        transform.position +=  (transform.forward * moveSpeed) * Time.deltaTime;
    }
}
