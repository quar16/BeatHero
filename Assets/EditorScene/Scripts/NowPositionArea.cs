using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NowPositionArea : MonoBehaviour, IDragHandler
{
    public GameObject Scores;
    public float scoreNumber;
    public float scale;
    public float start;
    public float end;
    public float gauge;
    public GameObject musicDial;
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float f = Input.mousePosition.x - 880;
        f = Mathf.Clamp(f, start, end);
        transform.localPosition = new Vector3(f, 0, 0);
    }
    public bool isTracking = false;
    private void Update()
    {
        if (isTracking)
            transform.localPosition = new Vector3(Mathf.Clamp(musicDial.transform.localPosition.x + scale, start, end), 0, 0);
        Scores.transform.localPosition = new Vector3(-810 + (-500 * (scoreNumber) + 1920) * ((transform.localPosition.x - start) / gauge), 0, 0);
    }
}
