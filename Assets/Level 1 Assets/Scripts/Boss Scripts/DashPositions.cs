using UnityEngine;

public class DashPositions : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    
    public Transform GetStartPos()
    {
        return startPos;
    }
    public Transform GetEndPos()
    {
        return endPos;
    }
}
