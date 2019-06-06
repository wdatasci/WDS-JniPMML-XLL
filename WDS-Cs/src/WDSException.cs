﻿using System;

namespace com.WDataSci.WDS
{
    public class WDSException : Exception //extends Exception
    {

        public String __Message = "unset message";

        public WDSException(String msg)
        {
            __Message = msg;
        }
        /* Java >>> *
        public WDSException(String msg, Throwable e)
        {
            StringWriter sw = new StringWriter();
            PrintWriter pw = new PrintWriter(sw);
            e.printStackTrace(pw);
            __Message = msg + "\nExceptionMessage:\n" + e.getMessage() + "\nStackTrace:\n" + sw.toString();
        }
        /* <<< Java */
        public WDSException(String msg, Exception e)
        {
            /* Java >>> *
            StringWriter sw = new StringWriter();
            PrintWriter pw = new PrintWriter(sw);
            e.printStackTrace(pw);
            __Message = msg + "\nExceptionMessage:\n" + e.getMessage() + "\nStackTrace:\n" + sw.toString();
            /* <<< Java */
            /* C# >>> */
            if ( e.InnerException != null )
                __Message = msg + "\nExceptionMessage:\n" + e.Message + "\n" + e.InnerException.Message + "\nStackTrace:\n" + e.InnerException.StackTrace;
            else
                __Message = msg + "\nExceptionMessage:\n" + e.Message + "\nStackTrace:\n" + e.StackTrace;
            /* <<< C# */
        }

        public WDSException(String msg, WDSException e)
        {
            /* Java >>> *
            StringWriter sw = new StringWriter();
            PrintWriter pw = new PrintWriter(sw);
            e.printStackTrace(pw);
            __Message = msg + "\nExceptionMessage:\n" + e.getMessage() + "\nStackTrace:\n" + sw.toString();
            /* <<< Java */
            /* C# >>> */
            if ( e.InnerException != null )
                __Message = msg + "\nExceptionMessage:\n" + e.getMessage() + "\n" + e.InnerException.Message + "\nStackTrace:\n" + e.InnerException.StackTrace;
            else
                __Message = msg + "\nExceptionMessage:\n" + e.getMessage();
            /* <<< C# */
        }

        //Java @Override
        public String getMessage() { return __Message; }

        public String toString()
        {
            return __Message;
        }

    }
}
