using UnityEngine;
using System.Collections;

public class FallableObject
{
    public static IEnumerator IFallAnimation(Vector3 fallPosition, GameObject obj, float fallFrames)
    {
        Vector3 fallStartPosition = obj.transform.position;
        obj.transform.Find("FeetCollider").gameObject.SetActive(false);

        int i = 0;

        while (i < fallFrames)
        {
            i++;
            obj.transform.position = Vector3.Lerp(fallStartPosition, fallPosition, i * 2 / fallFrames);
            obj.transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, i / fallFrames);
            yield return new WaitForEndOfFrame();
        }

        obj.SendMessage("EndFall");
    }
}
