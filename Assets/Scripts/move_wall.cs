using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_wall : MonoBehaviour
{
    Transform myTransform; // transform 情報を格納する変数
    Vector3 position_start; // 物体の初期位置を格納する変数
    Vector3 position_now; // 物体の現在位置を格納する変数
    int f_inverse = 1;
    float move_speed = 0.05f;
                          // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform; // 物体の transform 情報をリンクする
        position_start = myTransform.position; // 初期位置を取り出す
        position_now = position_start; // 最初は同じ場所
    }
    // Update is called once per frame
    void Update()
    {
        position_now.z -= move_speed * f_inverse; // １ステップごとに Z を-0.01 する
        if (position_start.z - position_now.z > 10 || position_now.z > position_start.z) // Z 方向に 10 移動したら，
        {
            f_inverse = f_inverse * -1;//移動する向きを反対にする。
            position_now.z -= move_speed * f_inverse;
            Invoke("Wait",2);
            move_speed = 0; // 無理やり移動速度を０にするｗ
        }
        myTransform.position = position_now;
    }

    void Wait()
    {
        move_speed = 0.05f;//移動速度をここで戻す。
    }

}
