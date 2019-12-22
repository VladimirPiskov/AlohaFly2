using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AlohaFly.Utils
{
    static class AttributeExtension
    {

        public static bool GetDataHidden(this PropertyInfo PI)
        {
            Attribute attr = PI.GetCustomAttribute(typeof(DisplayAttribute));
            string PropName = PI.Name;
            if (attr != null)
            {
                var DispAttr = ((DisplayAttribute)attr);
                try { if (!DispAttr.AutoGenerateField) return true; } catch { }
            }
            return false;
        }

        public static bool GetDataHidden(this PropertyDescriptor pd)
        {
            if (pd != null)
            {
                DisplayAttribute displayName = pd.Attributes[typeof(DisplayAttribute)] as DisplayAttribute;
                try
                {
                    if (displayName != null && !displayName.AutoGenerateField)
                    {
                        return true;
                    }
                }
                catch
                {
                }
            }
            return false;
        }


        public static string GetDispalyName(this PropertyInfo PI)
        {
            Attribute attr = PI.GetCustomAttribute(typeof(DisplayAttribute));
            string PropName = PI.Name;
            if (attr != null)
            {
                var DispAttr = ((DisplayAttribute)attr);
                return DispAttr.Name;
            }
            return "";
        }

        public static string GetDispalyName(this PropertyDescriptor pd)
        {
            if (pd != null)
            {
                DisplayAttribute displayName = pd.Attributes[typeof(DisplayAttribute)] as DisplayAttribute;
                try
                {
                    if (displayName != null)
                    {
                        return displayName.Name;
                    }
                }
                catch
                {
                }
            }
            return "";
        }

        public static int GetMaxWidth(this PropertyDescriptor pd)
        {
            if (pd != null)
            {
                MaxLengthAttribute displayName = pd.Attributes[typeof(MaxLengthAttribute)] as MaxLengthAttribute;
                try
                {
                    if (displayName != null)
                    {
                        return displayName.Length;
                    }
                }
                catch
                {
                }
            }
            return 1;
        }
    }
}
