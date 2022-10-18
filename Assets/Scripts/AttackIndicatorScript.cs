using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicatorScript : MonoBehaviour
{
    public float startScale;
    public float endScale;
    float scaleTime;

    public void SetCharacteristics(float lifetime){
        scaleTime = lifetime;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        StartCoroutine(Shrink());
    }

    IEnumerator Shrink(){
        float shrinkSpeed = 1 / scaleTime;
        float percent = 0;
        while(percent <= 1){
            percent += Time.deltaTime * shrinkSpeed;
            float currentScale = startScale - (startScale - endScale) * percent;
            transform.localScale = new Vector3(currentScale, currentScale, 1);
            yield return null;
        }
        Destroy(gameObject);
    }
}
