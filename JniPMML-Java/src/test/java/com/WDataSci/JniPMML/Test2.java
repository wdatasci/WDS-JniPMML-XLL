package com.WDataSci.JniPMML;

import com.WDataSci.WDS.WDSException;
import com.beust.jcommander.JCommander;
import org.dmg.pmml.FieldName;
import org.jpmml.evaluator.Evaluator;
import org.junit.Test;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.List;
import java.util.Map;

import static java.lang.Math.abs;
import static junit.framework.TestCase.assertTrue;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.fail;

public class Test2
{
    @Test
    public void shouldAnswerWithTrue()
    {
        if ( true ) {

            String[] sargs3 = {"--pmml", "..\\test\\data\\IrisMultinomReg.xml"
                    , "--input", "..\\test\\data\\Iris.csv"
                    , "--input-has-header"
                    , "--input-schema-type", "NamingConvention"
                    , "--input-type", "delimited"
                    , "--input-has-header"
                    //, "--input-schema", "..\\test\\data\\Iris_Input.xsd"
                    //, "--input-schema-rowset-element", "RecordSet"
                    , "--output", "..\\test\\output\\IrisMultinomReg_Test2_output.h5"
                    , "--output-HDF5-strings-with-fixed-Length", "32"
                    , "--output-type", "HDF5"
                    //,"--no-output-input-fields"
                    , "--no-verbose"

            };

            try {

                int i, j, k;


                //based on the Cmd class, look there for the steps, this test is for writing and reading
                //to Direct ByteBuffers

                CmdArgs args = new CmdArgs();
                JCommander jc = JCommander.newBuilder().addObject(args).build();
                jc.parse(sargs3);


                //instantiate the jpmml workspace
                JniPMML aJniPMML = new JniPMML();
                JniPMMLItem aJniPMMLItem = aJniPMML.GetItem();
                String aPMMLFileAsString = com.WDataSci.WDS.Util.FetchFileAsString(args.aPMMLFileName);
                String lrv = aJniPMMLItem.PMMLLoadFromString( aPMMLFileAsString);

                //create the map for the input data and the input set
                RecordSetMD aInputRecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Input).cUsingCmdArguments(args);

                int nInputColumns = 0;
                RecordSet aInputRecordSet = new RecordSet();

                aInputRecordSetMD.mReadMapFor(aJniPMMLItem, null, false);
                nInputColumns = aInputRecordSetMD.nColumns();

                //setup a wrap of bytes to write and read header data from
                byte[] _bTestMapBytes = new byte[65536];
                ByteBuffer bTestMapByteBuffer = ByteBuffer.wrap(_bTestMapBytes, 0, 65536).order(ByteOrder.BIG_ENDIAN);

                byte[] _bTestMapBytes2 = new byte[65536];
                ByteBuffer bTestMapByteBuffer2 = ByteBuffer.wrap(_bTestMapBytes2, 0, 65536).order(ByteOrder.BIG_ENDIAN);

                byte[] _bTestMapBytes3 = new byte[2 * 65536];
                ByteBuffer bTestMapByteBuffer3 = ByteBuffer.wrap(_bTestMapBytes3, 0, 2 * 65536).order(ByteOrder.BIG_ENDIAN);

                if ( true ) { // testing the writing and reading of header data to a ByteBuffer like object

                    //create a map to write out
                    RecordSetMD aTestMap1_Outgoing = new RecordSetMD(RecordSetMDEnums.eMode.Output)
                            .cAs(RecordSetMDEnums.eType.DBB)
                            .mCopyColumnsFrom(aInputRecordSetMD);

                    aTestMap1_Outgoing
                            .cSetHeaderBufferAs(bTestMapByteBuffer, nInputColumns, 40, (int) (2 * aInputRecordSetMD.nHeaderByteMaxLength()))
                            .mWriteMapToBuffer();

                    //We are reading a RecordSetMD, so the mode must be Input, ignoring mode for the Equals test
                    RecordSetMD aTestMap1_Incoming = new RecordSetMD(RecordSetMDEnums.eMode.Input)
                            .cAs(RecordSetMDEnums.eType.DBB)
                            .cSetHeaderBufferFrom(bTestMapByteBuffer)
                            .mReadMapFor(aJniPMMLItem, null, false);

                    if ( !aTestMap1_Outgoing.Equals(aTestMap1_Incoming, true) )
                        throw new com.WDataSci.WDS.WDSException("Error read RecordSetMD does not match written XDatamap");

                }

                aInputRecordSet.mReadRecordSet(aInputRecordSetMD);

                aJniPMMLItem.InputMatter.RecordSetMD = aInputRecordSetMD;

                //Short circuit if new inputs are provided.
                if ( aInputRecordSet.Records.size() == 0 ) return;

                //PreRunPrep can clean up the various output types
                aJniPMMLItem.mPreRunPrepOutputMap(80, 128);

                //Get and verify the evaluator
                Evaluator aJniPMMLEvaluator = aJniPMMLItem.PMMLEvaluator();
                aJniPMMLEvaluator.verify();


                //Evaluate the PMML on each row of the input set and returns a non-generic
                List<Map<FieldName, Object>> Results = aJniPMMLItem.PMMLEvaluate(aInputRecordSet, true, args.verbose);

                RecordSetMD aOutputRecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Output)
                        .cAs(RecordSetMDEnums.eType.DBB)
                        .mPrepForOutput(aInputRecordSetMD, aJniPMMLItem, Results)
                        .mColumnConsistency();

                //Writing out the Map
                aOutputRecordSetMD
                        .cSetHeaderBufferAs(bTestMapByteBuffer2, aOutputRecordSetMD.nColumns(), 40, 256)
                        .mWriteMapToBuffer();


                aOutputRecordSetMD.cSetRecordSetBufferAs(bTestMapByteBuffer3);
                aOutputRecordSetMD.DBBMatter.mWritePrepFor(aOutputRecordSetMD, Results.size());

                RecordSet aOutputRecordSet = new RecordSet(Results);
                aOutputRecordSet.mWriteRecordSet(aOutputRecordSetMD, aInputRecordSetMD, aInputRecordSet);

                ByteBuffer bTestMapByteBuffer4 = ByteBuffer.wrap(_bTestMapBytes3, 0, 2 * 65536).order(ByteOrder.BIG_ENDIAN);


                RecordSetMD aTestMap2 = new RecordSetMD(RecordSetMDEnums.eMode.Input)
                        .cAs(RecordSetMDEnums.eType.DBB)
                        .cSetHeaderBufferFrom(bTestMapByteBuffer2)
                        .mReadMapFor(null, null, true);

                aTestMap2
                        .cSetRecordSetBufferFrom(bTestMapByteBuffer4);

                RecordSet aTestRecordSet = new RecordSet().cAsInput();
                aTestRecordSet.mReadRecordSet(aTestMap2);

                CheckII(aOutputRecordSet.Records.size(), aTestRecordSet.Records_Orig.size(), "nRecords");
                CheckII(aOutputRecordSet.Records.get(0).keySet().size(), aTestRecordSet.Records.get(0).keySet().size(), "nFields");
                for (i = 0; i < aOutputRecordSet.Records.size(); i++) {
                    for (j = 0; j < aOutputRecordSetMD.nColumns(); j++) {
                        FieldName Afn = aOutputRecordSetMD.Column[j].MapKey;
                        Object A = aOutputRecordSet.Records.get(i).get(Afn);
                        FieldName Bfn = aTestMap2.Column[j].MapKey;
                        Object B = aTestRecordSet.Records.get(i).get(Bfn);
                        if ( CheckJJ(A, B, "Row " + i + " " + Afn.getValue() + " " + Bfn.getValue()) ) {
                            if (args.verbose)
                                System.out.println("Test: Row " + i + " " + Afn.getValue() + ":" + A.toString() + ", " + Bfn.toString() + ":" + B.toString());
                        }
                    }
                }


            } catch (Exception e) {
                WDSException je = new WDSException("Test1 Error", e);
                System.out.print(je.getMessage());
                fail();
            }
            assertTrue(true);
        }
    }

    public boolean CheckII(int a, int b, String s)
    throws com.WDataSci.WDS.WDSException
    {
        if ( a != b ) throw new com.WDataSci.WDS.WDSException(s);
        return true;
    }

    public boolean CheckJJ(Object a, Object b, String s)
    throws com.WDataSci.WDS.WDSException
    {
        if ( !com.WDataSci.WDS.Util.MatchingNullity(a,b) )
            throw new com.WDataSci.WDS.WDSException(s+", non-matching nullity");
        if ( a.getClass() == Double.class && b.getClass() == Double.class ) {
            if ( abs(((double) a) - ((double) b)) > 1e-6 )
                throw new com.WDataSci.WDS.WDSException(s);
        }
        else if ( !a.equals(b) ) throw new com.WDataSci.WDS.WDSException(s);
        return true;
    }

}


