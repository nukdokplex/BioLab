using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioLab.Utils
{
    public class Utils
    {
        public static bool IsStringsInLevensteinDistance(string s1, string s2, int distance)
        {
            int currentDistance = 0;
            
            if (s1 == s2)
            {
                return true;
            }

            if (s1.Length > s2.Length)
            {
                s2 = s2 + MultiplyStrings(" ", s1.Length - s2.Length);
            }

            if (s2.Length > s1.Length)
            {
                s1 = s1 + MultiplyStrings(" ", s2.Length - s1.Length);
            }

            for (int i = 1; i <= s1.Length - 1; i++)
            {
                if (s1[i] != s2[i])
                {
                    distance++;
                }
            }

            if (distance >= currentDistance)
            {
                return true;
            }
            return false;
        }

        public static string MultiplyStrings(string s, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i<= count - 1; i++)
            {
                stringBuilder.Append(s);
            }
            return stringBuilder.ToString();
        }
    }
}
