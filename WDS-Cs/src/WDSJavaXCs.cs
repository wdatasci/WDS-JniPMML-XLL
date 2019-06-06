using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using CsvHelper;
using System.Threading.Tasks;

//CodeDoc CJW:  This class is solely for the cross-maintenance of a few pieces of code which are 
//maintained in parallel with bundle of java code accessed via jni.
namespace com.WDataSci.WDS
{
    public static class JavaLikeExtensions
    {
        public static Boolean isEmpty(this String self) { return (self.Length == 0); }
        public static Boolean equals(this String self, String arg) { return self.Equals(arg); }
        public static int indexOf(this String self, String arg) { return self.IndexOf(arg); }
        public static int lastIndexOf(this String self, String arg) { return self.LastIndexOf(arg); }
        public static Boolean endsWith(this String self, String arg) { return self.EndsWith(arg); }
        public static Boolean startsWith(this String self, String arg) { return self.StartsWith(arg); }
        public static int length(this String self) {  return self.Length; }
        public static byte[] getBytes(this String self) {  return Encoding.Unicode.GetBytes(self); }
        public static String substring(this String self, int start, int len) {  return self.Substring(start,len); }
        public static String substring(this String self, int start) {  return self.Substring(start); }
        public static String replaceAll(this String self, String RegexToFind, String RegexToReplaceWith) { return Regex.Replace(self, RegexToFind, RegexToReplaceWith); }
        public static String ReplaceAll(this String self, String RegexToFind, String RegexToReplaceWith) { return Regex.Replace(self, RegexToFind, RegexToReplaceWith); }

        //for when using "new String(arg)" is required in java to create a copy, but not in C#
        public static String new_String(String arg) { return arg; }
        public static String toString(this String self) { return self; }
        public static String toString(this Object self) { return self.ToString(); }
        public static String toLowerCase(this String self) { return self.ToLower(); }
        public static String get(this String[] self, int i) { return self[i]; }
        public static int size(this String[] self) { return self.Length; }

        //public static void add<T>(this List<T> self, T v) { self.Add(v); }
        public static void add<T>(this System.Collections.Generic.List<T> self, T v) { self.Add(v); }
        public static void clear<T>(this System.Collections.Generic.List<T> self) { self.Clear(); }

        public static T get<T>(this List<T> self, int i) { return (T) self[i]; }

        public static Dictionary<A,B> get<A,B>(this List<Dictionary<A,B>> self, int i) { return self[i];  }

        public static B get<A,B>(this Dictionary<A,B> self, A arg) { return self[arg];  } 
        public static void add<A,B>(this List<Dictionary<A,B>> self, Dictionary<A,B> arg) { self.Add(arg);  } 
        public static void add<A,B>(this List<Map<A,B>> self, Map<A,B> arg) { self.Add(arg);  } 

        public static int size<T>(this List<T> self) { return self.Count;  }

        public static Boolean bIn(long arg, params long[] args) {
            foreach (long a in args) {
                if ( arg.Equals(a) ) return true;
            }
            return false;
        }

        public static void flush(this CsvWriter self) { self.Flush(); }
        public static void close(this CsvWriter self) { self.Dispose(); }
        public static void close(this StreamWriter self) { self.Dispose(); }
        public static void printRecord(this CsvWriter self,List<String> arg) {
            foreach (String s in arg) 
                self.WriteField(s);
            self.NextRecord();
        }
        public static void printRecord(this CsvWriter self,String[] arg) {
            foreach (String s in arg) 
                self.WriteField(s);
            self.NextRecord();
        }

        public static int getLength(this XmlNodeList self) { return self.Count; }

    }

    public class PrintWriter : System.IO.StringWriter {
        public void flush() { this.Flush();  }
        public void printf(String fmt, params Object[] args) { this.Write(fmt,args);  }
        public void println(String arg) { this.WriteLine(arg);  }
        public void println(Object arg) { this.WriteLine(arg.ToString());  }
    }

    public class Map<A,B> :  Dictionary<A,B> {
        public void put(A k, B v) {
            this.Add(k, v);
        }
        public B get(A arg) { return this[arg]; }
        public HashSet<A> keySet() {
            Dictionary<A,B>.KeyCollection k=this.Keys;
            HashSet<A> rv=new HashSet<A>();
            foreach ( A v in k ) rv.Add(v);
            return rv;
        }
        public A[] keyArray() {
            Dictionary<A,B>.KeyCollection k=this.Keys;
            A[] rv=new A[k.Count];
            int i=0;
            foreach ( A v in k ) {
                rv[i] = v;
                i++;
            }
            return rv;
        }
    } 

    public class ArrayList<A> :  List<A> { } 

}
