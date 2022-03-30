using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using System.Windows.Forms;


using ExcelDna.Registration;
using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;
using ExcelDna.IntelliSense;
using JNI;

namespace WDataSci.JniPMML
{

    /// <summary>
    /// AddIn:IExcelAddIn is the <i>main</i> class for this ExcelDna based addin.
    /// <para>The original purpose of the addin has been the call the <i>de facto</i> reference 
    /// implementation of PMML (jpmml) from Excel.  
    /// Although other implementations and implementations of other XML based 
    /// model specs may be included later, the direct linking of the usual implementation is 
    /// included because 1) it was not readily available and 2) for comparison and testing of 
    /// structures.</para>
    /// <para>This <i>main</i> class also holds the JNI JVM reference which is instantiated 
    /// during the AutoOpen call.</para>
    /// </summary>

    public partial class AddIn : IExcelAddIn
    {

        private static String pAssemblyLocation = null;

        //From the JNICode library....
        private static JavaNativeInterface Java;
        private static Dictionary<string, string> pair = new Dictionary<string, string>();
        private static String java_class_path = null;
        private static String java_module = null;
        private static String java_module_path = null;
        private static String java_init_class_name = "com/WDataSci/JniPMML/JniPMML";
        private static IntPtr java_init_classid = IntPtr.Zero;

        private static CommandBarPopup WDSJniPMMLMenuToDelete=null;

        //Object handling
        public static com.WDataSci.JniPMML.JniPMML __JniPMML;

        public void AutoOpen()
        {
            try {
                RegisterFunctions();
                IntelliSenseServer.Install();

                AppDomain cDomain = AppDomain.CurrentDomain;

                pAssemblyLocation = cDomain.BaseDirectory;

                //Even though jar tvf <> can expose the classes of a jar ExcelDna-packed into an assembly
                //javap might not be able to find the class, so holding on....
                //    //Here, we will first try to use the concept that a C# assembly is like a Java jar
                //    //and the ExcelDNA packer can include the core jar (note there can be no lzma compression
                //    //which might required an old-style ExcelDNA Post-Build line).

                //    String pWDSJniPMML = Assembly.GetExecutingAssembly().FullName;
                //    pWDSJniPMML = pAssemblyLocation+"\\"
                //    +pWDSJniPMML.Substring(0, pWDSJniPMML.IndexOf(", Version") - 1) + "-AddIn64-packed.xll";

                //The dependency on slf4j-simple has been packaged into the jar, this
                //enables finding all of the HDF5 dependencies from the HDFView location.
               
                //The jarhdf[5] and hdfobject jars still depend on external libs/dlls 
                //that should be on the path.  However, locations can be passed in on 
                //the Excel command line with /o:WDS:<libname>:<liblocation>
               
                String pWDSJniPMML = pAssemblyLocation + "\\WDS-JniPMML-0.5.0.jar";
                {
                    DirectoryInfo aDirectoryInfo = new DirectoryInfo(pAssemblyLocation);
                    FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("WDS*JniPMML*.jar");
                    if (lFileInfo==null || lFileInfo.Length==0) 
                        pWDSJniPMML = pAssemblyLocation + "\\Resources\\WDS-JniPMML-0.5.0.jar";
                }
                String pjarhdf5dir="";
                String pjarhdf5 = "";
                String pjarhdfdir="";
                String pjarhdf = "";
                String phdfobjectdir = "";
                String phdfobject = "";
                //String pslf4j = "";
                String pHDFView = "Unk";
                String pHDF5 = "Unk";


                //parse out the command line arguments in case /o:WDS - options are being used

                String[] args = System.Environment.GetCommandLineArgs();

                for ( int i = 0; i < args.Length; i++ ) {
                    if ( args[i].StartsWith("/o:WDS:WDS-JniPMML") ) {
                        String[] parts = args[i].Split(':');
                        pWDSJniPMML = parts[parts.Length - 1];
                    }
                    else if ( args[i].StartsWith("/o:WDS:jarhdf5") ) {
                        String[] parts = args[i].Split(':');
                        pjarhdf5 = parts[parts.Length - 1];
                    }
                    else if ( args[i].StartsWith("/o:WDS:jarhdf") ) {
                        String[] parts = args[i].Split(':');
                        pjarhdf = parts[parts.Length - 1];
                    }
                    else if ( args[i].StartsWith("/o:WDS:hdfobject") ) {
                        String[] parts = args[i].Split(':');
                        phdfobject = parts[parts.Length - 1];
                    }
                    //else if ( args[i].StartsWith("/o:WDS:slf4j") ) {
                        //String[] parts = args[i].Split(':');
                        //pslf4j = parts[parts.Length - 1];
                    //}
                    else if ( args[i].StartsWith("/o:WDS:path:HDFView") ) {
                        String[] parts = args[i].Split(':');
                        pHDFView = parts[parts.Length - 1];
                    }
                    else if ( args[i].StartsWith("/o:WDS:path:HDF5") ) {
                        String[] parts = args[i].Split(':');
                        pHDF5 = parts[parts.Length - 1];
                    }
                }




                //if not provided with command lines, check the paths
                if ( pHDFView.Equals("Unk") || pHDF5.Equals("Unk") ) {
                    //pull the locations for HDFView and HDF5 from the system path
                    String p = System.Environment.GetEnvironmentVariable("PATH");
                    if ( pHDFView.Equals("Unk") )
                        pHDFView = PathElementOf(p, "HDFView") + "\\lib";
                    if ( pHDF5.Equals("Unk") )
                        pHDF5 = PathElementOf(p, "HDF5\\1") + "\\lib";
                }
                if ( pWDSJniPMML == "" ) {
                    DirectoryInfo aDirectoryInfo = new DirectoryInfo(pAssemblyLocation);
                    FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("WDS*JniPMML*.jar");
                    pWDSJniPMML = lFileInfo[0].FullName;
                }
                if ( pjarhdf5 == "" ) {
                    //Use the jarhdf from the HDFView location first
                    //DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDF5);
                    DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDFView);
                    FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("jarhdf5*.jar");
                    pjarhdf5 = lFileInfo[0].FullName;
                    pjarhdf5dir = System.IO.Path.GetDirectoryName(pjarhdf5);
                }
                if ( pjarhdf == "" ) {
                    //DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDF5);
                    DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDFView);
                    FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("jarhdf-*.jar");
                    pjarhdf = lFileInfo[0].FullName;
                    pjarhdfdir = System.IO.Path.GetDirectoryName(pjarhdf5);
                }
                //if ( pslf4j == "" ) {
                //  //the slf4j-simple should be with HDF5
                //  //DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDFView);
                //  //FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("slf4j-api*.jar");
                //  DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDF5);
                //  FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("slf4j-simple*.jar");
                //  pslf4j = lFileInfo[0].FullName;
                //}
                if ( phdfobject == "" ) {
                    DirectoryInfo aDirectoryInfo = new DirectoryInfo(pHDFView);
                    FileInfo[] lFileInfo = aDirectoryInfo.GetFiles("hdfobj*.jar");
                    phdfobject = lFileInfo[0].FullName;
                    phdfobjectdir = System.IO.Path.GetDirectoryName(phdfobject);
                }

