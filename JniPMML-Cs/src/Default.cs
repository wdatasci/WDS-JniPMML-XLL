/* Java >>> *
package com.WDataSci.JniPMML;
/* <<< Java */
/* C# >>> */
using System;

namespace com.WDataSci.JniPMML 
{
/* <<< C# */

public class Default
{
    /* Java >>> *
    public static final boolean ISJAVA = true;
    public static final boolean ISCSHARP = false;
    public final static int HeaderStringMaxLength = 64;  //default value for column/field name lengths
    public final static int StringMaxLength = 64;        //default value for strings, fixed length or variable
    public final static Boolean anyVLenRead = true;
    public final static Boolean anyVLenWrite = false;
    /* <<< Java */

    /* C# >>> */
    public static Boolean ISJAVA = false;
    public static Boolean ISCSHARP = true;
    public static int HeaderStringMaxLength = 64;  //default value for column/field name lengths
    public static int StringMaxLength = 64;        //default value for strings, fixed length or variable
    public static Boolean anyVLenRead = true;
    public static Boolean anyVLenWrite = true;
    /* <<< C# */
}
/* C# >>> */
}
/* <<< C# */
