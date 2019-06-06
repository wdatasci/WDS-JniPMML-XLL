/* Java >>> *
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.Util;
/* <<< Java */
/* C# >>> */
using System;

using static com.WDataSci.WDS.JavaLikeExtensions;

namespace com.WDataSci.JniPMML
{
/* <<< C# */

public interface FieldMDIKey<T>
{
    //Java public 
    T MappedKey();
    //Java public 
    Boolean hasMapKey();
    //Java public 
    void MapToMapKey(T arg)
        //throws com.WDataSci.WDS.WDSException
        ;
}

/* C# >>> */
}
/* <<< C# */
