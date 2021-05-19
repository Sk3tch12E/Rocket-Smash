using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngle = 89f;

    private float vertical;
    private float horizontal;


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void start()
    {
        vertical = transform.localEulerAngles.x;
        horizontal = transform.localEulerAngles.y;        
        //ToggleCursor();
    }

    private void Update()
    {
        Look();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Destroy(UIManager.instance.gameObject);
            UIManager.instance.LoadScene(0);
        }
        //#if UNITY_EDITOR
        //        if (Input.GetKeyDown(KeyCode.Escape))
        //        {
        //            ToggleCursor();
        //        }
        //        if (!Cursor.visible)
        //        {
        //            Look();
        //        }
        //#else
        //        Cursor.lockState = CursorLockMode.Locked;
        //        Cursor.visible
        //        Look();
        //#endif
        //toggle cursor onload
        //look on update
        //if esc then unlock cursor, disconnect, load to menu
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float mouseVert = -Input.GetAxis("Mouse Y"); //- inverts to mouse up is look up now
        float mouseHoriz = Input.GetAxis("Mouse X");

        vertical += mouseVert * sensitivity * Time.deltaTime;
        horizontal += mouseHoriz * sensitivity * Time.deltaTime;

        //clamp the angle the player can look at to stop backflips
        vertical = Mathf.Clamp(vertical, -clampAngle, clampAngle);

        //rotate player around but not up
        transform.localRotation = Quaternion.Euler(vertical, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontal, 0f);
    }

    private void ToggleCursor()
    {
        Cursor.visible = !Cursor.visible;
        if(Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
