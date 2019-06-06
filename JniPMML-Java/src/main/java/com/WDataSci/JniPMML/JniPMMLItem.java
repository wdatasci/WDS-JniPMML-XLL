/* Java >>> */
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

/* C# >>> *
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
//C# : IExcelObservable, IDisposable
{
    //Java
    protected Integer HandleMajor = 0;
    //Java
    protected Integer HandleMinor = 0;

    //White matter (Connectivity)
    public __ConfigMatter ConfigMatter = new __ConfigMatter();
    //C# public readonly String __Tag;
    //C# public readonly int __Handle;
    //C# public readonly JniPMML __JniPMML;
    //C# public IExcelObserver __ExcelObserver;
    //C# public String HandleMajorMinor="-1.0";
    //C# public readonly JniPMML __JniPMML;
    //C# public IExcelObserver __ExcelObserver;
    //C# public String HandleMajorMinor = "-1.0";

    //Grey matter (Body and Work)
    public __PMMLMatter PMMLMatter = new __PMMLMatter();
    public __InputMatter InputMatter = new __InputMatter();
    public __OutputMatter OutputMatter = new __OutputMatter();

    public void PreDispose()
            throws com.WDataSci.WDS.WDSException
    {
        try {
            if ( this.PMMLMatter != null ) this.PMMLMatter.Dispose();
            this.PMMLMatter = null;
            if ( this.InputMatter != null ) this.InputMatter.Dispose();
            this.InputMatter = null;
            if ( this.OutputMatter != null ) this.OutputMatter.Dispose();
            this.OutputMatter = null;
            //C# GC.Collect();
            //C# GC.WaitForPendingFinalizers();
            //C# GC.Collect();
            //C# GC.WaitForPendingFinalizers();
        } catch ( Exception e ) {
            throw new com.WDataSci.WDS.WDSException("Error in JniPMMLItem.PreDispose",e);
        }
    }

        public void Reset()
                throws com.WDataSci.WDS.WDSException
        {
            try {
            this.PreDispose();
            this.PMMLMatter = new __PMMLMatter();
            this.InputMatter = new __InputMatter();
            this.OutputMatter = new __OutputMatter();
            } catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in JniPMMLItem.Reset",e);
            }
        }

        public void Dispose()
                throws com.WDataSci.WDS.WDSException
        {
            this.PreDispose();
            this.ConfigMatter = null;
            /* C# >>> *
            if ( this.__JniPMML != null ) {
                try {
                    this.__JniPMML.__Java.CallVoidMethod("ItemDispose", "(I)V", new List<object> { this.__Handle });
                    this.__JniPMML.Remove(this);
                } catch (Exception e) {

                }
            }
            /* C# >>> */
        }
        /* C# >>> *
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
        //Java
        protected org.dmg.pmml.PMML Doc = null;
        //Java
        protected org.jpmml.evaluator.ModelEvaluator Evaluator = null;

        //C# public XmlDocument Doc = null;
        //C# public XmlDocument Evaluator = null;

        public String _XMLString = null;
        public String _XMLFileName = null;

        public void Dispose()
        {
            this.Doc = null;
            this.Evaluator = null;
        }
        /* C# >>> *
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

        //Java
        public Document _XSDDoc = null;
        //C# public XmlDocument _XSDDoc = null;
        public String _XSDFileName = null;
        public String _XSDString = null;
        public void Dispose()
        throws com.WDataSci.WDS.WDSException, Exception
        {
            this._XSDDoc = null;
            this._XSDFileName = null;
            this._XSDString = null;
            if (this.RecordSet!=null) this.RecordSet.Dispose();
            this.RecordSet = null;
            if (this.RecordSetMD!=null) this.RecordSetMD.Dispose();
            this.RecordSetMD = null;
        }
        /* C# >>> *
        ~__InputMatter()
        {
            this.Dispose();
        }
        /* <<< C# */
    }

    public class __OutputMatter
    {
        public RecordSetMD RecordSetMD = null;
        public RecordSet RecordSet = null;
        public void Dispose()
        throws com.WDataSci.WDS.WDSException, Exception
        {
            if (this.RecordSet!=null) this.RecordSet.Dispose();
            this.RecordSet = null;
            if (this.RecordSetMD!=null) this.RecordSetMD.Dispose();
            this.RecordSetMD = null;
        }
        /* C# >>> *
        ~__OutputMatter()
        {
            this.Dispose();
        }
        /* <<< C# */
    }

    /* Java >>> */
    //Default constructor
    public JniPMMLItem()
    {
        synchronized ( this ) {
            this.HandleMajor = 0;
            this.HandleMinor = 0;
            this.ConfigMatter.InternalString = "An internal state string";
        }
    }
    /* <<< Java */


    public JniPMMLItem(Integer[] Handle)
    {
        synchronized ( this ) {
            this.HandleMajor = Handle[0];
            this.HandleMinor = Handle[1];
            this.ConfigMatter.InternalString = "An internal state string";
        }
    }

    public JniPMMLItem(int HandleMajor, int HandleMinor)
    {
        synchronized ( this ) {
            this.HandleMajor = HandleMajor;
            this.HandleMinor = HandleMinor;
            this.ConfigMatter.InternalString = "An internal state string";
        }
    }

    public String PMMLLoadFromString(String arg)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            String lname = "PMMLLoadFromString";
            String rv = "Error in " + lname;
            try {
                if ( arg == null || arg.isEmpty() ) throw new WDSException("null or empty second argument to " + lname);
                this.Reset();
                this.PMMLMatter._XMLFileName = null;
                this.PMMLMatter._XMLString = arg;
                InputStream is = IOUtils.toInputStream(arg, Charset.forName("UTF-8"));
                this.PMMLMatter.Doc = PMMLUtil.unmarshal(is);
                is.close();
                this.HandleMinor += 1;
                rv = this.HandleMajor + "." + this.HandleMinor;
            }
            catch ( Exception e ) {
                throw new WDSException("could not parse PMML string in " + lname, e);
            }
            return rv;
        }
    }

    public String PMMLLoadFromFile(String arg)
    throws Exception
    {
        synchronized ( this ) {
            String lname = "PMMLLoadFromFile";
            String rv = "Error in " + lname;
            try {
                if ( arg == null || arg.isEmpty() ) throw new WDSException("null or empty second argument to " + lname);
                this.PMMLMatter._XMLString = Util.FetchFileAsString(arg);
                rv = PMMLLoadFromString(this.PMMLMatter._XMLString);
                this.PMMLMatter._XMLFileName = arg;
                this.HandleMinor += 1;
            }
            catch ( Exception e ) {
                throw new WDSException("could not load and parse PMML from file " + arg + " in " + lname, e);
            }
            return rv;
        }
    }

    public String PMMLLoadedString()
    {
        if ( this.PMMLMatter._XMLString != null || !this.PMMLMatter._XMLString.isEmpty() ) {
            return this.PMMLMatter._XMLString;
        }
        return "PMML File String is either not or unloaded";
    }

    public String PMMLLoadedFileName()
    {
        if ( this.PMMLMatter._XMLFileName != null || !this.PMMLMatter._XMLFileName.isEmpty() ) {
            return this.PMMLMatter._XMLFileName;
        }
        return "PMML either loaded directly via string, not loaded, or unloaded";
    }

    public DataField[] PMMLDataFields()
    throws com.WDataSci.WDS.WDSException
    {
        try {
            List<DataField> l;
            synchronized ( this ) {
                l = this.PMMLMatter.Doc.getDataDictionary().getDataFields();
            }
            int n = l.size();
            DataField[] lDataFields = new DataField[n];
            for ( int i = 0; i < n; i++ ) {
                lDataFields[i] = l.get(i);
            }
            return lDataFields;
        }
        catch ( Exception e ) {
            throw new WDSException("Error  pulling dictionary DataFields:", e);
        }
    }

    public FieldName[] PMMLDataFieldNames()
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                List<DataField> lDataFields = this.PMMLMatter.Doc.getDataDictionary().getDataFields();
                int n = lDataFields.size();
                FieldName[] rv = new FieldName[n];
                for ( int i = 0; i < n; i++ ) {
                    rv[i] = (FieldName) lDataFields.get(i).getName();
                }
                return rv;
            }
            catch ( Exception e ) {
                throw new WDSException("Error  pulling dictionary DataFields:", e);
            }
        }
    }

    public String[] PMMLDataFieldStringNames()
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                List<DataField> lDataFields = this.PMMLMatter.Doc.getDataDictionary().getDataFields();
                int n = lDataFields.size();
                String[] rv = new String[n];
                for ( int i = 0; i < n; i++ ) {
                    rv[i] = lDataFields.get(i).getName().toString();
                }
                return rv;
            }
            catch ( Exception e ) {
                throw new WDSException("Error pulling dictionary DataFields as Strings:", e);
            }
        }
    }

    public int mReadMapFromHDF5()
    throws com.WDataSci.WDS.WDSException
    {
        throw new WDSException("Error, H5FDCore file driver is not fully implemented in HDF-Group jni");
    }

    public Document mReadMapFromXSDString(String aInputSchemaString)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                this.InputMatter._XSDFileName = null;
                this.InputMatter._XSDString = aInputSchemaString;
                InputStream is = IOUtils.toInputStream(this.InputMatter._XSDString, Charset.forName("UTF-8"));
                this.InputMatter._XSDDoc = DocumentBuilderFactory.newInstance().newDocumentBuilder().parse(is);
            }
            catch ( Exception e ) {
                this.InputMatter._XSDString = null;
                this.InputMatter._XSDFileName = null;
                this.InputMatter._XSDDoc = null;
                throw new WDSException("Error parsing InputMap from XSD String:", e);
            }
            return this.InputMatter._XSDDoc;
        }
    }

    public Document mReadMapFromXSDFile(String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                this.InputMatter._XSDFileName = aFileName;
                this.InputMatter._XSDString = com.WDataSci.WDS.Util.FetchFileAsString(aFileName);
                return this.mReadMapFromXSDString(this.InputMatter._XSDString);
            }
            catch ( Exception e ) {
                this.InputMatter._XSDString = null;
                this.InputMatter._XSDFileName = null;
                this.InputMatter._XSDDoc = null;
                throw new WDSException("Error parsing InputMap from XSD file:", e);
            }
        }
    }

    public int mReadMapFromByteBuffer(ByteBuffer arg)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                this.InputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Input)
                        .cAs(RecordSetMDEnums.eType.DBB)
                        .cSetHeaderBufferFrom(arg)
                        .mReadMapFor(this, null, false)
                ;
                this.HandleMinor += 1;
                return 0;
            }
            catch ( Exception e ) {
                throw new WDSException("Error parsing InputMap from ByteBuffer:", e);
            }
        }
    }

    public int mReadMapFromByteBufferTest(ByteBuffer arg, String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                File fid = new File(aFileName);
                PrintWriter pw = new PrintWriter(fid);
                try {
                    pw.printf("%s\n", "hey");
                    pw.printf("isDirect=%s\n", String.valueOf(arg.isDirect()));
                    pw.printf("isReadOnly=%s\n", String.valueOf(arg.isReadOnly()));
                    pw.printf("capacity=%d\n", arg.capacity());
                    pw.printf("about to enter RecordSetMD constructor");
                    pw.flush();
                    this.mReadMapFromByteBuffer(arg);
                    pw.printf("exited RecordSetMD constructor");
                    pw.close();
                    return 0;
                }
                catch ( Exception e ) {
                    WDSException je = new WDSException("Error in InputMapLoadFromByteBufferTest:", e);
                    pw.printf(je.getMessage());
                    pw.close();
                    throw je;
                }
            }
            catch ( Exception e2 ) {
                throw new WDSException("Error in InputMapLoadFromByteBufferTest:", e2);
            }
        }
    }


    public String mMapCheck(String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                if ( this.InputMatter.RecordSetMD == null )
                    throw new WDSException("Error in InputMapCheck, InputMap has not been processed");
                boolean bUsingFile = !(aFileName == null || aFileName.isEmpty() || aFileName.equals("AsMessage"));
                StringWriter sw = null;
                PrintWriter pw = null;
                if ( bUsingFile ) {
                    File fid = new File(aFileName);
                    pw = new PrintWriter(fid);
                }
                else {
                    sw = new StringWriter();
                    pw = new PrintWriter(sw);
                }
                pw.printf("Number of input columns:%d\n", this.InputMatter.RecordSetMD.nColumns());
                pw.printf("Header Max String Byte Length:%d\n", this.InputMatter.RecordSetMD.DBBMatter.Header.MaxStringLength);
                for ( int i = 0; i < this.InputMatter.RecordSetMD.nColumns(); i++ ) {
                    FieldMD ic = this.InputMatter.RecordSetMD.Column[i];
                    pw.printf("Column[%d].Name=%s\n", i, ic.Name);
                    if ( ic.hasMapKey() )
                        pw.printf("   Column[%d].PMMLFieldStringName=%s\n", i, ic.MapKey.getValue());
                    else
                        pw.printf("   Column[%d] is not mapped to a PMML DataField\n", i);
                    pw.printf("   Column[%d].DTyp=%s\n", i, ic.DTyp);
                    pw.printf("   Column[%d].StringMaxLength=%d\n", i, ic.StringMaxLength);
                    pw.printf("   Column[%d].ByteMaxLength=%d\n", i, ic.ByteMaxLength);
                }
                String rv = null;
                if ( bUsingFile )
                    rv = "Output to File:" + aFileName;
                else
                    rv = sw.toString();
                pw.close();
                return rv;
            }
            catch ( Exception e ) {
                throw new WDSException("Error in InputMapCheck:", e);
            }
        }
    }


    public org.jpmml.evaluator.Evaluator PMMLEvaluator()
    {
        synchronized ( this ) {
            if ( this.PMMLMatter.Evaluator == null )
                this.PMMLMatter.Evaluator = new org.jpmml.evaluator.ModelEvaluatorBuilder(this.PMMLMatter.Doc).build();
            return this.PMMLMatter.Evaluator;
        }
    }

    //CodeNote, CJW:  Most of this is just for error checking and un-doing the generic return

    public List<Map<FieldName, Object>> PMMLEvaluate(RecordSet aInputRecordSet, boolean bAnySystemOut, boolean bVerboseOutput)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {

            org.jpmml.evaluator.Evaluator aEvaluator = this.PMMLEvaluator();

            List<Map<FieldName, ?>> output = new ArrayList<>(0);

            try {

                for ( int i = 0; i < aInputRecordSet.Records.size(); i++ ) {
                    Map<FieldName,?> outrec=null;
                    try {
                        outrec=aEvaluator.evaluate(aInputRecordSet.Records.get(i));
                    } catch (Exception e) {
                        //throw new com.WDataSci.WDS.WDSException("Error in evaluator, row(0-based)="+i, e);
                    }
                    output.add(outrec);
                }
                if ( bAnySystemOut )
                    System.out.printf("input aInputRecordSet.Records.size=%d output.size=%d\n", aInputRecordSet.Records.size(), output.size());

            }
            catch ( Exception e ) {
                throw new WDSException("Error in PMML evaluation:", e);
            }

            if ( output.size() != aInputRecordSet.Records.size() )
                throw new WDSException(String.format("Error in PMML evaluation, # of evaluated outputs does not match # of evaluated inputs! (%d<>%d)", output.size(), aInputRecordSet.Records.size()));

            Set<FieldName> ks = null;
            for ( int i=0; ks==null && i<output.size(); i++) {
                if (output.get(i)!=null)
                    ks = output.get(i).keySet();
            }
            if (ks==null){
                int hey=1;
            }

            if ( bVerboseOutput && ks.size() > 0 && ks.toArray()[0] != null ) {
                System.out.printf("Output\nFirst row has fields and types:\n");
                for ( FieldName fn : ks ) {
                    Object o = output.get(0).get(fn);
                    System.out.printf("        key=%s, type=%s\n", fn.toString(), o.getClass().getName());
                }
                System.out.printf("Row Recap,\n%s\n", ks.toString());
                for ( int i = 0; i < output.size(); i++ ) {
                    System.out.printf("    row %d, %s=%s\n", i, output.get(i).keySet(), output.get(i).values());
                    if ( false ) {
                        for ( FieldName fn : ks ) {
                            Object o = output.get(i).get(fn);
                            System.out.printf("        key=%s, type=%s, value=%s\n", fn.toString(), o, o.getClass().getName());
                        }
                    }
                }
            }

            try {
                //to get rid of List<Map<FieldName,?>>
                List<Map<FieldName, Object>> rv = new ArrayList<>();
                int nColumns = this.OutputMatter.RecordSetMD.nColumns();
                for ( int i = 0; i < output.size(); i++ ) {
                    Map<FieldName, Object> row = new LinkedHashMap<>();
                    if (output.get(i)==null){
                        if (ks==null) {
                            for (int j=0;j<this.OutputMatter.RecordSetMD.nColumns();j++)
                                row.put(this.OutputMatter.RecordSetMD.Column[j].MapKey,null);
                        } else {
                            for (FieldName fn : ks)
                                row.put(fn, null);
                        }
                    } else if ( ks.size() > 0 && ks.toArray()[0] != null ) {
                        for ( FieldName fn : ks ) {
                            row.put(fn, output.get(i).get(fn));
                        }
                    }
                    else if ( ks.size() == nColumns && nColumns == 1 ) {
                        Object x = output.get(i).values().toArray()[0];
                        if ( x instanceof ClusterAffinityDistribution ) {
                            ClusterAffinityDistribution xc = (ClusterAffinityDistribution) x;
                            row.put(this.OutputMatter.RecordSetMD.Column[0].MapKey, xc.getDisplayValue());
                        }
                    }
                    rv.add(row);
                }

                return rv;
            }
            catch ( Exception e ) {
                throw new WDSException("Error converting jpmml List<Map<FieldName,?>> to List<Map<FieldName,Object>>:", e);
            }
        }
    }


    public int mEvaluateRecordSetAndHoldResults(ByteBuffer arg
    )
    throws com.WDataSci.WDS.WDSException
    {
        if ( this.InputMatter.RecordSetMD == null )
            throw new com.WDataSci.WDS.WDSException("Error, cannot Evaluate RecordSet if no Input RecordSetMD has been set");
        synchronized ( this ) {

            int rc = -1;

            try {

                this.InputMatter.RecordSet = new RecordSet().cAsInput();
                this.InputMatter.RecordSetMD.cSetRecordSetBufferFrom(arg);
                //this.InputMatter.RecordSetMD.cSetRecordSetBufferAs(arg);
                this.InputMatter.RecordSet.mReadRecordSet(this.InputMatter.RecordSetMD);

                if ( this.InputMatter.RecordSet.Records.size() == 0 ) return 0;

                //Get and verify the evaluator
                org.jpmml.evaluator.Evaluator aJniPMMLEvaluator = this.PMMLEvaluator();
                aJniPMMLEvaluator.verify();

                //Evaluate the PMML on each row of the input set and returns a non-generic
                this.OutputMatter.RecordSet = new RecordSet(this.PMMLEvaluate(this.InputMatter.RecordSet, false, false));

                rc = this.OutputMatter.RecordSet.Records.size();

            }
            catch ( Exception e ) {
                throw new WDSException("Error when evaluating from ByteBuffer and holding results", e);
            }

            return rc;

        }
    }

    public int nRowsOfOutputRecordSet()
    {
        if ( this.OutputMatter.RecordSet == null ) return 0;
        if ( this.OutputMatter.RecordSet.Records == null ) return 0;
        return this.OutputMatter.RecordSet.Records.size();
    }

    public int nColumnsOfOutputRecordSet()
    {
        if ( this.OutputMatter.RecordSetMD != null ) return this.OutputMatter.RecordSetMD.nColumns();
        if ( this.OutputMatter.RecordSet == null ) return 0;
        if ( this.OutputMatter.RecordSet.Records == null ) return 0;
        if ( this.OutputMatter.RecordSet.Records.size() == 0 ) return 0;
        return this.OutputMatter.RecordSet.Records.get(0).keySet().size();
    }

    public int mPreRunPrepOutputMap(int nColumnNameMaxByteLength, int nStringMaxLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {

                int i = -1;
                int j = -1;
                int k = -1;
                int jj = -1;

                if (this.OutputMatter.RecordSetMD==null)
                    this.OutputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Output)
                        .cAs(RecordSetMDEnums.eType.DBB)
                        .cWithOutRepeatInputSet()
                ;
                //aOutputXDataMap.RepeatInputSetWithSuffix("Input", "-");
                //Get and verify the evaluator
                org.jpmml.evaluator.Evaluator aJniPMMLEvaluator = this.PMMLEvaluator();
                aJniPMMLEvaluator.verify();
                List<org.dmg.pmml.Model> mlist = this.PMMLMatter.Doc.getModels();
                org.dmg.pmml.Model m = mlist.get(0);

                int nTotalColumns = 0;
                boolean isUsingOutput=false;

                org.dmg.pmml.Output mo = m.getOutput();
                List<org.dmg.pmml.OutputField> mo_of_list = null;
                if ( mo != null && mo.hasOutputFields() ) {
                    isUsingOutput=true;
                    mo_of_list = mo.getOutputFields();
                    nTotalColumns += mo_of_list.size();
                }
                List<org.jpmml.evaluator.TargetField> tf = aJniPMMLEvaluator.getTargetFields();
                boolean hasMiningValueFields=false;
                if ( !isUsingOutput && tf != null && tf.size() > 0 ) {
                    for (i=0;i<tf.size();i++) {
                        nTotalColumns+=1;
                        /* TODO
                        org.jpmml.evaluator.TargetField ltf=tf.get(i);
                        if (ltf.getField().getValues()!=null) {
                            int nv = ltf.getField().getValues().size();
                            nTotalColumns += nv;
                            hasMiningValueFields=true;
                        }
                        */
                    }
                }


                j = 0; //column index across types
                this.OutputMatter.RecordSetMD.Column = new FieldMD[nTotalColumns];


                if ( mo_of_list != null && mo_of_list.size() > 0 ) {

                    org.dmg.pmml.OutputField[] ksa = new org.dmg.pmml.OutputField[mo_of_list.size()];
                    mo_of_list.toArray(ksa);

                    int nColumns = ksa.length;
                    this.OutputMatter.RecordSetMD.Column = new FieldMD[nColumns];
                    int nInputMap = this.InputMatter.RecordSetMD.nColumns();

                    for ( k = 0; k < nColumns; k++, j++ ) {
                        this.OutputMatter.RecordSetMD.Column[j] = new FieldMD();
                        this.OutputMatter.RecordSetMD.Column[j].RTyp = FieldMDEnums.eRTyp.Output;
                        this.OutputMatter.RecordSetMD.Column[j].Name = ksa[k].getName().toString();
                        this.OutputMatter.RecordSetMD.Column[j].MapToMapKey(ksa[k].getName());

                        org.dmg.pmml.OutputField of = mo_of_list.get(k);
                        org.dmg.pmml.DataType ofdtyp = of.getDataType();

                        //if the output field does not have a type, does it match an input field? This can happen with a result feature.
                        boolean found = false;
                        if ( ofdtyp == null ) {
                            for ( i = 0; !found && i < nInputMap; i++ ) {
                                if ( this.InputMatter.RecordSetMD.Column[i].hasMapKey() && this.InputMatter.RecordSetMD.Column[i].MapKey.getValue().equals(this.OutputMatter.RecordSetMD.Column[j].Name) ) {
                                    found = true;
                                    this.OutputMatter.RecordSetMD.Column[j].Copy(this.InputMatter.RecordSetMD.Column[i]);
                                    break;
                                }
                            }
                            if ( found && of.getResultFeature() != null ) {
                                this.OutputMatter.RecordSetMD.Column[j].Name = this.OutputMatter.RecordSetMD.Column[j].Name + this.OutputMatter.RecordSetMD.ModeMatter.CompositeNameDlm + of.getResultFeature().toString();
                            }
                        }
                        if ( !found ) {
                            //If not found as an input field or a feature of one, extract the rest of the X mapping info from the PMML
                            if ( of.getDataType().equals(org.dmg.pmml.DataType.DOUBLE) || of.getDataType().equals(org.dmg.pmml.DataType.FLOAT) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Dbl;
                            }
                            else if ( of.getDataType().equals(org.dmg.pmml.DataType.INTEGER) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Int;
                                //there may not be a long PMML output type, double check if field is named like an input long
                                for ( found = false, i = 0; !found && i < nInputMap; i++ ) {
                                    if ( this.OutputMatter.RecordSetMD.Column[i].hasMapKey() && this.OutputMatter.RecordSetMD.Column[i].MapKey.getValue().equals(this.OutputMatter.RecordSetMD.Column[j].Name) ) {
                                        found = true;
                                        if ( this.OutputMatter.RecordSetMD.Column[i].DTyp.equals(FieldMDEnums.eDTyp.Lng) ) {
                                            this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Lng;
                                        }
                                    }
                                }
                            }
                            else if ( of.getDataType().equals(org.dmg.pmml.DataType.DATE) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Dte;
                            }
                            else if ( of.getDataType().equals(org.dmg.pmml.DataType.DATE_TIME) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.DTm;
                            }
                            else if ( of.getDataType().equals(org.dmg.pmml.DataType.STRING) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.VLS;
                                this.OutputMatter.RecordSetMD.Column[j].StringMaxLength=nStringMaxLength;
                            }
                            else if ( of.getDataType().equals(org.dmg.pmml.DataType.BOOLEAN) ) {
                                throw new WDSException("Error, OutputColumn DataType for boolean not implemented!");
                            }
                            else {
                                throw new WDSException("Error, un-implemented OutputColumn DataType !");
                            }
                        }
                    }
                }


                if ( !isUsingOutput && tf != null && tf.size() > 0 ) {

                    org.jpmml.evaluator.TargetField[] ksa = new org.jpmml.evaluator.TargetField[tf.size()];
                    tf.toArray(ksa);

                    int nColumns = ksa.length;
                    int nInputMap = this.InputMatter.RecordSetMD.nColumns();

                    for ( k = 0; k < nColumns; k++, j++ ) {
                        this.OutputMatter.RecordSetMD.Column[j] = new FieldMD();
                        this.OutputMatter.RecordSetMD.Column[j].RTyp = FieldMDEnums.eRTyp.Target;
                        org.jpmml.evaluator.TargetField t = ksa[k];
                        String s = null;
                        try {
                            org.dmg.pmml.FieldName fn = t.getFieldName();
                            s = fn.getValue();
                            if ( s == null || s.length() == 0 ) s = t.getDisplayName();
                        }
                        catch ( Exception e ) {
                        }
                        if ( s == null || s.length() == 0 ) {
                            try {
                                org.dmg.pmml.DataField df = t.getField();
                                if ( df != null )
                                    s = df.getDisplayName();
                            }
                            catch ( Exception e ) {

                            }
                        }
                        if ( s == null || s.length() == 0 ) s = "Target" + k;
                        this.OutputMatter.RecordSetMD.Column[j].MapToMapKey(s);
                        if (s!=null && t.getMiningField()!=null) {
                            //s+="_result";
                            s+="_" + t.getMiningField().getUsageType().toString();
                        }
                        this.OutputMatter.RecordSetMD.Column[j].Name = s;
                        org.dmg.pmml.DataType ofdtyp = t.getDataType();

                        //if the output field does not have a type, does it match an input field? This can happen with a result feature.
                        boolean found = false;
                        if ( ofdtyp == null ) {
                            for ( i = 0; !found && i < nInputMap; i++ ) {
                                if ( this.InputMatter.RecordSetMD.Column[i].hasMapKey() && this.InputMatter.RecordSetMD.Column[i].MapKey.getValue().equals(this.OutputMatter.RecordSetMD.Column[j].Name) ) {
                                    found = true;
                                    this.OutputMatter.RecordSetMD.Column[j].Copy(this.InputMatter.RecordSetMD.Column[i]);
                                    break;
                                }
                            }
                        }
                        if ( !found ) {
                            //If not found as an input field or a feature of one, extract the rest of the X mapping info from the PMML
                            if ( ofdtyp.equals(org.dmg.pmml.DataType.DOUBLE) || ofdtyp.equals(org.dmg.pmml.DataType.FLOAT) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Dbl;
                            }
                            else if ( ofdtyp.equals(org.dmg.pmml.DataType.INTEGER) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Int;
                                //there may not be a long PMML output type, double check if field is named like an input long
                                for ( found = false, i = 0; !found && i < nColumns; i++ ) {
                                    if ( this.OutputMatter.RecordSetMD.Column[i].hasMapKey() && this.OutputMatter.RecordSetMD.Column[i].MapKey.getValue().equals(this.OutputMatter.RecordSetMD.Column[j].Name) ) {
                                        found = true;
                                        if ( this.OutputMatter.RecordSetMD.Column[i].DTyp.equals(FieldMDEnums.eDTyp.Lng) ) {
                                            this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Lng;
                                        }
                                    }
                                }
                            }
                            else if ( ofdtyp.equals(org.dmg.pmml.DataType.DATE) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.Dte;
                            }
                            else if ( ofdtyp.equals(org.dmg.pmml.DataType.DATE_TIME) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.DTm;
                            }
                            else if ( ofdtyp.equals(org.dmg.pmml.DataType.STRING) ) {
                                this.OutputMatter.RecordSetMD.Column[j].DTyp = FieldMDEnums.eDTyp.VLS;
                                this.OutputMatter.RecordSetMD.Column[j].StringMaxLength=nStringMaxLength;
                            }
                            else if ( ofdtyp.equals(org.dmg.pmml.DataType.BOOLEAN) ) {
                                throw new WDSException("Error, OutputColumn TargetField DataType for boolean not implemented!");
                            }
                            else {
                                throw new WDSException("Error, un-implemented OutputColumn DataType !");
                            }
                        }
                        /* TODO
                        if (hasMiningValueFields) {
                            if (t.getField().getValues()!=null) {
                                List<org.dmg.pmml.Value> v_list=t.getField().getValues();
                                int nv = v_list.size();
                                for (i=0;i<nv;i++) {
                                    j++;
                                    this.OutputMatter.RecordSetMD.Column[j] = new FieldMD();
                                    this.OutputMatter.RecordSetMD.Column[j].RTyp = FieldMDEnums.eRTyp.Target;
                                    this.OutputMatter.RecordSetMD.Column[j].Name=s+"_entry_"+i;
                                    this.OutputMatter.RecordSetMD.Column[j].DTyp= FieldMDEnums.eDTyp.Dbl;
                                }
                            }
                        }
                        */

                    }
                }


                this.OutputMatter.RecordSetMD.mColumnConsistency();
                return this.OutputMatter.RecordSetMD.nColumns();
            }
            catch ( Exception e ) {
                throw new WDSException("Error parsing InputMap from ByteBuffer:", e);
            }
        }
    }

    public String mPreRunWriteOutputMapToByteBuffer(ByteBuffer arg, int nColumnNameMaxByteLength, int nStringMaxLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {
                if ( this.OutputMatter == null || this.OutputMatter.RecordSetMD == null )
                    this.mPreRunPrepOutputMap(nColumnNameMaxByteLength, nStringMaxLength);
                this.OutputMatter.RecordSetMD.cSetHeaderBufferAs(arg, this.OutputMatter.RecordSetMD.nColumns(), 40, 2 * nColumnNameMaxByteLength);
                this.OutputMatter.RecordSetMD.mWriteMapToBuffer();
                return this.HandleMajor + "." + this.HandleMinor;
            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error parsing InputMap from ByteBuffer:", e);
            }
        }
    }

    public String mWriteOutputMapToByteBuffer(ByteBuffer arg, int nColumns, int nColumnNameMaxByteLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {
            try {

                this.OutputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Output)
                        .cAs(RecordSetMDEnums.eType.DBB)
                        .cWithOutRepeatInputSet()
                ;
                //aOutputXDataMap.RepeatInputSetWithSuffix("Input", "-");
                //Prepare the base map
                this.OutputMatter.RecordSetMD.mPrepForOutput(this.InputMatter.RecordSetMD, this, this.OutputMatter.RecordSet.Records);

                //Set the buffer which has been allocated outside
                this.OutputMatter.RecordSetMD.cSetHeaderBufferAs(arg, nColumns, 40, 2 * nColumnNameMaxByteLength);

                //Write to the buffer
                //this.OutputMatter.RecordSetMD.WriteOutputMapToBuffer(arg, nColumns, nColumnNameMaxByteLength);
                this.OutputMatter.RecordSetMD.mWriteMapToBuffer();

                return this.HandleMajor + "." + this.HandleMinor;

            }
            catch ( Exception e ) {
                throw new WDSException("Error parsing InputMap from ByteBuffer:", e);
            }
        }
    }

    public int mWriteOutputRecordSetToByteBuffer(ByteBuffer arg, int nRows
            , long nRecordCoreLength, long nRecordVariableLength
            , long nCoreLength, long nTotalLength
    )
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized ( this ) {
            try {

                if ( this.OutputMatter.RecordSetMD == null || this.OutputMatter.RecordSetMD.DBBMatter.Header.Buffer == null )
                    throw new WDSException("Error int OutputSetWriteToByteBuffer, output map or header buffer not set");

                //Set the buffer which has been allocated outside
                this.OutputMatter.RecordSetMD.cSetRecordSetBufferAs(arg, nRows, nRecordCoreLength, nRecordVariableLength, nCoreLength, nTotalLength);

                //this.OutputMatter.RecordSetMD.mPrepForOutput(this.InputMatter.RecordSetMD, this, this.OutputMatter.RecordSet.Records);

                this.OutputMatter.RecordSet.mWriteRecordSet(this.OutputMatter.RecordSetMD, this.InputMatter.RecordSetMD, this.InputMatter.RecordSet);

                return 0;
            }
            catch ( Exception e ) {
                throw new WDSException("Error writing Output RecordSet to ByteBuffer:", e);
            }
        }
    }

    public int mEvaluateRecordSetWithFileOutput(ByteBuffer arg
            , String aFileName
            , String aFileType
            , int OutputHDF5FixedStringLength
    )
    throws com.WDataSci.WDS.WDSException
    {
        synchronized ( this ) {

            int i = -1;
            int j = -1;
            int k = -1;
            int rc = -1;

            try {

                rc = this.mEvaluateRecordSetAndHoldResults(arg);

                this.OutputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Output);
                RecordSetMDEnums.eType outtype = RecordSetMDEnums.eType.FromAlias(aFileType);

                if ( outtype.bIn(RecordSetMDEnums.eType.CSV, RecordSetMDEnums.eType.Dlm) )
                    this.OutputMatter.RecordSetMD.cAsDlmFile(aFileName, ",").cWithHeaderRow();
                else if ( outtype.bIn(RecordSetMDEnums.eType.TXT) )
                    this.OutputMatter.RecordSetMD.cAsDlmFile(aFileName, "\t").cWithHeaderRow();
                else if ( outtype.bIn(RecordSetMDEnums.eType.HDF5) )
                    this.OutputMatter.RecordSetMD.cAs(outtype).cToFile(aFileName).cWithDataSetName("OutputRecordSet");
                else
                    throw new WDSException("RecordSetMD output type not available in this call!");

                this.OutputMatter.RecordSetMD.cRepeatInputSetWithSuffix("Input", "-");

                //HDF5's limitation to fixed string Length issue....
                if ( outtype.bIn(RecordSetMDEnums.eType.HDF5) )
                    this.OutputMatter.RecordSetMD.ModeMatter.OutputMaxStringLength = OutputHDF5FixedStringLength;

                this.OutputMatter.RecordSetMD.mPrepForOutput(this.InputMatter.RecordSetMD, this, this.OutputMatter.RecordSet.Records);

                this.OutputMatter.RecordSet.mWriteRecordSet(this.OutputMatter.RecordSetMD, this.InputMatter.RecordSetMD, this.InputMatter.RecordSet);

            }
            catch ( Exception e ) {
                WDSException je = new WDSException("Error", e);
                System.out.printf("%s\n", je.getMessage());
                throw je;
            }

            return rc;
        }
    }

}
