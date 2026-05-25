using System.Collections.Generic;
using UnityEngine;

public class LinkValidator
{
    public static bool IsLinkValid(
        GameObject turretObj,
        GameObject linkedObj,
        List<List<GameObject>> rays)
    {
        int rayA = -1, indexA = -1;
        int rayB = -1, indexB = -1;

        for (int r = 0; r < rays.Count; r++)
        {
            for (int i = 0; i < rays[r].Count; i++)
            {
                if (rays[r][i] == turretObj) { rayA = r; indexA = i; }
                if (rays[r][i] == linkedObj) { rayB = r; indexB = i; }
            }
        }

        if (rayA == -1 || rayB == -1) return false;

        bool yanYana = rayA != rayB && indexA == indexB;
        bool arkaArkaya = rayA == rayB && Mathf.Abs(indexA - indexB) == 1;

        return yanYana || arkaArkaya;
    }
}