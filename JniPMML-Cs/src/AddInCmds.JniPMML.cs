using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using MOIE=Microsoft.Office.Interop.Excel;
using MOTE = Microsoft.Office.Tools.Excel;

using com.WDataSci.JniPMML;

using com.WDataSci.WDS;


namespace WDataSci.JniPMML
{

    /// <summary>
    /// The AddIn:IExcelAddIn is partialled in AddInCmds.JniPMML.cs, to add calls to the JniPMML wrapper.
    /// </summary>
    public partial class AddIn : IExcelAddIn
    {



        [ExcelFunction(
                Name = "JniPMML_HandleMajorFrom"
                , Category = "WDS.JniPMML"
                , Description = "Returns HandleMajor from \"Major.Minor\", or the string that has been parsed and is cached in the JVM.  Bad input (such as ExcelError) will return 0 which is the default first handle, returns -2 if not resolvable."
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static int HandleMajorFrom(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0
            )
        {
            if ( arg0 is ExcelDna.Integration.ExcelMissing ) return -2;
            if ( arg0 is ExcelDna.Integration.ExcelEmpty ) return -2;
            if ( arg0 is ExcelDna.Integration.ExcelError ) return -2;
            if ( arg0 == null ) return -2;

            int h = -1;
            if ( arg0 is double ) h = (int) Math.Floor((double) arg0);
            else if ( arg0 is int ) h = (int) arg0;
            else if ( arg0 is long ) h = (int) ((long) arg0);
            else {
                String s=arg0.ToString();
                double d=0;
                if ( !int.TryParse(s, out h) ) {
                    if ( AddIn.__JniPMML.isTag(s) )
                        h = AddIn.__JniPMML.Handle(s);
                    else if ( double.TryParse(s, out d) )
                        h = (int) Math.Floor(d);
                    else {
                        int i = s.indexOf(".");
                        if ( i > 0 )
                            s = s.substring(0, i);
                        try {
                            h = int.Parse(s);
                        }
                        catch ( Exception ) {
                            h = -2;
                        }
                    }
                }
            }
            if ( h < 0 || !AddIn.__JniPMML.isHandle(h) ) {
                if ( AddIn.__JniPMML.isTag(arg0.ToString()) )
                    h = AddIn.__JniPMML.Handle(arg0.ToString());
                else
                    h = -2;
            }
            return h;
        }



        [ExcelFunction(Name = "JniPMML_isValidHandle"
            , Category = "WDS.JniPMML"
            , ExplicitRegistration = true
            , HelpTopic = "Help Topic to be filled in"
            )]
        public static Boolean JniPMML_isValidHandle(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return false;
            List<object> cmargs = new List<object> { HandleMajorFrom(arg0) };
            Boolean rv = Java.CallMethod<Boolean>("isValidHandle", "(I)Z", cmargs);
            cmargs = null;
            return rv;
        }

        [ExcelFunction(Name = "JniPMML_HandleMajor"
            , Description = "Either returns the HandleMajor of the last or working index, or returns a new handle for the value (filling in all empty values lower)"
            , Category = "WDS.JniPMML"
            , ExplicitRegistration = true
            , HelpTopic = "Help Topic to be filled in"
            )]
        public static int JniPMML_HandleMajor(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0,
            [ExcelArgument(Name = "AdditionalRangeDependencies", Description = "Provides an Excel calculation dependency, otherwise not used")] Object deps
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return -1;
            List<object> cmargs = new List<object>{ HandleMajorFrom(arg0) };
            int rv = Java.CallMethod<int>("HandleMajor", "(I)I", cmargs);
            cmargs = null;
            return rv;
        }

        [ExcelFunction(Name = "JniPMML_Handle_LastUsed"
            , Description = "Last handle used in Java JniPMML"
            , Category = "WDS.JniPMML"
            , ExplicitRegistration = true
            , HelpTopic = "Help Topic to be filled in"
            , IsVolatile = false
            )]
        public static String JniPMML_Handle_LastUsed(
            [ExcelArgument(Name = "AdditionalRangeDependencies")] Object arg
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            List<object> cmargs = new List<object>(0);
            String rv = Java.CallMethod<String>("Handle", "()Ljava/lang/String;", cmargs);
            cmargs = null;
            return rv;
        }

        [ExcelFunction(Name = "JniPMML_Handle"
            , Category = "WDS.JniPMML"
            , ExplicitRegistration = true
            , HelpTopic = "Help Topic to be filled in"
            , IsVolatile = false
            )]
        public static String JniPMML_Handle(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0,
            [ExcelArgument(Name = "AdditionalRangeDependencies")] Object arg
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            List<object> cmargs = new List<object>{HandleMajorFrom(arg0) };
            String rv = Java.CallMethod<String>("Handle", "(I)Ljava/lang/String;", cmargs);
            cmargs = null;
            return rv;
        }

        public static JniPMMLItem UpdateMajorMinor(String aEvaluatorType, object[] args)
        {
            JniPMMLItem rv=null;
            if ( !AddIn.__JniPMML.TryGetObject(args[0].ToString(), out rv) ) return rv;
            rv.UpdateHandleMajorMinor();
            return rv;
        }

        [ExcelFunction(
                Name = "JniPMML_CreateHandle"
                , Category = "WDS.JniPMML"
                , Description = "Creates a handle and initializes with a PMML File, input as either a FileName or a String."
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static Object JniPMML_CreateHandle(
              [ExcelArgument(Name = "HandleType", Description = "[JniPMML|others....]")] String arg0
            , [ExcelArgument(Name = "Tag", Description = "String Handle")] String arg1
            , [ExcelArgument(Name = "FileNameOrString", Description = "Path to external PMML filename or entire file as a string (an entire string is determined by the usual XML starting characters)")] String arg3
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            int arg2 = 0;
            if ( !arg3.startsWith("<?") ) arg2 = 1;
            if ( arg0.Equals("JniPMML") ) {
                object[] chargs=new object[]{ arg1,arg2,arg3 };
                Object rv = null;
                lock (AddIn.__JniPMML)
                {
                    rv=AddIn.__JniPMML.CreateHandle(arg0, chargs, (arg, args) => AddIn.UpdateMajorMinor(arg, args));
                }
                chargs = null;
                return rv;
            }
            else {
                return "HandleType not implemented";
            }

        }

        [ExcelFunction(
                Name = "JniPMML_LoadFromString"
                , Category = "WDS.JniPMML"
                , Description = "Creates a handle and parses PMML model from string input, caching in JVM"
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String JniPMML_LoadFromString(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0,
            [ExcelArgument(Name = "FileName")] String arg1
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            int h = HandleMajorFrom(arg0);
            JniPMMLItem aJniPMMLItem;
            if ( !AddIn.__JniPMML.TryGetObject(h, out aJniPMMLItem) ) {
                aJniPMMLItem = new JniPMMLItem(h.ToString(), AddIn.__JniPMML);
                AddIn.__JniPMML.Add(aJniPMMLItem);
                aJniPMMLItem = null;
            }
            List<object> cmargs=new List<object> { aJniPMMLItem.__Handle, arg1 };
            String rv = Java.CallMethod<string>("mPMMLLoadFromString", "(ILjava/lang/String;)Ljava/lang/String;", cmargs);
            cmargs = null;
            return rv;
        }


        [ExcelFunction(
                Name = "JniPMML_LoadFromFile"
                , Category = "WDS.JniPMML"
                , Description = "Creates a handle and parses PMML model from an external file, caching in JVM"
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String JniPMML_LoadFromFile(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0,
            [ExcelArgument(Name = "FileName")] String arg1
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            int h = HandleMajorFrom(arg0);
            JniPMMLItem aJniPMMLItem;
            if ( !AddIn.__JniPMML.TryGetObject(h, out aJniPMMLItem) ) {
                aJniPMMLItem = new JniPMMLItem(h.ToString(), AddIn.__JniPMML);
                AddIn.__JniPMML.Add(aJniPMMLItem);
                aJniPMMLItem = null;
            }
            List<object> cmargs=new List<object> { aJniPMMLItem.__Handle, arg1 };
            String rv = Java.CallMethod<string>("mPMMLLoadFromString"
                        , "(ILjava/lang/String;)Ljava/lang/String;"
                        , cmargs);
            cmargs = null;
            return rv;
        }


        [ExcelFunction(
                Name = "JniPMML_LoadedString"
                , Category = "WDS.JniPMML"
                , Description = "Returns the string that has been parsed and is cached in the JVM"
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String JniPMML_LoadedString(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            int h = 0;
            try
            {
                h = HandleMajorFrom(arg0);
            } catch (Exception e)
            {
                return e.Message;
            }
            if (h < -1)
                return "Error, handle/tag not cached yet";
            List<object> cmargs = new List<object>{h};
            String rv=Java.CallMethod<string>("sPMMLLoadedString", "(I)Ljava/lang/String;", cmargs);
            cmargs = null;
            return rv;
        }



        [ExcelFunction(
                Name = "JniPMML_LoadedFileName"
                , Category = "WDS.JniPMML"
                , Description = "Returns the string that has been parsed and is cached in the JVM"
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String JniPMML_LoadedFileName(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0
            )
        {
            if ( ExcelDnaUtil.IsInFunctionWizard() ) return "In Function Wizard, holding calls to Java";
            int h = 0;
            try
            {
                h = HandleMajorFrom(arg0);
            } catch (Exception e)
            {
                return e.Message;
            }
            if (h < -1)
                return "Error, handle/tag not cached yet";
            List<object> cmargs = new List<object>{h};
            String rv=Java.CallMethod<string>("sPMMLLoadedFileName", "(I)Ljava/lang/String;", cmargs);
            cmargs = null;
            return rv;
        }


        [ExcelFunction(
            Name = "JniPMML_Eval_Volatile"
            , Category = "WDS.JniPMML"
            , Description = "A volatile self contained call to the JniPMML evaluator.  The first argument is just to turn it off/on to kill the drag on calculation time."
            , IsThreadSafe = true
            , IsMacroType = true
            , IsVolatile = false
            , ExplicitRegistration = true)]
        public static object[,] JniPMML_Eval_Volatile(
              [ExcelArgument(Name = "bToCalcSwitch", Description = "0/1")] int arg0
            , [ExcelArgument(Name = "PMMLInput", Description = "Path to external PMML filename or entire file as a string (an entire string is determined by the usual XML starting characters)")] string PMMLFile
            , [ExcelArgument(Name = "bInputDataHasHeaderRow", Description = "0/1, If input includes header row, output will include header row.")] int bInputDataHasHeaderRow
            , [ExcelArgument(Name = "InputTableReference", Description = "An XMLMap'd Table, column names are taken from the XMLMap", AllowReference = true)] object arg
            , [ExcelArgument(Name = "nOutputStringMaxLength", Description = "An alternate maximum string length for output fields, defaults to 64", AllowReference = false)] object _nOutputStringMaxLength
            )
        {
            //objects to be returned or GC'd
            object[,] rv = null;
            //short circuit....
            if ( ExcelDnaUtil.IsInFunctionWizard() ) {
                rv = new object[1, 1];
                rv[0, 0] = "In Function Wizard, holding calls to Java";
                return rv;
            }
            if ( arg0 == 0 ) {
                rv = new object[1, 1];
                rv[0, 0] = "Calc Turned Off";
                return rv;
            }

            String aTag="Internal";
            JniPMMLItem aJniPMMLItem=null;
            ExcelReference argref;
            MOIE.Application tapp;
            MOIE.Range trng;
            MOIE.XmlMap aXmlMap;
            MOIE.ListObject aListObject;

            int h=-1;
            try {
                aJniPMMLItem = AddIn.__JniPMML.CreateHandle("JniPMML", aTag, 0, PMMLFile);
                h = aJniPMMLItem.Handle();
                try {
                    argref = (ExcelReference) arg;
                    tapp = (ExcelDnaUtil.Application as MOIE.Application);
                    trng = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, argref, true)) as MOIE.Range;
                    aListObject = trng.ListObject;
                    aXmlMap = aListObject.XmlMap;
                }
                catch ( Exception ) {
                    throw new com.WDataSci.WDS.WDSException("Error, could not pull XMLMap from argument");
                }
                int nOutputStringMaxLength = __OptionalIntValue(_nOutputStringMaxLength, 64);
                object rv1=JniPMML_Eval_CacheHeaders_guts(h,aListObject.XmlMap,aListObject.XmlMap.Schemas[1].XML, nOutputStringMaxLength);
                rv = JniPMML_Eval(h, bInputDataHasHeaderRow, (object[,]) (trng.Value2));
                //rv = JniPMML_Eval(h, bInputDataHasHeaderRow, (object[,]) (trng.CurrentRegion.Value2));
            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                rv = new object[1, 1];
                rv[0, 0] = e.getMessage();
            }
            catch ( Exception e ) {
                rv = new object[1, 1];
                rv[0, 0] = e.Message;
            }
            finally {
                //Queuing for GC
                if ( h >= 0 && aJniPMMLItem != null )
                    aJniPMMLItem.__JniPMML.Remove(h);
                aJniPMMLItem = null;
                aXmlMap = null;
                aListObject = null;
                argref = null;
                trng = null;
                tapp = null;
            }

            return rv;
        }


        protected static object JniPMML_Eval_CacheHeaders_guts(
            int h
            , MOIE.XmlMap xm
            , String xmschema
            , int nOutputStringMaxLength
            )
        {

            //objects to be returned or GC'd
            object[,] rv = null;

            List<object> cmargs = null;

            RecordSetMD aRecordSetMD=null;

            byte[] bHeaderBlock=null;
            GCHandle bHeaderBlockGCH=new GCHandle();
            IntPtr bHeaderBlockPtr=IntPtr.Zero;
            Span<byte> bHeaderBlockSpanByte=null;
            DBB bHeaderBlockDBB=null;


            JniPMMLItem aJniPMMLItem=null;

            try {
                if ( !AddIn.__JniPMML.TryGetObject(h, out aJniPMMLItem) )
                    throw new com.WDataSci.WDS.WDSException("Error in JniPMMLEval_CacheHeaders_guts while pulling JniPMMLItem by handle");

                if ( !xm.IsExportable )
                    throw new com.WDataSci.WDS.WDSException("Error in JniPMMLEval_CacheHeaders_guts, XMLMap is not exportable");

                if ( aJniPMMLItem.InputMatter == null ) aJniPMMLItem.InputMatter = new JniPMMLItem.__InputMatter();
                if ( aJniPMMLItem.OutputMatter == null ) aJniPMMLItem.OutputMatter = new JniPMMLItem.__OutputMatter();

                aJniPMMLItem.InputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Internal);
                aJniPMMLItem.InputMatter.RecordSetMD
                    .cAs(RecordSetMDEnums.eType.DBB, RecordSetMDEnums.eSchemaType.XSD)
                    ;
                aRecordSetMD = aJniPMMLItem.InputMatter.RecordSetMD;

                aRecordSetMD.SchemaMatter.InputSchema = new XmlDocument();
                aRecordSetMD.SchemaMatter.InputSchema.LoadXml(xmschema);

                aRecordSetMD.mReadMapFor(null, null, true);

                int nColumns = aRecordSetMD.nColumns();

                long csize = 0;
                long hsize = 0;
                long rsize = 0;
                long cleadsize = 0;
                long hleadsize = 0;
                long hflensize = 0;
                long hvlensize = 0;
                long rleadsize = 0;
                long rflensize = 0;
                long rvlensize = 0;
                aRecordSetMD.mBytesRequired(1, out csize, out hsize, out rsize
                        , out cleadsize
                        , out hleadsize, out hflensize, out hvlensize
                        , out rleadsize, out rflensize, out rvlensize
                    );


                unsafe {

                    //the common class instance being called via jni
                    String aMethodName = "";
                    String aSignatureString = "";
                    IntPtr aMethodID = IntPtr.Zero;
                    //the argument list for the jni method calls
                    cmargs = new List<object>();

                    int nHeaderAlloc = (int)hsize;
                    if ( nHeaderAlloc < 65536 ) nHeaderAlloc = 65536;

                    bHeaderBlock = new byte[nHeaderAlloc];
                    bHeaderBlockGCH = GCHandle.Alloc(bHeaderBlock, GCHandleType.Pinned);
                    bHeaderBlockPtr = Java.Env().NewDirectByteBuffer(bHeaderBlockGCH.AddrOfPinnedObject(), nHeaderAlloc);
                    bHeaderBlockSpanByte = new Span<byte>(bHeaderBlock, 0, nHeaderAlloc);

                    fixed ( byte* bHeaderBlockSpanBytePtr = bHeaderBlockSpanByte ) {

                        bHeaderBlockDBB = new DBB(ref bHeaderBlock);
                        aRecordSetMD.cSetHeaderBufferAs(bHeaderBlockDBB, nColumns, 40, 2 * (int) aRecordSetMD.nHeaderByteMaxLength());
                        aRecordSetMD.mWriteMapToBuffer();

                        String rmsg = "";

                        cmargs.Clear();
                        cmargs.Add(h);
                        cmargs.Add(bHeaderBlockPtr);
                        aMethodName = "mReadMapFromByteBuffer";
                        aSignatureString = "(ILjava/nio/ByteBuffer;)Ljava/lang/String;";
                        aMethodID = Java.FindMethodID(java_init_classid, aMethodName, aSignatureString);
                        rmsg = Java.CallMethod<String>(aMethodID, false, aSignatureString, cmargs);


                        cmargs.Clear();
                        cmargs.Add(h);
                        cmargs.Add((int) aRecordSetMD.nHeaderStringMaxLength());
                        cmargs.Add((int) nOutputStringMaxLength);
                        //cmargs.Add((int) aRecordSetMD.ModeMatter.OutputMaxStringLength);
                        aMethodName = "mPreRunPrepOutputMap";
                        aSignatureString = "(III)I";
                        aMethodID = Java.FindMethodID(java_init_classid, aMethodName, aSignatureString);
                        int nOutputColumns = Java.CallMethod<int>(aMethodID, false, aSignatureString, cmargs);

                        if ( 4 + nOutputColumns * (40 + 2 * 128) > 65536 )
                            throw new WDSException("Error, too many columns comming back from java code for header data");

                        cmargs.Clear();
                        cmargs.Add(h);
                        cmargs.Add(bHeaderBlockPtr);
                        cmargs.Add((int) aRecordSetMD.nHeaderStringMaxLength());
                        cmargs.Add((int) nOutputStringMaxLength);
                        //cmargs.Add((int) aRecordSetMD.ModeMatter.OutputMaxStringLength);
                        aMethodName = "mPreRunWriteOutputMapToByteBuffer";
                        aSignatureString = "(ILjava/nio/ByteBuffer;II)Ljava/lang/String;";
                        aMethodID = Java.FindMethodID(java_init_classid, aMethodName, aSignatureString);
                        rmsg = Java.CallMethod<String>(aMethodID, false, aSignatureString, cmargs);

                        aJniPMMLItem.OutputMatter.RecordSetMD = new RecordSetMD(RecordSetMDEnums.eMode.Internal);
                        aJniPMMLItem.OutputMatter.RecordSetMD
                            .cAs(RecordSetMDEnums.eType.DBB)
                            .cSetHeaderBufferFrom(bHeaderBlockDBB)
                            .mReadMapFor(null, null, true)
                            ;

                        rv = new object[1, 1];
                        rv[0, 0] = rmsg;
                    }
                }
            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                rv = new object[1, 1];
                rv[0, 0] = e.getMessage();
            }
            catch ( Exception e ) {
                rv = new object[1, 1];
                rv[0, 0] = "Error, " + e.Message;
            }
            finally {
                //queuing up for GC
                aJniPMMLItem = null;
                xm = null;
                bHeaderBlockDBB = null;
                bHeaderBlockSpanByte.Clear();
                bHeaderBlockPtr = IntPtr.Zero;
                if (bHeaderBlockGCH.IsAllocated) bHeaderBlockGCH.Free();
                bHeaderBlock = null;
                cmargs = null;
                aRecordSetMD = null;
            }
            return rv;
        }


        [ExcelFunction(
            Name = "JniPMML_Eval_CacheHeaders_Volatile"
            , Category = "WDS.JniPMML"
            , Description = "Caches just the input and output headers for Eval, on both the C# and Java sides, subsequent calls to WDS.JniPMML__Headerless can follow.  Set the headerless calls to depend on this \"Major.Minor\" output."
            , IsThreadSafe = true
            , IsMacroType = true
            , IsVolatile = false
            , ExplicitRegistration = true)]
        public static object JniPMML_Eval_CacheHeaders_Volatile(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use a \"Major.Minor\" Handle output from CreateHandle to chain calcuation dependency")] Object arg0
            , [ExcelArgument(Name = "XmlMappedList", Description = "Point to a reference cell or range of the XmlMap'd list (one that does not change with data, such as the header)", AllowReference = true)] Object arg
            , [ExcelArgument(Name = "nOutputStringMaxLength", Description = "An alternate maximum string length for output fields, defaults to 64", AllowReference = false)] object _nOutputStringMaxLength
            )
        {

            //objects to be returned or GC'd
            object rv = null;
            //short circuit....
            if ( ExcelDnaUtil.IsInFunctionWizard() ) {
                rv = new object();
                rv = "In Function Wizard, holding calls to Java";
                return rv;
            }

            JniPMMLItem aJniPMMLItem=null;
            ExcelReference argref;
            MOIE.Application tapp = null;
            MOIE.Range trng;
            MOIE.ListObject aListObject;
            MOIE.XmlMap aXmlMap = null;

            try {
                int h = HandleMajorFrom(arg0);
                if ( (h < -1 && !AddIn.__JniPMML.TryGetObject(arg0.ToString(), out aJniPMMLItem)) || (-2 < h && h < 0) )
                    throw new com.WDataSci.WDS.WDSException("Error, Invalid Handle");
                else if ( aJniPMMLItem == null && !AddIn.__JniPMML.TryGetObject(h, out aJniPMMLItem) )
                    throw new com.WDataSci.WDS.WDSException("Error, Invalid Handle");
                if ( h < 0 )
                    h = aJniPMMLItem.Handle();

                tapp = (ExcelDnaUtil.Application as MOIE.Application);
                try {
                    argref = (ExcelReference) arg;
                    tapp = (ExcelDnaUtil.Application as MOIE.Application);
                    trng = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, argref, true)) as MOIE.Range;
                    aListObject = trng.ListObject;
                    aXmlMap = aListObject.XmlMap;
                }
                catch ( Exception ) {
                    throw new com.WDataSci.WDS.WDSException("Error, could not pull XMLMap from argument");
                }
                int nOutputStringMaxLength = __OptionalIntValue(_nOutputStringMaxLength, 64);
                rv = JniPMML_Eval_CacheHeaders_guts(h, aXmlMap, aXmlMap.Schemas[1].XML,nOutputStringMaxLength);
            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                rv = new object();
                rv = e.getMessage();
            }
            catch ( Exception e ) {
                rv = new object();
                rv = e.Message;
            }
            finally {
                //Queuing up for GC
                aJniPMMLItem = null;
                aListObject = null;
                aXmlMap = null;
                trng = null;
                tapp = null;
                argref = null;
            }
            return rv;
        }



        [ExcelFunction(
            Name = "JniPMML_Eval_OutputColumnHeadings"
            , Category = "WDS.JniPMML"
            , Description = "Returns just the field names associated with a cached model and headers"
            , IsThreadSafe = true
            , IsMacroType = true
            , IsVolatile = false
            , ExplicitRegistration = true)]
        public static object[,] JniPMML_Eval_OutputColumnHeadings(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use the \"Major.Minor\" Handle output for the matching Model to maintain dependency")] Object arg0
            )
        {

            object[,] rv = null;

            if ( ExcelDnaUtil.IsInFunctionWizard() ) {
                rv = new object[1, 1];
                rv[0, 0] = "In Function Wizard, holding calls to Java";
                return rv;
            }

            int h = HandleMajorFrom(arg0);
            JniPMMLItem aJniPMMLItem;
            if ( !AddIn.__JniPMML.TryGetObject(h, out aJniPMMLItem) ) {
                rv = new object[1, 1];
                rv[0, 0] = "Error, Invalid Handle";
                return rv;
            }
            if (aJniPMMLItem.OutputMatter==null || aJniPMMLItem.OutputMatter.RecordSetMD==null)
            {
                rv = new object[1, 1];
                rv[0, 0] = "Error, Header information not yet cached";
                return rv;
            }


            RecordSetMD aRecordSetMD = aJniPMMLItem.OutputMatter.RecordSetMD;

            rv = new object[1, aRecordSetMD.nColumns()];

            for ( int j = 0; j < aJniPMMLItem.OutputMatter.RecordSetMD.Column.Length; j++ )
                rv[0, j] = aJniPMMLItem.OutputMatter.RecordSetMD.Column[j].Name;

            aJniPMMLItem = null;

            return rv;

        }



        private static int JniPMML_Eval_guts(
            int h,
            ref JniPMMLItem aJniPMMLItem,
            int bInputDataHasHeaderRow,
            Object[,] data,
            String sFileType,
            String sFileName,
            out object[,] rv
            )
        {

            int rc=-1;

            RecordSetMD aRecordSetMD = aJniPMMLItem.InputMatter.RecordSetMD;

            List<object> cmargs = null;

            byte[] bHeaderBlock=null;
            GCHandle bHeaderBlockGCH=new GCHandle();
            IntPtr bHeaderBlockPtr=IntPtr.Zero;
            Span<byte> bHeaderBlockSpanByte=null;
            DBB bHeaderBlockDBB=null;

            byte[] bRecordSetBlock = null;
            GCHandle bRecordSetSetBlockGCH = new GCHandle();
            IntPtr bRecordSetBlockPtr = IntPtr.Zero;
            Span<byte> bRecordSetBlockSpanByte = null;
            DBB bRecordSetDBB=null;
            RecordSet rvRS = null;


            try {
                int rowstartindex = data.GetLowerBound(0);
                int nRows = data.GetUpperBound(0) - rowstartindex + 1;
                if ( bInputDataHasHeaderRow != 0 ) nRows -= 1;
                int nColumns = data.GetUpperBound(1) - data.GetLowerBound(1) + 1;

                long csize = 0;
                long hsize = 0;
                long cleadsize = 0;
                long hleadsize = 0;
                long hflensize = 0;
                long hvlensize = 0;
                long rleadsize = 0;
                long rflensize = 0;
                long rvlensize = 0;
                long rsize = 0;

                aJniPMMLItem.InputMatter.RecordSetMD.mBytesRequired(nRows, out csize, out hsize, out rsize
                        , out cleadsize
                        , out hleadsize, out hflensize, out hvlensize
                        , out rleadsize, out rflensize, out rvlensize
                    );

                int nRecordSetAlloc = (int)rsize;


                aJniPMMLItem.OutputMatter.RecordSetMD.mBytesRequired(nRows, out csize, out hsize, out rsize
                        , out cleadsize
                        , out hleadsize, out hflensize, out hvlensize
                        , out rleadsize, out rflensize, out rvlensize
                    );

                if ( rsize > nRecordSetAlloc ) nRecordSetAlloc = (int) rsize;

                nRecordSetAlloc = 65536 * ((int) (nRecordSetAlloc / 65536 + 1));

                int nOutputColumns=aJniPMMLItem.OutputMatter.RecordSetMD.nColumns();

                unsafe {

                    //the common class instance being called via jni
                    String aMethodName = "";
                    String aSignatureString = "";
                    IntPtr aMethodID = IntPtr.Zero;
                    //the argument list for the jni method calls
                    cmargs = new List<object>();

                    int nHeaderAlloc = 65536;
                    bHeaderBlock = new byte[nHeaderAlloc];
                    bHeaderBlockGCH = GCHandle.Alloc(bHeaderBlock, GCHandleType.Pinned);
                    bHeaderBlockPtr = Java.Env().NewDirectByteBuffer(bHeaderBlockGCH.AddrOfPinnedObject(), nHeaderAlloc);
                    bHeaderBlockSpanByte = new Span<byte>(bHeaderBlock, 0, nHeaderAlloc);

                    fixed ( byte* bHeaderBlockSpanBytePtr = bHeaderBlockSpanByte ) {

                        bHeaderBlockDBB = new DBB(ref bHeaderBlock);

                        bRecordSetBlock = new byte[nRecordSetAlloc];
                        bRecordSetSetBlockGCH = GCHandle.Alloc(bRecordSetBlock, GCHandleType.Pinned);
                        bRecordSetBlockPtr = Java.Env().NewDirectByteBuffer(bRecordSetSetBlockGCH.AddrOfPinnedObject(), nRecordSetAlloc);
                        bRecordSetBlockSpanByte = new Span<byte>(bRecordSetBlock, 0, nRecordSetAlloc);


                        fixed ( byte* bRecordSetBlockSpanBytePtr = bRecordSetBlockSpanByte ) {

                            bRecordSetDBB = new DBB(ref bRecordSetBlock);

                            aRecordSetMD.cSetRecordSetBufferAs(bRecordSetDBB);
                            aRecordSetMD.DBBMatter.mWriteRecordSet(aRecordSetMD, data, (bInputDataHasHeaderRow != 0));

                            try {

                                switch ( sFileType ) {

                                    case "CSV":
                                    case "TXT":
                                    case "HDF5":

                                        cmargs.Clear();
                                        cmargs.Add(bRecordSetBlockPtr);
                                        cmargs.Add(sFileName);
                                        cmargs.Add(sFileType);
                                        cmargs.Add(64);
                                        aMethodName = "mEvaluateRecordSetWithFileOutput";
                                        aSignatureString = "(Ljava/nio/ByteBuffer;Ljava/lang/String;Ljava/lang/String;I)I";
                                        aMethodID = Java.FindMethodID(java_init_classid, aMethodName, aSignatureString);
                                        rc = Java.CallMethod<int>(aMethodID, false, aSignatureString, cmargs);
                                        rv = null;
                                        break;

                                    default:

                                        cmargs.Clear();
                                        cmargs.Add(h);
                                        cmargs.Add(bRecordSetBlockPtr);
                                        aMethodName = "mEvaluateRecordSetAndHoldResults";
                                        aSignatureString = "(ILjava/nio/ByteBuffer;)I";
                                        aMethodID = Java.FindMethodID(java_init_classid, aMethodName, aSignatureString);
                                        int nrvRows = Java.CallMethod<int>(aMethodID, false, aSignatureString, cmargs);

                                        rvRS = new RecordSet();

                                        int outputrowstartindex = 0;
                                        if ( bInputDataHasHeaderRow != 0 ) outputrowstartindex = 1;
                                        rv = new object[nRows + outputrowstartindex, nOutputColumns];

                                        if ( rsize < nRecordSetAlloc ) {
                                            cmargs.Clear();
                                            cmargs.Add(h);
                                            cmargs.Add(bRecordSetBlockPtr);
                                            cmargs.Add((int) nrvRows);
                                            cmargs.Add((long) rflensize);
                                            cmargs.Add((long) rvlensize);
                                            cmargs.Add((long) (rleadsize + nrvRows * rvlensize));
                                            cmargs.Add((long) rsize);
                                            aMethodName = "mWriteOutputRecordSetToByteBuffer";
                                            aSignatureString = "(ILjava/nio/ByteBuffer;IJJJJ)I";
                                            aMethodID = Java.FindMethodID(java_init_classid, aMethodName, aSignatureString);
                                            rc = Java.CallMethod<int>(aMethodID, false, aSignatureString, cmargs);
                                            aJniPMMLItem.OutputMatter.RecordSetMD.cSetRecordSetBufferFrom(bRecordSetDBB);

                                            rvRS.mReadRecordSet(aJniPMMLItem.OutputMatter.RecordSetMD);

                                            if ( bInputDataHasHeaderRow != 0 ) {
                                                for ( int j = 0; j < aJniPMMLItem.OutputMatter.RecordSetMD.Column.Length; j++ )
                                                    rv[0, j] = aJniPMMLItem.OutputMatter.RecordSetMD.Column[j].Name;
                                            }
                                            for ( int j = 0; j < aJniPMMLItem.OutputMatter.RecordSetMD.Column.Length; j++ ) {
                                                for ( int i = 0, ii = outputrowstartindex; i < rvRS.Records_Orig.Count; i++, ii++ )
                                                    rv[ii, j] = rvRS.Records_Orig[i][j];
                                            }

                                        }
                                        break;
                                }

                            }
                            catch ( Exception e ) {
                                throw new com.WDataSci.WDS.WDSException("Error in JniPMML_Eval_guts", e);
                            }

                        }
                    }
                }
            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                rv = new object[1, 1];
                rv[0, 0] = e.getMessage();
            }
            catch ( Exception e ) {
                rv = new object[1, 1];
                rv[0, 0] = "Error, " + e.Message + ", " + e.StackTrace;
            }
            finally {
                cmargs = null;
                if ( aRecordSetMD != null ) aRecordSetMD.Dispose();
                aRecordSetMD = null;
                if ( bHeaderBlockDBB != null ) bHeaderBlockDBB.Dispose();
                bHeaderBlockDBB = null;
                if ( bRecordSetDBB != null ) bRecordSetDBB.Dispose();
                bRecordSetDBB = null;
                if ( rvRS != null ) rvRS.Dispose();
                rvRS = null;

                bRecordSetBlockSpanByte.Clear();
                if (bRecordSetSetBlockGCH.IsAllocated) bRecordSetSetBlockGCH.Free();
                bRecordSetBlock = null;

                bHeaderBlockSpanByte.Clear();
                if (bHeaderBlockGCH.IsAllocated) bHeaderBlockGCH.Free();
                bHeaderBlock = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
            if ( rv!=null && rv.Length == 1 && rv[0, 0].toString().startsWith("Error") )
                throw new com.WDataSci.WDS.WDSException(rv[0, 0].toString());
            return rc;
        }


        [ExcelFunction(
            Name = "JniPMML_Eval"
            , Category = "WDS.JniPMML"
            , Description = "Calls JniPMML.Eval based a previously set Header"
            , IsThreadSafe = true
            , IsMacroType = true
            , IsVolatile = false
            , ExplicitRegistration = true)]
        public static object[,] JniPMML_Eval(
            [ExcelArgument(Name = "HandleOrTag", Description = "Use the \"Major.Minor\" Handle output for the matching Model to maintain dependency")] Object arg0,
            [ExcelArgument(Name = "InputDataIncludesHeader", Description = "0/1 indicates both to skip eval on first row and whether to include output header")] int bInputDataHasHeaderRow,
            [ExcelArgument(Name = "InputData", Description = "Select Contiguous ListObject Rows, include header if needed for alignment")] Object[,] data
            )
        {

            object[,] rv = null;

            if ( ExcelDnaUtil.IsInFunctionWizard() ) {
                rv = new object[1, 1];
                rv[0, 0] = "In Function Wizard, holding calls to Java";
                return rv;
            }

            int h = HandleMajorFrom(arg0);
            JniPMMLItem aJniPMMLItem;
            if ( !AddIn.__JniPMML.TryGetObject(h, out aJniPMMLItem) ) {
                rv = new object[1, 1];
                rv[0, 0] = "Error, Invalid Handle";
                return rv;
            }

            int rc=JniPMML_Eval_guts(h, ref aJniPMMLItem, bInputDataHasHeaderRow, data, "NA", "", out rv);
            return rv;

        }

        [ExcelCommand(Description = "Evaluate cached PMML on XmlMapped List", ExplicitRegistration = true)]
        public static void JniPMML_Eval_XmlMappedList()
        {

            //Typing for possible GC purposes
            MOIE.Application tapp = null;
            ExcelReference argref;
            MOIE.Range trng = null;
            MOIE.Range trng2 = null;
            MOIE.XmlMap aXmlMap = null;
            MOIE.ListObject aListObject = null;
            MOIE.ListObject aListObject2 = null;
            MOIE.Workbook twb = null;
            MOIE.Sheets twbSheets = null;
            MOIE.Worksheet tws = null;
            object rv1=null;
            MessageBoxButtons msgboxbuttons;
            DialogResult msgboxresponse;

            JniPMMLItem aJniPMMLItem=null;

            int h=-1;
            Boolean bIsModelCached=true;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior=tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior=tapp.Calculation;

            try {


                int i, j, iP1, jP1;

                argref = (ExcelReference) XlCall.Excel(XlCall.xlfSelection);

                trng = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, argref, true)) as MOIE.Range;
                try {
                    aListObject = trng.ListObject;
                    aXmlMap = aListObject.XmlMap;
                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error, could  not pull XmlMap from selection", e);
                }

                int bIsPMMLInputFileOrString=0;
                String sFileName="";

                msgboxbuttons = MessageBoxButtons.YesNoCancel;
                msgboxresponse = MessageBox.Show("Use a Cached Model (Yes/no)?", "Confirm", msgboxbuttons);

                if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                    throw new com.WDataSci.WDS.WDSException("Cancel");

                if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes ) {
                    String HandleString=JniPMML_Handle_LastUsed(null);
                    String aTag;
                    try {
                        if ( HandleString.StartsWith("-1") )
                            throw new com.WDataSci.WDS.WDSException("ERROR, no model cached yet!  If workbook was just opened, possibly just recalc first and try again.");
                        aTag = tapp.InputBox("", "Tag or Handle for Cached Model", __JniPMML.Tag(HandleMajorFrom(HandleString)));
                    }
                    catch ( com.WDataSci.WDS.WDSException e ) {
                        throw e;
                    }
                    catch {
                        throw new com.WDataSci.WDS.WDSException("Cancel");
                    }
                    h = __JniPMML.Handle(aTag);
                    aJniPMMLItem = __JniPMML.Item[h];
                }
                if ( msgboxresponse == System.Windows.Forms.DialogResult.No || h < 0 ) {
                    bIsModelCached = false;
                    msgboxresponse = MessageBox.Show("Would you like to point to a PMML file (Yes/no)?", "Confirm", msgboxbuttons);
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                        throw new com.WDataSci.WDS.WDSException("Cancel");
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes )
                        using ( OpenFileDialog aOpenFileDialog = new OpenFileDialog() ) {
                            bIsPMMLInputFileOrString = 1;
                            aOpenFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                            aOpenFileDialog.Filter = "PMML File (*.xml)|*.xml|All files (*.*)|*.*";
                            aOpenFileDialog.FilterIndex = 2;
                            aOpenFileDialog.RestoreDirectory = true;
                            aOpenFileDialog.FileName = sFileName;
                            aOpenFileDialog.AddExtension = true;
                            aOpenFileDialog.DefaultExt = ".xml";
                            aOpenFileDialog.CheckFileExists = true;
                            aOpenFileDialog.CheckPathExists = false;
                            aOpenFileDialog.Title = "PMML File....";

                            if ( aOpenFileDialog.ShowDialog() == DialogResult.OK )
                                sFileName = aOpenFileDialog.FileName;
                            else
                                throw new com.WDataSci.WDS.WDSException("Cancel");
                        }
                    else {
                        bIsPMMLInputFileOrString = 0;
                        try {
                            MOIE.Range trng3 = tapp.InputBox("Use a PMML file contained as one string in cell, enter cell address (navigable)", "PMML Input", "Entire PMML File as a String",100,100,"",0,8) as MOIE.Range;
                            sFileName = trng3.Text;
                            trng3 = null;
                            if ( !sFileName.StartsWith("<?xml") ) {
                                if ( sFileName.IndexOf("!") < 0 )
                                    sFileName = "'[" + tapp.ActiveWorkbook.Name + "]" + aListObject.DataBodyRange.Worksheet.Name + "'!" + sFileName;
                                ExcelReference rf=XlCall.Excel(XlCall.xlfEvaluate,sFileName) as ExcelReference;
                                trng3 = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, rf, true)) as MOIE.Range;
                                sFileName = trng3.Text;
                                rf = null;
                                trng3 = null;
                            }
                        }
                        catch {
                            throw new com.WDataSci.WDS.WDSException("Cancel");
                        }
                    }
                    Random rndm = new Random();
                    aJniPMMLItem = __JniPMML.CreateHandle("JniPMML", "Internal" +rndm.Next(0,100000).ToString(), bIsPMMLInputFileOrString, sFileName);
                    h = aJniPMMLItem.Handle();
                    bIsModelCached = false;
                }
                if ( aJniPMMLItem == null )
                    throw new com.WDataSci.WDS.WDSException("Error, unable to use pre-cached or new JniPMML");

