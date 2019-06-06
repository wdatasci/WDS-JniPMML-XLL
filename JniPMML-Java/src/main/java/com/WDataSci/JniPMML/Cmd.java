package com.WDataSci.JniPMML;

//java imports

import com.WDataSci.WDS.Util;
import com.WDataSci.WDS.WDSException;
import com.beust.jcommander.JCommander;
import org.dmg.pmml.FieldName;
import org.jpmml.evaluator.Evaluator;

import java.util.Set;

public class Cmd
{


    static public void main(String... argv)
    throws Exception
    {

        CmdArgs args = new CmdArgs();
        JCommander jc = JCommander.newBuilder().addObject(args).build();
        jc.parse(argv);

        if ( args.help ) {
            jc.usage();
            return;
        }

        //A few checks on the command line arguments
        if ( args.aInputFileType.equals("check") )
            args.aInputFileType = args.aInputFileName.substring(args.aInputFileName.lastIndexOf(".") + 1);
        if ( RecordSetMDEnums.eSchemaType.FromAlias(args.aInputSchemaType).equals(RecordSetMDEnums.eSchemaType.NamingConvention) )
            args.bInputHasHeaderRow = true;
        if ( args.bInputHasHeaderRow )
            args.bCheckForHeaderRow = false;
        if ( args.aOutputFileType.equals("check") )
            args.aOutputFileType = args.aOutputFileName.substring(args.aOutputFileName.lastIndexOf(".") + 1);

        args.mProcessBaseDir();

        if ( args.verbose ) {
            System.out.println();
            System.out.print("Recap of select command line arguments:\n");
            System.out.printf("--pmml=%s\n", args.aPMMLFileName);
            System.out.printf("--input=%s\n", args.aInputFileName);
            System.out.printf("--input-schema=%s\n", args.aInputSchemaFileName);
            System.out.printf("--input-schema-rowsset-element=%s\n", args.aInputSchemaRecordSetName);
            System.out.printf("--input-schema-type=%s\n", args.aInputSchemaType);
            System.out.printf("--input-dlm=%s\n", args.aInputFileDlm);
            System.out.printf("--output=%s\n", args.aOutputFileName);
            System.out.println();
            System.out.print("Recap of internal argument structure:\n");
            args.mRecap();
            System.out.println();
        }
        //if (true) return;

        int i, j, k;

        //instantiate the jpmml workspace
        JniPMML aJniPMML = new JniPMML();
        JniPMMLItem aJniPMMLItem = aJniPMML.GetItem();

        //fetch the pmml file as a string, we are using this method to allow the string
        //version of the entire file to be passed in (as an option) from Excel
        String aPMMLFileAsString = Util.FetchFileAsString(args.aPMMLFileName);
        if ( args.verbose ) System.out.printf("PMML String:\n%s\nEOF\n", aPMMLFileAsString);

        //cache the parsed string in the workspace
        String lrv = aJniPMMLItem.PMMLLoadFromString( aPMMLFileAsString);
        if ( args.verbose ) System.out.printf("PMMLLoadFromString Return:\n%s\n", lrv);


        //create the map for the input data and the input set
        //RecordSetMD aInputRecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Input).cUsingCmdArguments(args);
        aJniPMMLItem.InputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Input).cUsingCmdArguments(args);
        RecordSetMD aInputRecordSetMD=aJniPMMLItem.InputMatter.RecordSetMD;
        int nInputMap = 0;

        if ( aInputRecordSetMD.Type.bIn(RecordSetMDEnums.eType.DBB, RecordSetMDEnums.eType.XML, RecordSetMDEnums.eType.JSON) )
            throw new WDSException("Error, cmd line InputMap type not implemented yet!");

        aInputRecordSetMD.mReadMapFor(aJniPMMLItem, null, false);
        nInputMap = aInputRecordSetMD.nColumns();

        //RecordSet aInputRecordSet = new RecordSet().cAsInput();
        aJniPMMLItem.InputMatter.RecordSet = new RecordSet().cAsInput();
        RecordSet aInputRecordSet=aJniPMMLItem.InputMatter.RecordSet;
        aInputRecordSet.mReadRecordSet(aInputRecordSetMD);


        if ( args.verbose ) {
            if ( !args.aInputFileType.equals("HDF5") ) {
                if ( args.bInputHasHeaderRow )
                    System.out.print("Input File has header row, but is ignored!\n");
                else
                    System.out.print("Input File does not have a header row!\n");
            }
            System.out.print("Input File Column Mapping:\n");
            for (i = 0; i < nInputMap; i++) {
                System.out.printf("    Input Column: %d, Schema Name: %s, Is Mapped: %s, Mapped Name: %s\n"
                        , i
                        , aInputRecordSetMD.Column[i].Name
                        , aInputRecordSetMD.Column[i].hasMapKey()
                        , aInputRecordSetMD.Column[i].MappedKeyValue()
                );
            }

        }


        if ( args.verbose ) {
            System.out.printf("aInputRecordSet.Records.size=%d\n", aInputRecordSet.Records.size());
        }

        //Only use if aJniPMMLItem is used to cache prior work (as in jni call from C#)
        //aJniPMMLItem.InputMatter.RecordSetMD=aInputRecordSetMD;

        //Short circuit if new inputs are provided.
        if ( aInputRecordSet.Records.size() == 0 ) return;

        if ( args.verbose ) {
            Set<FieldName> ks = aInputRecordSet.Records.get(0).keySet();
            System.out.printf("Row Recap,\n%s\n", ks.toString());
            for (i = 0; i < aInputRecordSet.Records.size(); i++) {
                System.out.printf("row %d, %s=%s\n", i, aInputRecordSet.Records.get(i).keySet(), aInputRecordSet.Records.get(i).values());
            }
        }

        //Get and verify the evaluator
        Evaluator aJniPMMLEvaluator = aJniPMMLItem.PMMLEvaluator();
        aJniPMMLEvaluator.verify();

        aJniPMMLItem.OutputMatter.RecordSetMD= new RecordSetMD(RecordSetMDEnums.eMode.Output)
                .cUsingCmdArguments(args)
                ;

        aJniPMML.mPreRunPrepOutputMap(aJniPMMLItem.HandleMajor,80); //(int) aJniPMMLItem.InputMatter.RecordSetMD.nHeaderStringMaxLength());

        //Evaluate the PMML on each row of the input set and returns a non-generic
        //List<Map<FieldName,Object>> Results=aJniPMMLItem.PMMLEvaluate(aInputRecordSet,true,args.verbose);
        //or wrap it right into a RecordSet
        RecordSet aOutputRecordSet = new RecordSet(aJniPMMLItem.PMMLEvaluate(aInputRecordSet, true, args.verbose));

        /*
        RecordSetMD aOutputRecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Output)
                .cUsingCmdArguments(args)
                .mPrepForOutput(aInputRecordSetMD, aJniPMMLItem, aOutputRecordSet.Records);
         */

        aOutputRecordSet.mWriteRecordSet(aJniPMMLItem.OutputMatter.RecordSetMD, aInputRecordSetMD, aInputRecordSet);
        //aOutputRecordSet.mWriteRecordSet(aOutputRecordSetMD, aInputRecordSetMD, aInputRecordSet);

        System.out.print("fin\n");

    }


}
