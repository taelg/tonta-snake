using UnityEngine;


public class ReturnToPoolBehavior : MonoBehaviour
{
    public System.Action<Component> returnCallback;

    private void OnDisable()
    {
        if (gameObject.scene.isLoaded)
            returnCallback?.Invoke(this.GetComponent<Component>());
    }
}
