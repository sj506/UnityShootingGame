using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TransformEffect : MonoBehaviour
{
    public static IEnumerator OnMove(Transform target, Vector3 start, Vector3 end, float moveTime = 1f, UnityAction action = null)
    {
        if (target == null) yield break;

        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime / moveTime;
            target.position = Vector3.Lerp(start, end, percent);
            yield return null;
        }

        if (action != null) action.Invoke();
    }

    public static IEnumerator OnRotate(Transform target, Vector3 start, Vector3 end, float rotateTime = 1f, UnityAction action = null)
    {
        if (target == null) yield break;

        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime / rotateTime;
            target.rotation = Quaternion.Euler(Vector3.Lerp(start, end, percent));
            yield return null;
        }

        if (action != null) action.Invoke();
    }

        public static IEnumerator OnScale(Transform target, Vector3 start, Vector3 end, float scaleTime = 1f, UnityAction action = null)
    {
        if (target == null) yield break;

        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime / scaleTime;
            target.localScale = Vector3.Lerp(start, end, percent);
            yield return null;
        }

        if (action != null) action.Invoke();
    }
}
