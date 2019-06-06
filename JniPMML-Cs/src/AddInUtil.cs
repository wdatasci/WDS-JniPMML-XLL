using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using MOIE=Microsoft.Office.Interop.Excel;
using MOTE = Microsoft.Office.Tools.Excel;

using com.WDataSci.JniPMML;

using com.WDataSci.WDS;

namespace WDataSci.JniPMML
{

    public partial class AddIn : IExcelAddIn
    {

        private static String __OptionalStringValue(object arg, String defv)
        {
            if ( arg == null ) return defv;
            if ( arg is ExcelDna.Integration.ExcelMissing ) return defv;
            if ( arg is ExcelDna.Integration.ExcelEmpty ) return defv;
            if ( arg is ExcelDna.Integration.ExcelError ) return defv;
            String rv = arg.ToString();
            return rv;
        }


        private static int __OptionalIntValue(object arg, int defv)
        {
            if ( arg == null ) return defv;
            if ( arg is ExcelDna.Integration.ExcelMissing ) return defv;
            if ( arg is ExcelDna.Integration.ExcelEmpty ) return defv;
            if ( arg is ExcelDna.Integration.ExcelError ) return defv;
            int rv = defv;
            try {
                rv = Convert.ToInt32(arg);
            }
            catch ( Exception ) {
                rv=defv;
            }
            return rv;
        }


    }
}
