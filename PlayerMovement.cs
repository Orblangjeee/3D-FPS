using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//역할 : player의 움직임을 전반적으로 관리한다. -> 캐릭터의 움직임을 CharacterController 컴포넌트를 통해 관리
// A. player를 앞/뒤/좌/우로 일정한 속력으로 움직인다.
//사용자의 입력 (a. 앞뒤/ b. 좌우), 일정한 속력

// B. player를 사용자의 입력에 따라 좌/우로 회전시킨다.
// 사용자의 입력 (마우스 좌-우), 회전 속력 (민감도), 사용자의 입력에 따른 회전 각도 저장

// C. player 를 사용자의 입력에 따라 상/하로 회전시킨다.
// 사용자의 입력 (마우스 상-하), 회전 속력 (B에서 만든걸 재사용), 사용자의 입력에 따른 회전 각도를 저장, 카메라 회전, 상하 회전 제한 각도

// D.자연스러운 중력
// ChracterController 컴포넌트, 중력 가속도, 중력 누적값 (중력가속도를 통해 나에게 누적된 중력값)

// E. 사용자의 입력에 따라 Player 점프
// 사용자의 입력(Jump), 파워, 최대점프가능횟수, 현재점프횟수

//player를 이동시키는 함수
// [문제] : palyer가 바라보는 방향이 아닌 unity 절대 축으로 이동한다. (글로벌)
// [해결] : palyer가 바라보는 방향으로 이동한다. (로컬)

public class PlayerMovement : MonoBehaviour
{
    #region 변수정의
    public float speed = 5f; //이동속력
    public float mouseSensitivity = 500f; //회전 속력(민감도)
    private float horizontalAngle; //사용자의 입력에 따른 좌-우 회전 각도를 저장
    private float verticalAngle; //사용자의 입력에 따른 상-하 회전 각도 저장
    public float limitAngle = 30; //회전 제한 각도
    
    private CharacterController cc; //캐릭터 컨트롤러 
    public float gravity = -9.81f; //중력 가속도 (9.81m/s)
    private float yVelocity; 

    public GameObject face; // 카메라

    public float jumpPower = 4.0f; 
    public bool isJump = false;
    public int jumpCount =1;
    private int jumping;
    #endregion
    
    void Start()
    {
        // 커서를 보이지 않게 하고, 화면 가운데에 고정시킨다. //esc누르면 마우스 등장, 클릭하면 사라짐
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>(); //초기화

    }

    void Update()
    {

        MovePlayer();
        RotateHorizontal();
        RotateVertical();
    }

    
    void MovePlayer() //player를 이동시키는 함수
    {
        //1. player 사용자의 입력에 따라
        float vertical = Input.GetAxis("Vertical");// a. 앞-뒤
        float horizontal = Input.GetAxis("Horizontal");// b 좌-우
        //Vector3 direction = new Vector3(horizontal, 0, vertical);//2. 사용자 입력에 따른 방향을 구한다 //3. 특정 방향으로 일정한 속력으로 움직인다.                                            
        //2-1. 사용자 입력에 따른 local축 방향을 구한다.(내가 보는 방향으로 움직임)
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;

        //[문제] 중력이 계속 증가 //[해결] 내가 땅에게 붙어있을 때 나에게 적용되는 중력을 reset
        /*if (cc.collisionFlags == CollisionFlags.Below) // 현재 충돌 상태 == 땅과 충돌한 상태
        {
            //중력값을 0으로 Reset
            yVelocity = 0;
        }*/
        if (cc.isGrounded == true) //더 세밀한 코드
        {
            yVelocity = 0f;
            isJump = false;
            jumping = jumpCount;
        }

        #region 점프 설명
        // 1. isJump가 상태를 체크합니다.
        // 1-1. (땅) isJump가 false면 점프가 가능한 상태가 됩니다. -> false인 상태에서 jump 버튼을 누르면 점프합니다.
        // 2. (점프) jump 버튼을 누른 순간 isjump가 true 상태가 됩니다. -> jump 버튼을 눌러도 더이상 점프가 되지 않습니다.
        // 3. (점프 -> 땅) jump 했다 다시 땅에 닿으면 isJump가 false 상태가 됩니다.
        #endregion

        // player가 jump 버튼 누르면 점프
       /* if (Input.GetButtonDown("Jump"))
        {
            if (isJump == false)
            {
                if (jumping > 0)
                {
                    //JumpPower만큼 솟구친다.
                    yVelocity = jumpPower;
                    jumping--;
                    isJump = false;
                } else if (jumping <= 0)
                {
                    isJump = true;
                }
            } 
            
        }*/

       if (jumping > 0 && Input.GetButtonDown("Jump"))
        {
            if (!isJump)
            {
                yVelocity = jumpPower;
                jumping--;
            }
            
        }


        //중력(F) 적용
        // 1. F(중력) = m(질량=1)*a(중력가속도=gravity)
        // 2. V(속도) = V0(중력누적값) + a(중력가속도)t
        yVelocity += gravity * Time.deltaTime;
        // 중력을 받도록 Player의 Y축 방향을 갱신한다.
        direction.y = yVelocity;

        //transform.position += direction * speed * Time.deltaTime; //(방향 * 속력)//p=p0+vt //충돌체크 X
        cc.Move(direction * speed * Time.deltaTime); //CharacterController를 통한 움직임 //충돌체크O
    }

    void RotateHorizontal()
    {
        //1. 사용자의 입력값 (마우스)
        // a. 좌-우 이동값
        float horizontal = Input.GetAxis("Mouse X");
        //2. 마우스 민감도에 따라 입력값이 변동
        float turnPlayer = horizontal * mouseSensitivity *Time.deltaTime;
        //3. 마우스 이동에 따른 좌-우 회전 누적.
        horizontalAngle += turnPlayer;
        //4. 누적된 회전 값을 player의 회전에 반영 (Y축 회전)
        transform.localEulerAngles = new Vector3(0, horizontalAngle, 0);
    }

    
    void RotateVertical()
    {
        // 1. 사용자의 입력값 (마우스) // a. 상-하 이동값 (-1 ~ +1)
        float vertical = Input.GetAxis("Mouse Y");
        // 2. 마우스 민감도에 따라 입력값이 변동
        float turnFace = vertical * mouseSensitivity *Time.deltaTime;
        // 3. 마우스 이동에 따른 상-하 회전값 누적
        verticalAngle += turnFace;

        //[문제] : 360도 회전 //[해결]: 상하 회전 각도 제한
        // VerticalAngle의 값이 + limitAngle보다 크면 -> verticalAngle == limitAngle 
        // VerticalAngle의 값이 - limitAngle보다 작다면 -> verticalAngle == -limitAngle
        verticalAngle = Mathf.Clamp(verticalAngle, -limitAngle, limitAngle);  
        /* mathf.clamp와 같은 식 
         if (verticalAngle > limitAngle)
        {
            verticalAngle = limitAngle;
        } else if (verticalAngle < -limitAngle)
        {
            verticalAngle =  -limitAngle;
        } */

        // 4. 누적된 회전 값을 Face(=Camera)의 회전에 반영 (X축 회전)
        face.transform.localEulerAngles = new Vector3(-verticalAngle, 0, 0);

        

    }
}
