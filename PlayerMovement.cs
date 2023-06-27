using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할 : Player의 움직임을 전반적으로 관리한다.
//  A. Player를 사용자의 입력에 따라 앞/뒤/좌/우로 일정한 속력으로 움직인다.
//    - 사용자의 입력 (a.앞-뒤 / b.좌-우)
//    - 일정한 속력
//  A-1. 중력 적용을 받습니다.
//    - CharacterController 컴포넌트
//    - 중력 가속도
//    - 중력 누적값 (*중력가속도를 통해 나에게 누적된 중력값)
//  B. Player를 사용자의 입력에 따라 좌/우로 회전시킨다.
//    - 사용자의 입력 (마우스 좌-우)
//    - 회전 속력 (민감도)
//    - 사용자의 입력에 따른 회전 각도를 저장
//  C. Player를 사용자의 입력에 따라 일정한 속력으로 상/하로 회전시킨다.
//    - 사용자의 입력 (마우스 상-하)
//    - 회전 속력 (B에서 만든 걸 재사용)
//    - 사용자의 입력에 따른 회전 각도를 저장
//    - 고개를 까딱거리는.. 카메라 회전
//    - 상-하 회전 제한 각도
//  D. 사용자의 입력에 따라 Player를 "n단" 점프하게 합니다.
//    - 사용자의 입력(Jump)
//    - Jump 의 파워
//    - 최대 점프 가능 횟수
//    - 현재 점프 가능 횟수

public class PlayerMovement : MonoBehaviour
{
    #region Movement & Gravity
    // 이동 속력
    public float moveSpeed = 5f;
    // CharacterController 컴포넌트
    private CharacterController cc;
    // 중력 가속도 ( 9.81m/s )
    public float gravity = -9.81f;
    // 중력 누적값 (*중력가속도를 통해 나에게 누적된 중력값)
    public float yVelocity;
    #endregion

    #region Jump
    // Jump 의 파워
    public float jumpPower = 4.0f;
    // 지금 Jump를 했는지 여부
    public bool isJump = false;
    // 최대 점프 가능 횟수
    public int maxJumpCount = 1;
    // 현재 점프 가능 횟수
    private int jumpCount = 0;
    #endregion

    #region Rotator
    // 회전 속력 (민감도)
    public float mouseSensitivity = 500f;
    // 사용자의 입력에 따른 좌-우 회전 각도를 저장
    private float horizontalAngle;
    // 사용자의 입력에 따른 상-하 회전 각도를 저장
    private float verticalAngle;
    // 고개를 까딱거리는.. 카메라 회전
    public GameObject face;
    // 상-하 회전 제한 각도 (초기값=90도)  -90 ~ +90
    public float limitAngle = 90f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // 마우스 커서를 보이지 않게하고,
        Cursor.visible = false;
        // 마우스 커서를 화면 가운데에 고정시킨다.
        Cursor.lockState = CursorLockMode.Locked;
        // ChacterController 컴포넌트 가져온다 (=초기화)
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

        RotateHorizontal();

        RotateVertical();
    }

    // Player를 이동시키는 함수
    // [문제!] : Player의 로컬 Z축이 아닌, Unity의 절대 Z축으로 이동한다
    // [해결]  : Player의 Local 방향으로 이동한다.
    private void MovePlayer()
    {
        // 1. Player를 사용자의 입력에 따라
        //    a.앞-뒤
        float vertical = Input.GetAxis("Vertical");
        //    b.좌-우로
        float horizontal = Input.GetAxis("Horizontal");
        // 2. 사용자 입력에 따른 World축 방향을 구한다.
        // Vector3 direction = new Vector3(horizontal, 0, vertical);
        // 2-1. 사용자 입력에 따른 Local축 방향을 구한다.
        // - Local 앞뒤 방향 * vetical / local 좌우 방향 * horizontal
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        // 3. 특정 방향으로 일정한 속력으로 움직인다.
        //    P = Po + Vt
        // transform.position += direction * moveSpeed * Time.deltaTime;

        // [문제] 중력이 나에게 계속 찍어눌러서 힘이 점점 커져간다.
        // [해결] 만일 내가 땅에 붙어있을 때, 나에게 적용되는 중력을 Reset
        // if (cc.collisionFlags == CollisionFlags.Below)
        if (cc.isGrounded == true)
        {
            // 중력값을 0으로 Reset
            yVelocity = 0f;
            // 땅에 닿아있으면 Jump가 maxJumpCount 회수만큼 가능한 상태
            jumpCount = maxJumpCount;
        }

        // 만일, 점프가능 횟수가 남아있는 상태에서 Player가 Jump 버튼을 누르면, 
        if (jumpCount > 0 && Input.GetButtonDown("Jump"))
        {
            // JumpPower만큼 솟구친다.
            yVelocity = jumpPower;
            // 점프를 하면.. 점프 가능 횟수를 -1 차감.
            jumpCount--;
        }




        // 중력(F) 적용
        // 1. F(중력) = m(질량=1)*a(중력가속도=gravity)
        // 2. V(속도) = Vo(중력누적값) + a(중력가속도) * t
        yVelocity += gravity * Time.deltaTime;
        // 중력을 받도록 Player의 Y축 방향을 갱신한다.
        direction.y = yVelocity;


        // * CharacterController를 통해 움직인다.
        cc.Move(direction * moveSpeed * Time.deltaTime);
    }

    //  B. Player를 사용자의 입력에 따라 좌/우로 회전시킨다.
    private void RotateHorizontal()
    {
        // 1. 사용자의 입력값 (마우스)
        //  a. 좌-우 이동값
        float horizontal = Input.GetAxis("Mouse X");
        // 2. 마우스 민감도에 따라 입력 값이 변동
        float turnPlayer = horizontal * mouseSensitivity * Time.deltaTime;
        // 3. * 마우스 이동에 따른 좌-우 회전값 누적
        horizontalAngle += turnPlayer;
        // 4. 누적된 회전 값을 Player의 회전에 반영
        transform.localEulerAngles = new Vector3(0, horizontalAngle, 0);
    }

    // C. Player 시점을 상-하로 회전시키는 함수
    // [문제] : 상하회전은 잘 되는데.. 360도 계속 회전해버린다.
    // [해결] : 상하 회전 각도 제한을 통해 해결한다.
    private void RotateVertical()
    {
        // 1. 사용자의 입력값을 가져온다. (상-하) ( -1 ~ +1 )
        float vertical = Input.GetAxis("Mouse Y");
        // 2. 마우스 민감도에 따라 입력 값이 변동
        float turnFace = vertical * mouseSensitivity * Time.deltaTime;
        // 3. * 마우스 이동에 따른 상-하 회전값 누적
        verticalAngle += turnFace;
        // * 상하 회전 각도 제한을 통해 해결한다.
        verticalAngle = Mathf.Clamp(verticalAngle, -limitAngle, limitAngle);
        // 4. 누적된 회전 값을 Face(=Camera)의 회전에 반영
        face.transform.localEulerAngles = new Vector3(-verticalAngle, 0, 0);
    }
}
