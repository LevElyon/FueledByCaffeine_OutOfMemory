using UnityEngine;

public class DashPositions : MonoBehaviour
{
    public BossHandler boss;
    public Transform startPos;
    public Transform endPos;
    public Transform midPos;
    
    public Vector2 GetStartPos()
    {
        if (Vector2.Distance(boss.transform.position, startPos.position) <= Vector2.Distance(boss.transform.position, endPos.position))
        {
            return startPos.position;
        }
        return endPos.position;
    }
    public Vector2 GetEndPos()
    {
        if (Vector2.Distance(boss.transform.position, startPos.position) >= Vector2.Distance(boss.transform.position, endPos.position))
        {
            return startPos.position;
        }
        return endPos.position;
    }

    public Vector2 GetMidPos()
    {
        return midPos.position;
    }
}
