////////////////////////////////////////////////////////////////////////////////////////////////  
//  An excellent resource for the JNI library is on the website 
// Java Native Interface: Programmer's Guide and Specification by Sheng Liang 
//  http://docs.oracle.com/javase/7/docs/technotes/guides/jni/
// for a list of all the functions 
// http://download.oracle.com/javase/6/docs/technotes/guides/jni/spec/functions.html
////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace JNI
{
    public unsafe class JavaNativeInterface : IDisposable
    {
        private const string JRE_REGISTRY_KEY1 = @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\JDK";
        private const string JRE_REGISTRY_KEY2 = @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Runtime Environment";
        private const string JRE_REGISTRY_KEY3 = @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Development Kit";
        private const string JRE_REGISTRY_KEY4 = @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\JRE";

        private IntPtr javaClass;
        private IntPtr javaObject;
        private string javaClassName;
        private JavaVM jvm;
        private JNIEnv env;

        public bool AttachToCurrentJVMThread { get; set; }

        public void LoadVM(Dictionary<string, string> options, bool AddToExistingJVM)
        {
            // Get the location of the current version of the JVM.dll          
            string jreVersion = "";
            string keyName = "";
            string jvmDir = "";
            string JavaHome = "";
            try
            {
                jreVersion = (string)Registry.GetValue(JRE_REGISTRY_KEY1, "CurrentVersion", null);
                keyName = Path.Combine(JRE_REGISTRY_KEY1, jreVersion);
                try
                {
                    JavaHome = (string)Registry.GetValue(keyName, "JavaHome", null);
                }
                catch
                {
                    JavaHome = (string)Registry.GetValue(keyName, "RuntimeLib", null);
                }
                jvmDir = Path.Combine(Path.Combine(Path.Combine(JavaHome, "bin"), "server"), "jvm.dll");
                if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                    jvmDir = Path.Combine(Path.Combine(Path.Combine(Path.Combine(JavaHome, "jre"), "bin"), "server"), "jvm.dll");
                if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                    throw new Exception("No jvm.dll");
            }
            catch
            {
                try
                {
                    jreVersion = (string)Registry.GetValue(JRE_REGISTRY_KEY2, "CurrentVersion", null);
                    keyName = Path.Combine(JRE_REGISTRY_KEY2, jreVersion);
                    jvmDir = Path.Combine(Path.Combine(Path.Combine(JavaHome, "bin"), "server"), "jvm.dll");
                    if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                        jvmDir = Path.Combine(Path.Combine(Path.Combine(Path.Combine(JavaHome, "jre"), "bin"), "server"), "jvm.dll");
                    if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                        throw new Exception("No jvm.dll");
                }
                catch
                {
                    try
                    {
                        jreVersion = (string)Registry.GetValue(JRE_REGISTRY_KEY3, "CurrentVersion", null);
                        keyName = Path.Combine(JRE_REGISTRY_KEY3, jreVersion);
                        jvmDir = Path.Combine(Path.Combine(Path.Combine(JavaHome, "bin"), "server"), "jvm.dll");
                        if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                            jvmDir = Path.Combine(Path.Combine(Path.Combine(Path.Combine(JavaHome, "jre"), "bin"), "server"), "jvm.dll");
                        if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                            throw new Exception("No jvm.dll");
                    }
                    catch
                    {

                        jreVersion = (string)Registry.GetValue(JRE_REGISTRY_KEY4, "CurrentVersion", null);
                        keyName = Path.Combine(JRE_REGISTRY_KEY4, jreVersion);
                        jvmDir = Path.Combine(Path.Combine(Path.Combine(JavaHome, "bin"), "server"), "jvm.dll");
                        if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                            jvmDir = Path.Combine(Path.Combine(Path.Combine(Path.Combine(JavaHome, "jre"), "bin"), "server"), "jvm.dll");
                        if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                            throw new Exception("No jvm.dll");
                    }
                }
            }

            if ((jvmDir.Length == 0) || (!File.Exists(jvmDir)))
                throw new Exception("Error determining the location of the Java Runtime Environment");

            // Set the directory to the location of the JVM.dll. 
            // This will ensure that the API call JNI_CreateJavaVM will work
            Directory.SetCurrentDirectory(Path.GetDirectoryName(jvmDir));

            var args = new JavaVMInitArgs();
            

            int jrev = 0;
            if (jreVersion.StartsWith("1."))
                jrev = Convert.ToInt32((decimal.Parse(jreVersion.Substring(0, 3)) - 1) / 2 * 10);
            else if (jreVersion.StartsWith("12."))
                jrev = 12;
            else if (jreVersion.StartsWith("17."))
                jrev = 17;
            else
                jrev = Convert.ToInt32(jreVersion);

            switch (jrev)
            {
                case 0:
                    throw new Exception("Unsupported java version. Please upgrade your version of the JRE.");

                case 1:
                    args.version = JNIVersion.JNI_VERSION_1_2;
                    break;
                case 2:
                    args.version = JNIVersion.JNI_VERSION_1_4;
                    break;
                case 3:
                    args.version = JNIVersion.JNI_VERSION_1_6;
                    break;
                case 4:
                    args.version = JNIVersion.JNI_VERSION_1_6;
                    break;
                case 9:
                    args.version = JNIVersion.JNI_VERSION_9;
                    break;
                //case 12:
                //    args.version = JNIVersion.JNI_VERSION_12;
                //    break;
                //case 17:
                //    args.version = JNIVersion.JNI_VERSION_17;
                //    break;
                default:
                    args.version = JNIVersion.JNI_VERSION_10;
                    break;
            }

            args.ignoreUnrecognized = JavaVM.BooleanToByte(true); // True

            if (options.Count > 0)
            {
                args.nOptions = options.Count;
                var opt = new JavaVMOption[options.Count];
                int i = 0;
                foreach (KeyValuePair<string, string> kvp in options)
                {
                    opt[i++].optionString = Marshal.StringToHGlobalAnsi(kvp.Key.ToString() + "=" + kvp.Value.ToString());
                }
                fixed (JavaVMOption* a = &opt[0])
                {
                    // prevents the garbage collector from relocating the opt variable as this is used in unmanaged code that the gc does not know about
                    args.options = a;
                }
            }

            if (!AttachToCurrentJVMThread)
            {
                IntPtr environment;
                IntPtr javaVirtualMachine;
                int result = JavaVM.JNI_CreateJavaVM(out javaVirtualMachine, out environment, &args);
                if (result != JNIReturnValue.JNI_OK)
                {
                    throw new Exception("Cannot create JVM " + result.ToString());
                }

                jvm = new JavaVM(javaVirtualMachine);
                env = new JNIEnv(environment);
            }
            else AttachToCurrentJVM(args);
        }

        private void AttachToCurrentJVM(JavaVMInitArgs args)
        {
            // This is only required if you want to reuse the same instance of the JVM
            // This is especially useful if you are using JNI in a webservice. see page 89 of the
            // Java Native Interface: Programmer's Guide and Specification by Sheng Liang
            if (AttachToCurrentJVMThread)
            {
                int nVMs;

                IntPtr javaVirtualMachine;
                int res = JavaVM.JNI_GetCreatedJavaVMs(out javaVirtualMachine, 1, out nVMs);
                if (res != JNIReturnValue.JNI_OK)
                {
                    throw new Exception("JNI_GetCreatedJavaVMs failed (" + res.ToString() + ")");
                }
                if (nVMs > 0)
                {
                    jvm = new JavaVM(javaVirtualMachine);
                    res = jvm.AttachCurrentThread(out env, args);
                    if (res != JNIReturnValue.JNI_OK)
                    {
                        throw new Exception("AttachCurrentThread failed (" + res.ToString() + ")");
                    }
                }
            }
        }

        //CJW to pass out the JNIEnv
        public JNIEnv Env()
        {
            return this.env;
        }

        //CJW
        public IntPtr FindClassID(string ClassName)
        {
            try
            {
                return env.FindClass(ClassName);
            }
            catch
            {
                throw new Exception(env.CatchJavaException());
            }
        }

        //CJW
        public IntPtr FindClassObjectID(IntPtr ClassID)
        {
            try
            {
                return env.GetObjectClass(ClassID);
            }
            catch
            {
                throw new Exception(env.CatchJavaException());
            }
        }

        //CJW
        public IntPtr FindMethodID(IntPtr ClassObjID, string MethodName, string Signature)
        {
            try
            {
                return env.GetMethodId(ClassObjID,MethodName,Signature);
            }
            catch
            {
                throw new Exception(env.CatchJavaException());
            }
        }

        //CJW
        public IntPtr FindStaticMethodID(IntPtr ClassObjID, string MethodName, string Signature)
        {
            try
            {
                return env.GetStaticMethodID(ClassObjID,MethodName,Signature);
            }
            catch
            {
                String s = env.CatchJavaException().ToString();
                throw new Exception(env.CatchJavaException());
            }
        }


        public void InstantiateJavaObject(string ClassName)
        {
            // need to create class before we can call any methods
            javaClassName = ClassName;
            try
            {
                javaClass = env.FindClass(javaClassName);

                IntPtr methodId = env.GetMethodId(javaClass, "<init>", "()V");
                javaObject = env.NewObject(javaClass, methodId, new JValue() {});
            }
            catch
            {
                throw new Exception(env.CatchJavaException());
            }
        }

        public void CallVoidMethod(string methodName, string sig, List<object> param)
        {
            try
            {
                IntPtr methodId = env.GetMethodId(javaClass, methodName, sig);
                env.CallVoidMethod(javaObject, methodId, ParseParameters(sig, param));
            }
            catch
            {
                throw new Exception(env.CatchJavaException());
            }
        }

        private JValue[] ParseParameters(string sig, List<object> param)
        {
            JValue[] retval = new JValue[param.Count];

            int startIndex = sig.IndexOf('(') + 1;

            for (int i = 0; i < param.Count; i++)
            {
                string paramSig = "";
                if (sig.Substring(startIndex, 1) == "[")
                    paramSig = sig.Substring(startIndex++, 1); 

                if (sig.Substring(startIndex, 1) == "L") {
                  paramSig = paramSig + sig.Substring(startIndex, sig.IndexOf(';', startIndex) - startIndex);
                  startIndex++; // skip past ;
                }
                else
                    paramSig = paramSig + sig.Substring(startIndex, 1);

                startIndex = startIndex + (paramSig.Length - (paramSig.IndexOf("[", StringComparison.Ordinal) + 1 ));
                
                if (param[i] is string)
                {
                    if (!paramSig.Equals("Ljava/lang/String"))
                    {
                        throw new Exception("Signature (" + paramSig + ") does not match parameter value (" + param[i].GetType().ToString() + ").");
                    }
                    retval[i] = new JValue() { l = env.NewString(param[i].ToString(), param[i].ToString().Length) };
                } else if (param[i] == null) {
                    retval[i] = new JValue(); // Just leave as default value
                }
                else if (paramSig.StartsWith("["))
                {
                    retval[i] = ProcessArrayType(paramSig, param[i]);
                }
                else if (paramSig.StartsWith("L"))
                {
                    
                    if (paramSig.Equals("Ljava/lang/Integer"))
                    {
                        if (param[i] is int)
                        {
                            IntPtr jclass = env.FindClass("Ljava/lang/Integer;");
                            //IntPtr jclass_methodId = env.GetMethodId(jclass, "<init>", "()Ljava/lang/Integer;");
                            IntPtr jclass_methodId = env.GetMethodId(jclass, "<init>", "(I)V");
                            retval[i] = new JValue() { l = env.NewObject(jclass, jclass_methodId, new JValue() { i = (int)param[i] } ) };
                        }

                    }

                    if (paramSig.Equals("Ljava/nio/ByteBuffer"))
                    {
                        if (param[i] is IntPtr)
                        {
                            //IntPtr jclass = env.FindClass("Ljava/nio/ByteBuffer;");
                            //IntPtr jclass_methodId = env.GetMethodId(jclass, "<init>", "(I)V");
                            retval[i] = new JValue() { l = (IntPtr) param[i] };
                        }

                    }

                }
                else
                {
                    retval[i] = new JValue();
                    FieldInfo paramField = retval[i].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).AsQueryable().FirstOrDefault(a => a.Name.ToUpper().Equals(paramSig));
                    if ((paramField != null) && ((param[i].GetType() == paramField.FieldType) || ((paramField.FieldType == typeof(bool)) && (param[i] is byte))))
                    {
                        paramField.SetValueDirect(__makeref(retval[i]),paramField.FieldType == typeof (bool)  // this is an undocumented feature to set struct fields via reflection
                                                      ? JavaVM.BooleanToByte((bool)param[i])
                                                      : param[i]);
                    }
                    else throw new Exception("Signature (" + paramSig + ") does not match parameter value (" + param[i].GetType().ToString() + ")."); 
                }                                                 
            }
            return retval;
        }

        private JValue ProcessArrayType(string paramSig, object param)
        {
            IntPtr arrPointer;
            if (paramSig.Equals("[I"))
                arrPointer = env.NewIntArray(((Array)param).Length, javaClass);
            else if (paramSig.Equals("[J"))
                arrPointer = env.NewLongArray(((Array)param).Length, javaClass);
            else if (paramSig.Equals("[C"))
                arrPointer = env.NewCharArray(((Array)param).Length, javaClass);
            else if (paramSig.Equals("[B"))
                arrPointer = env.NewByteArray(((Array)param).Length, javaClass);
            else if (paramSig.Equals("[S"))
                arrPointer = env.NewShortArray(((Array)param).Length, javaClass);
            else if (paramSig.Equals("[D"))
                arrPointer = env.NewDoubleArray(((Array)param).Length, javaClass);
            else if (paramSig.Equals("[F"))
                arrPointer = env.NewFloatArray(((Array)param).Length, javaClass);
            else if (paramSig.Contains("[Ljava/lang/String"))
            {
                IntPtr jclass = env.FindClass("Ljava/lang/String;");
                try
                {
                    arrPointer = env.NewObjectArray(((Array)param).Length, jclass, IntPtr.Zero);
                }
                finally
                {
                    env.DeleteLocalRef(jclass);
                }

            }
            else if (paramSig.Contains("[Ljava/lang/"))
                arrPointer = env.NewObjectArray(((Array)param).Length, javaClass, (IntPtr)param);
            else
            {
                throw new Exception("Signature (" + paramSig + ") does not match parameter value (" +
                                   param.GetType().ToString() + "). All arrays types should be defined as objects because I do not have enough time to defines every possible array type");
            }

            if (paramSig.Contains("[Ljava/lang/"))
            {  
                for (int j = 0; j < ((Array)param).Length; j++)
                {
                    object obj = ((Array)param).GetValue(j);

                    if (paramSig.Contains("[Ljava/lang/String"))
                    {
                        IntPtr str = env.NewString(obj.ToString(), obj.ToString().Length);
                        env.SetObjectArrayElement(arrPointer, j, str);
                    }
                    else
                        env.SetObjectArrayElement(arrPointer, j, (IntPtr)obj);
                }
            }
            else
              env.PackPrimitiveArray<int>((int[])param, arrPointer);

            return new JValue() { l = arrPointer };
        }

        public T CallMethod<T>(string methodName, string sig, List<object> param)
        {
            IntPtr methodId = env.GetMethodId(javaClass, methodName, sig);
            try
            {
                if (typeof (T) == typeof (byte))
                {
                    // Call the byte method 
                    byte res = env.CallByteMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                else if (typeof (T) == typeof (bool))
                {
                    // Call the boolean method 
                    bool res = env.CallBooleanMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                if (typeof (T) == typeof (char))
                {
                    // Call the char method 
                    char res = env.CallCharMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                else if (typeof (T) == typeof (short))
                {
                    // Call the short method 
                    short res = env.CallShortMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                else if (typeof (T) == typeof (int))
                {
                    // Call the int method               
                    int res = env.CallIntMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                else if (typeof (T) == typeof (long))
                {
                    // Call the long method 
                    long res = env.CallLongMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                else if (typeof (T) == typeof (float))
                {
                    // Call the float method 
                    float res = env.CallFloatMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res;
                }
                else if (typeof (T) == typeof (double))
                {
                    // Call the double method 
                    double res = env.CallDoubleMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T) (object) res; // need to fix this
                }
                else if (typeof (T) == typeof (string))
                {
                    // Call the string method 
                    IntPtr jstr = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));

                    string res = env.JStringToString(jstr);
                    env.DeleteLocalRef(jstr);
                    return (T) (object) res;
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    // Call the byte method
                    IntPtr jobj = env.CallStaticObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    if (jobj == IntPtr.Zero)
                    {
                        return default(T);
                    }
                    byte[] res = env.JStringToByte(jobj);
                    env.DeleteLocalRef(jobj);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(string[]))
                {
                    // Call the string array method
                    IntPtr jobj = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    if (jobj == IntPtr.Zero)
                    {
                        return default(T);
                    }

                    IntPtr[] objArray = env.GetObjectArray(jobj);
                    string[] res = new string[objArray.Length];

                    for (int i=0; i < objArray.Length; i++)
                    {
                        res[i] = env.JStringToString(objArray[i]);                        
                    }

                    env.DeleteLocalRef(jobj);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(int[]))
                {
                    // Call the int array method
                    IntPtr jobj = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    if (jobj == IntPtr.Zero)
                    {
                        return default(T);
                    }
                    int[] res = env.GetIntArray(jobj);
                    env.DeleteLocalRef(jobj);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(IntPtr))
                {
                    // Call the object method and deal with whatever comes back in the call code 
                    IntPtr res = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                return default(T);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + env.CatchJavaException());
            }
        }

        //CJW
        public T CallMethod<T>(IntPtr methodId, bool bIsStaticMethod, String sig, List<object> param)
        {
            try
            {
                if (typeof(T) == typeof(byte))
                {
                    // Call the byte method 
                    byte res;
                    if (bIsStaticMethod)
                        res = env.CallStaticByteMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallByteMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(bool))
                {
                    // Call the boolean method 
                    bool res;
                    if (bIsStaticMethod)
                        res = env.CallStaticBooleanMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallBooleanMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                if (typeof(T) == typeof(char))
                {
                    // Call the char method 
                    char res;
                    if (bIsStaticMethod)
                        res = env.CallStaticCharMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallCharMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(short))
                {
                    // Call the short method 
                    short res;
                    if (bIsStaticMethod)
                        res = env.CallStaticShortMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallShortMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(int))
                {
                    // Call the int method               
                    int res;
                    if (bIsStaticMethod)
                        res = env.CallStaticIntMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallIntMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(long))
                {
                    // Call the long method 
                    long res;
                    if (bIsStaticMethod)
                        res = env.CallStaticLongMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallLongMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(float))
                {
                    // Call the float method 
                    float res;
                    if (bIsStaticMethod)
                        res = env.CallStaticFloatMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallFloatMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(double))
                {
                    // Call the double method 
                    double res;
                    if (bIsStaticMethod)
                        res = env.CallStaticDoubleMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallDoubleMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res; // need to fix this
                }
                else if (typeof(T) == typeof(string))
                {
                    // Call the string method 
                    IntPtr jstr;
                    if (bIsStaticMethod)
                        jstr = env.CallStaticObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        jstr = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));

                    string res = env.JStringToString(jstr);
                    env.DeleteLocalRef(jstr);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    // Call the byte method
                    IntPtr jobj;
                    if (bIsStaticMethod)
                        jobj = env.CallStaticObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        jobj = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    if (jobj == IntPtr.Zero)
                    {
                        return default(T);
                    }
                    byte[] res = env.JStringToByte(jobj);
                    env.DeleteLocalRef(jobj);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(string[]))
                {
                    // Call the string array method
                    IntPtr jobj;
                    if (bIsStaticMethod)
                        jobj = env.CallStaticObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        jobj = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    if (jobj == IntPtr.Zero)
                    {
                        return default(T);
                    }

                    IntPtr[] objArray = env.GetObjectArray(jobj);
                    string[] res = new string[objArray.Length];

                    for (int i = 0; i < objArray.Length; i++)
                    {
                        res[i] = env.JStringToString(objArray[i]);
                    }

                    env.DeleteLocalRef(jobj);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(int[]))
                {
                    // Call the int array method
                    IntPtr jobj;
                    if (bIsStaticMethod)
                        jobj = env.CallStaticObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        jobj = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    if (jobj == IntPtr.Zero)
                    {
                        return default(T);
                    }
                    int[] res = env.GetIntArray(jobj);
                    env.DeleteLocalRef(jobj);
                    return (T)(object)res;
                }
                else if (typeof(T) == typeof(IntPtr))
                {
                    // Call the object method and deal with whatever comes back in the call code 
                    IntPtr res;
                    if (bIsStaticMethod)
                        res = env.CallStaticObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    else
                        res = env.CallObjectMethod(javaObject, methodId, ParseParameters(sig, param));
                    return (T)(object)res;
                }
                return default(T);
            }
            catch (Exception e)
            {
                throw new Exception("JNICode Error:"+e.Message);
            }
        }

        public string JavaVersion()
        {
            int majorVersion = env.GetMajorVersion();
            int minorVersion = env.GetMinorVersion();
            return majorVersion.ToString() + "." + minorVersion.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaNativeInterface()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            // free native resources if there are any.
            if (javaClass != IntPtr.Zero)
            {
                env.DeleteGlobalRef(javaClass);
                javaClass = IntPtr.Zero;
            }

            if (javaObject != IntPtr.Zero)
            {
                env.DeleteLocalRef(javaObject);
                javaObject = IntPtr.Zero;
            }

            if (disposing)
            {
                // free managed resources
                if (jvm != null)
                {
                    jvm.Dispose();
                    jvm = null;
                }

                if (env != null)
                {
                    env.Dispose();
                    env = null;
                }
            }
        }
    }
}
