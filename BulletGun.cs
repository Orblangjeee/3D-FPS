using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//역할 : 총이 향하는 방향으로 총알을 발사합니다
// 총알 공장 bullet Factory , 총구(총알 발사 위치) FirePos, 총알 발사 방향 FirePos(총구)가 향하는 방향, 사용자입력(마우스 왼)

public class BulletGun : MonoBehaviour
{
    public GameObject bulletFactory;
    public Transform firePos; //로컬축

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButton("Fire1")) // 마우스 왼쪽 버튼을 누르면
        {
            GameObject bullets = Instantiate(bulletFactory);
            bullets.transform.position = firePos.position;
            bullets.transform.forward = firePos.forward;

        }
        // 총알 생성
        // 총알을 총구에 위치 시킨다.
        // 총알의 방향(Z축)을 총구의 방향(Z축)과 일치시킨다.
        // 총알의 z축 방향 : bullet의 transform.forward
        // 총구의 z축 방향 : firePos의 transform.forward
    }
}
