using System;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace fastJSON
{

    #region Nested type: DynamicJsonObject

    /// <summary>
    /// 
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public sealed class DynamicJson : DynamicObject
    {
        private IDictionary<string, object> _dictionary;
        private List<object> _list { get; set; }

        public DynamicJson(string json)
        {
            var parse = fastJSON.JSON.Parse(json);

            if (parse is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)parse;
            else
                _list = (List<object>)parse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public DynamicJson(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            _dictionary = dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //if (!_dictionary.TryGetValue(binder.Name, out result))
            //{
            //    // return null to avoid exception.  caller can check for null this way...
            //    result = null;
            //    return true;
            //}

            //var dictionary = result as IDictionary<string, object>;
            //if (dictionary != null)
            //{
            //    result = new DynamicJsonObject(dictionary);
            //    return true;
            //}

            //var arrayList = result as ArrayList;
            //if (arrayList != null && arrayList.Count > 0)
            //{
            //    if (arrayList[0] is IDictionary<string, object>)
            //        result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
            //    else
            //        result = new List<object>(arrayList.Cast<object>());
            //}

            //return true;
            result = this._dictionary.FirstOrDefault(dictionary => dictionary.Key.ToLowerInvariant() == binder.Name.ToLowerInvariant()).Value;

            if (result is IDictionary<string, object>)
            {
                result = new DynamicJson(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<DynamicJson>((result as ArrayList).Cast<IDictionary<string, object>>().Select(x => new DynamicJson(x)));
            }
            else if (result is ArrayList)
            {
                result = new List<DynamicJson>((result as ArrayList).Cast<DynamicJson>());
            }

            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var data = this._dictionary.FirstOrDefault(dictionary => dictionary.Key.ToLower() == binder.Name.ToLower());

            if (!String.IsNullOrWhiteSpace(data.Key))
            {
                this._dictionary.Remove(data.Key);
            }

            this._dictionary[binder.Name] = value;
            return true;
        }
    }

    #endregion
}
