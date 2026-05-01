using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Goaltime : MonoBehaviour // この行はコピーしてはいけない．元のスクリプトのままにする．
{
    [SerializeField] private TextMeshProUGUI TextTime;
    [SerializeField] private TextMeshProUGUI GoalMesseage;

    public float elapsedTime;
    private int f_Goal; // ゴールに到達した時に１となるフラグを追加する
                        // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0.0F;
        f_Goal = 0; // 停止フラグを初期化する
    }
    // Update is called once per frame
    void Update()
    {
        if (f_Goal == 0) // ゴールに到達していない場合だけ時間を加算する
        {
            elapsedTime += Time.deltaTime;
        }
        TextTime.text = string.Format("Time {0:f2} sec", elapsedTime);
    }
    // 衝突を判定する処理を追加する
    void OnCollisionEnter(Collision other) // 衝突を判定する関数を呼ぶ
    {
        if (other.gameObject.name == "Goal") // 衝突した物体が「ゴール」なら（※）
        {
            f_Goal = 1; // 衝突フラグを上げる
            GoalMesseage.text = string.Format("Goal!", elapsedTime);
        }
    }
}
