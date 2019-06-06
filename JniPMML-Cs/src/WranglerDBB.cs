/* Java >>> *
package com.WDataSci.JniPMML;


import com.WDataSci.WDS.WDSException;
import org.dmg.pmml.FieldName;

import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.Map;
/* <<< Java */
/* C# >>> */
using System;
using System.Collections.Generic;

using MOIE=Microsoft.Office.Interop.Excel;

using static com.WDataSci.WDS.JavaLikeExtensions;
using com.WDataSci.WDS;

namespace com.WDataSci.JniPMML
{
    /* <<< C# */
    public class WranglerDBB
    {

        public __DBBMatter Header = null;
        public __DBBMatter RecordSet = null;

        public WranglerDBB()
        //throws WDSException
        {
            this.Header = new __DBBMatter();
            this.RecordSet = new __DBBMatter();
        }
        public void Dispose() {
            if (this.Header!=null) this.Header.Dispose();
            this.Header = null;
            if (this.RecordSet!=null) this.RecordSet.Dispose();
            this.RecordSet = null;
        }

        /* C# >>> */
        ~WranglerDBB()
        {
            this.Header = null;
            this.RecordSet = null;
        }
        /* <<< C# */

        public class __DBBMatter
        {
            public DBB Buffer = null;
            public long MaxStringLength = 64; //place holder until it can be passed in as an attribute or an option
            public long MaxStringByteLength = 128; //place holder until it can be passed in as an attribute or an option
            public Boolean bIsManagedInJava = false;
            public void Dispose()
            {
                this.Buffer = null;
            }
            /* C# >>> */
            ~__DBBMatter()
            {
                this.Dispose();
            }
            /* <<< C# */
        }

        public Boolean isValid()
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            if ( !this.Header.Buffer.isValid() ) return false;
            if ( !this.RecordSet.Buffer.isValid() ) return false;
            return true;
        }


