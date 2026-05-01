using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class contact : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Count_wallmessage;
    private int count_wall;
    // Start is called before the first frame update
    void Start()
    {
        count_wall = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 衝突を判定する処理を追加する
    void OnCollisionEnter(Collision other) // 衝突を判定する関数を呼ぶ
    {
        if (other.transform.parent.gameObject.name == "内壁" || other.transform.parent.gameObject.name == "外壁") // 衝突した物体が「ゴール」なら（※）
        {
            count_wall += 1; // 衝突フラグを上げる
            Count_wallmessage.text = string.Format("Hit Wall:{0}", count_wall);
        }
    }
}
