
namespace TestPlotly
{


    public class SecretManager
    {

        // TestPlotly.SecretManager.GetSecret<string>("DefaultDbPassword")
        // TestPlotly.SecretManager.GetSecret<string>("GoogleGeoCodingApiKey")
        public static T GetSecret<T>(string secretName)
        {
            string asmName = typeof(SecretManager).Assembly.FullName;

            int ipos = asmName.IndexOf(',');
            if (ipos != -1)
            {
                asmName = asmName.Substring(0, ipos);
            }
            
            return GetSecret<T>(secretName, asmName);
        } // End Function GetSecret 
        
        
        public static T GetSecret<T>(string secretName, string asmName)
        {
            T obj = default(T);

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                obj = SecretManagerHelper.GetEtcKey<T>("/etc/COR/" + asmName, secretName);
                if(obj == null)
                    obj = SecretManagerHelper.GetEtcKey<T>(@"/etc/COR/All", secretName);
            }
            else
            {
                obj = SecretManagerHelper.GetRegistryKey<T>(@"Software\COR\" + asmName, secretName);
                if(obj == null)
                    obj = SecretManagerHelper.GetRegistryKey<T>(@"Software\COR\All", secretName);
            }
            
            return obj;
        } // End Function GetSecret 


    } // End Class SecretManager 


    public class SecretManagerHelper
    {


        public static T GetRegistryKey<T>(string key, string value)
        {
            object obj = GetRegistryKey(key, value);
            return ObjectToGeneric<T>(obj);
        } // End Function GetRegistryKey 
        
        
        public static T GetEtcKey<T>(string path, string value)
        {
            string obj = null;
            
            string p = System.IO.Path.Combine(path, value);
            if(System.IO.File.Exists(p))
                obj = System.IO.File.ReadAllText(p, System.Text.Encoding.Default);

            if(obj == null)
                return ObjectToGeneric<T>((object)obj);

            // || obj.EndsWith(" ") || obj.EndsWith("\t") 
            while ( obj.EndsWith("\r") || obj.EndsWith("\n") )
                obj = obj.Substring(0, obj.Length - 1);

            return ObjectToGeneric<T>((object)obj);
        } // End Function GetRegistryKey 


        private static object GetRegistryKey(string key, string value)
        {
            object objReturnValue = null;
            // HKEY_CURRENT_USER

            //using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine
            using (Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser
                .OpenSubKey(key))
            {
                if (regKey != null)
                {
                    objReturnValue = regKey.GetValue(value);
                } // End if (regKey != null) 

            } // End Using regKey 

            return objReturnValue;
        } // End Function GetRegistryKey 
        

        private static T InlineTypeAssignHelper<T>(object UTO)
        {
            if (UTO == null)
            {
                T NullSubstitute = default(T);
                return NullSubstitute;
            } // End if (UTO == null) 

            return (T)UTO;
        } // End Template InlineTypeAssignHelper


        private static T ObjectToGeneric<T>(object objReturnValue)
        {
            string strReturnValue = null;
            System.Type tReturnType = typeof(T);
            
            if (!object.ReferenceEquals(tReturnType, typeof(System.Byte[])))
            {
                if(objReturnValue != null)
                    strReturnValue = System.Convert.ToString(objReturnValue);
            } // End if (!object.ReferenceEquals(tReturnType, typeof(System.Byte[])))

            try
            {

                if (object.ReferenceEquals(tReturnType, typeof(object)))
                {
                    return InlineTypeAssignHelper<T>(objReturnValue);
                }
                else if (object.ReferenceEquals(tReturnType, typeof(string)))
                {
                    return InlineTypeAssignHelper<T>(strReturnValue);
                } // End if string
                else if (object.ReferenceEquals(tReturnType, typeof(bool)))
                {
                    bool bReturnValue = false;
                    bool bSuccess = bool.TryParse(strReturnValue, out bReturnValue);

                    if (bSuccess)
                        return InlineTypeAssignHelper<T>(bReturnValue);

                    if (strReturnValue == "0")
                        return InlineTypeAssignHelper<T>(false);

                    return InlineTypeAssignHelper<T>(true);
                } // End if bool
                else if (object.ReferenceEquals(tReturnType, typeof(int)))
                {
                    int iReturnValue = int.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(iReturnValue);
                } // End if int
                else if (object.ReferenceEquals(tReturnType, typeof(uint)))
                {
                    uint uiReturnValue = uint.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(uiReturnValue);
                } // End if uint
                else if (object.ReferenceEquals(tReturnType, typeof(long)))
                {
                    long lngReturnValue = long.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(lngReturnValue);
                } // End if long
                else if (object.ReferenceEquals(tReturnType, typeof(ulong)))
                {
                    ulong ulngReturnValue = ulong.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(ulngReturnValue);
                } // End if ulong
                else if (object.ReferenceEquals(tReturnType, typeof(float)))
                {
                    float fltReturnValue = float.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(fltReturnValue);
                }
                else if (object.ReferenceEquals(tReturnType, typeof(double)))
                {
                    double dblReturnValue = double.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(dblReturnValue);
                }
                else if (object.ReferenceEquals(tReturnType, typeof(System.Net.IPAddress)))
                {
                    System.Net.IPAddress ipaAddress = null;

                    if (string.IsNullOrEmpty(strReturnValue))
                        return InlineTypeAssignHelper<T>(ipaAddress);

                    ipaAddress = System.Net.IPAddress.Parse(strReturnValue);
                    return InlineTypeAssignHelper<T>(ipaAddress);
                } // End if IPAddress
                else if (object.ReferenceEquals(tReturnType, typeof(System.Byte[])))
                {
                    if (objReturnValue == System.DBNull.Value)
                        return InlineTypeAssignHelper<T>(null);

                    return InlineTypeAssignHelper<T>(objReturnValue);
                }
                else if (object.ReferenceEquals(tReturnType, typeof(System.Guid)))
                {
                    if (string.IsNullOrEmpty(strReturnValue)) return InlineTypeAssignHelper<T>(null);

                    return InlineTypeAssignHelper<T>(new System.Guid(strReturnValue));
                } // End if GUID
                else if (object.ReferenceEquals(tReturnType, typeof(System.DateTime)))
                {
                    System.DateTime bReturnValue = System.DateTime.Now;
                    bool bSuccess = System.DateTime.TryParse(strReturnValue, out bReturnValue);

                    if (bSuccess)
                        return InlineTypeAssignHelper<T>(bReturnValue);

                    if (strReturnValue == "0")
                        return InlineTypeAssignHelper<T>(false);

                    return InlineTypeAssignHelper<T>(true);
                } // End if datetime
                else // No datatype matches
                {
                    throw new System.NotImplementedException("ExecuteScalar<T>: This type is not yet defined.");
                } // End else of if tReturnType = datatype

            } // End Try
            catch (System.Exception ex)
            {
                throw;
            } // End Catch

            return InlineTypeAssignHelper<T>(null);
        } // End Function ObjectToGeneric 


    } // End Class SecretManager 


} // End Namespace 
