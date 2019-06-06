/* Java >>> *
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.Util;
import com.WDataSci.WDS.WDSException;
import org.apache.commons.io.IOUtils;
import org.dmg.pmml.DataField;
import org.dmg.pmml.FieldName;
import org.dmg.pmml.clustering.ClusteringModel;
import org.jpmml.evaluator.clustering.ClusterAffinityDistribution;
import org.jpmml.evaluator.clustering.ClusteringModelEvaluator;
import org.jpmml.model.PMMLUtil;
import org.w3c.dom.Document;

import javax.xml.parsers.DocumentBuilderFactory;
import java.io.File;
import java.io.InputStream;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.nio.ByteBuffer;
import java.nio.charset.Charset;
import java.util.*;

//
// <p>JniPMMLItem is the main wrap class around a single PMML document and its evaluator.
// The evaluator implementation starts with the PMML reference evaluator, jpmml.evaluator, although this might change in
// later versions or be left as a comparable.</p>
// <p>A JniPMMLItem instance is intended to be called through Java Native Interface (jni),
// primarily from an Excel C# addin, but the implementation is left general enough
// to be called from other languages and a command line call version is included.</p>
// </p>

/* <<< Java */

/* C# >>> */
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

namespace com.WDataSci.JniPMML
{
    /// <summary>
    /// The JniPMMLItem class is mirrored on the C# and Java sides, but with a few 
    /// specific differences: On the C# side, JniPMMLItem also works as an 
    /// IExcelObservable object handle, while also holding the information needed to pass data
    /// to Java.  The handle storage is effectively handled on the Java side.
    /// </summary>

    /* <<< C# */

    public class JniPMMLItem
        //C#
        : IExcelObservable, IDisposable
    {
        //Java protected Integer HandleMajor=0;
        //Java protected Integer HandleMinor=0;


        //White matter (Connectivity)
        public __ConfigMatter ConfigMatter = new __ConfigMatter();
        //C#
        public readonly String __Tag;
        //C#
        public readonly int __Handle;
        //C#
        //public readonly JniPMML __JniPMML;
        public JniPMML __JniPMML;
        public IExcelObserver __ExcelObserver;
        public String HandleMajorMinor = "-1.0";

        //Grey matter (Body and Work)
        public __PMMLMatter PMMLMatter = new __PMMLMatter();
        public __InputMatter InputMatter = new __InputMatter();
        public __OutputMatter OutputMatter = new __OutputMatter();

        public void PreDispose()
        {
            this.__ExcelObserver = null;
            this.ConfigMatter = null;
            if (this.PMMLMatter!=null) this.PMMLMatter.Dispose();
            this.PMMLMatter = null;
            if (this.InputMatter!=null) this.InputMatter.Dispose();
            this.InputMatter = null;
            if (this.OutputMatter!=null) this.OutputMatter.Dispose();
            this.OutputMatter = null;
            //C#
            GC.Collect();
            //C#
            GC.WaitForPendingFinalizers();
            //C#
            GC.Collect();
            //C#
            GC.WaitForPendingFinalizers();
        }

        public void Reset()
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.PreDispose();
            this.PMMLMatter = new __PMMLMatter();
            this.InputMatter = new __InputMatter();
            this.OutputMatter = new __OutputMatter();
        }

