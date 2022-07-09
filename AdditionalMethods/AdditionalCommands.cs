using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Get_Requests_From_Client_For_Project_Test.AdditionalMethods
{
    /// <summary>
    /// The additional comands.
    /// </summary>
    public static class AdditionalCommands
    {
        /// <summary>
        /// Get least busy local machine from dictionary.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>The least busy local machine.</returns>
        public static KeyValuePair<string, long> GetLeastBusyMachine(this ConcurrentDictionary<string, long> val)
        {
            long min = long.MaxValue;
            KeyValuePair<string, long> answer = new();
            foreach(KeyValuePair<string, long> v in val)
            {
                if (v.Value <= min)
                {
                    answer = v;
                }
            }
            return answer;
        }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object data)
        {
            try
            {
                if (data == null)
                {
                    return default;
                }

                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch
            {
                return default;
            }
        }

    }
}
