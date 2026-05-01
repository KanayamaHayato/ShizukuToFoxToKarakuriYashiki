using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_wall : MonoBehaviour
{
    Transform myTransform; // •¨‘Ì‚Ì transform î•ñ‚ðŠi”[‚·‚é•Ï”
    Vector3 origin = new Vector3(5f, 1f, -5f); // ‰ñ“]’†S
    Vector3 axis = new Vector3(0f, 1f, 0f); // ‰ñ“]Ž²iY Ž²j
    float rotate_speed = 3f;
    int f_inverse = 1;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform; // transform î•ñ‚ðŽæ“¾
    }
    // Update is called once per frame
    void Update()
    {
        myTransform.RotateAround(origin, axis, rotate_speed * f_inverse);
        if ((int)myTransform.eulerAngles.y > 90 || (int)myTransform.eulerAngles.y < 0)
        {
            f_inverse = f_inverse * -1;//ˆÚ“®‚·‚éŒü‚«‚ð”½‘Î‚É‚·‚éB
            myTransform.RotateAround(origin, axis, rotate_speed * f_inverse); // origin ‚ð’†S‚É axis Žü‚è‚Érotate_speed“x‰ñ“]‚·‚é
            Invoke("Wait", 2);
            rotate_speed = 0;
        }
        
    }

    void Wait()
    {
        rotate_speed = 3f;//ˆÚ“®‘¬“x‚ð‚±‚±‚Å–ß‚·B
    }

}
