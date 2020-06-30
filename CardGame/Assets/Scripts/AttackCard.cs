using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;        // 引用 UI 額外功能 API
using System.Collections;

public class AttackCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private UILineRenderer line;     // UI 線條渲染
    private Transform arrow;         // 箭頭

    private Vector2 posBegin, posDrag; // 開始拖拉位置, 拖拉中的位置

    private static bool canAttack;     // 可以攻擊
    private static Transform parent;   // 被攻擊方父物件
    private static Transform target;   // 被攻擊方

    private void Awake()
    {
        line = GameObject.Find("線條").GetComponent<UILineRenderer>();                  // 取得 線條
        arrow = GameObject.Find("箭頭").transform;                                      // 取得 箭頭
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        line.enabled = true;

        // 介面.x = 螢幕.x - 螢幕.高度 / 2
        posBegin.x = eventData.position.x - Screen.width / 2;
        // 介面.y = 螢幕.y - 螢幕.寬度 / 2
        posBegin.y = eventData.position.y - Screen.height / 2;

        line.Points[0] = posBegin;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 介面.x = 螢幕.x - 螢幕.高度 / 2
        posDrag.x = eventData.position.x - Screen.width / 2;
        // 介面.y = 螢幕.y - 螢幕.寬度 / 2
        posDrag.y = eventData.position.y - Screen.height / 2;

        line.Points[1] = posDrag;

        line.Resoloution = (posDrag - posBegin).magnitude / 100;        // 解析度 = 拖拉點 - 起始點 / 100

        arrow.GetComponent<RectTransform>().anchoredPosition = posDrag; // 箭頭.座標 = 拖拉中的位置

        arrow.up = posDrag - posBegin;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        line.enabled = false;
        arrow.position = Vector2.one * 1000;

        if (canAttack && transform.parent != parent)
        {
            StartCoroutine(Attack());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canAttack = true;
        parent = transform.parent;
        target = transform;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canAttack = false;
        parent = null;
        target = null;
    }

    private IEnumerator Attack()
    {
        transform.parent.SetAsLastSibling();    // 設為最後一個子物件

        Vector3 pos = transform.position;       // 原始位置

        while (transform.position.y != target.position.y)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, 0.7f * Time.deltaTime * 5);
            yield return null;
        }

        transform.position = pos;
    }
}
