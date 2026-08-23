using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Shape : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public enum Type
    {
        square,
        circle,
        triangle,
        pentagon
    }

    public Type type;
    public long itemId;
    public Sprite square;
    public Sprite circle;
    public Sprite triangle;
    public Sprite pentagon;
    public void Start()
    {
        if (type == Type.square) GetComponent<Image>().sprite = square;
        if (type == Type.circle) GetComponent<Image>().sprite = circle;
        if (type == Type.triangle) GetComponent<Image>().sprite = triangle;
        if (type == Type.pentagon) GetComponent<Image>().sprite = pentagon;
    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        CheckIfInHole(data.position);
    }

    private void CheckIfInHole(Vector2 mousePos)
    {
        for (int i = 0; i < GameManager.I.holeParent.childCount; i++)
        {
            Hole hole = GameManager.I.holeParent.GetChild(i).GetComponent<Hole>();

            RectTransform rt = hole.GetComponent<RectTransform>();
            Vector2 localMousePos = rt.InverseTransformPoint(mousePos);
            if (rt.rect.Contains(localMousePos))
            {
                if (type == hole.type) //shape matches hole
                {
                    if (GameManager.I.UnlockedType(hole.type)) //hole is unlocked
                    {
                        GameManager.I.SendCheck(itemId);
                        Destroy(gameObject);
                        GameManager.I.PlayGoodSound();
                    }
                }
                else
                {
                    GameManager.I.PlayBadSound();
                }
            }
        }
    }
}
