using Umeshu.Uf;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GameElementRigidbody : GameElementComponent<Rigidbody>
{
    private bool sleep = false;
    protected override void ActivateComponent(Rigidbody _component) => sleep = false;
    protected override void DesactivateComponent(Rigidbody _component) => sleep = true;
    protected override void ResetComponent(Rigidbody _component) => _component.ResetRigidbodyVelocity();

    private void FixedUpdate()
    {
        if (sleep) Component.Sleep();
    }
}