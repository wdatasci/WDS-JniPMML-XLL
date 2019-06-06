// Setting the SuppressUnmanagedCodeSecurity should improve performance of these calls as 
// it will allow managed code to call into unmanaged code without a stack walk. Resulting in a 
// substantial performance savings in applications that make multiple JNI calls
    
using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.Security.Permissions;


namespace JNI
{
    public unsafe class JNIEnv : IDisposable
    {
        private IntPtr Env;
        private JNINativeInterface functions;
        private JavaVM javaVM;

        internal JNIEnv(IntPtr jnienv)
        {
            this.Env = jnienv;
            functions = *(*(JNINativeInterfacePtr*) jnienv.ToPointer()).functions;
        }

        public int GetVersion()
        {
            if (getVersion == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetVersion, ref getVersion);
            }

            int version = getVersion(Env);
            CheckJavaExceptionAndThrow();
            return version;
        }

        public int GetMajorVersion()
        {
            return GetVersion() >> 16;
        }

        public int GetMinorVersion()
        {
            return GetVersion()%65536;
        }

        public JavaVM GetJavaVM()
        {
            if (getJavaVM == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetJavaVM, ref getJavaVM);
            }

            if (javaVM == null)
            {
                IntPtr jvm;
                getJavaVM.Invoke(Env, out jvm);
                javaVM = new JavaVM(jvm);
            }
            return javaVM;
        }

        internal IntPtr GetSuperClass(IntPtr obj)
        {
            if (getSuperClass == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetSuperclass, ref getSuperClass);
            }
            IntPtr res = getSuperClass.Invoke(Env, obj);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr FindClass(string name)
        {
            if (findClass == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.FindClass, ref findClass);
            }
            IntPtr res = findClass.Invoke(Env, name);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public byte IsAssignableFrom(IntPtr subclassHandle, IntPtr superclassHandle)
        {
            if (isAssignableFrom == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.IsAssignableFrom, ref isAssignableFrom);
            }
            byte res = isAssignableFrom.Invoke(Env, subclassHandle, superclassHandle);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal IntPtr GetObjectClass(IntPtr obj)
        {
            if (getObjectClass == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetObjectClass, ref getObjectClass);
            }
            IntPtr jniClass = getObjectClass.Invoke(Env, obj);
            CheckJavaExceptionAndThrow();
            return jniClass;
        }

        public IntPtr GetMethodId(IntPtr jniClass, string name, string sig)
        {
            if (getMethodId == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetMethodID, ref getMethodId);
            }

            IntPtr res = getMethodId.Invoke(Env, jniClass, name, sig);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr GetFieldID(IntPtr jniClass, string name, string sig)
        {
            if (getFieldID == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetFieldID, ref getFieldID);
            }
            IntPtr res = getFieldID.Invoke(Env, jniClass, name, sig);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr GetStaticFieldID(IntPtr classHandle, string name, string sig)
        {
            if (getStaticFieldID == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticFieldID, ref getStaticFieldID);
            }
            IntPtr res = getStaticFieldID(Env, classHandle, name, sig);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr GetStaticMethodID(IntPtr jniClass, string name, string sig)
        {
            if (getStaticMethodId == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticMethodID, ref getStaticMethodId);
            }
            IntPtr res = getStaticMethodId.Invoke(Env, jniClass, name, sig);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr NewObject(IntPtr classHandle, IntPtr methodID, params JValue[] args)
        {
            if (newObject == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewObjectA, ref newObject);
            }

            IntPtr res = newObject(Env, classHandle, methodID, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal IntPtr AllocObject(IntPtr classHandle)
        {
            if (allocObject == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.AllocObject, ref allocObject);
            }
            IntPtr res = allocObject(Env, classHandle);
            CheckJavaExceptionAndThrow();
            return res;
        }

        // RegisterNatives\UnRegisterNatives will not work until I fix the the class JNINativeMethod
        public int RegisterNatives(IntPtr classHandle, JNINativeMethod* methods, int nMethods)
        {
            if (registerNatives == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.RegisterNatives, ref registerNatives);
            }
            int res = registerNatives.Invoke(Env, classHandle, methods, nMethods);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public int UnregisterNatives(IntPtr classHandle)
        {
            if (unregisterNatives == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.UnregisterNatives, ref unregisterNatives);
            }
            int res = unregisterNatives.Invoke(Env, classHandle);
            CheckJavaExceptionAndThrow();
            return res;
        }

        #region Reflection Support

        public IntPtr ToReflectedField(IntPtr classHandle, IntPtr fieldID, bool isStatic)
        {
            if (toReflectedField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ToReflectedField, ref toReflectedField);
            }
            IntPtr res = toReflectedField.Invoke(Env, classHandle, fieldID,
                                                 JavaVM.BooleanToByte(isStatic));
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr ToReflectedMethod(IntPtr classHandle, IntPtr methodId, bool isStatic)
        {
            if (toReflectedMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ToReflectedMethod, ref toReflectedMethod);
            }
            IntPtr res = toReflectedMethod.Invoke(Env, classHandle, methodId,
                                                  JavaVM.BooleanToByte(isStatic));
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr FromReflectedMethod(IntPtr methodId)
        {
            if (fromReflectedMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.FromReflectedMethod, ref fromReflectedMethod);
            }
            IntPtr res = fromReflectedMethod.Invoke(Env, methodId);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr FromReflectedField(IntPtr FieldId)
        {
            if (fromReflectedField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.FromReflectedField, ref fromReflectedField);
            }
            IntPtr res = fromReflectedField.Invoke(Env, FieldId);
            CheckJavaExceptionAndThrow();
            return res;
        }

        #endregion

        #region Call instance Methods

        // Calling this method raises a pInvokeStackImbalance error in .net 4 if you do not call their A couterpart
        // i.e do not use functions.CallObjectMethod but use functions.CallObjectMethodA
        public IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, params JValue[] args)
        {
            if (callObjectMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallObjectMethodA, ref callObjectMethod);
            }
            IntPtr res = callObjectMethod(Env, obj, methodID, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public bool CallBooleanMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callBooleanMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallBooleanMethodA, ref callBooleanMethod);
            }
            bool res = callBooleanMethod(Env, obj, methodId, args) != 0;
            CheckJavaExceptionAndThrow();
            return res;
        }

        public int CallIntMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callIntMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallIntMethodA, ref callIntMethod);
            }
            int res = callIntMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public short CallShortMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callShortMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallShortMethodA, ref callShortMethod);
            }
            short res = callShortMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public long CallLongMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callLongMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallLongMethodA, ref callLongMethod);
            }
            long res = callLongMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public byte CallByteMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callByteMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallByteMethodA, ref callByteMethod);
            }
            byte res = callByteMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public double CallDoubleMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callDoubleMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallDoubleMethodA, ref callDoubleMethod);
            }
            double res = callDoubleMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public float CallFloatMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callFloatMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallFloatMethodA, ref callFloatMethod);
            }
            float res = callFloatMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public char CallCharMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callCharMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallCharMethodA, ref callCharMethod);
            }
            var res = (char) callCharMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public void CallVoidMethod(IntPtr obj, IntPtr methodId, params JValue[] args)
        {
            if (callVoidMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallVoidMethodA, ref callVoidMethod);
            }
            callVoidMethod(Env, obj, methodId, args);
            CheckJavaExceptionAndThrow();
            return;
        }

        #endregion

        #region Call Static Methods

        public void CallStaticVoidMethod(IntPtr jniClass, IntPtr methodId, params JValue[] args)
        {
            if (callStaticVoidMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticVoidMethodA, ref callStaticVoidMethod);
            }
            callStaticVoidMethod(Env, jniClass, methodId, args);
            CheckJavaExceptionAndThrow();
        }

        public IntPtr CallStaticObjectMethod(IntPtr obj, IntPtr methodID, params JValue[] args)
        {
            if (callStaticObjectMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticObjectMethodA, ref callStaticObjectMethod);
            }
            IntPtr res = callStaticObjectMethod(Env, obj, methodID, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public int CallStaticIntMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticIntMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticIntMethodA, ref callStaticIntMethod);
            }
            int res = callStaticIntMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public long CallStaticLongMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticLongMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticLongMethodA, ref callStaticLongMethod);
            }
            long res = callStaticLongMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public double CallStaticDoubleMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticDoubleMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticDoubleMethodA, ref callStaticDoubleMethod);
            }
            double res = callStaticDoubleMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public float CallStaticFloatMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticFloatMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticFloatMethodA, ref callStaticFloatMethod);
            }
            float res = callStaticFloatMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public short CallStaticShortMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticShortMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticShortMethodA, ref callStaticShortMethod);
            }
            short res = callStaticShortMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public char CallStaticCharMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticCharMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticCharMethodA, ref callStaticCharMethod);
            }
            var res = (char) callStaticCharMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public bool CallStaticBooleanMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticBooleanMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticBooleanMethodA, ref callStaticBooleanMethod);
            }
            bool res = callStaticBooleanMethod(Env, jniClass, MethodId, args) != 0;
            CheckJavaExceptionAndThrow();
            return res;
        }

        public byte CallStaticByteMethod(IntPtr jniClass, IntPtr MethodId, params JValue[] args)
        {
            if (callStaticByteMethod == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.CallStaticByteMethodA, ref callStaticByteMethod);
            }
            byte res = callStaticByteMethod(Env, jniClass, MethodId, args);
            CheckJavaExceptionAndThrow();
            return res;
        }

        #endregion

        #region Array definitions

        public IntPtr NewObjectArray(int len, IntPtr classHandle, IntPtr init)
        {
            if (newObjectArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewObjectArray, ref newObjectArray);
            }

            IntPtr res = newObjectArray(Env, len, classHandle, init);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr NewIntArray(int len, IntPtr classHandle)
        {
            if (newIntArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewIntArray, ref newIntArray);
            }

            IntPtr res = newIntArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }


        public IntPtr NewLongArray(int len, IntPtr classHandle)
        {
            if (newLongArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewLongArray, ref newLongArray);
            }

            IntPtr res = newLongArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr NewCharArray(int len, IntPtr classHandle)
        {
            if (newCharArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewCharArray, ref newCharArray);
            }

            IntPtr res = newCharArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr NewShortArray(int len, IntPtr classHandle)
        {
            if (newShortArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewShortArray, ref newShortArray);
            }

            IntPtr res = newShortArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }


        public IntPtr NewDoubleArray(int len, IntPtr classHandle)
        {
            if (newDoubleArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewDoubleArray, ref newDoubleArray);
            }

            IntPtr res = newDoubleArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr NewFloatArray(int len, IntPtr classHandle)
        {
            if (newFloatArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewFloatArray, ref newFloatArray);
            }

            IntPtr res = newFloatArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }

        public void SetObjectArrayElement(IntPtr array, int index, IntPtr val)
        {
            if (setObjectArrayElement == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetObjectArrayElement, ref setObjectArrayElement);
            }

           setObjectArrayElement(Env, array, index, val);

            CheckJavaExceptionAndThrow();
        }

        public IntPtr NewByteArray(int len, IntPtr classHandle)
        {
            if (newByteArray == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewByteArray, ref newByteArray);
            }

            IntPtr res = newByteArray(Env, len);

            CheckJavaExceptionAndThrow();
            return res;
        }

        #endregion

        #region getters instance

        public IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
        {
            if (getObjectField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetObjectField, ref getObjectField);
            }
            IntPtr res = getObjectField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }


        public int GetArrayLength(IntPtr obj)
        {
            if (getArrayLength == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetArrayLength, ref getArrayLength);
            }
            int len = getArrayLength(Env, obj);
            CheckJavaExceptionAndThrow();
            return len;
        }

        internal byte[] GetByteArray(IntPtr obj)
        {
            if (getByteArrayElements == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetByteArrayElements, ref getByteArrayElements);
            }

            byte* res = getByteArrayElements(Env, obj, null);
            int len = this.GetArrayLength(obj);
            byte[] bResult = new byte[len];
            IntPtr byteSource = (IntPtr)res;
            Marshal.Copy(byteSource, bResult, 0, len);
            CheckJavaExceptionAndThrow();
            return bResult;
        }


        internal IntPtr[] GetObjectArray(IntPtr obj)
        {
            if (getByteArrayElements == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetObjectArrayElement, ref getObjectArrayElement);
            }

            int len = this.GetArrayLength(obj);
            IntPtr[] oResult = new IntPtr[len];
            for (int i=0; i < len; i++) {
                IntPtr res = getObjectArrayElement(Env, obj, i);
                oResult[i] = res;
            }

            CheckJavaExceptionAndThrow();
            return oResult;
        }

        public int[] GetIntArray(IntPtr obj)
        {
            if (getIntArrayElements == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetIntArrayElements, ref getIntArrayElements);
            }

            if (releaseIntArrayElements == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ReleaseIntArrayElements, ref releaseIntArrayElements);
            }

            int len = this.GetArrayLength(obj);
            byte isCopy;
            int* elems = getIntArrayElements(Env, obj, &isCopy);
            int[] res = new int[len];
            for (int i = 0; i < len; i++)
                res[i] = (int)elems[i];

            if (isCopy == JNIBooleanValue.JNI_TRUE)
                releaseIntArrayElements(Env, obj, elems, JNIReturnValue.JNI_ABORT);

            CheckJavaExceptionAndThrow();
            return res;
        }

        public bool GetBooleanField(IntPtr obj, IntPtr fieldID)
        {
            if (getBooleanField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetBooleanField, ref getBooleanField);
            }
            bool res = getBooleanField(Env, obj, fieldID) != 0;
            CheckJavaExceptionAndThrow();
            return res;
        }

        public byte GetByteField(IntPtr obj, IntPtr fieldID)
        {
            if (getByteField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetByteField, ref getByteField);
            }
            byte res = getByteField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public short GetShortField(IntPtr obj, IntPtr fieldID)
        {
            if (getShortField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetShortField, ref getShortField);
            }
            short res = getShortField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public long GetLongField(IntPtr obj, IntPtr fieldID)
        {
            if (getLongField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetLongField, ref getLongField);
            }
            long res = getLongField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public int GetIntField(IntPtr obj, IntPtr fieldID)
        {
            if (getIntField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetIntField, ref getIntField);
            }
            int res = getIntField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public double GetDoubleField(IntPtr obj, IntPtr fieldID)
        {
            if (getDoubleField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetDoubleField, ref getDoubleField);
            }
            double res = getDoubleField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public float GetFloatField(IntPtr obj, IntPtr fieldID)
        {
            if (getFloatField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetFloatField, ref getFloatField);
            }
            float res = getFloatField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public char GetCharField(IntPtr obj, IntPtr fieldID)
        {
            if (getCharField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetCharField, ref getCharField);
            }
            var res = (char) getCharField(Env, obj, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        #endregion

        #region getters static

        public IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
        {
            if (getStaticObjectField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticObjectField, ref getStaticObjectField);
            }
            IntPtr res = getStaticObjectField(Env, clazz, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }


        public bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID)
        {
            if (getStaticBooleanField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticBooleanField, ref getStaticBooleanField);
            }
            bool res = getStaticBooleanField(Env, clazz, fieldID) != 0;
            CheckJavaExceptionAndThrow();
            return res;
        }

        public byte GetStaticByteField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticByteField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticByteField, ref getStaticByteField);
            }
            byte res = getStaticByteField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public short GetStaticShortField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticShortField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticShortField, ref getStaticShortField);
            }
            short res = getStaticShortField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public long GetStaticLongField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticLongField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticLongField, ref getStaticLongField);
            }
            long res = getStaticLongField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public int GetStaticIntField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticIntField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticIntField, ref getStaticIntField);
            }
            int res = getStaticIntField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public double GetStaticDoubleField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticDoubleField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticDoubleField, ref getStaticDoubleField);
            }
            double res = getStaticDoubleField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public float GetStaticFloatField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticFloatField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticFloatField, ref getStaticFloatField);
            }
            float res = getStaticFloatField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public char GetStaticCharField(IntPtr classHandle, IntPtr fieldID)
        {
            if (getStaticCharField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStaticCharField, ref getStaticCharField);
            }
            var res = (char) getStaticCharField(Env, classHandle, fieldID);
            CheckJavaExceptionAndThrow();
            return res;
        }

        #endregion

        #region setters instance

        internal void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr value)
        {
            if (setObjectField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetObjectField, ref setObjectField);
            }
            setObjectField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetIntField(IntPtr obj, IntPtr fieldID, int value)
        {
            if (setIntField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetIntField, ref setIntField);
            }
            setIntField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetBooleanField(IntPtr obj, IntPtr fieldID, bool value)
        {
            if (setBooleanField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetBooleanField, ref setBooleanField);
            }
            setBooleanField(Env, obj, fieldID, JavaVM.BooleanToByte(value));
            CheckJavaExceptionAndThrow();
        }

        internal void SetByteField(IntPtr obj, IntPtr fieldID, byte value)
        {
            if (setByteField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetByteField, ref setByteField);
            }
            setByteField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetCharField(IntPtr obj, IntPtr fieldID, char value)
        {
            if (setCharField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetCharField, ref setCharField);
            }
            setCharField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetShortField(IntPtr obj, IntPtr fieldID, short value)
        {
            if (setShortField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetShortField, ref setShortField);
            }
            setShortField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetLongField(IntPtr obj, IntPtr fieldID, long value)
        {
            if (setLongField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetLongField, ref setLongField);
            }
            setLongField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetFloatField(IntPtr obj, IntPtr fieldID, float value)
        {
            if (setFloatField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetFloatField, ref setFloatField);
            }
            setFloatField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetDoubleField(IntPtr obj, IntPtr fieldID, double value)
        {
            if (setDoubleField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetDoubleField, ref setDoubleField);
            }
            setDoubleField(Env, obj, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        # endregion

        #region setters static

        internal void SetStaticObjectField(IntPtr classHandle, IntPtr fieldID, IntPtr value)
        {
            if (setStaticObjectField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticObjectField, ref setStaticObjectField);
            }
            setStaticObjectField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticIntField(IntPtr classHandle, IntPtr fieldID, int value)
        {
            if (setStaticIntField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticIntField, ref setStaticIntField);
            }
            setStaticIntField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticBooleanField(IntPtr classHandle, IntPtr fieldID, bool value)
        {
            if (setStaticBooleanField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticBooleanField, ref setStaticBooleanField);
            }
            setStaticBooleanField(Env, classHandle, fieldID, JavaVM.BooleanToByte(value));
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticByteField(IntPtr classHandle, IntPtr fieldID, byte value)
        {
            if (setStaticByteField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticByteField, ref setStaticByteField);
            }
            setStaticByteField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticCharField(IntPtr classHandle, IntPtr fieldID, char value)
        {
            if (setStaticCharField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticCharField, ref setStaticCharField);
            }
            setStaticCharField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticShortField(IntPtr classHandle, IntPtr fieldID, short value)
        {
            if (setStaticShortField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticShortField, ref setStaticShortField);
            }
            setStaticShortField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticLongField(IntPtr classHandle, IntPtr fieldID, long value)
        {
            if (setStaticLongField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticLongField, ref setStaticLongField);
            }
            setStaticLongField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticFloatField(IntPtr classHandle, IntPtr fieldID, float value)
        {
            if (setStaticFloatField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticFloatField, ref setStaticFloatField);
            }
            setStaticFloatField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        internal void SetStaticDoubleField(IntPtr classHandle, IntPtr fieldID, double value)
        {
            if (setStaticDoubleField == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.SetStaticDoubleField, ref setStaticDoubleField);
            }
            setStaticDoubleField(Env, classHandle, fieldID, value);
            CheckJavaExceptionAndThrow();
        }

        #endregion

        #region string methods

        public IntPtr NewString(String unicode, int len)
        {
            if (newString == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewString, ref newString);
            }
            IntPtr res = newString(Env, unicode, len);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr NewStringUFT(IntPtr UFT)
        {
            if (newStringUTF == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewStringUTF, ref newStringUTF);
            }
            IntPtr res = newStringUTF(Env, UFT);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal IntPtr GetStringChars(IntPtr JStr, byte* b)
        {
            if (getStringChars == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetStringChars, ref getStringChars);
            }
            IntPtr res = getStringChars(Env, JStr, b);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal void ReleaseStringChars(IntPtr JStr, IntPtr chars)
        {
            if (releaseStringChars == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ReleaseStringChars, ref releaseStringChars);
            }
            releaseStringChars(Env, JStr, chars);
            CheckJavaExceptionAndThrow();
        }

        internal byte[] JStringToByte(IntPtr JStr)
        {
            if (JStr != null)
            {
                return GetByteArray(JStr);
            }
            else return null;
        }

        internal string JStringToString(IntPtr JStr)
        {
            if (JStr != null)
            {
                byte b;
                IntPtr chars = GetStringChars(JStr, &b);
                string result = Marshal.PtrToStringUni(chars);
                ReleaseStringChars(JStr, chars);
                return result;
            }
            else return null;
        }

        #endregion

        #region buffer  -- I can not see any reason why these would be used

        public IntPtr NewDirectByteBuffer(IntPtr address, long capacity)
        {
            if (newDirectByteBuffer == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewDirectByteBuffer, ref newDirectByteBuffer);
            }
            IntPtr res = newDirectByteBuffer.Invoke(Env, address, capacity);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public IntPtr GetDirectBufferAddress(IntPtr buf)
        {
            if (getDirectBufferAddress == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetDirectBufferAddress, ref getDirectBufferAddress);
            }
            IntPtr res = getDirectBufferAddress.Invoke(Env, buf);
            CheckJavaExceptionAndThrow();
            return res;
        }

        public long GetDirectBufferCapacity(IntPtr buf)
        {
            if (getDirectBufferCapacity == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetDirectBufferCapacity, ref getDirectBufferCapacity);
            }
            long res = getDirectBufferCapacity.Invoke(Env, buf);
            CheckJavaExceptionAndThrow();
            return res;
        }

        #endregion

        #region references

        public IntPtr NewGlobalRef(IntPtr objectHandle)
        {
            if (newGlobalRef == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewGlobalRef, ref newGlobalRef);
            }
            if (objectHandle != null)
            {
                IntPtr res = newGlobalRef(Env, objectHandle);
                return res;
            }
            else return System.IntPtr.Zero;
        }

        internal IntPtr NewLocalRef(IntPtr objectHandle)
        {
            if (newLocalRef == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.NewLocalRef, ref newLocalRef);
            }
            if (objectHandle != null)
            {
                IntPtr res = newLocalRef(Env, objectHandle);
                return res;
            }
            else return System.IntPtr.Zero;
        }

        internal IntPtr PopLocalFrame(IntPtr result)
        {
            if (popLocalFrame == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.PopLocalFrame, ref popLocalFrame);
            }
            IntPtr res = popLocalFrame(Env, result);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal int PushLocalFrame(int capacity)
        {
            if (pushLocalFrame == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.PushLocalFrame, ref pushLocalFrame);
            }
            int res = pushLocalFrame(Env, capacity);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal int EnsureLocalCapacity(int capacity)
        {
            if (ensureLocalCapacity == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.EnsureLocalCapacity, ref ensureLocalCapacity);
            }
            int res = ensureLocalCapacity(Env, capacity);
            CheckJavaExceptionAndThrow();
            return res;
        }

        internal void DeleteGlobalRef(IntPtr objectHandle)
        {
            if (deleteGlobalRef == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.DeleteGlobalRef, ref deleteGlobalRef);
            }
            if (objectHandle != null)
            {
                deleteGlobalRef(Env, objectHandle);
            }
        }

        internal void DeleteLocalRef(IntPtr objectHandle)
        {
            if (deleteLocalRef == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.DeleteLocalRef, ref deleteLocalRef);
            }
            if (objectHandle != null)
            {
                deleteLocalRef(Env, objectHandle);
            }
        }

        #endregion

        #region exceptions

        public IntPtr ExceptionOccurred()
        {
            if (exceptionOccurred == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ExceptionOccurred, ref exceptionOccurred);
            }
            IntPtr res = exceptionOccurred(Env);

            return res;
        }

        public void FatalError(string message)
        {
            if (fatalError == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.FatalError, ref fatalError);
            }
            fatalError(Env, Marshal.StringToHGlobalUni(message));
        }

        public void ExceptionClear()
        {
            if (exceptionClear == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ExceptionClear, ref exceptionClear);
            }
            exceptionClear(Env);
        }

        public void ExceptionDescribe()
        {
            if (exceptionDescribe == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ExceptionDescribe, ref exceptionDescribe);
            }
            exceptionDescribe(Env);
        }

        internal void Throw(IntPtr objectHandle)
        {
            if (_throw == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.Throw, ref _throw);
            }
            int iResult = _throw(Env, objectHandle);
            if (iResult != JNIReturnValue.JNI_OK)
            {
                throw new Exception("Can't throw");
            }
        }

        public void ThrowNew(IntPtr classHandle, string message)
        {
            if (throwNew == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ThrowNew, ref throwNew);
            }
            IntPtr uni = Marshal.StringToHGlobalUni(message);
            int iResult = throwNew(Env, classHandle, uni);
            if (iResult != JNIReturnValue.JNI_OK)
            {
                throw new Exception("Can't throw");
            }
            Marshal.FreeHGlobal(uni);
        }

        public string CatchJavaException()
        {
            IntPtr occurred = ExceptionOccurred();
            if (occurred != null && occurred !=IntPtr.Zero)
            {
                try {
                    ExceptionClear();
                    IntPtr ExceptionClass = this.GetObjectClass(occurred);
                    IntPtr mid = GetMethodId(ExceptionClass, "toString", "()Ljava/lang/String;");
                    IntPtr jstr = CallObjectMethod(occurred, mid, new JValue() {});

                    return JStringToString(jstr);
                } catch (Exception e) {
                    return e.Message + "\n" + e.StackTrace;
                }
            }
            return "";
        }

        public unsafe bool CheckJavaExceptionAndThrow()
        {
            if (exceptionCheck == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ExceptionCheck, ref exceptionCheck);
            }
            //CJW
            bool flag = (exceptionCheck(Env) != 0);
            if (flag)
            {
                string jexcp = CatchJavaException();
                throw new Exception("ExceptionCheck() failed: " + jexcp);
            }
            return flag;
        }

        #endregion

        #region HelperMethods

        internal void PackPrimitiveArray<T>(T[] sourceArray, IntPtr pointerToArray)
        {
            byte isCopy = 0;
            byte[] byteArray = new byte[sourceArray.Length*Marshal.SizeOf(typeof (T))];
            Buffer.BlockCopy(sourceArray, 0, byteArray, 0, sourceArray.Length*Marshal.SizeOf(typeof (T)));
            byte* pb = (byte*) this.GetPrimitiveArrayCritical(pointerToArray, &isCopy);
            if (pb == null)
            {
                throw new Exception("An error occurred whilst packing the array.");
            }
            try
            {
                Marshal.Copy(byteArray, 0, (new IntPtr(pb)), sourceArray.Length*Marshal.SizeOf(typeof (T)));
            }
            finally
            {
                this.ReleasePrimitiveArrayCritical(pointerToArray, pb, 0);
            }
        }

        internal void ReleasePrimitiveArrayCritical(IntPtr array, void* carray, int mode)
        {
            if (releasePrimitiveArrayCritical == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.ReleasePrimitiveArrayCritical,
                                                     ref releasePrimitiveArrayCritical);
            }
            releasePrimitiveArrayCritical(Env, array, carray, mode);
            CheckJavaExceptionAndThrow();
        }

        internal void* GetPrimitiveArrayCritical(IntPtr array, byte* isCopy)
        {
            if (getPrimitiveArrayCritical == null)
            {
                JavaVM.GetDelegateForFunctionPointer(functions.GetPrimitiveArrayCritical, ref getPrimitiveArrayCritical);
            }
            var res = getPrimitiveArrayCritical(Env, array, isCopy);
            CheckJavaExceptionAndThrow();
            return res;
        }



    #endregion
        #region Nested type: Delegates

        private JNINativeInterface_.AllocObject allocObject;
        private JNINativeInterface_.CallBooleanMethod callBooleanMethod;
        private JNINativeInterface_.CallByteMethod callByteMethod;
        private JNINativeInterface_.CallCharMethod callCharMethod;
        private JNINativeInterface_.CallDoubleMethod callDoubleMethod;
        private JNINativeInterface_.CallFloatMethod callFloatMethod;
        private JNINativeInterface_.CallIntMethod callIntMethod;
        private JNINativeInterface_.CallLongMethod callLongMethod;
        private JNINativeInterface_.CallVoidMethod callVoidMethod;

        private JNINativeInterface_.CallNonvirtualBooleanMethod callNonvirtualBooleanMethod;
        private JNINativeInterface_.CallNonvirtualByteMethod callNonvirtualByteMethod;
        private JNINativeInterface_.CallNonvirtualCharMethod callNonvirtualCharMethod;
        private JNINativeInterface_.CallNonvirtualDoubleMethod callNonvirtualDoubleMethod;
        private JNINativeInterface_.CallNonvirtualFloatMethod callNonvirtualFloatMethod;
        private JNINativeInterface_.CallNonvirtualIntMethod callNonvirtualIntMethod;
        private JNINativeInterface_.CallNonvirtualLongMethod callNonvirtualLongMethod;
        private JNINativeInterface_.CallNonvirtualObjectMethod callNonvirtualObjectMethod;
        private JNINativeInterface_.CallNonvirtualShortMethod callNonvirtualShortMethod;
        private JNINativeInterface_.CallNonvirtualVoidMethod callNonvirtualVoidMethod;
        private JNINativeInterface_.CallObjectMethod callObjectMethod;
        private JNINativeInterface_.CallShortMethod callShortMethod;
        private JNINativeInterface_.CallStaticBooleanMethod callStaticBooleanMethod;
        private JNINativeInterface_.CallStaticByteMethod callStaticByteMethod;
        private JNINativeInterface_.CallStaticCharMethod callStaticCharMethod;
        private JNINativeInterface_.CallStaticDoubleMethod callStaticDoubleMethod;
        private JNINativeInterface_.CallStaticFloatMethod callStaticFloatMethod;
        private JNINativeInterface_.CallStaticIntMethod callStaticIntMethod;
        private JNINativeInterface_.CallStaticLongMethod callStaticLongMethod;
        private JNINativeInterface_.CallStaticObjectMethod callStaticObjectMethod;
        private JNINativeInterface_.CallStaticShortMethod callStaticShortMethod;
        private JNINativeInterface_.CallStaticVoidMethod callStaticVoidMethod;
        
        private JNINativeInterface_.DefineClass defineClass;
        private JNINativeInterface_.DeleteGlobalRef deleteGlobalRef;
        private JNINativeInterface_.DeleteLocalRef deleteLocalRef;
        private JNINativeInterface_.DeleteWeakGlobalRef deleteWeakGlobalRef;
        private JNINativeInterface_.EnsureLocalCapacity ensureLocalCapacity;
        private JNINativeInterface_.ExceptionCheck exceptionCheck;
        private JNINativeInterface_.ExceptionClear exceptionClear;
        private JNINativeInterface_.ExceptionDescribe exceptionDescribe;
        private JNINativeInterface_.ExceptionOccurred exceptionOccurred;
        private JNINativeInterface_.FatalError fatalError;
        private JNINativeInterface_.FindClass findClass;
        private JNINativeInterface_.FromReflectedField fromReflectedField;
        private JNINativeInterface_.FromReflectedMethod fromReflectedMethod;
        private JNINativeInterface_.GetArrayLength getArrayLength;
        private JNINativeInterface_.GetBooleanArrayElements getBooleanArrayElements;
        private JNINativeInterface_.GetBooleanArrayRegion getBooleanArrayRegion;
        private JNINativeInterface_.GetBooleanField getBooleanField;
        private JNINativeInterface_.GetByteArrayElements getByteArrayElements;
        private JNINativeInterface_.GetByteArrayRegion getByteArrayRegion;
        private JNINativeInterface_.GetByteField getByteField;
        private JNINativeInterface_.GetCharArrayElements getCharArrayElements;
        private JNINativeInterface_.GetCharArrayRegion getCharArrayRegion;
        private JNINativeInterface_.GetCharField getCharField;
        private JNINativeInterface_.GetDirectBufferAddress getDirectBufferAddress;
        private JNINativeInterface_.GetDirectBufferCapacity getDirectBufferCapacity;
        private JNINativeInterface_.GetDoubleArrayElements getDoubleArrayElements;
        private JNINativeInterface_.GetDoubleArrayRegion getDoubleArrayRegion;
        private JNINativeInterface_.GetDoubleField getDoubleField;
        private JNINativeInterface_.GetFieldID getFieldID;
        private JNINativeInterface_.GetFloatArrayElements getFloatArrayElements;
        private JNINativeInterface_.GetFloatArrayRegion getFloatArrayRegion;
        private JNINativeInterface_.GetFloatField getFloatField;
        private JNINativeInterface_.GetIntArrayElements getIntArrayElements;
        private JNINativeInterface_.GetIntArrayRegion getIntArrayRegion;
        private JNINativeInterface_.GetIntField getIntField;
        private JNINativeInterface_.GetJavaVM getJavaVM;
        private JNINativeInterface_.GetLongArrayElements getLongArrayElements;
        private JNINativeInterface_.GetLongArrayRegion getLongArrayRegion;
        private JNINativeInterface_.GetLongField getLongField;
        private JNINativeInterface_.GetMethodId getMethodId;
        private JNINativeInterface_.GetObjectArrayElement getObjectArrayElement;
        private JNINativeInterface_.GetObjectClass getObjectClass;
        private JNINativeInterface_.GetObjectField getObjectField;
        private JNINativeInterface_.GetPrimitiveArrayCritical getPrimitiveArrayCritical;
        private JNINativeInterface_.GetShortArrayElements getShortArrayElements;
        private JNINativeInterface_.GetShortArrayRegion getShortArrayRegion;
        private JNINativeInterface_.GetShortField getShortField;
        private JNINativeInterface_.GetStaticBooleanField getStaticBooleanField;
        private JNINativeInterface_.GetStaticByteField getStaticByteField;
        private JNINativeInterface_.GetStaticCharField getStaticCharField;
        private JNINativeInterface_.GetStaticDoubleField getStaticDoubleField;
        private JNINativeInterface_.GetStaticFieldID getStaticFieldID;
        private JNINativeInterface_.GetStaticFloatField getStaticFloatField;
        private JNINativeInterface_.GetStaticIntField getStaticIntField;
        private JNINativeInterface_.GetStaticLongField getStaticLongField;
        private JNINativeInterface_.GetStaticMethodId getStaticMethodId;
        private JNINativeInterface_.GetStaticObjectField getStaticObjectField;
        private JNINativeInterface_.GetStaticShortField getStaticShortField;
        private JNINativeInterface_.GetStringChars getStringChars;
        private JNINativeInterface_.GetStringCritical getStringCritical;
        private JNINativeInterface_.GetStringLength getStringLength;
        private JNINativeInterface_.GetStringRegion getStringRegion;
        private JNINativeInterface_.GetStringUTFChars getStringUTFChars;
        private JNINativeInterface_.GetStringUTFLength getStringUTFLength;
        private JNINativeInterface_.GetStringUTFRegion getStringUTFRegion;
        private JNINativeInterface_.GetSuperclass getSuperClass;
        private JNINativeInterface_.GetVersion getVersion;
        private JNINativeInterface_.IsAssignableFrom isAssignableFrom;
        private JNINativeInterface_.IsSameObject isSameObject;
        private JNINativeInterface_.MonitorEnter monitorEnter;
        private JNINativeInterface_.MonitorExit monitorExit;
        private JNINativeInterface_.NewBooleanArray newBooleanArray;
        private JNINativeInterface_.NewByteArray newByteArray;
        private JNINativeInterface_.NewCharArray newCharArray;
        private JNINativeInterface_.NewDirectByteBuffer newDirectByteBuffer;
        private JNINativeInterface_.NewDoubleArray newDoubleArray;
        private JNINativeInterface_.NewFloatArray newFloatArray;
        private JNINativeInterface_.NewGlobalRef newGlobalRef;
        private JNINativeInterface_.NewIntArray newIntArray;
        private JNINativeInterface_.NewLocalRef newLocalRef;
        private JNINativeInterface_.NewLongArray newLongArray;
        private JNINativeInterface_.NewObject newObject;
        private JNINativeInterface_.NewObjectArray newObjectArray;
        private JNINativeInterface_.NewShortArray newShortArray;

        private JNINativeInterface_.NewString newString;
        private JNINativeInterface_.NewStringUTF newStringUTF;
        private JNINativeInterface_.NewWeakGlobalRef newWeakGlobalRef;
        private JNINativeInterface_.PopLocalFrame popLocalFrame;
        private JNINativeInterface_.PushLocalFrame pushLocalFrame;
        private JNINativeInterface_.RegisterNatives registerNatives;
        private JNINativeInterface_.UnregisterNatives unregisterNatives;
        private JNINativeInterface_.ReleaseBooleanArrayElements releaseBooleanArrayElements;
        private JNINativeInterface_.ReleaseByteArrayElements releaseByteArrayElements;
        private JNINativeInterface_.ReleaseCharArrayElements releaseCharArrayElements;
        private JNINativeInterface_.ReleaseDoubleArrayElements releaseDoubleArrayElements;
        private JNINativeInterface_.ReleaseFloatArrayElements releaseFloatArrayElements;
        private JNINativeInterface_.ReleaseIntArrayElements releaseIntArrayElements;
        private JNINativeInterface_.ReleaseLongArrayElements releaseLongArrayElements;
        private JNINativeInterface_.ReleasePrimitiveArrayCritical releasePrimitiveArrayCritical;
        private JNINativeInterface_.ReleaseShortArrayElements releaseShortArrayElements;
        private JNINativeInterface_.ReleaseStringChars releaseStringChars;
        private JNINativeInterface_.ReleaseStringCritical releaseStringCritical;
        private JNINativeInterface_.ReleaseStringUTFChars releaseStringUTFChars;
        private JNINativeInterface_.SetBooleanArrayRegion setBooleanArrayRegion;
        private JNINativeInterface_.SetBooleanField setBooleanField;
        private JNINativeInterface_.SetByteArrayRegion setByteArrayRegion;
        private JNINativeInterface_.SetByteField setByteField;
        private JNINativeInterface_.SetCharArrayRegion setCharArrayRegion;
        private JNINativeInterface_.SetCharField setCharField;
        private JNINativeInterface_.SetDoubleArrayRegion setDoubleArrayRegion;
        private JNINativeInterface_.SetDoubleField setDoubleField;
        private JNINativeInterface_.SetFloatArrayRegion setFloatArrayRegion;
        private JNINativeInterface_.SetFloatField setFloatField;
        private JNINativeInterface_.SetIntArrayRegion setIntArrayRegion;
        private JNINativeInterface_.SetIntField setIntField;
        private JNINativeInterface_.SetLongArrayRegion setLongArrayRegion;
        private JNINativeInterface_.SetLongField setLongField;
        private JNINativeInterface_.SetObjectArrayElement setObjectArrayElement;
        private JNINativeInterface_.SetObjectField setObjectField;
        private JNINativeInterface_.SetShortArrayRegion setShortArrayRegion;
        private JNINativeInterface_.SetShortField setShortField;
        private JNINativeInterface_.SetStaticBooleanField setStaticBooleanField;
        private JNINativeInterface_.SetStaticByteField setStaticByteField;
        private JNINativeInterface_.SetStaticCharField setStaticCharField;
        private JNINativeInterface_.SetStaticDoubleField setStaticDoubleField;
        private JNINativeInterface_.SetStaticFloatField setStaticFloatField;
        private JNINativeInterface_.SetStaticIntField setStaticIntField;
        private JNINativeInterface_.SetStaticLongField setStaticLongField;
        private JNINativeInterface_.SetStaticObjectField setStaticObjectField;
        private JNINativeInterface_.SetStaticShortField setStaticShortField;
        private JNINativeInterface_.Throw _throw;
        private JNINativeInterface_.ThrowNew throwNew;
        private JNINativeInterface_.ToReflectedField toReflectedField;
        private JNINativeInterface_.ToReflectedMethod toReflectedMethod;


        #endregion
        private struct JNINativeInterfacePtr
        {
            public JNINativeInterface* functions;
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        ~JNIEnv()
        {
            this.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        protected virtual void Dispose(bool disposing)
        {       
            if (disposing)
            {
                // free managed resources
                if (javaVM != null)
                {
                    javaVM.Dispose();
                    javaVM = null;
                }
            }
        }
    }

    public struct JNINativeInterface
    {
        public IntPtr reserved0;
        public IntPtr reserved1;
        public IntPtr reserved2;
        public IntPtr reserved3;
        public IntPtr GetVersion;
        public IntPtr DefineClass;
        public IntPtr FindClass;
        // Reflection support
        public IntPtr FromReflectedMethod;
        public IntPtr FromReflectedField;
        public IntPtr ToReflectedMethod;

        public IntPtr GetSuperclass;
        public IntPtr IsAssignableFrom;
        // Reflection support
        public IntPtr ToReflectedField;

        public IntPtr Throw;
        public IntPtr ThrowNew;
        public IntPtr ExceptionOccurred;
        public IntPtr ExceptionDescribe;
        public IntPtr ExceptionClear;
        public IntPtr FatalError;

        // Local Reference Management
        public IntPtr PushLocalFrame;
        public IntPtr PopLocalFrame;

        public IntPtr NewGlobalRef;
        public IntPtr DeleteGlobalRef;
        public IntPtr DeleteLocalRef;
        public IntPtr IsSameObject;
        public IntPtr NewLocalRef;
        public IntPtr EnsureLocalCapacity;
        public IntPtr AllocObject;

        public IntPtr NewObject;
        public IntPtr NewObjectV;
        public IntPtr NewObjectA;

        public IntPtr GetObjectClass;
        public IntPtr IsInstanceOf;
        public IntPtr GetMethodID;
        public IntPtr CallObjectMethod;
        public IntPtr CallObjectMethodV;
        public IntPtr CallObjectMethodA;
        public IntPtr CallBooleanMethod;
        public IntPtr CallBooleanMethodV;
        public IntPtr CallBooleanMethodA;
        public IntPtr CallByteMethod;
        public IntPtr CallByteMethodV;
        public IntPtr CallByteMethodA;
        public IntPtr CallCharMethod;
        public IntPtr CallCharMethodV;
        public IntPtr CallCharMethodA;
        public IntPtr CallShortMethod;
        public IntPtr CallShortMethodV;
        public IntPtr CallShortMethodA;
        public IntPtr CallIntMethod;
        public IntPtr CallIntMethodV;
        public IntPtr CallIntMethodA;  
        public IntPtr CallLongMethod;
        public IntPtr CallLongMethodV;
        public IntPtr CallLongMethodA;
        public IntPtr CallFloatMethod;
        public IntPtr CallFloatMethodV;
        public IntPtr CallFloatMethodA;
        public IntPtr CallDoubleMethod;
        public IntPtr CallDoubleMethodV;
        public IntPtr CallDoubleMethodA;
        public IntPtr CallVoidMethod;
        public IntPtr CallVoidMethodV;
        public IntPtr CallVoidMethodA;
        public IntPtr CallNonvirtualObjectMethod;
        public IntPtr CallNonvirtualObjectMethodV;
        public IntPtr CallNonvirtualObjectMethodA;
        public IntPtr CallNonvirtualBooleanMethod;
        public IntPtr CallNonvirtualBooleanMethodV;
        public IntPtr CallNonvirtualBooleanMethodA;
        public IntPtr CallNonvirtualByteMethod;
        public IntPtr CallNonvirtualByteMethodV;
        public IntPtr CallNonvirtualByteMethodA;
        public IntPtr CallNonvirtualCharMethod;
        public IntPtr CallNonvirtualCharMethodV;
        public IntPtr CallNonvirtualCharMethodA;
        public IntPtr CallNonvirtualShortMethod;
        public IntPtr CallNonvirtualShortMethodV;
        public IntPtr CallNonvirtualShortMethodA;
        public IntPtr CallNonvirtualIntMethod;
        public IntPtr CallNonvirtualIntMethodV;
        public IntPtr CallNonvirtualIntMethodA;
        public IntPtr CallNonvirtualLongMethod;
        public IntPtr CallNonvirtualLongMethodV;
        public IntPtr CallNonvirtualLongMethodA;
        public IntPtr CallNonvirtualFloatMethod;
        public IntPtr CallNonvirtualFloatMethodV;
        public IntPtr CallNonvirtualFloatMethodA;
        public IntPtr CallNonvirtualDoubleMethod;
        public IntPtr CallNonvirtualDoubleMethodV;
        public IntPtr CallNonvirtualDoubleMethodA;
        public IntPtr CallNonvirtualVoidMethod;
        public IntPtr CallNonvirtualVoidMethodV;
        public IntPtr CallNonvirtualVoidMethodA;
        public IntPtr GetFieldID;
        public IntPtr GetObjectField;
        public IntPtr GetBooleanField;
        public IntPtr GetByteField;
        public IntPtr GetCharField;
        public IntPtr GetShortField;
        public IntPtr GetIntField;
        public IntPtr GetLongField;
        public IntPtr GetFloatField;
        public IntPtr GetDoubleField;
        public IntPtr SetObjectField;
        public IntPtr SetBooleanField;
        public IntPtr SetByteField;
        public IntPtr SetCharField;
        public IntPtr SetShortField;
        public IntPtr SetIntField;
        public IntPtr SetLongField;
        public IntPtr SetFloatField;
        public IntPtr SetDoubleField;
        public IntPtr GetStaticMethodID;
        public IntPtr CallStaticObjectMethod;
        public IntPtr CallStaticObjectMethodV;
        public IntPtr CallStaticObjectMethodA;
        public IntPtr CallStaticBooleanMethod;
        public IntPtr CallStaticBooleanMethodV;
        public IntPtr CallStaticBooleanMethodA;
        public IntPtr CallStaticByteMethod;
        public IntPtr CallStaticByteMethodV;
        public IntPtr CallStaticByteMethodA;
        public IntPtr CallStaticCharMethod;
        public IntPtr CallStaticCharMethodV;
        public IntPtr CallStaticCharMethodA;
        public IntPtr CallStaticShortMethod;
        public IntPtr CallStaticShortMethodV;
        public IntPtr CallStaticShortMethodA;
        public IntPtr CallStaticIntMethod;
        public IntPtr CallStaticIntMethodV;
        public IntPtr CallStaticIntMethodA;
        public IntPtr CallStaticLongMethod;
        public IntPtr CallStaticLongMethodV;
        public IntPtr CallStaticLongMethodA;
        public IntPtr CallStaticFloatMethod;
        public IntPtr CallStaticFloatMethodV;
        public IntPtr CallStaticFloatMethodA;
        public IntPtr CallStaticDoubleMethod;
        public IntPtr CallStaticDoubleMethodV;
        public IntPtr CallStaticDoubleMethodA;
        public IntPtr CallStaticVoidMethod;
        public IntPtr CallStaticVoidMethodV;
        public IntPtr CallStaticVoidMethodA;
        public IntPtr GetStaticFieldID;
        public IntPtr GetStaticObjectField;
        public IntPtr GetStaticBooleanField;
        public IntPtr GetStaticByteField;
        public IntPtr GetStaticCharField;
        public IntPtr GetStaticShortField;
        public IntPtr GetStaticIntField;
        public IntPtr GetStaticLongField;
        public IntPtr GetStaticFloatField;
        public IntPtr GetStaticDoubleField;
        public IntPtr SetStaticObjectField;
        public IntPtr SetStaticBooleanField;
        public IntPtr SetStaticByteField;
        public IntPtr SetStaticCharField;
        public IntPtr SetStaticShortField;
        public IntPtr SetStaticIntField;
        public IntPtr SetStaticLongField;
        public IntPtr SetStaticFloatField;
        public IntPtr SetStaticDoubleField;

        public IntPtr NewString;
        public IntPtr GetStringLength;
        public IntPtr GetStringChars;
        public IntPtr ReleaseStringChars;
        public IntPtr NewStringUTF;
        public IntPtr GetStringUTFLength;
        public IntPtr GetStringUTFChars;
        public IntPtr ReleaseStringUTFChars;
        public IntPtr GetArrayLength;
        public IntPtr NewObjectArray;
        public IntPtr GetObjectArrayElement;
        public IntPtr SetObjectArrayElement;
        public IntPtr NewBooleanArray;
        public IntPtr NewByteArray;
        public IntPtr NewCharArray;
        public IntPtr NewShortArray;
        public IntPtr NewIntArray;
        public IntPtr NewLongArray;
        public IntPtr NewFloatArray;
        public IntPtr NewDoubleArray;
        public IntPtr GetBooleanArrayElements;
        public IntPtr GetByteArrayElements;
        public IntPtr GetCharArrayElements;
        public IntPtr GetShortArrayElements;
        public IntPtr GetIntArrayElements;
        public IntPtr GetLongArrayElements;
        public IntPtr GetFloatArrayElements;
        public IntPtr GetDoubleArrayElements;
        public IntPtr ReleaseBooleanArrayElements;
        public IntPtr ReleaseByteArrayElements;
        public IntPtr ReleaseCharArrayElements;
        public IntPtr ReleaseShortArrayElements;
        public IntPtr ReleaseIntArrayElements;
        public IntPtr ReleaseLongArrayElements;
        public IntPtr ReleaseFloatArrayElements;
        public IntPtr ReleaseDoubleArrayElements;
        public IntPtr GetBooleanArrayRegion;
        public IntPtr GetByteArrayRegion;
        public IntPtr GetCharArrayRegion;
        public IntPtr GetShortArrayRegion;
        public IntPtr GetIntArrayRegion;
        public IntPtr GetLongArrayRegion;
        public IntPtr GetFloatArrayRegion;
        public IntPtr GetDoubleArrayRegion;
        public IntPtr SetBooleanArrayRegion;
        public IntPtr SetByteArrayRegion;
        public IntPtr SetCharArrayRegion;
        public IntPtr SetShortArrayRegion;
        public IntPtr SetIntArrayRegion;
        public IntPtr SetLongArrayRegion;
        public IntPtr SetFloatArrayRegion;
        public IntPtr SetDoubleArrayRegion;
        public IntPtr RegisterNatives;
        public IntPtr UnregisterNatives;
        public IntPtr MonitorEnter;
        public IntPtr MonitorExit;
        public IntPtr GetJavaVM;
        
        // String Operations
        public IntPtr GetStringRegion;
        public IntPtr GetStringUTFRegion;
        
        // Array Operations
        public IntPtr GetPrimitiveArrayCritical;
        public IntPtr ReleasePrimitiveArrayCritical;

        // String Operations
        public IntPtr GetStringCritical;
        public IntPtr ReleaseStringCritical;
        
        // Weak Global References
        public IntPtr NewWeakGlobalRef;
        public IntPtr DeleteWeakGlobalRef;
        
        // Exceptions
        public IntPtr ExceptionCheck;
        
        // J2SDK1_4
        public IntPtr NewDirectByteBuffer;
        public IntPtr GetDirectBufferAddress;
        public IntPtr GetDirectBufferCapacity;
    }

    internal unsafe struct JNINativeInterface_
    {
        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr AllocObject(IntPtr EnvironmentHandle, IntPtr jniClass);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte CallBooleanMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodID, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte CallByteMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate ushort CallCharMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate double CallDoubleMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate float CallFloatMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int CallIntMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodID, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long CallLongMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr CallObjectMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate short CallShortMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void CallVoidMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte CallNonvirtualBooleanMethod(
            IntPtr obj, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte CallNonvirtualByteMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                        IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate ushort CallNonvirtualCharMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                          IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate double CallNonvirtualDoubleMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                            IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate float CallNonvirtualFloatMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                          IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int CallNonvirtualIntMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                      IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long CallNonvirtualLongMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                        IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr CallNonvirtualObjectMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                            IntPtr jMethodId, params JValue[] args
            );

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate short CallNonvirtualShortMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                          IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void CallNonvirtualVoidMethod(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jniClass,
                                                        IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte CallStaticBooleanMethod(IntPtr EnvironmentHandle, IntPtr jniClass,
                                                       IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte CallStaticByteMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate ushort CallStaticCharMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate double CallStaticDoubleMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate float CallStaticFloatMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int CallStaticIntMethod(
            IntPtr EnvironmentHandle, IntPtr obj, IntPtr jMethodID, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long CallStaticLongMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr CallStaticObjectMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate short CallStaticShortMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int CallStaticVoidMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodID, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr DefineClass(IntPtr EnvironmentHandle,
                                             IntPtr name, IntPtr loader,
                                             IntPtr buf, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        [SuppressUnmanagedCodeSecurity]
        internal delegate void DeleteGlobalRef(IntPtr EnvironmentHandle, IntPtr gref);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        [SuppressUnmanagedCodeSecurity]
        internal delegate void DeleteLocalRef(IntPtr EnvironmentHandle, IntPtr lref);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void DeleteWeakGlobalRef(IntPtr EnvironmentHandle, IntPtr wref);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int EnsureLocalCapacity(IntPtr EnvironmentHandle, int capacity);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte ExceptionCheck(IntPtr EnvironmentHandle);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ExceptionClear(IntPtr EnvironmentHandle);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ExceptionDescribe(IntPtr EnvironmentHandle);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr ExceptionOccurred(IntPtr EnvironmentHandle);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void FatalError(IntPtr EnvironmentHandle, IntPtr msg);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr FindClass(IntPtr EnvironmentHandle, [MarshalAs(UnmanagedType.LPStr)] string name);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetSuperclass(IntPtr EnvironmentHandle, IntPtr subclassHandle);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte IsAssignableFrom(IntPtr EnvironmentHandle, IntPtr subclassHandle, IntPtr superclassHandle
            );

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr FromReflectedField(IntPtr EnvironmentHandle, IntPtr field);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr FromReflectedMethod(IntPtr EnvironmentHandle, IntPtr method);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetArrayLength(IntPtr EnvironmentHandle, IntPtr array);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte* GetBooleanArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetBooleanArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, byte* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte GetBooleanField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte* GetByteArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetByteArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, byte* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte GetByteField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate ushort* GetCharArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetCharArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, char* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate ushort GetCharField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetDirectBufferAddress(IntPtr EnvironmentHandle, IntPtr buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long GetDirectBufferCapacity(IntPtr EnvironmentHandle, IntPtr buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate double* GetDoubleArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetDoubleArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, double* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate double GetDoubleField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetFieldID(
            IntPtr EnvironmentHandle, IntPtr jniClass, [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string sig);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate float* GetFloatArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetFloatArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, float* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate float GetFloatField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int* GetIntArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetIntArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, int* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetIntField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetJavaVM(IntPtr EnvironmentHandle, out IntPtr vm);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long* GetLongArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetLongArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, long* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long GetLongField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetMethodId(
            IntPtr EnvironmentHandle, IntPtr jniClass, [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string sig);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetObjectArrayElement(IntPtr EnvironmentHandle, IntPtr array, int index);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetObjectClass(IntPtr EnvironmentHandle, IntPtr obj);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetObjectField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void* GetPrimitiveArrayCritical(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate short* GetShortArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetShortArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, short* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate short GetShortField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte GetStaticBooleanField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte GetStaticByteField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate ushort GetStaticCharField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate double GetStaticDoubleField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetStaticFieldID(
            IntPtr EnvironmentHandle, IntPtr jniClass, [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string sig);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate float GetStaticFloatField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetStaticIntField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate long GetStaticLongField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetStaticMethodId(IntPtr EnvironmentHandle, IntPtr jniClass,
                                                   [MarshalAs(UnmanagedType.LPStr)] string name,
                                                   [MarshalAs(UnmanagedType.LPStr)] string sig);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetStaticObjectField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate short GetStaticShortField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetStringChars(IntPtr EnvironmentHandle, IntPtr str, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetStringCritical(IntPtr EnvironmentHandle, IntPtr str, byte* isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetStringLength(IntPtr EnvironmentHandle, IntPtr str);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetStringRegion(IntPtr EnvironmentHandle, IntPtr str, int start, int len, char* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr GetStringUTFChars(IntPtr EnvironmentHandle, IntPtr str, IntPtr isCopy);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetStringUTFLength(IntPtr EnvironmentHandle, IntPtr str);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void GetStringUTFRegion(IntPtr EnvironmentHandle, IntPtr str, int start, int len, char* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int GetVersion(IntPtr EnvironmentHandle);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate byte IsSameObject(IntPtr EnvironmentHandle, IntPtr o1, IntPtr o2);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int MonitorEnter(IntPtr EnvironmentHandle, IntPtr obj);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int MonitorExit(IntPtr EnvironmentHandle, IntPtr obj);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewBooleanArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewByteArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewCharArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewDirectByteBuffer(IntPtr EnvironmentHandle, IntPtr address, long capacity);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewDoubleArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewFloatArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        [SuppressUnmanagedCodeSecurity]
        internal delegate IntPtr NewGlobalRef(IntPtr EnvironmentHandle, IntPtr lobj);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewIntArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewLocalRef(IntPtr EnvironmentHandle, IntPtr reference);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewLongArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewObject(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, params JValue[] args);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewObjectArray(IntPtr EnvironmentHandle, int len, IntPtr jniClass, IntPtr init);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewShortArray(IntPtr EnvironmentHandle, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewString(
            IntPtr EnvironmentHandle, [MarshalAs(UnmanagedType.LPWStr)] string unicode, int len);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewStringUTF(IntPtr EnvironmentHandle, IntPtr utf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr NewWeakGlobalRef(IntPtr EnvironmentHandle, IntPtr obj);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr PopLocalFrame(IntPtr EnvironmentHandle, IntPtr result);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int PushLocalFrame(IntPtr EnvironmentHandle, int capacity);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int RegisterNatives(
            IntPtr EnvironmentHandle, IntPtr jniClass, JNINativeMethod* methods, int nMethods);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int UnregisterNatives(IntPtr EnvironmentHandle, IntPtr jniClass);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseBooleanArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* elems, int mode
            );

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseByteArrayElements(IntPtr EnvironmentHandle, IntPtr array, byte* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseCharArrayElements(IntPtr EnvironmentHandle, IntPtr array, ushort* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseDoubleArrayElements(
            IntPtr EnvironmentHandle, IntPtr array, double* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseFloatArrayElements(IntPtr EnvironmentHandle, IntPtr array, float* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseIntArrayElements(IntPtr EnvironmentHandle, IntPtr array, int* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseLongArrayElements(IntPtr EnvironmentHandle, IntPtr array, long* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleasePrimitiveArrayCritical(
            IntPtr EnvironmentHandle, IntPtr array, void* carray, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseShortArrayElements(IntPtr EnvironmentHandle, IntPtr array, short* elems, int mode);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseStringChars(IntPtr EnvironmentHandle, IntPtr str, IntPtr chars);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseStringCritical(IntPtr EnvironmentHandle, IntPtr str, IntPtr cstring);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void ReleaseStringUTFChars(IntPtr EnvironmentHandle, IntPtr str, IntPtr chars);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetBooleanArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, byte* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetBooleanField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, byte val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetByteArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, byte* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetByteField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, byte val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetCharArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, char* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetCharField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, ushort val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetDoubleArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, double* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetDoubleField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, double val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetFloatArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, float* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetFloatField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, float val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetIntArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, int* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetIntField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, int val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetLongArrayRegion(IntPtr EnvironmentHandle, IntPtr array, int start, int len, long* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetLongField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, long val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetObjectArrayElement(IntPtr EnvironmentHandle, IntPtr array, int index, IntPtr val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetObjectField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, IntPtr val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetShortArrayRegion(
            IntPtr EnvironmentHandle, IntPtr array, int start, int len, short* buf);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetShortField(IntPtr EnvironmentHandle, IntPtr obj, IntPtr jFieldId, short val);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticBooleanField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, byte value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticByteField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, byte value
            );

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticCharField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, ushort value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticDoubleField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, double value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticFloatField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, float value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticIntField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, int value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticLongField(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, long value
            );

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticObjectField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, IntPtr value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate void SetStaticShortField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, short value);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int Throw(IntPtr EnvironmentHandle, IntPtr obj);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int ThrowNew(IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr msg);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr ToReflectedField(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jFieldId, byte isStatic);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate IntPtr ToReflectedMethod(
            IntPtr EnvironmentHandle, IntPtr jniClass, IntPtr jMethodId, byte isStatic);

        [UnmanagedFunctionPointer(JavaVM.CC)]
        internal delegate int UnregisterJavaPtrs(IntPtr EnvironmentHandle, IntPtr jniClass);
    }
}