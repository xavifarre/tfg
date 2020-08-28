using UnityEngine;
using System.Collections;

public class FallableObject
{
    public static IEnumerator IFallAnimation(Vector3 fallPosition, GameObject obj, float fallTime)
    {
        obj.layer = LayerMask.NameToLayer("IgnoreAll");

        Vector3 fallStartPosition = obj.transform.position;
        fallPosition = fallPosition + Vector3.up * -5;
        obj.transform.Find("FeetCollider").gameObject.SetActive(false);
        string layer = obj.GetComponent<SpriteRenderer>().sortingLayerName;
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        float i = 0;

        while (i < fallTime)
        {
            i+=Time.deltaTime;
            obj.transform.position = Vector3.Lerp(fallStartPosition, fallPosition, i / fallTime);
            obj.transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, i / fallTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        obj.GetComponent<SpriteRenderer>().sortingLayerName = layer;
        obj.GetComponent<IFallableObject>().EndFall();
    }
}
