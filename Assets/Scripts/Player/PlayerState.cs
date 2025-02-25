using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // ============================================
    // 키
    // ============================================
    public float vAxis { get; private set; }
    public float hAxis { get; private set; }
    public bool keyJump { get; private set; }
    public bool keySlide { get; private set; }
    public bool keyAltCamera { get; private set; }

    // ============================================
    // 상태
    // ============================================
    public bool     isMoving        { get; private set; }
    public bool     isRunning       { get; private set; }
    public bool     isJumping       { get; private set; }
    public bool     isSliding       { get; private set; }
    public bool     isClimbing      { get; private set; }
    public bool     isFalling       { get; private set; }
    public bool     isAccelFalling  { get; private set; }
    public bool     isFallingImpact { get; private set; }
    public bool     isParkouring    { get; private set; }


    public void SetState(string stateName, bool value)
    {
        if (stateName == "isFalling" && isParkouring) return; // 파쿠르 중이면 낙하 상태 X


        switch (stateName)
        {
            case "isMoving":
                isMoving = value;
                break;
            case "isRunning":
                isRunning = value;
                break;
            case "isJumping":
                isJumping = value;
                break;
            case "isSliding":
                isSliding = value;
                break;
            case "isClimbing":
                isClimbing = value;
                break;
            case "isFalling":
                isFalling = value;
                break;
            case "isAccelFalling":
                isAccelFalling = value;
                break;
            case "isFallingImpact":
                isFallingImpact = value;
                break;
            case "isParkouring":
                isParkouring = value;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        isMoving = Mathf.Abs(hAxis) > 0.1f || Mathf.Abs(vAxis) > 0.1f;
        isRunning = Input.GetButton("Run");
        keyJump = Input.GetButtonDown("Jump");
        keySlide = Input.GetButton("Slide");
        keyAltCamera = Input.GetKey(KeyCode.LeftAlt);
    }

}



