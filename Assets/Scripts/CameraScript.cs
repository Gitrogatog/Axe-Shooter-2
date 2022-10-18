using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject subject;
    bool isShaking = false;
    public static CameraScript instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if (subject == null)
        {
            subject = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isShaking && subject != null){
            transform.position = new Vector3(subject.transform.position.x, subject.transform.position.y, -10);
        }
        
    }

    public void Shake(float duration, float magnitude){
        StartCoroutine(ShakeCamera(duration, magnitude));
    }

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        isShaking = true;
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;
        while(elapsed < duration && subject != null)
        {
            float x = Random.Range(-1f, 1f) * magnitude * 0.1f;
            float y = Random.Range(-1f, 1f) * magnitude * 0.1f;
            transform.localPosition = new Vector3(subject.transform.position.x+x, subject.transform.position.y+y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isShaking = false;
        if(subject != null){
            transform.localPosition = subject.transform.position;
        }
        else{
            transform.localPosition = originalPos;
        }
        
    }
}
