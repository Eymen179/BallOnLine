using UnityEngine;

public class TutorialRigidbodyLine : MonoBehaviour
{
    public Transform lineTransform;
    private Vector2 lineStartPos;
    private Rigidbody2D rb;
    private Quaternion startRot;
    private LineRenderer lr;

    public float distance = 9f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        startRot = transform.rotation;
        if (lineTransform != null)
        {
            lineStartPos = lineTransform.position;
        }
        else
        {
            lineStartPos = transform.position;
        }

        CalculateAndSetCenterOfMass();
    }

    private void CalculateAndSetCenterOfMass()
    {
        if (rb == null || lr == null || lr.positionCount == 0) return;

        Vector2 centroid = Vector2.zero;
        int count = lr.positionCount;

        for (int i = 0; i < count; i++)
        {
            centroid += (Vector2)lr.GetPosition(i);
        }
        centroid /= count;

        // Fizik motorunun aðýrlýðý o aþaðýdaki boþ pivot yerine çizginin merkezinde hesaplamasýný saðlar:
        rb.centerOfMass = centroid;
    }
    private void Update()
    {
        if (lineTransform != null)
        {
            if (Vector2.Distance(transform.position, lineStartPos) > distance)
            {
                lineTransform.position = lineStartPos;
                transform.rotation = startRot;
                // Rigidbody üzerindeki birikmiþ hýz ve açýsal ivmeyi sýfýrla
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }
        }
    }
}
