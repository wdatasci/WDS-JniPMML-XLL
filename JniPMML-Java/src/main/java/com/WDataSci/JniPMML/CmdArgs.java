package com.WDataSci.JniPMML;

import com.beust.jcommander.Parameter;

import java.lang.annotation.Annotation;
import java.lang.reflect.Field;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

import com.WDataSci.WDS.Util;
import static com.WDataSci.WDS.Util.BaseDirAndPath;

public class CmdArgs
{

    @Parameter
    private List<String> internal_parms = new ArrayList<>();

    @Parameter(names = {"--help"}, help = true, description="command line help invocation", order = 1000, arity =0)
    public boolean help = false;

    @Parameter(names = {"--no-verbose"}, description = "cmd-line verbose can only be turned off when tagged [--verbose defaults to true, tag --no-verbose to invert]", order = 1000, arity =0)
    public boolean verbose = true;

    @Parameter(names = {"--base-dir"}, description = "base path for which other file/path can be taken relative to [. (default)], will be applied to file/paths without a root start", order = 1)
    public String aBaseDir = ".";

    @Parameter(names = {"--pmml"}, description = "path to pmml xml file", required = true, order = 1)
    public String aPMMLFileName = null;

    @Parameter(names = {"--input-type"}, description = "input type [delimited (default) | HDF5]", order = 2)
    public String aInputFileType = "check";

    @Parameter(names = {"--input"}, description = "path to input text file", required = true, order = 2)
    public String aInputFileName = null;

    @Parameter(names = {"--input-schema"}, description = "(for non HDF5 input data) path to input schema file, which follows type defined by --input-schema-type",
            order = 4)
    public String aInputSchemaFileName = null;

    @Parameter(names = {"--input-schema-recordset-element"
            , "--input-schema-rowset-element"
            , "--input-HDF5-DataSet"
    }
            , description = "(for non HDF5 input data) the name of the row set element in the input schema"
            , order = 5)
    public String aInputSchemaRecordSetName = null;

    @Parameter(names = {"--input-schema-type"}, description = "(for non HDF5 input data) input schema type [xsd (default) | xml | json]", order = 100)
    public String aInputSchemaType = "xsd";

    @Parameter(names = {"--no-input-check-for-header"}, description = "(for non HDF5 input data) check if input has header row to skip [defaults --input-check-for-header, tag --no-input-check-for-header to invert]", order = 1000, arity = 0)
    public boolean bCheckForHeaderRow = true;

    @Parameter(names = {"--input-has-header"}, description = "(for non HDF5 input data) input text file has header row to skip [defaults to false if not present, tag for true]", order = 1000, arity = 0)
    public boolean bInputHasHeaderRow = false;

    @Parameter(names = {"--input-dlm"}, description = "(for non HDF5 input data) input text file delimiter", order = 1000)
    public String aInputFileDlm = ",";

    @Parameter(names = {"--output"}, description = "path to output text file [HDF5 input will return HDF5 output if not tagged]", required = true, order = 6)
    public String aOutputFileName = null;

    @Parameter(names = {"--output-type"}, description = "output type [delimited (default, csv) | HDF5]", order = 6)
    public String aOutputFileType = "check";

    @Parameter(names = {"--output-HDF5-strings-with-fixed-Length"}, description = "(for HDF5 output data) as of HDF-Java 1.10.5/1.11.4, HDF-Java cannot write CompoundDS with variable Length strings, used fixed string Length (-1 for variable when available)", order = 1000)
    public int aOutputHDF5FixedStringLength = 64;

    @Parameter(names = {"--output-header"}, description = "(for non HDF5 output data) flag to output header row [defaults to false if not present, tag for true]", order = 1000, arity = 0)
    public boolean bOutputHeaderRow = false;

    @Parameter(names = {"--output-HDF5-dataset-name"
    }, description = "(for HDF5 output data) the output will be a new HDF5 file with Compound Dataset in /<dataset name>"
            , order = 5)
    public String aOutputHDF5DataSetName = "OutputRecordSet";

