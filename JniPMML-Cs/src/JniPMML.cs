using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using com.WDataSci.WDS;
using static com.WDataSci.WDS.JavaLikeExtensions;
using FieldName = com.WDataSci.JniPMML.FieldName;
using ExcelDna.Integration;
using WDataSci.JniPMML;
using static WDataSci.JniPMML.AddIn;
using JNI;

namespace com.WDataSci.JniPMML
{
    public class JniPMML
    {
        private Dictionary<String, int> __Handle=null;
        private Dictionary<int, String> __Tag=null;
        public Dictionary<int, JniPMMLItem> Item=null;
        public readonly JavaNativeInterface __Java=null;
        public readonly IntPtr __java_init_classid=IntPtr.Zero;

        public JniPMML(JavaNativeInterface Java, IntPtr java_init_classid)
        {
            this.__Handle= new Dictionary<string, int>();
            this.__Tag=new Dictionary<int, string>();
            this.Item= new Dictionary<int, JniPMMLItem>();
            this.__Java = Java;
            this.__java_init_classid = java_init_classid;
        }

        public void Dispose()
        {
            if ( this.__Tag != null && this.__Tag.Keys.Count > 0 ) {
                int[] keys = new int[this.__Tag.Keys.Count];
                this.__Tag.Keys.CopyTo(keys, 0);
                for ( int i = 0 ; i < keys.Length ; i++ )
                    this.Remove(keys[i]);
                keys = null;
            }
        }

        ~JniPMML()
        {
            this.Dispose();
        }


        public int Handle(String aTag)
        {
            try
            {
                return this.__Handle[aTag];
            } catch(Exception)
            {
                return -1;
            }
        }

        public Boolean isHandle(int aHandle) { return this.__Tag.Keys.Contains(aHandle); }

        public String Tag(int aHandle)
        {
            try
            {
                return this.__Tag[aHandle];
            } catch(Exception)
            {
                return null;
            }
        }

        public Boolean isTag(String aTag) { return this.__Handle.Keys.Contains(aTag); }

        public void Remove(int aHandle) { if (aHandle > 0) { String aTag = this.Tag(aHandle); this.Item[aHandle].PreDispose(); this.__Handle.Remove(aTag); this.__Tag.Remove(aHandle); this.Item.Remove(aHandle); } }
        public void Remove(String aTag) { int aHandle = this.Handle(aTag); this.Item[aHandle].PreDispose();  this.__Handle.Remove(aTag);  this.__Tag.Remove(aHandle); this.Item.Remove(aHandle); }
        public void Remove(JniPMMLItem arg) { int aHandle = arg.Handle(); this.Item[aHandle].PreDispose();  String aTag = arg.Tag();this.__Handle.Remove(aTag);  this.__Tag.Remove(aHandle); this.Item.Remove(aHandle); }

        public JniPMMLItem Add(JniPMMLItem aJniPMMLItem)
        {
            if (aJniPMMLItem.__Tag.startsWith("Internal") && this.__Tag.ContainsKey(aJniPMMLItem.__Handle) ) {
                //clean up  any Internal's, only one can run at a time
                String[] aHandles = this.__Handle.Keys.ToArray();
                foreach ( String aTag in aHandles ) {
                    if ( aTag.startsWith("Internal") )
                        this.Remove(aTag);
                }
            }
            this.__Handle.Add(aJniPMMLItem.__Tag, aJniPMMLItem.__Handle);
            this.__Tag.Add(aJniPMMLItem.__Handle, aJniPMMLItem.__Tag);
            this.Item.Add(aJniPMMLItem.__Handle, aJniPMMLItem);
            return aJniPMMLItem;
        }

        public JniPMMLItem GetOrAddItemWithTag(String aTag)
        {
            JniPMMLItem aJniPMMLItem;
            lock (this)
            {
                if (!this.TryGetObject(aTag, out aJniPMMLItem))
                {
                    aJniPMMLItem = new com.WDataSci.JniPMML.JniPMMLItem(aTag, this);
                    this.Add(aJniPMMLItem);
                }
            }
            return aJniPMMLItem;
        }

        public JniPMMLItem GetOrAddItemWithHandle(int aHandle)
        {
            JniPMMLItem aJniPMMLItem;
            if ( !this.TryGetObject(aHandle, out aJniPMMLItem) ) {
                aJniPMMLItem = new com.WDataSci.JniPMML.JniPMMLItem(aHandle.ToString(), this);
            }
            return aJniPMMLItem;
        }

        public JniPMMLItem CreateHandle(String aEvaluatorType, String aTag, object bFileOrString, String src)
        {
            JniPMMLItem aJniPMMLItem = null;
            lock (this)
            {
                aJniPMMLItem = this.GetOrAddItemWithTag(aTag);
                String method;
                if (bFileOrString.Equals(0) && src.startsWith("<?"))
                    method = "mPMMLLoadFromString";
                else
                    method = "mPMMLLoadFromFile";
                List<object> cmargs = new List<object> { aJniPMMLItem.__Handle, src };
                aJniPMMLItem.HandleMajorMinor = AddIn.__JniPMML.__Java.CallMethod<string>(method
                    , "(ILjava/lang/String;)Ljava/lang/String;"
                    , cmargs);
            }
            return aJniPMMLItem;
        }

        public object CreateHandle(String aEvaluatorType, object[] args, Func<String, object[], JniPMMLItem> lFunc)
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            return ExcelAsyncUtil.Observe(aEvaluatorType
                , args
                , () => {
                   JniPMMLItem aJniPMMLItem = this.CreateHandle(aEvaluatorType, args[0].ToString(), args[1], args[2].ToString());
                   return aJniPMMLItem;
                }
                );
        }

        public Boolean TryGetObject(int aHandle, out JniPMMLItem value)
        {
            if (this.Item.TryGetValue(aHandle, out value))
            {
                return true;
            }
            value = null;
            return false;
        }


        public Boolean TryGetObject(String aTag, out JniPMMLItem value)
        {
            if (!this.__Handle.ContainsKey(aTag))
            {
                value = null;
                return false;
            }
            int aHandle = this.__Handle[aTag];
            if (this.Item.TryGetValue(aHandle, out value))
            {
                return true;
            }
            value = null;
            return false;
        }


    }
}