        public void Dispose()
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.PreDispose();
            this.ConfigMatter = null;
            /* C# >>> */
            if ( this.__JniPMML != null && this.__JniPMML.isHandle(this.__Handle ) ) {
                try {
                    this.__JniPMML.__Java.CallVoidMethod("ItemDispose", "(I)V", new List<object> { this.__Handle });
                    this.__JniPMML.Remove(this);
                } catch (Exception e) {

                }
            }
            /* <<< C# */
        }
        /* C# >>> */
        ~JniPMMLItem()
        {
            this.Dispose();
        }
        /* <<< C# */


        public class __ConfigMatter
        {
            public String InternalString = "";
        }

        public class __PMMLMatter
        {
            //Java protected org.dmg.pmml.PMML Doc = null;
            //Java protected org.jpmml.evaluator.ModelEvaluator Evaluator = null;

            //C#
            public XmlDocument Doc = null;
            //C#
            public XmlDocument Evaluator = null;

            public String _XMLString = null;
            public String _XMLFileName = null;

            public void Dispose()
            {
                this.Doc = null;
                this.Evaluator = null;
            }
            /* C# >>> */
            ~__PMMLMatter()
            {
                this.Dispose();
            }
            /* C# >>> */
        }

        public class __InputMatter
        {
            public RecordSetMD RecordSetMD = null;
            public RecordSet RecordSet = null;

            //Java public Document _XSDDoc = null;
            //C#
            public XmlDocument _XSDDoc = null;
            public String _XSDFileName = null;
            public String _XSDString = null;
            public void Dispose()
            //throws com.WDataSci.WDS.WDSException, Exception
            {
                this._XSDDoc = null;
                this._XSDFileName = null;
                this._XSDString = null;
                if (this.RecordSet!=null) this.RecordSet.Dispose();
                this.RecordSet = null;
                if (this.RecordSetMD!=null) this.RecordSetMD.Dispose();
                this.RecordSetMD = null;
            }
            /* C# >>> */
            ~__InputMatter()
            {
                this.Dispose();
            }
            /* C# >>> */
        }

        public class __OutputMatter
        {
            public RecordSetMD RecordSetMD = null;
            public RecordSet RecordSet = null;
            public void Dispose()
            //throws com.WDataSci.WDS.WDSException, Exception
            {
                if (this.RecordSet!=null) this.RecordSet.Dispose();
                this.RecordSet = null;
                if (this.RecordSetMD!=null) this.RecordSetMD.Dispose();
                this.RecordSetMD = null;
            }
            /* C# >>> */
            ~__OutputMatter()
            {
                this.Dispose();
            }
            /* <<< C# */
        }

        /* Java >>> *
        //Default constructor
        public JniPMMLItem()
        {
            synchronized (this) {
                this.HandleMajor = 0;
                this.HandleMinor = 0;
                this.ConfigMatter.InternalString = "An internal state string";
            }
        }
        /* <<< Java */


        public JniPMMLItem(String aTag, JniPMML aJniPMML)
        {
            lock (aJniPMML)
            {
                lock (aJniPMML.__Java)
                {
                    try
                    {
                        this.__Handle = aJniPMML.__Java.CallMethod<int>("HandleMajor", "(I)I", new List<object> { -1 });
                    } catch (Exception e)
                    {
                        throw new com.WDataSci.WDS.WDSException("Error in call to JniPMML, HandleMajor", e);
                    }
                    this.__Tag = aTag;
                    this.__JniPMML = aJniPMML;
                    aJniPMML.Add(this);
                }
            }
        }

        public int Handle() { return this.__JniPMML.Handle(this.__Tag); }
        public String Tag() { return this.__Tag; }

        public void UpdateHandleMajorMinor()
        {
            this.HandleMajorMinor = this.__JniPMML.__Java.CallMethod<String>("Handle"
                        , "(I)Ljava/lang/String;"
                        , new List<object> { this.__Handle });
        }

        public IDisposable Subscribe(IExcelObserver arg)
        {
            this.__ExcelObserver = arg;
            this.__ExcelObserver.OnNext(this.HandleMajorMinor);
            return this;
        }

        internal FieldName [] PMMLDataFieldNames()
        {
            FieldName [] rv = new FieldName [1];
            rv [0] = new FieldName("");
            return rv;
        }

        public String [] PMMLDataFieldStringNames()
        {
            FieldName [] flds = this.PMMLDataFieldNames();
            if ( flds == null ) return null;
            String [] rv = new String [flds.Length];
            for ( int i = 0 ; i < rv.Length ; i++ ) {
                rv [i] = flds [i].getValue();
            }

            return rv;

        }

    }
}



