/* Java >>> *
package com.WDataSci.WDS;

import org.apache.commons.io.Charsets;
import org.apache.commons.io.FileUtils;

import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
/* <<< Java */
/* C# >>> */
using System;
using System.IO;
using System.Text.RegularExpressions;
using ExcelDna.Integration;
 
using static com.WDataSci.WDS.JavaLikeExtensions;

namespace com.WDataSci.WDS {
    /* <<< C# */

    public class Util {

        [ExcelFunction(
                Name = "FetchFileAsString"
                , Category = "WDS"
                , Description = "Pulls the contents of a file and returns as one string."
                , ExplicitRegistration = true
                )]
        public static String FetchFileAsString
        //throws WDSException
        (
            [ExcelArgument(Name = "FileName")] String arg
            )
        {
            String rv = "Error:";
            try {
                //Java assert (arg != null);
                if ( arg == null || arg.isEmpty() ) {
                    rv = "Error: Empty File Name";
                    return rv;
                }
                /* C# >>> */
                rv = System.IO.File.ReadAllText(arg);
                /* <<< C# */
                /* Java >>> *
                try {
                    try {
                        rv = FileUtils.readFileToString(new File(arg), Charsets.toCharset("UTF8"));
                    } catch (IOException e) {
                        rv = "Error: IOException: In JVM, cannot fetch file contents";
                        return rv;
                    } catch (Exception e) {
                        rv = "Error: Exception: In JVM, cannot fetch file contents";
                        return rv;
                    }
                } catch (AssertionError e) {
                    rv = "Error: Check Arguments AssertionError: " + e.getMessage();
                    return rv;
                }
                /* <<< Java */
            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in FetchFileAsString", e);
            }
            return rv;
        }

        /* C# ****
         * For this function in ExcelDna as part of just WDS-Cs.xll, for some reason,, this was fine and operated correctly.  But, there was 
         * a registration error message being thrown as part of JniPMML-Cs.  An equivalent function was added as part of WDS-VB.xll/JniPMML-VB.xll.
        [ExcelFunction(
                Name = "bIn"
                , Category = "WDS"
                , Description = "Returns true if first argument value is any of the optional arguments"
                , ExplicitRegistration = false
                , IsHidden =true
                )]
            //[ExcelArgument(Name = "PrimarySubject",Description = "Compares string value against all other arguments", AllowReference =false)]
            //[ExcelArgument(Name = "CompareValues",Description = "Compares each against the first agument", AllowReference =false)]
        ***** C# */
        public static Boolean bIn(String arg0, params String[] args) {
            if ( arg0 == null ) return false;
            //Java for (String s : args) 
            //C#
            foreach ( String s in args ) 
            {
                if ( arg0.equals(s) ) return true;
            }
            return false;
        }

        //overloading for nullity checks

        public static Boolean MatchingNullity(String A, String B) {
            if ( A == null && B == null ) return true;
            if ( A != null && B != null ) return true;
            return false;
        }

        public static Boolean MatchingNullityAndValueEquals(String A, String B) {
            if ( !MatchingNullity(A, B) ) return false;
            if ( A == null ) return true;
            if ( !A.equals(B) ) return false;
            return true;
        }

        public static Boolean MatchingNullity(Object A, Object B) {
            if ( A == null && B == null ) return true;
            if ( A != null && B != null ) return true;
            return false;
        }

