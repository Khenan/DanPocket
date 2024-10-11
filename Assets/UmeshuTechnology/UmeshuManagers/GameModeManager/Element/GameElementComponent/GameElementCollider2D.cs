using Umeshu.Uf;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GameElementCollider2D : GameElementComponent<Collider2D>
{
    protected override void ActivateComponent(Collider2D _component) => _component.enabled = true;
    protected override void DesactivateComponent(Collider2D _component) => _component.enabled = false;
    protected override void ResetComponent(Collider2D _component) { }
}