using UnityEngine;
using UnityEditor;

public interface IInteractuableObject
{
    void Interact();
    Vector2 GetPos();
}