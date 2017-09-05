using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    class DataUtil
    {
        public static string bytesToHexString(byte[] src, int start, int len)
        {
            /*
            StringBuilder stringBuilder = new StringBuilder("");
            if (src == null || src.Length <= 0 || src.Length < len)
            {
                return null;
            }
            for (int i = start; i < len; i++)
            {
                int v = src[i] & 0xFF;
                String hv = Inte.toHexString(v);
                if (hv.length() < 2)
                {
                    stringBuilder.append(0);
                }
                stringBuilder.append(hv.toUpperCase());
            }
            return stringBuilder.toString();
            */
            return null;
        }

        /***
         * 电子秤重量数据转double
         */
        public static double byteWeightDataToDouble(byte[] data)
        {
            int length = 0;
            if (data == null || (length = data.Length) == 0)
            {
                Console.WriteLine("重量数据不对,数据为空");
                return 0.0;
            }
            //32 32 32 57 48 46 52 13   90.4(57 48 46 52)
            if (data[length - 1] != 13)
            {
                Console.WriteLine("重量数据不对");
                return 0.0;
            }
            
            int index = 0;
            for(int i = 0; i < length - 1; i++)
            {
                if (data[i] != 32)
                {
                    index = i;
                    break;
                }
            }
            double result = 0.0;

            result = Convert.ToDouble(Encoding.ASCII.GetString(data,index, length - index - 1));

            return result;
        }
        /**
         * 重量原始byte[]数据以逗号隔开保存
         * 
         * **/
        public static string byteWeightDataToStringSplit(byte[] data)
        {
            return byteWeightDataToStringSplit(data,",");
        }

        public static string byteWeightDataToStringSplit(byte[] data,string split)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i]);
                if (i != data.Length - 1)
                {
                    builder.Append(split);
                }
            }

            return builder.ToString();
        }
    }
}
