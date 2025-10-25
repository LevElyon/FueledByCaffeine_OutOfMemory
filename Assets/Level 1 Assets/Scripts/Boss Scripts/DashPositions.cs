using UnityEngine;

public class DashPositions : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    
    public Vector2 GetStartPos()
    {
        return startPos.position;
    }
    public Vector2 GetEndPos()
    {
        return endPos.position;
    }
}
