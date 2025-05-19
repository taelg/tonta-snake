using UnityEngine;

public class VFXManager : MonoBehaviour
{

    [SerializeField] private Transform pointsLabel;
    [SerializeField] private PointVFXPool pointFXPool;

    private Vector2 pointsLabelPos;

    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        AwakeSingleton();
        pointsLabelPos = pointsLabel.position;
    }

    private void AwakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
            Destroy(this.gameObject);
    }

    public void AnimatePoint(Vector3 pointStartPos)
    {
        PointVFXBehavior pointFX = pointFXPool.GetNext();
        pointFX.transform.position = pointStartPos;
        pointFX.gameObject.SetActive(true);
        pointFX.AnimatePoint(pointsLabelPos);
    }
}
