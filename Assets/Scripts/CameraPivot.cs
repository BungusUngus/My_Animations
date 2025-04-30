using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public float sensitivity = 120;
    public float minClamp = -50, maxClamp = 50;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate our pivot point globally on y rotation, and locally on x rotation. Stops any rolling (z) rotations.
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime,0, Space.World);
        transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime, 0, 0, Space.Self);
        
        //Clamp the current x rotation to min-->max clamp.
        //Ensure rotation is set to a -180,180 range instead of 0,360
        float xRot = transform.eulerAngles.x;
        if (xRot > 180) xRot -= 360;
        xRot = Mathf.Clamp(xRot, minClamp, maxClamp);
        transform.eulerAngles = new Vector3(xRot, transform.eulerAngles.y, 0);
    }
}
