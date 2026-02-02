package com.WDataSci.JniPMML;

import org.dmg.pmml.Field;
import org.w3c.dom.Document;

import java.nio.ByteBuffer;
import java.util.*;

/**
 * <p>JniPMML is the main bridge between callers (at the moment, either from C# or a Java command line) and a wrap
 * around a PMML document and its evaluator.
 * To allow a caching of multiple documents and input/output maps, JniPMML contains a collection of JniPMMLItems
 * which can be called by handle or the last one used.
 * <p>The intent is to pass data frame like blocks of data as inputs for evaluation.  Because of incomplete interfaces
 * in HDF5, this is done via a ByteBuffer from C#, but Wranglers for FlatFiles, HDF5, DBB (Direct ByteBuffer) and
 * others are or will be implemented.
 * </p>
 */
public class JniPMML
{
    static {
        System.loadLibrary("hdf5_java");
    }

    //Handle includes Major/Minor and treated as an array, but Minor is maintained by individual Items
    protected Integer[] Handle = {-1, -1};

    //The Items are indexed only by Handle[MAJOR=0];
    protected List<JniPMMLItem> Items = new ArrayList<>(0);

    //Default constructor
    public JniPMML() { }

    public boolean isValidHandle(int arg)
    {
        return (arg >= 0 && arg < this.Items.size() && this.Items.get(arg) != null);
    }

    public int HandleMajor()
    {
        return this.Handle[0];
    }

    public int HandleMajor(int arg)
            throws com.WDataSci.WDS.WDSException
    {
        try {
            synchronized (this) {
                //  if (true) return -3;
                //if (true) throw new com.WDataSci.WDS.WDSException("HEY--"+this.toString());
                /* a value of -1 is used to request a new value */
                if (arg < 1) {
                    arg = this.ItemNew().HandleMajor;
                    //arg = this.Items.size() - 1;
                }
                if (arg > this.Items.size() && arg < this.Items.size() + 10) {
                    for (int i = this.Items.size(); i < arg + 1; i++)
                        this.ItemNew();
                }
                if (this.Items.get(arg) == null) this.Items.set(arg, new JniPMMLItem());
                return this.Items.get(arg).HandleMajor;
            }
        } catch (Exception e) {
            throw new com.WDataSci.WDS.WDSException("Error in HandleMajor",e);
        }
    }

    public String Handle()
    {
        return this.Handle[0] + "." + this.Handle[1];
    }

    public String Handle(int arg)
            throws com.WDataSci.WDS.WDSException
    {
        this.HandleMajor(arg);
        return this.Items.get(arg).HandleMajor + "." + this.Items.get(arg).HandleMinor;
    }

    public int HandleMinor()
            throws com.WDataSci.WDS.WDSException
    {
        return this.Items.get(this.Handle[0]).HandleMinor;
    }

    public int HandleMinor(int arg)
            throws com.WDataSci.WDS.WDSException
    {
        this.HandleMajor(arg);
        return this.Items.get(arg).HandleMinor;
    }

