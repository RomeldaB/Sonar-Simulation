using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // Manually defined coefficient values based in the reflective properties of the surface
    static float getCoefficient(string name)
    {
        if (name.Contains("Rock"))
            return 1;
        else if (name.Contains("Sand"))
            return 0.5f;
        return -1;
    }

    // Backscatter calculation using the angle between ray and normal and the above coefficient
    public static float calculateBackscatter (string name, Vector3 direction, Vector3 normal) {
        float cos = Mathf.Cos(DegreeToRadian(180 - Vector3.Angle(direction, normal)));
        float coeff =  getCoefficient(name);
        return coeff * cos;
    }

    public static float DegreeToRadian(float angle)
    {
        return Mathf.PI / 180 * angle;
    }

}
