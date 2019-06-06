using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using ExcelDna.Registration;
using ExcelDna.Utilities;

namespace com.WDataSci.WDS
{
    public class WDSAddIn : IExcelAddIn
    {
        public void AutoOpen()
        {
            RegisterFunctions();
        }

        public void AutoClose()
        {
        }

        public void RegisterFunctions()
        {
            ExcelRegistration.GetExcelFunctions()
                            .ProcessParamsRegistrations()
                            .Select(UpdateHelpTopic)
                            .RegisterFunctions();
        }

        public ExcelFunctionRegistration UpdateHelpTopic(ExcelFunctionRegistration funcReg)
        {
            funcReg.FunctionAttribute.HelpTopic = "http://WDataSci.com";
            return funcReg;
        }
    }

}
