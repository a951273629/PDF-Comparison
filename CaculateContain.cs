using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTextSharpReadPDF
{
    class CaculateContain
    {
        //private MaxInfo current;
        public bool isExistSameString(string targetString, string mainString)
        {
            // 将主字符串转换为小写，以便进行不区分大小写的模糊查找
            string lowerMainString = mainString.ToLower().Trim();
            string lowerTargetString = targetString.ToLower().Trim();

            return lowerMainString.Contains(lowerTargetString);

        }
        public MaxInfo commonSameBest(string str1, string str2)
        {
            if (str1 == null || str2 == null || str2.Equals("") || str1.Equals("")) return new MaxInfo(0, 0, 0);
            int[][] dp = new int[str1.Length][];
            //对dp矩阵的第一列赋值
            for (int i = 0; i < str1.Length; i++)
            {
                dp[i] = new int[str2.Length];

                if (str2[0] == str1[i])
                    dp[i][0] = 1;
                else
                {
                    dp[i][0] = 0;
                }
            }
            //对dp矩阵的第一行赋值
            for (int j = 0; j < str2.Length; j++)
            {
                if (str1[0] == str2[j])
                    dp[0][j] = 1;
                else
                {
                    dp[0][j] = 0;
                }
            }
            for (int i = 1; i < str1.Length; i++)
                for (int j = 1; j < str2.Length; j++)
                {
                    if (str1[i] == str2[j])
                    {
                        dp[i][j] = dp[i - 1][j - 1] + 1;
                    }
                    else
                    {
                        dp[i][j] = 0;
                    }
                }
            //int max = dp[0][0];
            MaxInfo maxInfo = new MaxInfo(dp[0][0], 0, 0);
            for (int i = 0; i < str1.Length; i++)
                for (int j = 0; j < str2.Length; j++)
                {
                    //max = Math.Max(max, dp[i][j]);
                    maxInfo = MaxInfo.Max(maxInfo, new MaxInfo(dp[i][j], i, j));
                }
            //this.current = maxInfo;
            return maxInfo;
        }
        public string commonSameString(string com, MaxInfo info)
        {
            return com.Substring(info.j - info.max + 1, info.max);
        }
    }
    class MaxInfo
    {
        public int max;
        public int i;
        public int j;

        public MaxInfo()
        {
        }

        public MaxInfo(int max, int i, int j)
        {
            this.max = max;
            this.i = i;
            this.j = j;
        }

        public static MaxInfo Max(MaxInfo a, MaxInfo b)
        {
            if (a.max >= b.max)
            {
                return a;
            }
            return b;
        }

        public override string ToString()
        {
            return $"max:{max},i:{i},j:{j}\n";
        }
    }
}
