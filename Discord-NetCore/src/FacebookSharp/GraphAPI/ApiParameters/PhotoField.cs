using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FacebookSharp.GraphAPI.ApiParameters
{
    public class PhotoField : IFieldHelper
    {
        public IList<PhotoFields> Fields { get; set; }
        public PhotoField()
        {
            Fields = new List<PhotoFields>();
        }
        public enum PhotoFields
        {
            [Description("id")]
            Id,
            [Description("album")]
            Album,
            [Description("backdated_time")]
            BackdatedTime,
            [Description("backdated_time_granularity")]
            backdatedTimeGranularity,
            [Description("from")]
            From,
            [Description("height")]
            Height,
            [Description("images")]
            Images,
            [Description("link")]
            Link,
            [Description("name")]
            Name,
            [Description("source")]
            Source
        }
        /// <summary>
        /// Generates the specified fields and puts them into a format readable by the graph api.
        /// Uses API requests
        /// </summary>
        /// <returns></returns>
        public string GenerateFields()
        {
            var strOutput = "fields=";
            for (var i = 0; i < Fields.Count; i++)
            {
                strOutput += EnumHelper.GetEnumDescription(Fields[i]);
                if (i < Fields.Count - 1)
                    strOutput += ',';
            }
            return strOutput;
        }
    }
}
