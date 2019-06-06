/* Java >>> */
package com.WDataSci.WDS;

import org.apache.commons.io.Charsets;
import org.apache.commons.io.FileUtils;

import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
/* <<< Java */
/* C# >>> *
using System;
using System.IO;
using System.Text.RegularExpressions;
 
using static com.WDataSci.WDS.JavaLikeExtensions;
namespace com.WDataSci.WDS {
/* <<< C# */

    public class Util
    {

        public static String FetchFileAsString(String arg)
        throws WDSException
        {
            String rv = "Error:";
            try {
                //Java
                assert (arg != null);
                if ( arg == null || arg.isEmpty() ) {
                    rv = "Error: Empty File Name";
                    return rv;
                }
                /* C# >>> *
                   rv = System.IO.File.ReadAllText(arg);
                /* <<< C# */
                /* Java >>> */
                try {
                    try {
                        rv = FileUtils.readFileToString(new File(arg), Charsets.toCharset("UTF8"));
                    }
                    catch ( IOException e ) {
                        rv = "Error: IOException: In JVM, cannot fetch file contents";
                        return rv;
                    }
                    catch ( Exception e ) {
                        rv = "Error: Exception: In JVM, cannot fetch file contents";
                        return rv;
                    }
                }
                catch ( AssertionError e ) {
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

        public static boolean bIn(String arg0, String... args)
        {
            if ( arg0 == null ) return false;
            //Java
            for ( String s : args )
            //C# foreach ( String s in args )
            {
                if ( arg0.equals(s) ) return true;
            }
            return false;
        }

        //overloading for nullity checks

        public static boolean MatchingNullity(String A, String B)
        {
            if ( A == null && B == null ) return true;
            if ( A != null && B != null ) return true;
            return false;
        }

        public static boolean MatchingNullityAndValueEquals(String A, String B)
        {
            if ( !MatchingNullity(A, B) ) return false;
            if ( A == null ) return true;
            if ( !A.equals(B) ) return false;
            return true;
        }

        public static boolean MatchingNullity(Object A, Object B)
        {
            if ( A == null && B == null ) return true;
            if ( A != null && B != null ) return true;
            return false;
        }

        /**
         * CleanAsNMToken returns a clean and valid NMToken (name token) string for a given input
         * <p>Following XMLSchema data types, a NMToken cannot contain single or double quotes, or commas.  These characters are stripped from the input
         * and any leading, trailing, or interior spaces are removed. The primary characters are Java Regular Expression <i>word</i> characters (A-Z_a-z0-9),
         * period, underscore, colon, and dash.  It technically also includes CombiningChars and Extenders, but the regular expression here does not implement those
         * in this version.</p>
         */
        public static String CleanAsNMToken(String arg)
        throws Exception
        {
            //Java
            String rv = arg.replaceAll( "^[^:A-Z_a-z\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02FF\\u0370-\\u037D\\u037F-\\u1FFF\\u200C-\\u200D\\u2070-\\u218F\\u2C00-\\u2FEF\\u3001-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFFD]+", "");
            //Java
            rv = rv.replaceAll("[^:A-Z_a-z\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02FF\\u0370-\\u037D\\u037F-\\u1FFF\\u200C-\\u200D\\u2070-\\u218F\\u2C00-\\u2FEF\\u3001-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFFD-.0-9\\u00B7\\u0300-\\u036F\\u203F-\\u2040]+", "");
            //C# String rv = Regex.Replace(arg, "^[^:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD]+", "");
            //C# rv = Regex.Replace(arg, "[^:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD-.0-9\u00B7\u0300-\u036F\u203F-\u2040]+", "");
            return rv;
        }

        /**
         * CleanAsToken returns a clean and valid XMLSchema string data type Token for a given input.
         * <p>Following XMLSchema data types, a Token does not have leading or trailing spaces, tabs, carriage returns,
         * linefeeds, and interior multiple space sequences are converted to single spaces.
         * CleanAsToken maps all non-printable characters to space before conversion.</p>
         */
        public static String CleanAsToken(String arg)
        throws Exception
        {
            //Java
            String rv = arg.replaceAll("[\00-\040]", " ").replaceAll("^\\s+", "").replaceAll("\\s+$", "").replaceAll("\\s{2,}?", " ");
            /* C# >>> *
            //String rv = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(arg,"[\000-\040]", " "),"^\\s+", ""),"\\s+$", ""),"\\s{2,}?", " ");
            String rv = Regex.Replace(arg, "[\u0000-\u001F]", " ");
            rv = Regex.Replace(rv, "^\\s+", "");
            rv = Regex.Replace(rv, "\\s+$", "");
            rv = Regex.Replace(rv,"\\s{2,}?", " ");
            /* <<< C# */
            return rv;
        }

        /**
         * CleanQuotes returns a string with double or single quotes removed.
         */
        public static String CleanQuotes(String arg)
        throws Exception
        {
            //Java
            String rv = arg.replaceAll("[\\\"\\\']", " ");
            //C# String rv = Regex.Replace(arg,"[\\\"\\\']", " ");
            return rv;
        }

        /**
         * CleanAsString (overloaded) allows the regular expression and the replaceAll target to be inputs (for testing)
         */
        public static String CleanAsString(String arg, String regex_exp, String regex_repl)
        throws Exception
        {
            //Java
            String rv = arg.replaceAll(regex_exp, regex_repl);
            //C# String rv = Regex.Replace(arg,regex_exp, regex_repl);
            return rv;
        }

        public static String BaseDirAndPath(String aBaseDir, String aPath)
        {
            String rv = null;
            if (aPath==null) return rv;
            //Java
            java.nio.file.Path p = null;
            if ( !( aBaseDir == null
                            || aBaseDir.isEmpty()
                            || aPath.startsWith("/")
                            || aPath.startsWith("\\")
                            || (aPath.length() > 2 && aBaseDir.substring(1, 1).equals(":"))
            ) ) {
                p = Paths.get(com.WDataSci.WDS.Util.PathAndName(aBaseDir, aPath));
                rv = p.normalize().toString();
            }
            else {
                p = Paths.get(aPath);
                rv = p.normalize().toString();
            }
            return rv;
        }

        public static String PathAndName(String aPath, String aFileName)
        {
            String rv = null;
            //Java
            java.nio.file.Path p = null;
            if ( aPath != null && !aPath.isEmpty() ) {
                //Java
                p = Paths.get(aPath);
                //Java
                rv = p.toString() + p.getFileSystem().getSeparator() + aFileName;
                //C# rv = System.IO.Path.Combine(aPath, aFileName);
            }
            else
                rv = new String(aFileName);
            return rv;
        }


    }
/* C# >>> *
}
/* <<< C# */
