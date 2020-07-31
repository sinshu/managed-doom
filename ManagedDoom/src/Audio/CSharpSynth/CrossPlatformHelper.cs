/* These are a list of symbols supported by this project; Each one representing a different dev environment.
 * Remember to set these in the CompilerSymbols in the project properties tab.
 * 
 * -ENVIRONMENT SYMBOLS-
 * UNITY (uses unity resource loading routine)
 * DEFAULT(anything else defaults to windows pc api)
 *
 * -COMPILE TYPE-
 * DEBUG (enables assertion and other debugging)
 * RELEASE (removes assertions and other debugging from the output)
 */

using System.IO;
using System.Diagnostics;
using System.Globalization;
#if UNITY
using UnityEngine;
#endif

namespace AudioSynthesis
{
    internal static class CrossPlatformHelper
    {
#if UNITY
        public static Stream OpenResource(string name)
        {
			TextAsset ta = Resources.Load(name, typeof(TextAsset)) as TextAsset;
			if(ta != null)
				return new MemoryStream(ta.bytes);	
			throw new Exception("resource not found : " + name);	
		}		
		public static bool ResourceExists(string name)
        {
			return true;	
		}
        public static Stream CreateResource(string name)
        {
            throw new Exception("this code is unfinished.");
        }
        public static void RemoveResource(string name)
        {
            throw new Exception("this code is unfinished.");
        }
#else
        public static Stream OpenResource(string name)
        {
            return File.Open(name, FileMode.Open, FileAccess.Read);
        }
        public static bool ResourceExists(string name)
        {
            return File.Exists(name);
        }
        public static Stream CreateResource(string name)
        {
            return File.Create(name);
        }
        public static void RemoveResource(string name)
        {
            File.Delete(name);
        }
#endif

        //debug
        [Conditional("DEBUG")]
        public static void DebugMessage(object format, params object[] paramList)
        {
            string formatting = format as string;
            if (formatting != null)
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, formatting, paramList));
            else
                Debug.WriteLine(format);
        }
        [Conditional("DEBUG")]
        public static void DebugMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
        [Conditional("DEBUG")]
        public static void Assert(bool value)
        {
            System.Diagnostics.Debug.Assert(value);
        }
        [Conditional("DEBUG")]
        public static void Assert(bool value, string msg)
        {
            System.Diagnostics.Debug.Assert(value, msg);
        }
    }
}
