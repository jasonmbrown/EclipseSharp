using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Extensions {
    public static class Extensions {

        #region Dictionaries
        public static V TryGet<K, V>(this Dictionary<K, V> dict, K key, V def) {
            try {
                return dict[key];
            } catch {
                return def;
            }
        }
        #endregion

        #region Strings
        public static Int32 ToInt32(this String input) {
            try {
                return Convert.ToInt32(input);
            } catch {
                return 0;
            }
        }
        #endregion

        #region Cryptography
        public static String ToSha256(this String input) {
            var sha = new SHA256Managed();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "");
        }
        #endregion

    }
}