    @Parameter(names = {"--no-output-input-fields"}, description = "turn off the input data copy [defaults --output-input-fields to true, tag --no-output-input-fields to invert]", order = 1000, arity = 0)
    public boolean bOutputInputFields = true;

    @Parameter(names = {"--output-input-fields-suffix"}, description = "(when copying input data) suffix on input field names to avoid duplicates", order = 7)
    public String aOutputInputFieldNameSuffix = "Input";

    @Parameter(names = {"--output-field-composite-name-dlm"}, description = "the composite name delimiter for output names (as with a suffix or feature) ['-' default]", order = 7)
    public String aOutputCompositeFieldDlm = "-";

    @Parameter(names = {"--output-dlm"}, description = "(for non HDF5 output data) output text file delimiter", order = 1000)
    public String aOutputFileDlm = "InputDlm";

    /*
    public void mRunChecks(JCommander args) {
        if (args.aInputFileType.equals("check"))
            this.aInputFileType=this.aInputFileName.substring(this.aInputFileName.lastIndexOf(".")+1);
        if (this.bInputHasHeaderRow)
            this.bCheckForHeaderRow=false;
        if (this.aOutputFileType.equals("check"))
            this.aOutputFileType=this.aOutputFileName.substring(this.aOutputFileName.lastIndexOf(".")+1);

    }
    */

    public void mProcessBaseDir(){
        this.aPMMLFileName=BaseDirAndPath(this.aBaseDir,this.aPMMLFileName);
        this.aInputFileName=BaseDirAndPath(this.aBaseDir, this.aInputFileName);
        this.aInputSchemaFileName=BaseDirAndPath(this.aBaseDir, this.aInputSchemaFileName);
        this.aOutputFileName=BaseDirAndPath(this.aBaseDir, this.aOutputFileName);
    }

    public String mRecapParameters()
    {
        Field[] fields = CmdArgs.class.getFields();
        com.fasterxml.jackson.databind.ObjectMapper objMapper = new com.fasterxml.jackson.databind.ObjectMapper();
        Map<String, Object> flds = objMapper.convertValue(this, Map.class);
        String rv="Java Internals";
        rv+=";System Class Path|"+System.getProperty("java.class.path").replace(';','|');
        //ClassLoader aClassLoader=CmdArgs.class.getClassLoader();
        //ClassLoader aClassLoader=ClassLoader.getPlatformClassLoader();
        //ClassLoader aClassLoader=ClassLoader.getSystemClassLoader();
        //URL[] aURLArray=((URLClassLoader) aClassLoader).getURLs();
        //rv+=";ClassPath";
        //for (URL aURL : aURLArray)
            //rv+="|"+aURL.getFile();
        rv+=";Parameters";
        //to skip multiples that happen when more than one alias is used
        String fname_last="";
        for (Field f:fields) {
            if ( !f.getName().equals(fname_last) ) {
                for (Annotation a : f.getAnnotations()) {
                    com.beust.jcommander.Parameter p = (com.beust.jcommander.Parameter) a;
                    rv += ";" + f.getName()
                            + ":" + Arrays.toString(p.names()).replace(',', '|').strip()
                            + ":" + p.description()
                            + ":" + f.getType().getName()
                            + ":" + p.order()
                            + ":" + p.arity()
                            + ":" + flds.get(f.getName())
                            + ":" + p.required()
                    ;
                }
                fname_last = f.getName();
            }
        }
        return rv;
    }

    public void mRecap()
    {
        Field[] fields = CmdArgs.class.getFields();
        //CodeRef: CJW hint from https://stackoverflow.com/questions/2126714/java-get-all-variable-names-in-a-class
        com.fasterxml.jackson.databind.ObjectMapper objMapper = new com.fasterxml.jackson.databind.ObjectMapper();
        Map<String, Object> flds = objMapper.convertValue(this, Map.class);

        for (Map.Entry<String, Object> fld : flds.entrySet()) {
            System.out.printf("%s:%s\n", fld.getKey(), fld.getValue());
        }
        System.out.println(this.mRecapParameters().replace(';','\n'));
    }

}
