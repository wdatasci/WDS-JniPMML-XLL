/* Java >>> *
package com.WDataSci.JniPMML;

//xml imports

import com.WDataSci.WDS.WDSException;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathFactory;
import java.nio.file.Paths;

/* <<< Java */
/* C# >>> */

using System;
using System.Xml;

using static com.WDataSci.WDS.JavaLikeExtensions;

namespace com.WDataSci.JniPMML
{

    /* <<< C# */


    public class Util
    {


        public static String RecordSingleName(String arg)
        //throws com.WDataSci.WDS.WDSException
        {
            String rv = null;
            if ( arg.endsWith("Set") ) {
                rv = arg.substring(0, arg.length() - 3);
            }
            else if ( arg.endsWith("s") ) {
                rv = arg.substring(0, arg.length() - 1);
            }
            else
                throw new com.WDataSci.WDS.WDSException("RecordSingleName: Cannot identify usual name for a single row from RecordSet name (should end in \"Set\" or \"s\")");
            return rv;
        }

        /**
         * RecordSetElementName - Returns the name attribute of the RecordSet element of the InputSchema
         * <p>The RecordSet element of the input schema contains zero or more single rows of data fields.</p>
         * <p>Unless specified elsewhere, the convention used here is a singular term for a single row
         * (such as Record, Row, Vector, or Observation), and a plural form for the container of multiple rows
         * (such as RecordSet, Records, Rows or RowSet).</p>
         * <p>The schema of the data fields provides the cross map between the input data set and the PMML data dictionary.
         * The input fields can queried from the InputSchema under the assumption that the XPath query follows the concept:</p>
         * <blockquote>
         * <p>/RecordSet/Record/<i>Fields</i></p>
         * </blockquote>
         * <p>The RecordSetElementName function extracts the corresponding name from the InputSchema.</p>
         * <p>When not provided as an input, the convention used here is to have a schema which
         * is consistent with what can be easily used for the exportable XMLMap of a ListObject in Excel.</p>
         * <p>A simple form of such an  XML has the pattern:</p>
         * <p>&lt;RecordSet&gt;</p>
         * <blockquote>
         * &lt;Record&gt;
         * <blockquote>
         * <p>&lt;Field1&gt;<i>value1</i>&lt;/Field1&gt;</p>
         * <p>&lt;Field2&gt;<i>value2</i>&lt;/Field2&gt;</p>
         * <p>&lt;Field3&gt;<i>value3</i>&lt;/Field3&gt;</p>
         * </blockquote>
         * &lt;/Record&gt;
         * </blockquote>
         * <p>&lt;/RecordSet&gt;</p>
         * <p>However, the XMLSchema associated with a simple structure looks like:</p>
         * <p>&lt;?xml version="1.0"?&gt;</p>
         * <p>&lt;xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xi="http://www.w3.org/2001/XInclude" attributeFormDefault="unqualified" elementFormDefault="qualified"&gt;</p>
         * <blockquote>
         * <p>&lt;xs:element name="RecordSet"&gt;</p>
         * <blockquote>
         * &lt;xs:complexType&gt;&lt;xs:sequence&gt;
         * <blockquote>
         * &lt;xs:element name="Record" maxOccurs="unbounded"&gt;
         * <blockquote>
         * &lt;xs:complexType&gt;&lt;xs:sequence&gt;
         * <blockquote>
         * <p>&lt;xs:element name="Field1" type="?"/&gt;</p>
         * <p>&lt;xs:element name="Field2" type="?"/&gt;</p>
         * <p>&lt;xs:element name="Field3" type="?"/&gt;</p>
         * </blockquote>
         * &lt;/xs:sequence&gt;&lt;/xs:complexType&gt;
         * </blockquote>
         * &lt;/xs:element&gt;
         * </blockquote>
         * &lt;/xs:sequence&gt;&lt;/xs:complexType&gt;
         * </blockquote>
         * &lt;/xs:element&gt;
         * </blockquote>
         * <p>&lt;/xs:schema&gt;</p>
         */

        //Java public static String RecordSetElementName(Document xInputSchema)
        //C#
        public static String RecordSetElementName(XmlDocument xInputSchema)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( xInputSchema == null )
                throw new com.WDataSci.WDS.WDSException("Invalid XMLSchema input to RecordSetElementName used to extract value when not used as an input");
            /* Java >>> *
            XPath xPath = XPathFactory.newInstance().newXPath();
            NodeList xnl = null;
            try {
                String xpath_exp = "/node()/child::*[local-name()='element']";
                xnl = (NodeList) xPath.evaluate(xpath_exp, xInputSchema, XPathConstants.NODESET);
                if ( xnl.getLength() == 0 )
                    throw new com.WDataSci.WDS.WDSException("Without RecordSetElementName provided, cannot find any 'element' off of root in InputSchema");
                if ( xnl.getLength() > 1 )
                    throw new com.WDataSci.WDS.WDSException("Without RecordSetElementName provided, cannot find a single 'element' off of root in InputSchema");
                return ((Element) xnl.item(0)).getAttribute("name");
            } catch (Exception e) {
                throw new com.WDataSci.WDS.WDSException("Without RecordSetElementName provided, cannot find single 'element' off of root in InputSchema\n" + e.getStackTrace().toString());
            }
            /* <<< Java */
            /* C# >>> */
            XmlNodeList xnl = null;
            try {
                String xpath_exp = "/node()/child::*[local-name()='element']";
                xnl = (XmlNodeList) xInputSchema.SelectNodes(xpath_exp);
                if ( xnl.Count == 0 )
                    throw new com.WDataSci.WDS.WDSException("Without RecordSetElementName provided, cannot find any 'element' off of root in InputSchema");
                if ( xnl.Count > 1 )
                    throw new com.WDataSci.WDS.WDSException("Without RecordSetElementName provided, cannot find a single 'element' off of root in InputSchema");
                return xnl[0].SelectNodes("@name")[0].Value;
            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Without RecordSetElementName provided, cannot find single 'element' off of root in InputSchema\n", e);
            }
            /* <<< C# */
        }


    }

    /* C# >>> */
}
/* <<< C# */
