using System;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class CollisionDetector : MonoBehaviour
{
    // OnTriggerStayイベント時に実行したい関数を登録する変数（引数にColliderを取る）
    [SerializeField]
    private UnityEvent<Collider> onTriggerStayEvent = new UnityEvent<Collider>();
    //  OnTriggerExitイベント時に実行したい関数を登録する変数（引数にColliderを取る）
    [SerializeField]
    private UnityEvent<Collider> onTriggerExitEvent = new UnityEvent<Collider>();
    /// <summary>
    /// Is TriggerがONで他のGameObjectがCollider内にいるときに呼ばれ続ける
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // InspectorタブのonTriggerStayEventで指定された処理を実行する
        onTriggerStayEvent.Invoke(other);
    }
    /// <summary>
    /// Is TriggerがONで他のGameObjectがColliderから出たときに呼ばれる
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        // InspectorタブのonTriggerExitEventで指定された処理を実行する
        onTriggerExitEvent.Invoke(other);
    }
}
