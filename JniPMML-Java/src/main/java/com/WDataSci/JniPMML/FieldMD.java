/* Java >>> */
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.Util;
import org.dmg.pmml.FieldName;

/* <<< Java */
/* C# >>> *

using System;

using FieldName = com.WDataSci.JniPMML.FieldName;

using static com.WDataSci.WDS.JavaLikeExtensions;

namespace com.WDataSci.JniPMML
{
/* <<< C# */


    //Java 
    public class FieldMD extends FieldBaseMD implements FieldMDIKey<FieldName>
    //C# public class FieldMD : FieldBaseMD, FieldMDIKey<FieldName>
    {

        public FieldName MapKey = null;

        public FieldName MappedKey() { return this.MapKey; }

        public String MappedKeyValue() {
            if (this.hasMapKey())
                return this.MapKey.getValue();
            else
                return null;
        }

        public boolean hasMapKey()
        {
            return (this.MapKey != null);
        }

        public void MapToMapKey(FieldName aFieldName)
        throws com.WDataSci.WDS.WDSException
        {
            this.MapKey = new FieldName(new String(aFieldName.getValue()));
        }

        public FieldMD MapToMapKey(String aFieldStringName)
        {
            this.MapKey = new FieldName(new String(aFieldStringName));
            return this;
        }

        public FieldMD()
        //C# : base()
        {
            //Java
            super();
        }

        public FieldMD(FieldMD arg)
            //C# : base(arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            //Java
            super(arg);
            if ( arg.MapKey != null )
                this.MapKey = new FieldName(new String(arg.MapKey.getValue()));
        }

        /* Java >>> */
        public FieldMD(String Name, int hclass, int hlength, int horder, int hsign)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            super(Name, hclass, hlength, horder, hsign);
        }
        /* <<< Java */

        public boolean Equals(FieldMD arg)
        {
            //C# if ( !base.Equals(arg) ) return false;
            //Java
            if ( !super.Equals(arg) ) return false;
            if ( !Util.MatchingNullity(this.MapKey, arg.MapKey) ) return false;
            if ( this.MapKey != null && !this.MapKey.getValue().equals(arg.MapKey.getValue()) )
                return false;
            return true;
        }

        public void Copy(FieldMD arg)
        throws com.WDataSci.WDS.WDSException, Exception
        {
            //C# base.Copy(arg);
            //Java
            super.Copy(arg);
            if ( arg.MapKey == null ) this.MapKey = null;
            else this.MapKey = new FieldName(new String(arg.MapKey.getValue()));
        }

    }

/* C# >>> *
}
/* <<< C# */
