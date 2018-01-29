using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace The5
{
    namespace Math
    {
        public static class Trigonometry
        {
            /** Law of cosines
             * for general tirangle with lengths a b c with angles A° B° C°
             * c²= a² + b² - 2AB*cos(C°) 
             * C° = acos( (a² + b² - c²)/(2*a*b)) 
             */

            public static float angleOppositeC(float a, float b, float c)
            {
                //Debug.Log(a + " " + b + " " + c);
                float cAng = (a * a + b * b - c * c) / (2 * a * b);
                //Debug.Log("cAng: " + cAng);
                //Debug.Log(Mathf.Acos(cAng));
                return Mathf.Acos(cAng) * Mathf.Rad2Deg;                
            }

            public static float angleOppositeC_sqr(float a, float b, float c_sqr)
            {
                float cAng = (a * a + b * b - c_sqr) / (2 * a * b);
                return Mathf.Acos(cAng) * Mathf.Rad2Deg;
            }

        }




    }
}