        public WranglerDBB cSetHeaderBufferAs(DBB arg, int nRecords, int nRecordCoreLength, int nRecordVariableLength)
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.Header.Buffer = new DBB(arg, true)
                    .cAsUsualLayout("WDSH", DBB.Default.nLeadingBytes, nRecords, nRecordCoreLength, nRecordVariableLength);
            //The usual header has only two string;
            this.Header.MaxStringByteLength = nRecordVariableLength / 2;
            this.Header.MaxStringLength = this.Header.MaxStringByteLength / 2;
            arg.isValid();
            return this;
        }

        public WranglerDBB cSetHeaderBufferFrom(DBB arg)
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.Header.Buffer = null;
            this.Header.Buffer = arg;
            this.Header.Buffer.cReadExistingLayout();
            if ( !this.Header.Buffer.LayoutStyle.equals("WDSH") )
                throw new com.WDataSci.WDS.WDSException("Error, layout for Header must be WDSH");
            this.Header.Buffer.isValid();
            this.Header.MaxStringByteLength = this.Header.Buffer.nRecordVLenBytes / 2;
            this.Header.MaxStringLength = this.Header.MaxStringByteLength / 2;
            return this;
        }

        public WranglerDBB cSetRecordSetBufferAs(DBB arg)
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.RecordSet.Buffer = null;
            this.RecordSet.Buffer = new DBB(arg, true);
            this.RecordSet.Buffer.isValid();
            return this;
        }

        public WranglerDBB cSetRecordSetBufferAs(DBB arg, long nRecords, long nRecordCoreLength,
                                                  long nRecordVariableLength, long nCoreLength, long nTotalLength)
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.RecordSet.Buffer = null;
            this.RecordSet.Buffer = new DBB(arg, true);
            this.RecordSet.Buffer.cAsUsualLayout("WDSD", DBB.Default.nLeadingBytes, nRecords, nRecordCoreLength, nRecordVariableLength);
            this.RecordSet.Buffer.isValid();
            return this;
        }

        public WranglerDBB cSetRecordSetBufferFrom(DBB arg)
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            if ( this.Header == null )
                throw new com.WDataSci.WDS.WDSException("Error, DBB Wrangler Header must be provided before RecordSetBuffer can be set");
            this.RecordSet.Buffer = null;
            this.RecordSet.Buffer = arg;
            this.RecordSet.Buffer.cReadExistingLayout();
            if ( !this.RecordSet.Buffer.LayoutStyle.equals("WDSD") )
                throw new com.WDataSci.WDS.WDSException("Error, layout for RecordSet must be WDSD");
            this.RecordSet.Buffer.isValid();
            return this;
        }

        public void mReadMap(RecordSetMD aRecordSetMD, JniPMMLItem aJniPMML, PrintWriter pw, Boolean bFillDictionaryNames)
        //throws com.WDataSci.WDS.WDSException
        {
            try {

                int i = -1;
                int ii = -1;
                int j = -1;
                int jj = -1;
                int k = -1;
                int kk = -1;


                //Java Boolean bUsingPMML = (aJniPMML != null && aJniPMML.PMMLMatter.Doc != null);
                //C#
                Boolean bUsingPMML = false;

                String[] lFieldStringNames = null;
                int nDataFieldNames = 0;
                if ( bUsingPMML ) {
                    lFieldStringNames = aJniPMML.PMMLDataFieldStringNames();
                    nDataFieldNames = lFieldStringNames.Length;
                }

                if ( pw != null ) pw.printf("In RecordSetMD constructor\n");
                if ( pw != null ) pw.flush();

                //point to file or memory
                DBB buffer = this.Header.Buffer;
                buffer.position(0, 0, 0);

                try {

                    //get compound dataset information

                    long nColumns = buffer.nRecords;
                    long nBlockMaxStringByteLength = this.Header.MaxStringByteLength;

                    int nBlockCoreColumnSize = (int) (buffer.nRecordFLenBytes);
                    int nBlockAllocatedSize = (int) (buffer.nDBBRequiredBytes);

                    if ( nBlockAllocatedSize > this.Header.Buffer.Length )
                        throw new com.WDataSci.WDS.WDSException("Error, HeaderBuffer capacity, " + this.Header.Buffer.Length
                                                                        + ", is less then what should be BlockAllocatedSize, " + nBlockAllocatedSize
                        );

                    int nBlockCoreSize = (int) (buffer.nDBBLeadingBytes + buffer.nDBBFLenBytes);

                    if ( pw != null ) pw.printf("In RecordSetMD constructor, nColumns=%d\n", nColumns);
                    if ( pw != null ) pw.flush();

                    byte[] namebuffer = new byte[(int) (nBlockMaxStringByteLength)];


                    //iterate through columns (dataset members)

                    aRecordSetMD.Column = new FieldMD[(int) (nColumns)];

                    int bptr = 0;

                    for ( ii = 0 ; ii < nColumns ; ii++, bptr += (int) nBlockCoreColumnSize ) {

                        buffer.position(buffer.ptr, bptr, buffer.vlenptr);

                        aRecordSetMD.Column[ii] = new FieldMD();
                        FieldMD col = aRecordSetMD.Column[ii];

                        //the first two fields are always names and taken to be variable length
                        col.Name = buffer.GetLayerVLenString(1, nBlockMaxStringByteLength);

                        //Check for PMML DataFieldName map
                        //If not mapped externally, the next VLenString pointer will be 0 and will come back as empty
                        String tmpname = buffer.GetLayerVLenString(1, nBlockMaxStringByteLength);

                        //Search for PMML DataFieldName map, take input supplied map first, then the usual search
                        /* Java >>> *
                        Boolean found = false;
                        if ( bUsingPMML && tmpname.length() > 0 ) {
                            for (j = 0; !found && j < nDataFieldNames; j++) {
                                if ( tmpname.equals(lFieldStringNames[j]) ) {
                                    col.MapToMapKey(lFieldStringNames[j]);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if ( bUsingPMML && !found ) {
                            for (j = 0; !found && j < nDataFieldNames; j++) {
                                if ( col.Name.equals(lFieldStringNames[j]) ) {
                                    col.MapToMapKey(lFieldNames[j]);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        /* <<< Java */
                        if ( !bUsingPMML && bFillDictionaryNames ) {
                            col.MapToMapKey(col.Name);
                        }

                        //Java col.DTyp = FieldMDEnums.eDTyp.FromInt(buffer.GetLayerInt(1));
                        //C#
                        col.DTyp = FieldMDExt.eDTyp_FromInt((int) buffer.GetLayerInt(1));

                        //See notes above on Date and DateTime types
                        //Since the header block is being passed in, Date and DateTime types are provided.

                        col.ByteMemLength = (long) buffer.GetLayerLong(1);
                        col.ByteMaxLength = (long) buffer.GetLayerLong(1);
                        if ( col.DTyp.bIn(FieldMDEnums.eDTyp.VLS, FieldMDEnums.eDTyp.Str) )
                            col.StringMaxLength = (int) (col.ByteMaxLength / 2);

                        //no longer using the last byte for bIsMappedToPMMLFieldName, so there is filler space at the end

                    }

                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error in RecordSetMD processing of Header DBB:", e);
                }

                if ( pw != null ) pw.printf("leaving RecordSetMD constructor\n");
                if ( pw != null ) pw.flush();

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in ProcessInputMapFor", e);
            }
        }


        public void mReadRecordSet(RecordSetMD aInputRecordSetMD, RecordSet aInputRecordSet, PrintWriter pw)
        //throws WDSException
        {
            int ri = 0;
            try {
                if ( aInputRecordSet.isEmpty() ) {
                    //Java aInputRecordSet.Records = new ArrayList<>(0);
                    //C#
                    aInputRecordSet.Records = new List<Map<FieldName, Object>>(0);
                    //Java aInputRecordSet.Records_Orig = new ArrayList<>(0);
                    //C#
                    aInputRecordSet.Records_Orig = new List<Object []>(0);
                }
                int nColumns = aInputRecordSetMD.nColumns();
                int i = -1;
                int j = -1;
                int jj = -1;

                DBB buf = aInputRecordSetMD.DBBMatter.RecordSet.Buffer;

                int nRows = (int) aInputRecordSetMD.DBBMatter.RecordSet.Buffer.nRecords;

                /* Java >>> *
                Double lvd = 0.0;
                Long lvl = 0L;
                Integer lvi = 0;
                Boolean lvbln = false;
                /* <<< Java */
                /* C# >>> */
                double? lvd = 0.0;
                long? lvl = 0L;
                int? lvi = 0;
                Boolean? lvbln = false;
                /* <<< C# */
                String lvs = "";

                int bptr = 0;
                for ( i = 0 ; i < nRows ; i++, bptr += (int) aInputRecordSetMD.DBBMatter.RecordSet.Buffer.nRecordFLenBytes ) {
                    ri = i;
                    buf.position(buf.ptr, bptr, buf.vlenptr);

                    Object[] inputRow_orig = new Object[nColumns];
                    //Java Map<FieldName, Object> inputRow = new LinkedHashMap<>();
                    //C#
                    Map<FieldName, Object> inputRow = new Map<FieldName, Object>();
                    for ( jj = 0 ; jj < nColumns ; jj++ ) {
                        switch ( aInputRecordSetMD.Column[jj].DTyp ) {
                            //case Dbl:
                            case FieldMDEnums.eDTyp.Dbl:
                                lvd = buf.GetLayerDouble(1);
                                //Java if (lvd==null || lvd.isNaN() || Double.isInfinite(lvd) ) lvd=null;
                                //C#
                                if ( lvd == null || Double.IsNaN(lvd.Value) || lvd == Double.MinValue || lvd == Double.MaxValue
                                    || Double.IsNegativeInfinity(lvd.Value) || Double.IsPositiveInfinity(lvd.Value) ) lvd = null;
                                inputRow_orig[jj] = lvd;
                                if ( aInputRecordSetMD.Column[jj].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[jj].MapKey, lvd);
                                break;
                            //case Lng:
                            case FieldMDEnums.eDTyp.Lng:
                                lvl = buf.GetLayerLong(1);
                                //Java if ( lvl == null || lvl.equals(Long.MIN_VALUE) || lvl.equals(Long.MAX_VALUE) ) lvl = null;
                                //C#
                                if ( lvl == null || lvl == long.MinValue || lvl == long.MaxValue ) lvl = null;
                                inputRow_orig[jj] = lvl;
                                if ( aInputRecordSetMD.Column[jj].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[jj].MapKey, lvl);
                                break;
                            //case Int:
                            case FieldMDEnums.eDTyp.Int:
                                lvi = buf.GetLayerInt(1);
                                //Java if ( lvi == null || lvi.equals(Integer.MIN_VALUE) || lvi.equals(Integer.MAX_VALUE) ) lvi = null;
                                //C#
                                if ( lvi == null || lvi == int.MinValue || lvi == int.MaxValue ) lvi = null;
                                inputRow_orig[jj] = lvi;
                                if ( aInputRecordSetMD.Column[jj].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[jj].MapKey, lvi);
                                break;
                            //case Str:
                            case FieldMDEnums.eDTyp.Str:
                                lvs = buf.GetLayerFLenString(1, aInputRecordSetMD.Column[jj].ByteMaxLength);
                                inputRow_orig[jj] = lvs;
                                if ( aInputRecordSetMD.Column[jj].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[jj].MapKey, lvs);
                                break;
                            //case VLS:
                            case FieldMDEnums.eDTyp.VLS:
                                lvs = buf.GetLayerVLenString(1, aInputRecordSetMD.Column[jj].ByteMaxLength);
                                inputRow_orig[jj] = lvs;
                                if ( aInputRecordSetMD.Column[jj].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[jj].MapKey, lvs);
                                break;
                            //case Bln:
                            case FieldMDEnums.eDTyp.Bln:
                                lvi = buf.GetLayerInt(1);
                                //Java if ( lvi == null || lvi.equals(Integer.MIN_VALUE) || lvi.equals(Integer.MAX_VALUE) ) lvi = null;
                                //C$
                                if ( lvi == null || lvi == int.MinValue || lvi == int.MaxValue ) lvi = null;
                                lvbln = null;
                                if ( lvi != null ) lvbln = (lvi != 0);
                                inputRow_orig[jj] = lvbln;
                                if ( aInputRecordSetMD.Column[jj].hasMapKey() )
                                    inputRow.put(aInputRecordSetMD.Column[jj].MapKey, lvbln);
                                break;
                            //case Byt:
                            case FieldMDEnums.eDTyp.Byt:
                            default:
                                throw new com.WDataSci.WDS.WDSException("Error, column type not implemented");
                        }
                    }
                    aInputRecordSet.Records.add(inputRow);
                    aInputRecordSet.Records_Orig.add(inputRow_orig);
                }

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error reading from DBB, row " + ri + ":", e);
            }
        }


        public int mWriteMap(RecordSetMD aRecordSetMD)
        //throws com.WDataSci.WDS.WDSException
        {
            try {

                if ( this.Header.Buffer == null )
                    throw new com.WDataSci.WDS.WDSException("Error, Header buffer not set before WriteMap!");

                long nColumns = aRecordSetMD.nColumns();
                int nColumnNameMaxByteLength = (int) (this.Header.MaxStringByteLength);

                DBB buf = this.Header.Buffer;
                buf.cAsUsualLayout("WDSH", nColumns, 40, 2 * nColumnNameMaxByteLength);
                buf.position(0, 0, 0);

                int bptr = 0;

                //write leading data
                buf.PutLayerFLenString(0, "WDSH", 8, 0);
                buf.PutLayerLong(0, buf.nDBBRequiredBytes);
                buf.PutLayerLong(0, buf.nDBBLeadingBytes);
                buf.PutLayerLong(0, buf.nDBBFLenBytes);
                buf.PutLayerLong(0, buf.nDBBVLenBytes);
                buf.PutLayerLong(0, buf.nRecords);
                buf.PutLayerLong(0, buf.nRecordFLenBytes);
                buf.PutLayerLong(0, buf.nRecordVLenBytes);

                buf.position(buf.ptr, 0, 0);
                //here bptr is relative only to layer 1
                bptr = 0;

                for ( int jj = 0 ; jj < nColumns ; jj++, bptr += (int) buf.nRecordFLenBytes ) {
                    aRecordSetMD.Column[jj].Consistency();
                    buf.position(buf.ptr, bptr, buf.vlenptr);
                    buf.PutLayerVLenString(1, aRecordSetMD.Column[jj].Name, nColumnNameMaxByteLength, 2);
                    if ( aRecordSetMD.Column[jj].MapKey != null ) {
                        buf.PutLayerVLenString(1, aRecordSetMD.Column[jj].MapKey.getValue(), nColumnNameMaxByteLength, 2);
                    }
                    else {
                        buf.PutLayerVLenString(1, "", nColumnNameMaxByteLength, 2);
                    }
                    buf.PutLayerInt(1, aRecordSetMD.Column[jj].DTyp.AsInt());
                    buf.PutLayerLong(1, aRecordSetMD.Column[jj].ByteMemLength);
                    buf.PutLayerLong(1, aRecordSetMD.Column[jj].ByteMaxLength);
                }

                return 0;

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in writing output map to DBB", e);
            }

        }

        /* C# >>> */
        public void mBytesRequired(RecordSetMD aRecordSetMD, long nRecords
            , out long rsize, out long rflensize, out long rvlensize
                        )
        {
            rsize = 0;
            rflensize = 0;
            rvlensize = 0;
            int nColumns = aRecordSetMD.nColumns();
            for ( int jj = 0 ; jj < nColumns ; jj++ ) {
                aRecordSetMD.Column[jj].Consistency();
                rflensize += aRecordSetMD.Column[jj].ByteMemLength;
                if ( aRecordSetMD.Column[jj].isVLen() )
                    rvlensize += aRecordSetMD.Column[jj].ByteMaxLength;
            }
            rsize = nRecords * rflensize + nRecords * rvlensize;
        }
        /* <<< C# */

        /* Java >>> *
        public void mBytesRequired(RecordSetMD aRecordSetMD, long nRecords, long[] csize, long[] hsize, long[] rsize
                , long[] cleadsize
                , long[] hleadsize, long[] hflensize, long[] hvlensize
                , long[] rleadsize, long[] rflensize, long[] rvlensize
        )
        throws com.WDataSci.WDS.WDSException
        {
            csize[0] = 0;
            hsize[0] = 0;
            rsize[0] = 0;
            cleadsize[0] = 8 * 8;
            hleadsize[0] = 8 * 8;
            hflensize[0] = 40;
            hvlensize[0] = 2 * aRecordSetMD.nHeaderByteMaxLength();
            rleadsize[0] = 8 * 8;

            rflensize[0] = 0;
            rvlensize[0] = 0;
            int nColumns = aRecordSetMD.nColumns();
            for ( int jj = 0; jj < nColumns; jj++ ) {
                aRecordSetMD.Column[jj].Consistency();
                rflensize[0] += aRecordSetMD.Column[jj].ByteMemLength;
                if ( aRecordSetMD.Column[jj].isVLen() )
                    rvlensize[0] += aRecordSetMD.Column[jj].ByteMaxLength;
            }

            hsize[0] = hleadsize[0] + nColumns * 40 + 2 * nColumns * aRecordSetMD.nHeaderByteMaxLength();
            rsize[0] = rleadsize[0] + nRecords * rflensize[0] + nRecords * rvlensize[0];
            csize[0] = cleadsize[0] + hsize[0] + rsize[0];

        }

        /* <<< Java */

        /* C# >>> */
        public void mBytesRequired(RecordSetMD aRecordSetMD, long nRecords, out long csize, out long hsize, out long rsize
                        , out long cleadsize
                        , out long hleadsize, out long hflensize, out long hvlensize
                        , out long rleadsize, out long rflensize, out long rvlensize
                        )
        {
            csize = 0;
            hsize = 0;
            rsize = 0;
            cleadsize = 8 * 8;
            hleadsize = 8 * 8;
            hflensize = 40;
            hvlensize = 2 * aRecordSetMD.nHeaderByteMaxLength();
            rleadsize = 8 * 8;

            int nColumns = aRecordSetMD.nColumns();
            this.mBytesRequired(aRecordSetMD, nRecords, out rsize, out rflensize, out rvlensize);

            hsize = hleadsize + nColumns * 40 + 2 * nColumns * aRecordSetMD.nHeaderByteMaxLength();
            rsize = rleadsize + nRecords * rflensize + nRecords * rvlensize;
            csize = cleadsize + hsize + rsize;

        }

        /* <<< C# */

        public void mWritePrepFor(RecordSetMD aRecordSetMD, long nRecords)
        //throws com.WDataSci.WDS.WDSException
        {

            long rflen = 0;
            long rvlen = 0;
            for ( int jj = 0 ; jj < aRecordSetMD.nColumns() ; jj++ ) {
                aRecordSetMD.Column[jj].Consistency();
                rflen += aRecordSetMD.Column[jj].ByteMemLength;
                if ( aRecordSetMD.Column[jj].isVLen() )
                    rvlen += aRecordSetMD.Column[jj].ByteMaxLength;
            }

            this.RecordSet.Buffer.nRecords = nRecords;
            this.RecordSet.Buffer.nRecordFLenBytes = rflen;
            this.RecordSet.Buffer.nRecordVLenBytes = rvlen;

            this.RecordSet.Buffer.LayoutStyle = "WDSD";
            this.RecordSet.Buffer.nDBBLeadingBytes = 8 * 8;

            this.RecordSet.Buffer.nDBBFLenBytes = nRecords * rflen;
            this.RecordSet.Buffer.nDBBVLenBytes = nRecords * rvlen;

            this.RecordSet.Buffer.nDBBRequiredBytes = this.RecordSet.Buffer.nDBBLeadingBytes
                    + this.RecordSet.Buffer.nDBBFLenBytes
                    + this.RecordSet.Buffer.nDBBVLenBytes;

            if ( this.RecordSet.Buffer.nDBBRequiredBytes > this.RecordSet.Buffer.Length
                    || (this.RecordSet.Buffer.nDBBRequiredBytes + this.RecordSet.Buffer.offset) > this.RecordSet.Buffer.data.Length )
                throw new com.WDataSci.WDS.WDSException("Error, capacity of underlying byte[] is insufficient in DBB.ProcessRecordSetParameters");

            this.RecordSet.Buffer.bHasFLenVLenSplit = true;
            this.RecordSet.Buffer.flenoffset = this.RecordSet.Buffer.offset + this.RecordSet.Buffer.nDBBLeadingBytes;
            this.RecordSet.Buffer.flenlength = this.RecordSet.Buffer.nDBBFLenBytes;
            this.RecordSet.Buffer.vlenoffset = this.RecordSet.Buffer.offset + this.RecordSet.Buffer.nDBBLeadingBytes + this.RecordSet.Buffer.nDBBFLenBytes;
            this.RecordSet.Buffer.flenlength = this.RecordSet.Buffer.nDBBVLenBytes;
            this.RecordSet.Buffer.position(0, 0, 0);

        }


        public int mWriteRecordSet(RecordSetMD outRecordSetMD
                , RecordSet aOutputRecordSet
                , RecordSetMD inRecordSetMD
                , RecordSet aInputRecordSet
        )
        //throws com.WDataSci.WDS.WDSException
        {
            try {

                if ( this.RecordSet.Buffer == null )
                    throw new com.WDataSci.WDS.WDSException("Error, RecordSet buffer not set before WriteSet!");

                DBB buf = this.RecordSet.Buffer;

                int nRows = aOutputRecordSet.Records.size();
                this.mWritePrepFor(outRecordSetMD, nRows);

                buf.position(0, 0, 0);

                int bptr = 0;

                //write leading data
                buf.PutLayerFLenString(0, "WDSD", 8, 0);
                buf.PutLayerLong(0, buf.nDBBRequiredBytes);
                buf.PutLayerLong(0, buf.nDBBLeadingBytes);
                buf.PutLayerLong(0, buf.nDBBFLenBytes);
                buf.PutLayerLong(0, buf.nDBBVLenBytes);
                buf.PutLayerLong(0, buf.nRecords);
                buf.PutLayerLong(0, buf.nRecordFLenBytes);
                buf.PutLayerLong(0, buf.nRecordVLenBytes);

                int nColumns = outRecordSetMD.nColumns();

                int nInputColumns = inRecordSetMD.nColumns();


                bptr = 0;
                for ( int i = 0 ; i < nRows ; i++, bptr += (int) buf.nRecordFLenBytes ) {
                    buf.position(buf.ptr, bptr, buf.vlenptr);

                    Map<FieldName, Object> outRow = aOutputRecordSet.Records.get(i);
                    Object[] inRow = null;

                    if ( outRecordSetMD.ModeMatter.bRepeatInputFields )
                        inRow = aInputRecordSet.Records_Orig.get(i);

                    Boolean bInInputSet = outRecordSetMD.ModeMatter.bRepeatInputFields;
                    int j = 0;
                    int jj = 0;

                    for ( jj = 0, j = 0 ; jj < nColumns ; jj++, j++ ) {

                        if ( bInInputSet && jj == nInputColumns ) {
                            bInInputSet = false;
                            j = 0;
                        }

                        Object lv = 0;

                        if ( bInInputSet )
                            lv = inRow[j];
                        else
                            lv = outRow.get(outRecordSetMD.Column[j].MapKey);

                        switch ( outRecordSetMD.Column[jj].DTyp ) {
                            //case Dbl:
                            case FieldMDEnums.eDTyp.Dbl:
                                if ( lv == null ) lv = Double.NaN;
                                buf.PutLayerDouble(1, (double) lv);
                                break;
                            //case Lng:
                            case FieldMDEnums.eDTyp.Lng:
                                //Java if ( lv == null ) lv = Long.MIN_VALUE;
                                //C#
                                if ( lv == null ) lv = long.MinValue;
                                buf.PutLayerLong(1, (long) lv);
                                break;
                            //case Int:
                            case FieldMDEnums.eDTyp.Int:
                                //Java if ( lv == null ) lv = Integer.MIN_VALUE;
                                //C#
                                if ( lv == null ) lv = int.MinValue;
                                buf.PutLayerInt(1, (int) lv);
                                break;
                            //case Dte:
                            case FieldMDEnums.eDTyp.Dte:
                                if ( lv == null ) lv = Double.NaN;
                                buf.PutLayerDouble(1, (double) lv);
                                break;
                            //case DTm:
                            case FieldMDEnums.eDTyp.DTm:
                                if ( lv == null ) lv = Double.NaN;
                                buf.PutLayerDouble(1, (double) lv);
                                break;
                            //case Str:
                            case FieldMDEnums.eDTyp.Str:
                                buf.PutLayerFLenString(1, (String) lv, (int) outRecordSetMD.Column[jj].ByteMaxLength, 2);
                                break;
                            //case VLS:
                            case FieldMDEnums.eDTyp.VLS:
                                buf.PutLayerVLenString(1, (String) lv, (int) outRecordSetMD.Column[jj].ByteMaxLength, 2);
                                break;
                            default:
                                throw new com.WDataSci.WDS.WDSException("Error, unImplemented column type" + outRecordSetMD.Column[jj].DTyp.ToString());
                        }
                    }
                }

                return 0;

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in writing output map to DBB", e);
            }

        }


        /* C# >>> */
        public int mWriteRecordSet(RecordSetMD inRecordSetMD
                , MOIE.ListObject aListObject
                )
        {
            try {

                if ( this.RecordSet.Buffer == null )
                    throw new com.WDataSci.WDS.WDSException("Error, RecordSet buffer not set before WriteSet!");

                DBB buf = this.RecordSet.Buffer;

                int nRows = aListObject.ListRows.Count;
                int nColumns = aListObject.ListColumns.Count;

                if ( nColumns != inRecordSetMD.nColumns() )
                    throw new com.WDataSci.WDS.WDSException("Error, Excel ListObject #columns does not match RecordSetMD #columns");

                this.mWritePrepFor(inRecordSetMD, nRows);

                buf.position(0, 0, 0);


                //write leading data
                buf.PutLayerFLenString(0, "WDSD", 8, 0);
                buf.PutLayerLong(0, buf.nDBBRequiredBytes);
                buf.PutLayerLong(0, buf.nDBBLeadingBytes);
                buf.PutLayerLong(0, buf.nDBBFLenBytes);
                buf.PutLayerLong(0, buf.nDBBVLenBytes);
                buf.PutLayerLong(0, buf.nRecords);
                buf.PutLayerLong(0, buf.nRecordFLenBytes);
                buf.PutLayerLong(0, buf.nRecordVLenBytes);

                int bptr = 0;
                //bptr = (int) buf.nDBBLeadingBytes;



                int nInputColumns = inRecordSetMD.nColumns();


                object[,] r = aListObject.Range.Value2;

                for ( int i = 0, iP2 = 2; i < nRows; i++, iP2++ ) {
                    if ( i > 0 ) bptr += (int) this.RecordSet.Buffer.nRecordFLenBytes;
                    long lbptr = bptr;
                    buf.position(buf.ptr, (int) bptr, buf.vlenptr);

                    for ( int j = 0, jP1 = 1 ; j < nColumns ; j++, jP1++ ) {
                        Object obj = r[iP2, jP1];
                        switch ( inRecordSetMD.Column[j].DTyp ) {
                            case FieldMDEnums.eDTyp.Dbl:
                                buf.PutLayerDouble(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Lng:
                                buf.PutLayerLong(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Dte:
                                buf.PutLayerDouble(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.DTm:
                                buf.PutLayerDouble(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Int:
                                buf.PutLayerInt(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Str:
                                buf.PutLayerFLenString(1, Convert.ToString(obj), (int) inRecordSetMD.Column[j].ByteMaxLength, 2);
                                break;
                            case FieldMDEnums.eDTyp.VLS:
                                buf.PutLayerVLenString(1, Convert.ToString(obj), (int) inRecordSetMD.Column[j].ByteMaxLength, 2);
                                break;
                            default:
                                throw new Exception("Hey");
                        }
                    }

                }

                return 0;

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in writing output map to DBB", e);
            }
        }

        public int mWriteRecordSet(RecordSetMD inRecordSetMD
            , Object[,] r
            , Boolean bIncludesHeaderRow
                )
        {
            try {

                if ( this.RecordSet.Buffer == null )
                    throw new com.WDataSci.WDS.WDSException("Error, RecordSet buffer not set before WriteSet!");

                DBB buf = this.RecordSet.Buffer;

                int rowstartindex = r.GetLowerBound(0);
                int nRows = r.GetUpperBound(0) - rowstartindex + 1;
                if ( bIncludesHeaderRow ) {
                    nRows -= 1;
                    rowstartindex += 1;
                }
                int nColumns = r.GetUpperBound(1) - r.GetLowerBound(1) + 1;

                if ( nColumns != inRecordSetMD.nColumns() )
                    throw new com.WDataSci.WDS.WDSException("Error, Excel ListObject #columns does not match RecordSetMD #columns");

                this.mWritePrepFor(inRecordSetMD, nRows);

                buf.position(0, 0, 0);


                //write leading data
                buf.PutLayerFLenString(0, "WDSD", 8, 0);
                buf.PutLayerLong(0, buf.nDBBRequiredBytes);
                buf.PutLayerLong(0, buf.nDBBLeadingBytes);
                buf.PutLayerLong(0, buf.nDBBFLenBytes);
                buf.PutLayerLong(0, buf.nDBBVLenBytes);
                buf.PutLayerLong(0, buf.nRecords);
                buf.PutLayerLong(0, buf.nRecordFLenBytes);
                buf.PutLayerLong(0, buf.nRecordVLenBytes);

                int bptr = 0;
                //bptr = (int) buf.nDBBLeadingBytes;



                int nInputColumns = inRecordSetMD.nColumns();




                for ( int i = 0, ii = rowstartindex ; i < nRows ; i++, ii++ ) {
                    if ( i > 0 ) bptr += (int) this.RecordSet.Buffer.nRecordFLenBytes;
                    long lbptr = bptr;
                    buf.position(buf.ptr, (int) bptr, buf.vlenptr);

                    for ( int j = 0, jj = r.GetLowerBound(1) ; j < nColumns ; j++, jj++ ) {
                        Object obj = r [ii, jj];
                        switch ( inRecordSetMD.Column [j].DTyp ) {
                            case FieldMDEnums.eDTyp.Dbl:
                                buf.PutLayerDouble(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Lng:
                                buf.PutLayerLong(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Dte:
                                buf.PutLayerDouble(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.DTm:
                                buf.PutLayerDouble(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Int:
                                buf.PutLayerInt(1, obj);
                                break;
                            case FieldMDEnums.eDTyp.Str:
                                buf.PutLayerFLenString(1, Convert.ToString(obj), (int) inRecordSetMD.Column [j].ByteMaxLength, 2);
                                break;
                            case FieldMDEnums.eDTyp.VLS:
                                buf.PutLayerVLenString(1, Convert.ToString(obj), (int) inRecordSetMD.Column [j].ByteMaxLength, 2);
                                break;
                            default:
                                throw new Exception("Hey");
                        }

                    }

                }

                return 0;

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in writing output map to DBB", e);
            }
        }
        /* <<< C# */
    }

    /* C# >>> */
}
/* <<< C# */