        /**
         * CleanAsNMToken returns a clean and valid NMToken (name token) string for a given input, following XML 1.1, through \uFFFF.
         */
        [ExcelFunction(
                Name = "CleanAsNMToken"
                , Category = "WDS"
                , Description = "Returns a clean and valid \\i\\c* NMToken (name token) string for a given input, following XML 1.1, through \\uFFFF. Note use CleanAsNMTokenXSD where the first character is not treated differently."
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String CleanAsNMToken(
            [ExcelArgument(Name = "aInputString",
                Description ="A general string")] String arg
            )
        //throws Exception 
        {
            //from https://www.w3.org/TR/2006/REC-xml11-20060816/
            //NameStartChar::= ":" | [A - Z] | "_" | [a - z] | [#xC0-#xD6] | [#xD8-#xF6] | [#xF8-#x2FF] | [#x370-#x37D] | [#x37F-#x1FFF] | [#x200C-#x200D] | [#x2070-#x218F] | [#x2C00-#x2FEF] | [#x3001-#xD7FF] | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]
            //NameStartChar::= ":" | [A - Z] | "_" | [a - z] | [ latin without math                   ] | [ greek, cyrillic            ] | [#x200C-#x200D] | [#x2070-#x218F] | [#x2C00-#x2FEF] | [#x3001-#xD7FF] | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]
            //NameChar::= NameStartChar | "-" | "." | [0 - 9] | #xB7       | [#x0300-#x036F] | [#x203F-#x2040]
            //NameChar::= NameStartChar | "-" | "." | [0 - 9] | middle dot | [#x0300-#x036F] | [#x203F-#x2040]
            //Name::= NameStartChar(NameChar) *
            //Names::= Name(#x20 Name)*
            //Nmtoken::= (NameChar) +
            //Nmtokens::= Nmtoken(#x20 Nmtoken)*

            //Java String rv = arg.replaceAll("[^\\w&&[^-]&&[^._:]]", "");
            //C#
            String rv = arg.replaceAll("^[^:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD]+", "");
            //C#
            rv = rv.replaceAll("[^:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD-.0-9\u00B7\u0300-\u036F\u203F-\u2040]+", "");
            return rv;
        }

        /**
         * CleanAsNMTokenXSD returns a clean and valid NMToken (name token) string for a given input, following XML 1.1, through \uFFFF.
         */
        [ExcelFunction(
                Name = "CleanAsNMTokenXSD"
                , Category = "WDS"
                , Description = "Returns a clean and valid \\c* NMToken (name token) string for a given input, through \\uFFFF. Note use CleanAsNMTokenXSD where the first character is not treated differently."
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String CleanAsNMTokenXSD(
            [ExcelArgument(Name = "aInputString",
                Description ="A general string")] String arg
            )
        //throws Exception 
        {
            //C#
            String rv=arg.replaceAll("[^:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD-.0-9\u00B7\u0300-\u036F\u203F-\u2040]+", "");
            return rv;
        }

        /* C# >>> */
        [ExcelFunction(
                Name = "CleanStringWithRegex"
                , Category = "WDS"
                , Description = "Performs a C# eval of Regex.Replace(InputString,RegexToFind,RegexToReplaceWith)"
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String CleanStringWithRegex(
            [ExcelArgument(Name = "InputString", Description ="A general string")] String arg0
            , [ExcelArgument(Name = "RegexToFind", Description ="A Regex Expression")] String arg1
            , [ExcelArgument(Name = "RegexToReplaceWith", Description ="Replacement String")] String arg2
            )
            //throws Exception 
            {
            String rv = arg0.replaceAll(arg1, arg2);
            return rv;
        }
        /* <<< C# */

        [ExcelFunction(
                Name = "CleanAsToken"
                , Category = "WDS"
                , Description = "Returns a TOKEN of the input string where white-space is normalized.  Additionally, ascii non-printables are removed."
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String CleanAsToken(
            [ExcelArgument(Name = "aInputString",
                Description ="A general string")] String arg
            )
            //throws Exception 
            {
            //Java String rv = arg.replaceAll("[\00-\040]", " ").replaceAll("^\\s+", "").replaceAll("\\s+$", "").replaceAll("\\s{2,}?", " ");
            /* C# >>> */
            //String rv = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(arg,"[\000-\040]", " "),"^\\s+", ""),"\\s+$", ""),"\\s{2,}?", " ");
            String rv = Regex.Replace(arg, "[\u0000-\u001F]", " ");
            rv = Regex.Replace(rv, "^\\s+", "");
            rv = Regex.Replace(rv, "\\s+$", "");
            rv = Regex.Replace(rv,"\\s+"," ");
            /* <<< C# */
            return rv;
        }

        [ExcelFunction(
                Name = "CleanQuotes"
                , Category = "WDS"
                , Description = "Removes double or single quotes."
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String CleanQuotes(String arg)
            //throws Exception 
            {
            //Java String rv = arg.replaceAll("[\\\"\\\']", " ");
            //C#
            String rv = Regex.Replace(arg,"[\\\"\\\']", " ");
            return rv;
        }

        [ExcelFunction(
                Name = "CleanDeadWhiteSpaceInXML"
                , Category = "WDS"
                , Description = "Removes inter-element space and non-printables in XML"
                , IsVolatile = false
                , ExplicitRegistration = true
                )]
        public static String CleanDeadWhiteSpaceInXML(
            [ExcelArgument(Name = "aInputString",
                Description ="A general string")] String arg
            )
            {
            String rv = Regex.Replace(arg, ">[\u0000-\u001F]+<", "><");
            rv = Regex.Replace(rv, "^\\s+", "");
            rv = Regex.Replace(rv, "\\s+$", "");
            return rv;
        }

        public static String PathAndName(String aPath, String aFileName) {
            String rv = null;
            //Java java.nio.file.Path p = null;
            if ( aPath != null && !aPath.isEmpty() ) {
                //Java p = Paths.get(aPath);
                //Java rv = p.toString() + p.getFileSystem().getSeparator() + aFileName;
                //C#
                rv = System.IO.Path.Combine(aPath, aFileName);
            }
            else
                rv = new_String(aFileName);
            return rv;
        }

    }
    /* C# >>> */
}
/* <<< C# */