    public int HandleNext()
            throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            if (this.Handle[0]<0 || this.Items.size()==0) return 0;
            int rv = this.Items.size();
            return rv;
            //return this.Items.size() + 1;
            //return this.Handle[0] + 1;
        }
    }

    public JniPMMLItem ItemNew()
            throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            this.Handle[0] = this.HandleNext();
            this.Handle[1] = -1;
            this.Items.add(new JniPMMLItem(this.Handle));
            return this.Items.get(this.Handle[0]);
        }
    }

    public int ItemNewHandle()
            throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            this.ItemNew();
            return this.Handle[0].intValue();
        }
    }


    public JniPMMLItem GetItem()
            throws com.WDataSci.WDS.WDSException
    {
        if ( this.Handle[0] < 0 )
            return this.ItemNew();
        return this.Items.get(this.Handle[0]);
    }

    public JniPMMLItem GetItem(int arg)
            throws com.WDataSci.WDS.WDSException
    {
        if (arg<0 || arg>=this.Items.size())
            throw new com.WDataSci.WDS.WDSException("Error, JniPMML Item "+arg+" not available");
        JniPMMLItem rv = this.Items.get(arg);
        if (rv==null)
            throw new com.WDataSci.WDS.WDSException("Error, JniPMML Item "+arg+" was null");
        this.Handle[0] = arg;
        this.Handle[1] = rv.HandleMinor;
        return rv;
    }

    public void ItemDispose(int arg)
        throws com.WDataSci.WDS.WDSException
    {
        try {
            if (this.Items==null)
                throw new com.WDataSci.WDS.WDSException("Error, JniPMML.Items not initialized");
            if ( arg < 0 || arg >= this.Items.size() ) return;
            if ( this.Items.get(arg) != null ) this.Items.get(arg).Dispose();
            //this.Items.set(arg, null);
        } catch (Exception e) {
            throw new com.WDataSci.WDS.WDSException("Error in ItemDispose",e);
        }
    }


    public String mPMMLLoadFromString(String arg)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized (this) {
            return this.ItemNew().PMMLLoadFromString(arg);
        }
    }

    public String mPMMLLoadFromString(int arg0, String arg)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized (this) {
            return this.GetItem(arg0).PMMLLoadFromString(arg);
        }
    }

    public String mPMMLLoadFromFile(String arg)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized (this) {
            return this.ItemNew().PMMLLoadFromFile(arg);
        }
    }

    public String mPMMLLoadFromFile(int arg0, String arg)
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized (this) {
            return this.GetItem(arg0).PMMLLoadFromFile(arg);
        }
    }

    public String sPMMLLoadedString()
    throws com.WDataSci.WDS.WDSException { return this.GetItem().PMMLLoadedString(); }

    public String sPMMLLoadedString(int arg0)
    throws com.WDataSci.WDS.WDSException { return this.GetItem(arg0).PMMLLoadedString(); }

    public String sPMMLLoadedFileName()
    throws com.WDataSci.WDS.WDSException { return this.GetItem().PMMLLoadedFileName(); }

    public String sPMMLLoadedFileName(int arg0)
    throws com.WDataSci.WDS.WDSException { return this.GetItem(arg0).PMMLLoadedFileName(); }

    public int mReadMapFromHDF5()
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mReadMapFromHDF5();
        }
    }

    public Document mReadMapFromXSDString(String aInputSchemaString)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mReadMapFromXSDString(aInputSchemaString);
        }
    }

    public Document mReadMapFromXSDString(int arg0, String aInputSchemaString)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mReadMapFromXSDString(aInputSchemaString);
        }
    }

    public Document mReadMapFromXSDFile(String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mReadMapFromXSDFile(aFileName);
        }
    }

    public Document mReadMapFromXSDFile(int arg0, String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mReadMapFromXSDFile(aFileName);
        }
    }

    public int mReadMapFromByteBuffer(ByteBuffer arg)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mReadMapFromByteBuffer(arg);
        }
    }

    public String mReadMapFromByteBuffer(int arg0, ByteBuffer arg)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            JniPMMLItem aJniPMMLItem = this.GetItem(arg0);
            int rc = aJniPMMLItem.mReadMapFromByteBuffer(arg);
            return aJniPMMLItem.HandleMajor + "." + aJniPMMLItem.HandleMinor;
        }
    }

    public int mReadMapFromByteBufferTest(ByteBuffer arg, String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mReadMapFromByteBufferTest(arg, aFileName);
        }
    }

    public int mReadMapFromByteBufferTest(int arg0, ByteBuffer arg, String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mReadMapFromByteBufferTest(arg, aFileName);
        }
    }


    public String mMapCheck(String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mMapCheck(aFileName);
        }
    }

    public String mMapCheck(int arg0, String aFileName)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mMapCheck(aFileName);
        }
    }


    public org.jpmml.evaluator.Evaluator PMMLEvaluator()
            throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().PMMLEvaluator();
        }
    }

    public org.jpmml.evaluator.Evaluator PMMLEvaluator(int arg0)
            throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).PMMLEvaluator();
        }
    }

    //CodeNote, CJW:  Most of this is just for error checking and un-doing the generic return
    public List<Map<String, Object>> PMMLEvaluate(RecordSet aInputRecordSet, boolean bAnySystemOut, boolean bVerboseOutput)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().PMMLEvaluate(aInputRecordSet, bAnySystemOut, bVerboseOutput);
        }
    }

    public List<Map<String, Object>> PMMLEvaluate(int arg0, RecordSet aInputRecordSet, boolean bAnySystemOut, boolean bVerboseOutput)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).PMMLEvaluate(aInputRecordSet, bAnySystemOut, bVerboseOutput);
        }
    }


    public int mEvaluateRecordSetAndHoldResults(ByteBuffer arg
    )
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mEvaluateRecordSetAndHoldResults(arg);
        }
    }

    public int mEvaluateRecordSetAndHoldResults(int arg0, ByteBuffer arg
    )
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mEvaluateRecordSetAndHoldResults(arg);
        }
    }

    public int nRowsOfOutputRecordSet()
    throws com.WDataSci.WDS.WDSException { return this.GetItem().nRowsOfOutputRecordSet(); }

    public int nRowsOfOutputRecordSet(int arg0)
    throws com.WDataSci.WDS.WDSException { return this.GetItem(arg0).nRowsOfOutputRecordSet(); }

    public int nColumnsOfOutputRecordSet()
    throws com.WDataSci.WDS.WDSException { return this.GetItem().nColumnsOfOutputRecordSet(); }

    public int nColumnsOfOutputRecordSet(int arg0)
    throws com.WDataSci.WDS.WDSException { return this.GetItem(arg0).nColumnsOfOutputRecordSet(); }

    public String mWriteOutputMapToByteBuffer(ByteBuffer arg, int nColumns, int nColumnNameMaxByteLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mWriteOutputMapToByteBuffer(arg, nColumns, nColumnNameMaxByteLength);
        }
    }

    public int mPreRunPrepOutputMap(int nColumnNameMaxByteLength, int nStringMaxLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mPreRunPrepOutputMap(nColumnNameMaxByteLength, nStringMaxLength);
        }
    }

    public int mPreRunPrepOutputMap(int arg0, int nColumnNameMaxByteLength, int nStringMaxLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mPreRunPrepOutputMap(nColumnNameMaxByteLength, nStringMaxLength);
        }
    }

    public String mPreRunWriteOutputMapToByteBuffer(ByteBuffer arg, int nColumnNameMaxByteLength, int nStringMaxLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mPreRunWriteOutputMapToByteBuffer(arg, nColumnNameMaxByteLength, nStringMaxLength);
        }
    }

    public String mPreRunWriteOutputMapToByteBuffer(int arg0, ByteBuffer arg, int nColumnNameMaxByteLength, int nStringMaxLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mPreRunWriteOutputMapToByteBuffer(arg, nColumnNameMaxByteLength, nStringMaxLength);
        }
    }

    public String mWriteOutputMapToByteBuffer(int arg0, ByteBuffer arg, int nColumns, int nColumnNameMaxByteLength)
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mWriteOutputMapToByteBuffer(arg, nColumns, nColumnNameMaxByteLength);
        }
    }


    public int mWriteOutputRecordSetToByteBuffer(ByteBuffer arg, int nRows
            , long nRecordCoreLength, long nRecordVariableLength
            , long nCoreLength, long nTotalLength
    )
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized (this) {
            return this.GetItem().mWriteOutputRecordSetToByteBuffer(arg, nRows, nRecordCoreLength, nRecordVariableLength, nCoreLength, nTotalLength);
        }
    }

    public int mWriteOutputRecordSetToByteBuffer(int arg0, ByteBuffer arg, int nRows
            , long nRecordCoreLength, long nRecordVariableLength
            , long nCoreLength, long nTotalLength
    )
    throws com.WDataSci.WDS.WDSException, Exception
    {
        synchronized (this) {
            return this.GetItem(arg0).mWriteOutputRecordSetToByteBuffer(arg, nRows, nRecordCoreLength, nRecordVariableLength, nCoreLength, nTotalLength);
        }
    }

    public int mEvaluateRecordSetWithFileOutput(ByteBuffer arg
            , String aFileName
            , String aFileType
            , int OutputHDF5FixedStringLength
    )
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem().mEvaluateRecordSetWithFileOutput(arg, aFileName, aFileType, OutputHDF5FixedStringLength);
        }
    }

    public int mEvaluateRecordSetWithFileOutput(int arg0, ByteBuffer arg
            , String aFileName
            , String aFileType
            , int OutputHDF5FixedStringLength
    )
    throws com.WDataSci.WDS.WDSException
    {
        synchronized (this) {
            return this.GetItem(arg0).mEvaluateRecordSetWithFileOutput(arg, aFileName, aFileType, OutputHDF5FixedStringLength);
        }
    }

    public static String mCmdArgsRecap()
    {
        CmdArgs args = new CmdArgs();
        return args.mRecapParameters();
    }

    public static String mCmdRun(String arg)
    {
        List<String> largs = new ArrayList<>(0);
        String[] args = arg.trim().split("--");
        for (int i = 0; i < args.length; i++) {
            args[i]=args[i].trim();
            if (args[i].length()>0) {
                int j = args[i].indexOf(' ');
                if ( j < 0 ) {
                    largs.add("--" + args[i]);
                }
                else {
                    largs.add("--" + args[i].substring(0, j).trim());
                    largs.add(args[i].substring(j + 1).trim());
                }
            }
        }
        args = new String[largs.size()];
        args=largs.toArray(args);
        try {
            Cmd.main(args);
            return "Successfull";
        } catch (com.WDataSci.WDS.WDSException e) {
            String rv="";
            for (int i=0;i<args.length;i++) rv+="||"+args[i];
            return e.getMessage() + "\n" + arg + "||" +rv;
        } catch (Exception e) {
            String rv="";
            for (int i=0;i<args.length;i++) rv+="||"+args[i];
            return e.getMessage() + "\n" + arg + "||" +rv;
        }
    }

}

