using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using MetaShare.Common.Core.Entities;

namespace CompilerTool.WebApi.Models
{
    public class RequestData
    {
        public string PagerJsonString { get; set; }
        public string EntityJsonString { get; set; }
        public string ColumnNamesJsonString { get; set; }
    }

    [DataContract]
    public class RequestData<T>
    {
        [DataMember]
        public Pager Pager { get; set; }

        [DataMember]
        public T Entity { get; set; }

        [DataMember]
        public List<string> Columns { get; set; }

        public void Deserialize(RequestData requestData)
        {
            if (requestData == null) throw new ArgumentNullException("requestData");
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            Pager = javaScriptSerializer.Deserialize<Pager>(requestData.PagerJsonString);
            Entity = javaScriptSerializer.Deserialize<T>(requestData.EntityJsonString);
            Columns = javaScriptSerializer.Deserialize<List<string>>(requestData.ColumnNamesJsonString);
        }

        public Dictionary<string, string> Serialize()
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            string pagerJsonString = javaScriptSerializer.Serialize(this.Pager);
            string entityJsonString = javaScriptSerializer.Serialize(this.Entity);
            string propertyNamesJsonString = javaScriptSerializer.Serialize(this.Columns);

            Dictionary<string, string> requestDataDictionary = new Dictionary<string, string>();
            requestDataDictionary.Add("PagerJsonString", pagerJsonString);
            requestDataDictionary.Add("EntityJsonString", entityJsonString);
            requestDataDictionary.Add("ColumnNamesJsonString", propertyNamesJsonString);
            return requestDataDictionary;
        }
    }
}