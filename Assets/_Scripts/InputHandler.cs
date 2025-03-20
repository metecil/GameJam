using UnityEngine;
using UnityEngine.Events;

public class InputHandler : SingletonMonoBehavior<InputHandler>
{
    public UnityEvent<Vector3> OnMove;

    private void Update()
    {
        //invoke movement here
    }
}
