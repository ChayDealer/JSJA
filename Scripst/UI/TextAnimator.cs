using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    private List<Animator> _animators;

    public float waitBetween = 0.2f;
    public float waitInEnd = 1.0f;


    private void OnEnable()
    {
        _animators = new List<Animator>(gameObject.GetComponentsInChildren<Animator>());
        StartCoroutine(DoTextAnimation());

        if (_animators != null)
        {
            StartCoroutine(DoTextAnimation());
        }
    }

    private void OnDisable()
    {
        StopCoroutine(DoTextAnimation());
        RectTransform rt;
        foreach (var animator in _animators)
        {
            rt = animator.gameObject.GetComponent<RectTransform>();
            rt.localPosition = new Vector2(rt.localPosition.x, 0);
        }
    }

    IEnumerator DoTextAnimation()
    {
        while (true)
        {
            foreach (var animator in _animators)
            {
                animator.SetTrigger("DoLetterAnimation");
                yield return new WaitForSeconds(waitBetween);
            }

            yield return new WaitForSeconds(waitInEnd);
        }
    }
}