                java_class_path = pWDSJniPMML
                    + ";" + phdfobject
                    + ";" + pjarhdf5
                    + ";" + pjarhdf
                    //+ ";" + pslf4j
                ;

                java_module_path = phdfobjectdir;
                if ( !java_module_path.Contains(pjarhdf5dir) ) java_module_path += pjarhdf5dir;
                if ( !java_module_path.Contains(pjarhdfdir) ) java_module_path += pjarhdfdir;


                if ( !pair.ContainsKey("-Djava.class.path") )
                    pair.Add("-Djava.class.path", java_class_path);

                //java_module = "hdfobject;jarhdf5;jarhdf;slf4j.simple";
                //java_module = "hdfobject;jarhdf5;jarhdf";
                //if ( !pair.ContainsKey("--add-modules") )
                //pair.Add("--add-modules", java_module);

                if ( !pair.ContainsKey("-Djava.module.path") )
                    pair.Add("-Djava.module.path", java_module_path);

                if ( !pair.ContainsKey("-Djava.library.path") )
                    pair.Add("-Djava.library.path", pHDFView + ";" + pHDF5);

                //if (!pair.ContainsKey("-D64")) pair.Add("-D64", "" );
                if (!pair.ContainsKey("-Xcheck:jni ")) pair.Add("-Xcheck:jni ", "" );

                //if (!pair.ContainsKey("-Xms64M ")) pair.Add("-Xms64M ", "" );


                //initialize JNI object
                Java = new JavaNativeInterface();

                Java.LoadVM(pair, false);

                Java.InstantiateJavaObject(java_init_class_name);
                java_init_classid = Java.FindClassID(java_init_class_name);
                __JniPMML = new com.WDataSci.JniPMML.JniPMML(Java, java_init_classid);

            }
            catch ( Exception e ) {
                MessageBox.Show("Yeah, here," + e.Message);
                throw new com.WDataSci.WDS.WDSException("Error into AutoLoad", e);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }


        public void AutoClose()
        {
            IntelliSenseServer.Uninstall();
            Java.Dispose();
            WDSJniPMMLMenuToDelete.Delete(true);
            ExcelCommandBarUtil.UnloadCommandBars();

            __JniPMML = null;
            Java = null;
            pair = null;
            java_class_path = null;
            java_init_classid = IntPtr.Zero;
            java_init_class_name = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }


        public void RegisterFunctions()
        {
            ExcelRegistration.GetExcelCommands().RegisterCommands();

            /* to walk through which functions are being registered.....
            int i = 0;
            List<String> los = new List<string>(0);
            foreach (ExcelFunctionRegistration f in ExcelRegistration.GetExcelFunctions()) {
                los.Add(f.FunctionLambda.ToString());
                List<ExcelFunctionRegistration> ief = new List<ExcelFunctionRegistration>();
                ief.Add(f);
                i++;
                ExcelRegistration.RegisterFunctions(ief);
            }
            */
            ExcelRegistration.GetExcelFunctions().RegisterFunctions();

        }

        public ExcelFunctionRegistration UpdateHelpTopic(ExcelFunctionRegistration funcReg)
        {
            funcReg.FunctionAttribute.HelpTopic = "http://WDataSci.com";
            return funcReg;
        }

        public String PathElementOf(String path, String loc)
        {
            try
            {

                int iloc = path.IndexOf(loc);
                int i = path.IndexOf(";", iloc + 1);
                if (i < 0) i = path.Length;
                int j = path.LastIndexOf(";", iloc);
                String elem = path.Substring(j + 1, i - j);
                if (elem.EndsWith(";")) elem = elem.Substring(0, elem.Length - 1);
                if (elem.EndsWith("\\")) elem = elem.Substring(0, elem.Length - 1);
                if (elem.EndsWith("\\bin")) elem = elem.Substring(0, elem.Length - 4);
                if (elem.EndsWith("\\lib")) elem = elem.Substring(0, elem.Length - 4);
                return elem;
            } catch (Exception e)
            {
                return "Error, " + e.Message;
            }

        }

        public void Dispose()
        {
            this.AutoClose();
        }

        ~AddIn()
        {
            this.AutoClose();
        }

    }


}

