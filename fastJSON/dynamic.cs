using System.Linq;
#if net4
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace fastJSON
{
    internal sealed class DynamicJson : DynamicObject
    {
        private IDictionary<string, object> _dictionary { get; set; }
        private List<object> _list { get; set; }

        public DynamicJson(string json)
        {
            var parse = JSON.Parse(json);

            var objects = parse as IDictionary<string, object>;
            if (objects != null)
                _dictionary = objects;
            else
                _list = (List<object>)parse;
        }

        private DynamicJson(IDictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }

        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result)
        {
            var index = (int)indexes[0];
            result = _list[index];
            var objects = result as IDictionary<string, object>;
            if (objects != null)
                result = new DynamicJson(objects);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_dictionary.TryGetValue(binder.Name, out result) == false)
                if (_dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                    return false;// throw new Exception("property not found " + binder.Name);

            var objects = result as IDictionary<string, object>;
            if (objects != null)
            {
                result = new DynamicJson(objects);
            }
            else if (result is List<object>)
            {
                var list = 
                    (from item in (List<object>) result
                     let dictionary = item as IDictionary<string, object>
                     select dictionary != null ? new DynamicJson(dictionary) : item).ToList();
                result = list;
            }

            return _dictionary.ContainsKey(binder.Name);
        }
    }
}
#endif