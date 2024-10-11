using Umeshu.Uf;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GameElementRigidbody2D : GameElementComponent<Rigidbody2D>
{
    protected override void ActivateComponent(Rigidbody2D _component) => _component.simulated = true;
    protected override void DesactivateComponent(Rigidbody2D _component) => _component.simulated = false;
    protected override void ResetComponent(Rigidbody2D _component) => _component.ResetRigidbodyVelocity();
}