                int nOutputStringMaxLength = 64;
                rv1 = JniPMML_Eval_CacheHeaders_guts(h, aListObject.XmlMap, aListObject.XmlMap.Schemas[1].XML, nOutputStringMaxLength);

                String sOutputFileType="NA";
                String sOutputFileName="";

                Boolean isContinuing=true;
                msgboxbuttons = MessageBoxButtons.YesNoCancel;
                msgboxresponse = MessageBox.Show("Return to a worksheet (Yes) or a file (No)?", "Confirm", msgboxbuttons);

                if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                    throw new com.WDataSci.WDS.WDSException("Cancel");

                if ( msgboxresponse == System.Windows.Forms.DialogResult.No ) {
                    msgboxresponse = MessageBox.Show("Write to HDF5 (Yes) or a flat file (No)?", "Confirm", msgboxbuttons);
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                        throw new com.WDataSci.WDS.WDSException("Cancel");
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes ) {
                        sOutputFileType = "HDF5";
                        using ( SaveFileDialog aSaveFileDialog = new SaveFileDialog() ) {
                            sOutputFileName = "test.h5";
                            aSaveFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                            aSaveFileDialog.Filter = "HDF5 Files (*.H5)|*.h5|All files (*.*)|*.*";
                            aSaveFileDialog.FilterIndex = 2;
                            aSaveFileDialog.RestoreDirectory = true;
                            aSaveFileDialog.FileName = sOutputFileName;
                            aSaveFileDialog.CheckPathExists = false;
                            aSaveFileDialog.CheckFileExists = false;
                            aSaveFileDialog.DefaultExt = ".h5";
                            aSaveFileDialog.AddExtension = true;
                            aSaveFileDialog.Title = "Export XmlMap'd ListObject to HDF5 (*.h5) File....";

                            if ( aSaveFileDialog.ShowDialog() == DialogResult.OK ) {
                                sOutputFileName = aSaveFileDialog.FileName;
                                if ( !sOutputFileName.ToLower().EndsWith(".h5") )
                                    sOutputFileName += ".h5";
                            }
                            else
                                isContinuing = false;

                        }
                    }
                    else {
                        sOutputFileType = "CSV";
                        using ( SaveFileDialog aSaveFileDialog = new SaveFileDialog() ) {
                            sOutputFileName = "test.csv";
                            aSaveFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                            aSaveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                            aSaveFileDialog.FilterIndex = 2;
                            aSaveFileDialog.RestoreDirectory = true;
                            aSaveFileDialog.FileName = sOutputFileName;
                            aSaveFileDialog.CheckPathExists = false;
                            aSaveFileDialog.CheckFileExists = false;
                            aSaveFileDialog.DefaultExt = ".csv";
                            aSaveFileDialog.AddExtension = true;
                            aSaveFileDialog.Title = "Export XmlMap'd ListObject to CSV (*.csv) File....";

                            if ( aSaveFileDialog.ShowDialog() == DialogResult.OK ) {
                                sOutputFileName = aSaveFileDialog.FileName;
                                if ( sOutputFileName.ToLower().endsWith(".txt") )
                                    sOutputFileType = "TXT";
                                if ( !sOutputFileName.ToLower().EndsWith(".csv") )
                                    sOutputFileName += ".h5";
                            }
                            else
                                isContinuing = false;

                        }
                    }
                }

