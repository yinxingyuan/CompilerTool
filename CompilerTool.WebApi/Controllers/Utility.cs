using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MetaShare.Common.Core.Entities;

namespace CompilerTool.WebApi.Controllers
{
	public class Utility
	{
		#region private variables
    
        #endregion

		#region Update Entity

		public delegate object GetPropertyValueByRequestKey(object requestData, string key, Type propertyType);

		internal static MetaShare.Common.Core.Entities.Common GetEntityFromRequest(MetaShare.Common.Core.Entities.Common newEntity, NameValueCollection nameValueCollection)
        {
            if (newEntity == null)  throw new Exception("Entity can not be null.");
     
            ICollection<string> keys = nameValueCollection.AllKeys;
            UpdateEntityValue(newEntity, nameValueCollection, keys, ConvertStringToPropertyTypeValue);

            return newEntity; 
        }

		internal static string GetUpdateEntityId(dynamic entity)
        {
            if(entity.Id != null)
            {
                return entity.Id;
            }
            if(entity.ID != null)
            {
                return entity.ID;
            }
            if (entity.iD != null)
            {
                return entity.iD;
            }
            if (entity.id != null)
            {
                return entity.id;
            }
            return null;
        }

		internal static void UpdateEntityValue(object newEntity, object json, ICollection<string> keys, GetPropertyValueByRequestKey getPropertyValueByRequestKey)
        {
            Type type = newEntity.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                foreach (string key in keys)
                {
                    if (propertyInfo.Name.ToLower().Equals(key.ToLower()))
                    {
                        object propertyValue = getPropertyValueByRequestKey(json, key, propertyInfo.PropertyType);
                        propertyInfo.SetValue(newEntity, propertyValue);
                    }
                }
            }
        }
		public static object ConvertStringToPropertyTypeValue(object @object, string key, Type propertyInfoPropertyType)
        {
            NameValueCollection nameValueCollection = @object as NameValueCollection;
            if (nameValueCollection == null) return null;

            string keyValue = nameValueCollection.Get(key);
            object value = CovertFromPrimitiveType(keyValue, propertyInfoPropertyType);
            if (value != null)
            {
                return value;
            }

            dynamic childJObject = JsonConvert.DeserializeObject(keyValue);
            if (childJObject != null)
            {
                MetaShare.Common.Core.Entities.Common relatedEntity = Activator.CreateInstance(propertyInfoPropertyType) as MetaShare.Common.Core.Entities.Common;
                if (relatedEntity != null)
                {
                    ICollection<string> keys = GetJsonKeys(childJObject);
                    UpdateEntityValue(relatedEntity, childJObject, keys, new GetPropertyValueByRequestKey(ConvertJTokenToPropertyTypeValue));

                    return relatedEntity;
                }
            }

            return null;
        }

		public static object ConvertJTokenToPropertyTypeValue(object @object, string key, Type propertyInfoPropertyType)
        {
            JObject json = @object as JObject;
            if (json == null) return null;

            JToken keyValue = json.GetValue(key);
            if (keyValue != null)
            {
                object value = CovertFromJTokePrimitiveType(keyValue, propertyInfoPropertyType);
                if (value != null)
                {
                    return value;
                }
                if (keyValue.Type == JTokenType.Object)
                {
                    dynamic childJObject = keyValue;
                    object relatedEntity = Activator.CreateInstance(propertyInfoPropertyType);

                    ICollection<string> keys = GetJsonKeys(childJObject);
                    GetPropertyValueByRequestKey getPropertyValueByRequestKey = ConvertJTokenToPropertyTypeValue;
                    UpdateEntityValue(relatedEntity, childJObject, keys, getPropertyValueByRequestKey);

                    return relatedEntity;
                }
                return null;
            }

            return null;
        }

