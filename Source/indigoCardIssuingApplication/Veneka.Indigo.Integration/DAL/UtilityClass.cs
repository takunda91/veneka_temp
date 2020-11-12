using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.DAL
{
    public class UtilityClass
    {
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string DisplayPartialPAN(string clearPAN)
        {
            //if (clearPAN != null && clearPAN.Length > 10)
            //{
            //    return clearPAN.Substring(0, 6) + "*".PadRight(clearPAN.Length - 10, '*') +
            //           clearPAN.Substring(clearPAN.Length - 4, 4);
            //}


            //rather format the card for ecobank impementation
            if (clearPAN != null && clearPAN.Length > 10)
            {
                return clearPAN.Substring(0, 6) + "-" + clearPAN.Substring(6, clearPAN.Length - 10) + "-" +
                       clearPAN.Substring(clearPAN.Length - 4, 4);
            }

            return clearPAN;
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        public static DataTable CreateKeyValueTable<K, V>(Dictionary<K, V> dictionary)
        {
            string key = "key";
            string value = "value";

            DataTable dt = new DataTable();
            dt.Columns.Add(key, typeof(long));
            dt.Columns.Add(value, typeof(string));

            foreach (var item in dictionary)
            {
                dt.Rows.Add(CreateRow<K, V>(item.Key, item.Value, dt.NewRow()));
            }

            return dt;
        }

        public static DataTable CreateKeyValueTable<V>(List<V> list)
        {
            string key = "key";
            string value = "value";

            DataTable dt = new DataTable();
            dt.Columns.Add(key, typeof(long));
            dt.Columns.Add(value, typeof(string));

            int i = 0;
            foreach (var item in list)
            {
                dt.Rows.Add(CreateRow<int, V>(i, item, dt.NewRow()));
                i++;
            }

            return dt;
        }

        public static DataTable CreateBiKeyBinaryValueTable2(List<Tuple<long, long, byte[]>> list)
        {
            string key1 = "key1";
            string key2 = "key2";
            string value = "value";

            DataTable dt = new DataTable();
            dt.Columns.Add(key1, typeof(long));
            dt.Columns.Add(key2, typeof(long));
            dt.Columns.Add(value, typeof(byte[]));

            foreach (var item in list)
            {
                dt.Rows.Add(CreateRow<long, long, byte[]>(item.Item1, item.Item2, item.Item3, dt.NewRow()));
            }

            return dt;
        }

        public static DataTable CreateKeyValueTable2<V>(Dictionary<long, V> list)
        {
            string key = "key";
            string value = "value";

            DataTable dt = new DataTable();
            dt.Columns.Add(key, typeof(long));
            dt.Columns.Add(value, typeof(V));

            foreach (var item in list)
            {
                dt.Rows.Add(CreateRow<long, V>(item.Key, item.Value, dt.NewRow()));
            }

            return dt;
        }

        private static DataRow CreateRow<K, V>(K key, V value, DataRow workRow)
        {
            workRow["key"] = key;
            workRow["value"] = value;

            return workRow;
        }

        public static DataTable CreateBiKeyValueTable(List<Tuple<long, long, string>> bikeydata)
        {
            string key1 = "key1";
            string key2 = "key2";
            string value = "value";

            DataTable dt = new DataTable();
            dt.Columns.Add(key1, typeof(long));
            dt.Columns.Add(key2, typeof(long));
            dt.Columns.Add(value, typeof(string));

            foreach (var item in bikeydata)
            {
                dt.Rows.Add(CreateRow(item.Item1, item.Item2, item.Item3, dt.NewRow()));
            }

            return dt;
        }

        private static DataRow CreateRow(long key1, long key2, string value, DataRow workRow)
        {
            workRow["key1"] = key1;
            workRow["key2"] = key2;
            workRow["value"] = value;

            return workRow;
        }

        private static DataRow CreateRow<K1, K2, V>(K1 key1, K2 key2, V value, DataRow workRow)
        {
            workRow["key1"] = key1;
            workRow["key2"] = key2;
            workRow["value"] = value;

            return workRow;
        }
        
    }
}
