using System;
using System.Collections.Generic;

namespace fastJSON
{
    public sealed class DatasetSchema
    {
        public List<string> Info ;//{ get; set; }
        public string Name ;//{ get; set; }
    }

    /// <summary>
    /// DataMember attribute clone for .net v2 v3.5
    /// </summary>
    public class DataMemberAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