		internal static ICollection<string> GetJsonKeys(JObject json)
        {
            Type jsontype = json.GetType();

            FieldInfo fieldproperties = jsontype.GetField("_properties", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldproperties != null)
            {
                var valueproperties = fieldproperties.GetValue(json);
                Type jPropertyKeyedCollection = valueproperties.GetType();
                PropertyInfo keys = jPropertyKeyedCollection.GetProperty("Keys", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
                if (keys != null)
                {
                    var valuekeys = keys.GetValue(valueproperties, null);

                    return (ICollection<string>) valuekeys;
                }
            }

            return null;
        }

		#region covert to PrimitiveType from string

		private static object CovertFromPrimitiveType(string keyValue, Type propertyInfoPropertyType)
        {
            if (propertyInfoPropertyType == typeof(string))
            {
                return keyValue;
            }
            if (propertyInfoPropertyType == typeof(int) || propertyInfoPropertyType.IsEnum)
            {
                int value = Convert.ToInt32(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(bool))
            {
                bool value = Convert.ToBoolean(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTimeOffset))
            {
                DateTimeOffset value = Convert.ToDateTime(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(bool?))
            {
                if (keyValue == null) return null;
                bool? value = Convert.ToBoolean(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(long))
            {
                long value = Convert.ToInt64(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTime))
            {
                DateTime value = Convert.ToDateTime(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTimeOffset?))
            {
                if (keyValue == null) return null;
                DateTimeOffset value = Convert.ToDateTime(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTime?))
            {
                if (keyValue == null) return null;
                DateTime? value = Convert.ToDateTime(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(decimal?))
            {
                if (keyValue == null) return null;
                decimal? value = Convert.ToDecimal(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(double?))
            {
                if (keyValue == null) return null;
                double? value = Convert.ToDouble(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(char?))
            {
                if (keyValue == null) return null;
                char? value = Convert.ToChar(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(short))
            {
                short value = Convert.ToInt16(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(ushort))
            {
                ushort value = Convert.ToUInt16(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(char))
            {
                char value = Convert.ToChar(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(byte))
            {
                byte value = Convert.ToByte(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(int?))
            {
                if (keyValue == null) return null;
                int value = Convert.ToInt32(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(short?))
            {
                if (keyValue == null) return null;
                short value = Convert.ToInt16(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(ushort?))
            {
                if (keyValue == null) return null;
                ushort value = Convert.ToUInt16(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(byte?))
            {
                if (keyValue == null) return null;
                byte value = Convert.ToByte(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(long?))
            {
                if (keyValue == null) return null;
                long value = Convert.ToInt64(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(float?))
            {
                if (keyValue == null) return null;
                float? value = Convert.ToSingle(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(decimal))
            {
                decimal value = Convert.ToDecimal(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(uint?))
            {
                if (keyValue == null) return null;
                uint? value = Convert.ToUInt32(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(ulong?))
            {
                if (keyValue == null) return null;
                ulong? value = Convert.ToUInt64(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(double))
            {
                double value = Convert.ToDouble(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(float))
            {
                float value = Convert.ToSingle(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(uint))
            {
                uint value = Convert.ToUInt32(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(ulong))
            {
                ulong value = Convert.ToUInt64(keyValue);
                return value;
            }
            if (propertyInfoPropertyType == typeof(byte[]))
            {
                //byte[] value = Convert.FromBase64String(keyValue);
                //return value;
            }

            return null;
        }

		#endregion

		#region    covert to PrimitiveType from JToken

		private static object CovertFromJTokePrimitiveType(JToken keyValue, Type propertyInfoPropertyType)
        {
            if (propertyInfoPropertyType == typeof(string))
            {
                string value = (string) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(int) || propertyInfoPropertyType.IsEnum)
            {
                int value = (int) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(bool))
            {
                bool value = (bool) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTimeOffset))
            {
                DateTimeOffset value = (DateTimeOffset) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(bool?))
            {
                bool? value = (bool?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(long))
            {
                long value = (long) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTime))
            {
                DateTime value = (DateTime) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTimeOffset?))
            {
                DateTimeOffset? value = (DateTimeOffset?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(DateTime?))
            {
                DateTime? value = (DateTime?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(decimal?))
            {
                decimal? value = (decimal?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(double?))
            {
                double? value = (double?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(char?))
            {
                char? value = (char?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(short))
            {
                short value = (short) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(ushort))
            {
                ushort value = (ushort) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(char))
            {
                char value = (char) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(byte))
            {
                byte value = (byte) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(int?))
            {
                int? value = (int?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(short?))
            {
                short? value = (short?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(ushort?))
            {
                ushort? value = (ushort?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(byte?))
            {
                byte? value = (byte?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(long?))
            {
                long? value = (long?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(float?))
            {
                float? value = (float?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(decimal))
            {
                decimal value = (decimal) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(uint?))
            {
                uint? value = (uint?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(ulong?))
            {
                ulong? value = (ulong?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(double))
            {
                double value = (double) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(float))
            {
                float value = (float) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(uint))
            {
                uint value = (uint) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(ulong))
            {
                ulong value = (ulong) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(byte[]))
            {
                byte[] value = (byte[]) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(Guid))
            {
                Guid value = (Guid) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(Guid?))
            {
                Guid? value = (Guid?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(TimeSpan?))
            {
                TimeSpan? value = (TimeSpan?) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(TimeSpan))
            {
                TimeSpan value = (TimeSpan) keyValue;
                return value;
            }
            if (propertyInfoPropertyType == typeof(Uri))
            {
                Uri value = (Uri) keyValue;
                return value;
            }
            return null;
        }

		#endregion

		#endregion

		#region SelectBy

		internal static List<string> GetColumnStrings(MetaShare.Common.Core.Entities.Common newEntity, ICollection<string> keys)
        {
            List<string> allKeys = new List<string>();
            if (newEntity != null)
            {
                Type type = newEntity.GetType();
                PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
                foreach (string key in keys)
                {
                    string newKey = string.Empty;
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (propertyInfo.Name.ToLower().Equals(key.ToLower()))
                        {
                            newKey = key;
                            if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(string) && !propertyInfo.PropertyType.IsEnum)
                            {
                                MetaShare.Common.Core.Entities.Common relatedEntity = Activator.CreateInstance(propertyInfo.PropertyType) as MetaShare.Common.Core.Entities.Common;
                                if (relatedEntity != null)
                                {
                                    newKey = key + "Id";
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(newKey))
                    {
                        allKeys.Add(newKey);
                    }
                }
            }

            return allKeys;
        }

		#endregion

	}
}