                if ( isContinuing ) {

                    object[,] rv=null;

                    int rc=JniPMML_Eval_guts(h, ref aJniPMMLItem, 1, aListObject.Range.Value2, sOutputFileType,sOutputFileName,out rv);

                    switch ( sOutputFileType ) {

                        case "CSV":
                        case "TXT":
                        case "HDF5":
                            if ( rc < 0 )
                                throw new com.WDataSci.WDS.WDSException(rv[0, 0].ToString());
                            MessageBox.Show(String.Format("Evaluated {0} records", rc));
                            break;
                        default:

                            twb = tapp.ActiveWorkbook;
                            twbSheets = twb.Sheets;
                            tws = twbSheets.Add();


                            //The column headings row is included in the call and written to rv
                            int nColumns=rv.GetLength(1);
                            int nRows=rv.GetLength(0);

                            tapp.ScreenUpdating = false;
                            tapp.Calculation = MOIE.XlCalculation.xlCalculationManual;
                            for ( j = 0, jP1 = 1; j < nColumns; j++, jP1++ )
                                for ( i = 0, iP1 = 1; i < nRows; i++, iP1++ )
                                    tws.Cells[iP1, jP1] = rv[i, j];

                            trng2 = tws.Range[tws.Cells[1, 1], tws.Cells[1 + nRows, nColumns]];
                            aListObject2 = (MOIE.ListObject) tws.ListObjects.AddEx(MOIE.XlListObjectSourceType.xlSrcRange, trng2, null, MOIE.XlYesNoGuess.xlYes);

                            String name="JniPMML-OutputRecordSet";
                            try {
                                tws.Name = name;
                            }
                            catch {
                                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                                Boolean found = false;
                                i = 1;
                                while ( !found ) {
                                    try {
                                        name = tapp.InputBox("", "Default name used already, enter new name", name + " (" + i + ")");
                                        tws.Name = name;
                                        found = true;
                                    }
                                    catch {
                                        i++;
                                        if ( i > 5 )
                                            throw new com.WDataSci.WDS.WDSException("Error, limit on number of name options");
                                    }
                                }
                            }
                            break;
                    }
                }

            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( !e.getMessage().Equals("Cancel") )
                    MessageBox.Show(e.getMessage() + "\n" + e.StackTrace.ToString());
            }
            catch ( Exception e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show("Error!\n" + e.Message + "\n" + e.StackTrace.ToString());
            }
            finally {

                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( tapp.Calculation != calculation_prior ) tapp.Calculation = calculation_prior;
                if ( !bIsModelCached )
                    __JniPMML.Remove(h);
                aJniPMMLItem = null;

                aListObject = null;
                aListObject2 = null;
                argref = null;
                aXmlMap = null;
                rv1 = null;
                tapp = null;
                trng = null;
                trng2 = null;
                twb = null;
                twbSheets = null;
                tws = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }

        [ExcelCommand(Description = "Add XmlMap to List Object"
            , ExplicitRegistration = true
            )]
        public static void JniPMML_XmlMap_Helper()
        {

            //Typing for possible GC purposes
            MOIE.Application tapp = null;
            ExcelReference argref;
            MOIE.Range trng = null;
            MOIE.Range trng2 = null;
            MOIE.XmlMap aXmlMap = null;
            MOIE.ListObject aListObject = null;
            MOIE.ListObject aListObject2 = null;
            MOIE.Workbook twb = null;
            MOIE.Sheets twbSheets = null;
            MOIE.Worksheet tws = null;
            object rv1=null;
            JniPMMLItem aJniPMMLItem=null;
            XmlDocument aXmlDocument=null;
            XmlNodeList aXmlNodeList=null;

            MessageBoxButtons msgboxbuttons;
                DialogResult msgboxresponse;


            int h=-1;
            Boolean bIsModelCached=false;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior=tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior=tapp.Calculation;

            try {


                int i, j, iP1, jP1, ii, iiP1;

                argref = (ExcelReference) XlCall.Excel(XlCall.xlfSelection);
                twb = tapp.ActiveWorkbook;
                tws = twb.ActiveSheet;

                try {
                    trng = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, argref, true)) as MOIE.Range;
                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error, could not get range of selection", e);
                }

                try {
                    aListObject = trng.ListObject;
                }
                catch ( Exception e ) {
                    throw new com.WDataSci.WDS.WDSException("Error, could not get ListObject of selection", e);
                }

                Boolean found =false;
                try {
                    aXmlMap = aListObject.XmlMap;
                    MessageBox.Show("Already Found XmlMap:" + aXmlMap.Name);
                    found = true;
                }
                catch ( Exception e ) {
                    found = false;
                }
                if ( found )
                    throw new com.WDataSci.WDS.WDSException("Cancel");

                int workingcase=0;

                String sFile="?";
                msgboxbuttons = MessageBoxButtons.YesNoCancel;
                bIsModelCached = false;
                msgboxresponse = MessageBox.Show("Would you like to point to an XSD file (Yes/no)?", "Confirm", msgboxbuttons);
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                    throw new com.WDataSci.WDS.WDSException("Cancel");
                if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes )
                    using ( OpenFileDialog aOpenFileDialog = new OpenFileDialog() ) {
                        aOpenFileDialog.InitialDirectory = tapp.ActiveWorkbook.Path;
                        aOpenFileDialog.Filter = "XSD File (*.xsd)|*.xsd|All files (*.*)|*.*";
                        aOpenFileDialog.FilterIndex = 1;
                        aOpenFileDialog.RestoreDirectory = true;
                        //aOpenFileDialog.FileName = sFileName;
                        aOpenFileDialog.AddExtension = true;
                        aOpenFileDialog.DefaultExt = ".xsd";
                        aOpenFileDialog.CheckFileExists = true;
                        aOpenFileDialog.CheckPathExists = true;
                        aOpenFileDialog.Title = "XML Schema (XSD) File....";

                        if ( aOpenFileDialog.ShowDialog() == DialogResult.OK )
                            sFile = aOpenFileDialog.FileName;
                        else
                            throw new com.WDataSci.WDS.WDSException("Cancel");
                        workingcase = 1;
                    }
                else {
                    msgboxresponse = MessageBox.Show("Use an XSD string (Yes) or use inference-and/or-dictionary (No)?", "Confirm", msgboxbuttons);
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                        throw new com.WDataSci.WDS.WDSException("Cancel");
                    if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes ) {
                        try {
                            MOIE.Range trng3 = tapp.InputBox("Use an XSD as one string contained in a cell, enter cell address (navigable)", "XSD Input", "Entire XSD File as a String",100,100,"",0,8) as MOIE.Range;
                            sFile = trng3.Text;
                            trng3 = null;
                            if ( !sFile.StartsWith("<?xml") ) {
                                if ( sFile.IndexOf("!") < 0 )
                                    sFile = "'[" + tapp.ActiveWorkbook.Name + "]" + aListObject.DataBodyRange.Worksheet.Name + "'!" + sFile;
                                ExcelReference rf=XlCall.Excel(XlCall.xlfEvaluate,sFile) as ExcelReference;
                                trng3 = tapp.Evaluate(XlCall.Excel(XlCall.xlfReftext, rf, true)) as MOIE.Range;
                                sFile = trng3.Text;
                                rf = null;
                                trng3 = null;
                            }
                            workingcase = 1;
                        }
                        catch {
                            throw new com.WDataSci.WDS.WDSException("Cancel");
                        }
                    }
                    else {
                        msgboxresponse = MessageBox.Show("Infer (Yes) or use cached dictionary (No)?", "Confirm", msgboxbuttons);
                        if ( msgboxresponse == System.Windows.Forms.DialogResult.Cancel )
                            throw new com.WDataSci.WDS.WDSException("Cancel");
                        if ( msgboxresponse == System.Windows.Forms.DialogResult.Yes ) {
                            workingcase = 2;
                            sFile = "Infer";
                        }
                        else {
                            String HandleString=JniPMML_Handle_LastUsed(null);
                            String aTag;
                            try {
                                if ( HandleString.StartsWith("-1") )
                                    throw new com.WDataSci.WDS.WDSException("ERROR, no model cached yet!  If workbook was just opened, possibly just recalc first and try again.");
                                aTag = tapp.InputBox("", "Tag or Handle for Cached Model", __JniPMML.Tag(HandleMajorFrom(HandleString)));
                            }
                            catch ( com.WDataSci.WDS.WDSException e ) {
                                throw e;
                            }
                            catch {
                                throw new com.WDataSci.WDS.WDSException("Cancel");
                            }
                            h = __JniPMML.Handle(aTag);
                            aJniPMMLItem = __JniPMML.Item[h];
                            bIsModelCached = true;
                            workingcase = 3;
                            sFile = "Infer";
                        }
                    }
                }

                if ( sFile.Equals("?") )
                    throw new com.WDataSci.WDS.WDSException("Cancel");

                if (workingcase>=2) {
                    sFile = WranglerXSD.XSDHeader();
                    sFile += WranglerXSD.XSDTypes();
                    sFile += WranglerXSD.XSDRecordSet_Open("RecordSet","Record");
                    String[] names=null;
                    String[] types=null;
                    if (workingcase==3) {
                        String PMMLString=JniPMML_LoadedString(h);
                        aXmlDocument=new XmlDocument();
                        aXmlDocument.LoadXml(PMMLString);
                        aXmlNodeList = aXmlDocument.SelectNodes("//DataField");
                        names = new string[aXmlNodeList.Count];
                        types = new string[aXmlNodeList.Count];
                        for (i=0;i<aXmlNodeList.Count;i++) {
                            names[i] = aXmlNodeList[i].Attributes.GetNamedItem("name").Value;
                            types[i] = aXmlNodeList[i].Attributes.GetNamedItem("dataType").Value;
                        }
                        aXmlNodeList = null;
                        aXmlDocument = null;
                    }
                    object[,] data=aListObject.DataBodyRange.Value2;
                    int lnRows=data.GetUpperBound(0)-data.GetLowerBound(0)+1;
                    int lnColumns=data.GetUpperBound(1)-data.GetLowerBound(1)+1;
                    for ( i = 0, iP1=1; i < aListObject.ListColumns.Count; i++, iP1++ ) {
                        FieldMDEnums.eDTyp aDTyp=FieldMDEnums.eDTyp.Unk;
                        j = -1;
                        int[] typl={ -1 };
                        if (workingcase==3) {
                            for ( j = 0; j < names.Length; j++ ) {
                                if ( names[j].Equals(aListObject.ListColumns[iP1].Name) ) {
                                    aDTyp = FieldMDExt.eDTyp_FromAlias(types[j], ref typl);
                                    break;
                                }
                            }
                            if ( j == names.Length || aDTyp.bIn(FieldMDEnums.eDTyp.Unk)) j = -1;
                        }
                        if (j<0) {
                            trng2 = aListObject.ListColumns[iP1].DataBodyRange;
                            for ( ii = 0, iiP1 = 1; aDTyp.Equals(FieldMDEnums.eDTyp.Unk) && ii < lnRows; ii++, iiP1++ ) {
                                if ( data[iiP1, iP1] is ExcelDna.Integration.ExcelMissing ) continue;
                                if ( data[iiP1, iP1] is ExcelDna.Integration.ExcelEmpty ) continue;
                                if ( data[iiP1, iP1] is ExcelDna.Integration.ExcelError ) continue;
                                if ( data[iiP1, iP1] is double? ) {
                                    double? lvd=(double?) data[iiP1, iP1];
                                    if ( lvd == null || Double.IsNaN(lvd.Value) || lvd == Double.MinValue || lvd == Double.MaxValue
                                        || Double.IsNegativeInfinity(lvd.Value) || Double.IsPositiveInfinity(lvd.Value) ) lvd = null;
                                    if ( lvd != null && Math.Abs(lvd.Value - Math.Floor(lvd.Value)) > 1e-6 ) {
                                        aDTyp = FieldMDEnums.eDTyp.Dbl;
                                        break;
                                    }
                                }
                                else
                                if ( data[iiP1, iP1] is String ) {
                                    String lvs=data[iiP1, iP1].ToString();
                                    if ( Regex.Replace(lvs, "[0-9,.\\s]", "").Length > 0 ) {
                                        aDTyp = FieldMDEnums.eDTyp.VLS;
                                        break;
                                    }
                                    double lvd=0;
                                    int lvi=0;
                                    if ( !Double.TryParse(lvs, out lvd) && !int.TryParse(lvs, out lvi) ) {
                                        aDTyp = FieldMDEnums.eDTyp.VLS;
                                        break;
                                    }
                                }
                            }
                            if (aDTyp.Equals(FieldMDEnums.eDTyp.Unk)) {
                                aDTyp = FieldMDEnums.eDTyp.Int;
                            }
                        }
                        sFile += WranglerXSD.XSDColumn(aListObject.ListColumns[iP1].Name, aDTyp.ToString());
                    }
                    sFile += WranglerXSD.XSDRecordSet_Close();
                    sFile += WranglerXSD.XSDFooter();
                }

                //trng[0,0].Offset[0, 10].Value = sFile;

                aXmlMap = twb.XmlMaps.Add(sFile);

                RecordSetMD aRecordSetMD= new RecordSetMD(RecordSetMDEnums.eMode.Internal);
                aRecordSetMD
                    .cAs(RecordSetMDEnums.eType.DBB, RecordSetMDEnums.eSchemaType.XSD)
                    ;
                aRecordSetMD.SchemaMatter.InputSchema = new XmlDocument();
                aRecordSetMD.SchemaMatter.InputSchema.LoadXml(aXmlMap.Schemas[1].XML);
                aRecordSetMD.mReadMapFor(null, null, true);

                int nColumns = aRecordSetMD.nColumns();

                for ( j = 0, jP1 = 1; j < nColumns; j++, jP1++ ) {
                    aListObject.ListColumns[jP1].XPath.SetValue(aXmlMap
                        , "/" + aRecordSetMD.SchemaMatter.RecordSetElementName
                        + "/" + aRecordSetMD.SchemaMatter.RecordElementName
                        + "/" + aRecordSetMD.Column[j].Name);
                }

                aRecordSetMD = null;


            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( !e.getMessage().Equals("Cancel") )
                    MessageBox.Show(e.getMessage() + "\n" + e.StackTrace.ToString());
            }
            catch ( Exception e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show("Error!\n" + e.Message + "\n" + e.StackTrace.ToString());
            }
            finally {

                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( tapp.Calculation != calculation_prior ) tapp.Calculation = calculation_prior;
                if ( !bIsModelCached )
                    __JniPMML.Remove(h);

                aListObject = null;
                aListObject2 = null;
                argref = null;
                aXmlMap = null;
                rv1 = null;
                tapp = null;
                trng = null;
                trng2 = null;
                twb = null;
                twbSheets = null;
                tws = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }


        [ExcelCommand(Description = "Create Worksheet to demo or call JniPMML java cmd line"
            //, MenuName = "WDS JniPMML Cmd"
            //, MenuText = "JniPMML Cmd Line Prep Sheet"
            , ExplicitRegistration = true
            )]
        public static void JniPMML_Cmd_Prep()
        {
            MOIE.Application tapp = null;
            MOIE.Range trng = null;
            MOIE.Range r = null;
            MOIE.Workbook twb = null;
            MOIE.Sheets twbSheets = null;
            MOIE.Worksheet tws = null;
            List<object> cmargs=null;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior=tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior=tapp.Calculation;
            try {

                int i, j, iP1, jP1;

                twb = tapp.ActiveWorkbook;
                twbSheets = twb.Sheets;
                tws = twbSheets.Add();

                String name="JniPMML-Cmd-Line";
                try {
                    tws.Name = name;
                }
                catch {
                    if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                    Boolean found = false;
                    i = 1;
                    while ( !found ) {
                        try {
                            name = tapp.InputBox("", "Default name used already, enter new name", name + " (" + i + ")");
                            tws.Name = name;
                            found = true;
                        } catch {
                            i++;
                            if ( i > 5 )
                                throw new com.WDataSci.WDS.WDSException("Error, limit on number of name options");
                        }
                    }
                }
                String aMethodName = "mCmdArgsRecap";
                String aSignatureString = "()Ljava/lang/String;";

                cmargs = new List<object>(0);
                String rv="Err";

                unsafe {
                    IntPtr aMethodID = Java.FindStaticMethodID(java_init_classid, aMethodName, aSignatureString);
                    rv = Java.CallMethod<string>(aMethodID, true, aSignatureString, cmargs);
                }
                tapp.Calculation = MOIE.XlCalculation.xlCalculationManual;
                iP1 = 1;
                tws.Cells[iP1++, 1] = "Summary of Java command line arguments for JniPMML";
                trng = tws.Range[tws.Cells[1, 1], tws.Cells[1, 5]];
                trng.Merge(true);
                trng.HorizontalAlignment = MOIE.XlHAlign.xlHAlignLeft;
                trng.Font.Bold = true;
                trng = null;

                
                tws.Cells[iP1, 1] = "AddIn Assembly Info";
                tws.Cells[iP1, 2] = "Path";
                tws.Cells[iP1, 3] = pAssemblyLocation;
                iP1++;


                tws.Cells[iP1, 1] = "Running Java";
                tws.Cells[iP1, 2] = Java.JavaVersion();
                iP1++;

                String[] rvs=rv.Split(';');
                i = -1;
                Boolean bSetupCommand=false;
                int jshift=0;
                int sortstartrow=0;
                int sortstoprow=0;
                while ( ++i < rvs.Length ) {
                    String s=rvs[i];
                    String[] sa;
                    switch ( s ) {
                        case "Java Internals":
                            tws.Cells[iP1++, 1] = s;
                            break;
                        case "Parameters":
                            tws.Cells[iP1, 1] = "Cmd Builder             ";
                            tws.Cells[iP1, 2] = "Combined Arguments   >>>";
                            r = tws.Cells[iP1, 3];
                            r.FormulaR1C1 = "=concatenate_dlm(R[2]C:R[" + (2 + rvs.Length) + "]C,\" \")";
                            r.Interior.Color = MOIE.XlRgbColor.rgbLightGreen;
                            trng = tws.Range[r, r.Offset[0, 10]];
                            trng.Merge(true);
                            trng.HorizontalAlignment = MOIE.XlHAlign.xlHAlignLeft;
                            trng = null;
                            r = null;
                            iP1++;
                            jP1 = 1;
                            tws.Cells[iP1, jP1++] = "Keys/Flags";
                            tws.Cells[iP1, jP1++] = "Values";
                            tws.Cells[iP1, jP1++] = "Cmd Line Arg";
                            tws.Cells[iP1, jP1++] = s;
                            tws.Cells[iP1, jP1++] = "Key/Flag";
                            tws.Cells[iP1, jP1++] = "Description";
                            tws.Cells[iP1, jP1++] = "Type";
                            tws.Cells[iP1, jP1++] = "Order";
                            tws.Cells[iP1, jP1++] = "Arity";
                            tws.Cells[iP1, jP1++] = "Default Value";
                            tws.Cells[iP1, jP1++] = "Required";
                            jshift = 2;
                            iP1++;
                            bSetupCommand = true;
                            break;
                        default:
                            if ( s.StartsWith("System Class Path") )
                                sa = s.Split('|');
                            else
                                sa = s.Split(':');
                            for ( j = 0, jP1 = jshift + 2; j < sa.Length; j++, jP1++ )
                                tws.Cells[iP1, jP1] = sa[j].Replace(pAssemblyLocation+"\\","");
                            if ( bSetupCommand ) {
                                if ( sortstartrow == 0 ) sortstartrow = iP1;
                                sortstoprow = iP1;
                            }
                            iP1++;
                            break;
                    }
                }
                trng = tws.Range[tws.Cells[sortstartrow, 1], tws.Cells[sortstoprow, 15]];
                trng.Sort(trng.Cells[1, 8]);
                trng = null;
                for ( iP1 = sortstartrow; iP1 <= sortstoprow; iP1++ ) {
                    r = tws.Cells[iP1, 1];
                    String lsa=r.Offset[0,4].Value.ToString();
                    if ( lsa.IndexOf('[') > -1 ) lsa = lsa.Remove(lsa.IndexOf('['), 1);
                    if ( lsa.IndexOf(']') > -1 ) lsa = lsa.Remove(lsa.IndexOf(']'), 1);
                    String[] vlsa=lsa.Split('|');
                    for ( j = 0; j < vlsa.Length; j++ ) {
                        vlsa[j] = vlsa[j].Trim();
                        if ( vlsa[j].StartsWith("--") ) vlsa[j] = "'" + vlsa[j];
                        if ( j > 0 ) vlsa[0] += "," + vlsa[j];
                    }
                    r.Validation.Add(MOIE.XlDVType.xlValidateList, MOIE.XlDVAlertStyle.xlValidAlertStop, MOIE.XlFormatConditionOperator.xlBetween, vlsa[0]);
                    r.Validation.InCellDropdown = true;
                    r.Validation.IgnoreBlank = true;
                    r.Offset[0, 2].FormulaR1C1 = "=if(not(isblank(RC[-2])),RC[-2]&if(RC[6]<>0,\" \"&if(isblank(RC[-1]),RC[7],RC[-1]),\"\"),\"\")";
                    r.AddComment("Internal Variable:" + r.Offset[0, 3].Value.ToString() + " (" + r.Offset[0, 5].Value.ToString() + " )\n" + r.Offset[0, 4].Value.ToString());
                    r.Comment.Visible = true;
                    r.Comment.Shape.Width = 1000;
                    r.Comment.Visible = false;
                    r = null;
                }
                if (tws!=null) tws.Columns.AutoFit();
            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( !e.getMessage().Equals("Cancel") )
                    MessageBox.Show(e.getMessage() + "\n" + e.StackTrace.ToString());
            }
            catch ( Exception e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show("Error!\n" + e.Message + "\n" + e.StackTrace.ToString());
            }
            finally {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( tapp.Calculation != calculation_prior ) tapp.Calculation = calculation_prior;
                r = null;
                trng = null;
                tws = null;
                twbSheets = null;
                tapp = null;
                cmargs = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }


        [ExcelCommand(Description = "Runs the JniPMML command based on the arguments in a cell."
            //, MenuName = "WDS JniPMML Cmd"
            //, MenuText = "JniPMML Cmd Line Prep Sheet"
            , ExplicitRegistration = true
            )]
        public static void JniPMML_Cmd()
        {
            MOIE.Application tapp = null;
            MOIE.Range trng = null;
            List<object> cmargs=null;
            tapp = (ExcelDnaUtil.Application as MOIE.Application);
            Boolean screenupdating_prior=tapp.ScreenUpdating;
            MOIE.XlCalculation calculation_prior=tapp.Calculation;
            try {

                trng = tapp.InputBox("Select cell with JniPMML Cmd arguments", "JniPMML Command Line Input", "?",100,100,"",0,8) as MOIE.Range;

                String aMethodName = "mCmdRun";
                String aSignatureString = "(Ljava/lang/String;)Ljava/lang/String;";
                cmargs = new List<object> { trng.Text };
                String rv="Err";

                unsafe {
                    IntPtr aMethodID = Java.FindStaticMethodID(java_init_classid, aMethodName, aSignatureString);
                    rv = Java.CallMethod<string>(aMethodID, true, aSignatureString, cmargs);
                }
                MessageBox.Show(rv);

            }
            catch ( com.WDataSci.WDS.WDSException e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( !e.getMessage().Equals("Cancel") )
                    MessageBox.Show(e.getMessage() + "\n" + e.StackTrace.ToString());
            }
            catch ( Exception e ) {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                MessageBox.Show("Error!\n" + e.Message + "\n" + e.StackTrace.ToString());
            }
            finally {
                if ( tapp.ScreenUpdating != screenupdating_prior ) tapp.ScreenUpdating = screenupdating_prior;
                if ( tapp.Calculation != calculation_prior ) tapp.Calculation = calculation_prior;
                trng = null;
                tapp = null;
                cmargs = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return;
        }

    }

}

