using UnityEngine;
using UnityEngine.Events;

public class InputHandler : SingletonMonoBehavior<InputHandler>
{
    public UnityEvent<Vector3> OnMove;
    public UnityEvent OnFire;

    private void Update()
    {
        //invoke movement here
    }
}
