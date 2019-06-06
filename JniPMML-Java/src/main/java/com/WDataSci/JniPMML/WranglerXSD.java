/* Java >>> */
package com.WDataSci.JniPMML;

import com.WDataSci.WDS.WDSException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathFactory;
import java.io.PrintWriter;
/* <<< Java */
/* C# >>> *
using System;
using System.Xml;

using com.WDataSci.WDS;

namespace com.WDataSci.JniPMML
{
/* <<< C# */


    public class WranglerXSD
    {

        public void mReadMapFor(RecordSetMD aRecordSetMD, JniPMMLItem aJniPMMLItem, PrintWriter pw, boolean bFillDictionaryNames)
        throws WDSException
        {

            try {

                int i = -1;
                int ii = -1;
                int j = -1;
                int jj = -1;
                int k = -1;
                int kk = -1;


                //are we using a JniPMML object (as when called from C# and does it have PMMLMatter
                boolean bUsingJniPMML = (aJniPMMLItem != null);
                boolean bCheckingAgainstPMML = (aJniPMMLItem != null && aJniPMMLItem.PMMLMatter.Doc != null);

                String[] lFieldStringNames = null;
                int nDataFieldNames = 0;

                if ( bCheckingAgainstPMML ) {
                    lFieldStringNames = aJniPMMLItem.PMMLDataFieldStringNames();
                    nDataFieldNames = lFieldStringNames.length;
                }


                if ( aRecordSetMD.SchemaMatter == null )
                    throw new com.WDataSci.WDS.WDSException("Error, aRecordSetMD.SchemaMatter not populated");

                //if using JniPMML, does it already have InputSchema Matter
                if ( aRecordSetMD.SchemaMatter.InputSchemaFileName == null && bUsingJniPMML && aJniPMMLItem.InputMatter._XSDFileName != null )
                    aRecordSetMD.SchemaMatter.InputSchemaFileName = aJniPMMLItem.InputMatter._XSDFileName;
                if ( aRecordSetMD.SchemaMatter.InputSchemaString == null && bUsingJniPMML && aJniPMMLItem.InputMatter._XSDString != null )
                    aRecordSetMD.SchemaMatter.InputSchemaString = aJniPMMLItem.InputMatter._XSDString;
                if ( aRecordSetMD.SchemaMatter.InputSchema == null && bUsingJniPMML && aJniPMMLItem.InputMatter._XSDDoc != null )
                    aRecordSetMD.SchemaMatter.InputSchema = aJniPMMLItem.InputMatter._XSDDoc;

                //else go get it
                if ( aRecordSetMD.SchemaMatter.InputSchema == null ) {
                    if ( aRecordSetMD.SchemaMatter.InputSchemaString == null ) {
                        if ( aRecordSetMD.SchemaMatter.InputSchemaFileName == null )
                            throw new com.WDataSci.WDS.WDSException("Error, InputMap XSD not provided or valid!");
                        aRecordSetMD.SchemaMatter.InputSchemaString = com.WDataSci.WDS.Util.FetchFileAsString(aRecordSetMD.SchemaMatter.InputSchemaFileName);
                    }
                    /* Java >>> */
                    if ( bUsingJniPMML )
                        aRecordSetMD.SchemaMatter.InputSchema = aJniPMMLItem.mReadMapFromXSDString(aRecordSetMD.SchemaMatter.InputSchemaString);
                    else
                        aRecordSetMD.SchemaMatter.InputSchema = new JniPMMLItem().mReadMapFromXSDString(aRecordSetMD.SchemaMatter.InputSchemaString);
                    /* <<< Java */
                    /* C# >>> *
                    aRecordSetMD.SchemaMatter.InputSchema = new XmlDocument();
                    aRecordSetMD.SchemaMatter.InputSchema.LoadXml(aRecordSetMD.SchemaMatter.InputSchemaString);
                    /* <<< C# */
                }

                this.mReadMapFor(aRecordSetMD.SchemaMatter.InputSchema, aRecordSetMD, aJniPMMLItem, pw, bFillDictionaryNames);

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error mapping input columns:", e);
            }
        }


        //Java
        public void mReadMapFor(Document aDoc, RecordSetMD aRecordSetMD, JniPMMLItem aJniPMMLItem, PrintWriter pw, Boolean bFillDictionaryNames)
        //C# public void mReadMapFor(XmlDocument aDoc, RecordSetMD aRecordSetMD, JniPMMLItem aJniPMMLItem, PrintWriter pw, Boolean bFillDictionaryNames)
        throws com.WDataSci.WDS.WDSException
        {

            try {

                int jj = -1;
                int j = -1;


                //are we using a JniPMML object (as when called from C# and does it have PMMLMatter
                Boolean bUsingJniPMML = (aJniPMMLItem != null);
                Boolean bCheckingAgainstPMML = (aJniPMMLItem != null && aJniPMMLItem.PMMLMatter.Doc != null);

                String[] lFieldStringNames = null;
                int nDataFieldNames = 0;

                if ( bCheckingAgainstPMML ) {
                    lFieldStringNames = aJniPMMLItem.PMMLDataFieldStringNames();
                    nDataFieldNames = lFieldStringNames.length;
                }


                if ( aRecordSetMD.SchemaMatter == null )
                    throw new com.WDataSci.WDS.WDSException("Error, aRecordSetMD.SchemaMatter not populated");

                String rns = aRecordSetMD.SchemaMatter.RecordSetElementName;
                String rn = aRecordSetMD.SchemaMatter.RecordElementName;

                if ( rns == null || rns.isEmpty() )
                    rns = Util.RecordSetElementName(aRecordSetMD.SchemaMatter.InputSchema);
                if ( rn == null || rn.isEmpty() ) rn = Util.RecordSingleName(rns);
                if ( rns.equals(rn) )
                    throw new com.WDataSci.WDS.WDSException("Error, RecordSetMD needs a valid RecordSetElementName, it's singular version, and they cannot be equal");

                if ( aRecordSetMD.SchemaMatter.RecordSetElementName == null )
                    aRecordSetMD.SchemaMatter.RecordSetElementName = rns;
                if ( aRecordSetMD.SchemaMatter.RecordElementName == null )
                    aRecordSetMD.SchemaMatter.RecordElementName = rn;

                String xPathQ = "/node()/child::*[local-name()='element' and @name='" + rns + "']" +
                        "//*[local-name()='element' and @name='" + rn + "']" +
                        "//*[local-name()='element']";
                //Java 
                XPath xPath = XPathFactory.newInstance().newXPath();
                //Java 
                NodeList xnl = (NodeList) xPath.evaluate(xPathQ, aDoc, XPathConstants.NODESET);
                //C# XmlNodeList xnl=aDoc.SelectNodes(xPathQ);

                int xnlLength = xnl.getLength();

                aRecordSetMD.Column = new FieldMD[xnlLength];

                //for when the Length is packed into the XSD type for limiting strings
                int[] typl = new int[1];
                for ( jj = 0; jj < xnlLength; jj++ ) {

                    aRecordSetMD.Column[jj] = new FieldMD();
                    FieldMD cm = aRecordSetMD.Column[jj];

                    //Get the XML/XSD element
                    //Java
                    Element xn = (Element) xnl.item(jj);
                    //C# XmlNode xn=xnl[jj];

                    //Get the name and initialize the mapped names
                    //Java
                    cm.Name = xn.getAttribute("name");
                    //C# cm.Name = xn.Attributes.GetNamedItem("name").Value;

                    //Get the XML/XSD - type
                    //Java
                    String typ = xn.getAttribute("type");
                    //C# String typ = xn.Attributes.GetNamedItem("type").Value;
                    //Java
                    cm.DTyp = FieldMDEnums.eDTyp.FromAlias(typ, typl);
                    //C# cm.DTyp = FieldMDExt.eDTyp_FromAlias(typ, ref typl);
                    if ( typl[0] > 0 )
                        cm.StringMaxLength = typl[0];
                    else if ( typl[0] < 0 ) {
                        xPathQ = "//*[local-name()='simpleType' and @name='" + typ + "']//*[local-name()='maxLength']";
                        //Java
                        NodeList xnr = (NodeList) xPath.evaluate(xPathQ, aJniPMMLItem.InputMatter._XSDDoc, XPathConstants.NODESET);
                        //C# XmlNodeList xnr = aDoc.SelectNodes(xPathQ);
                        if ( xnr.getLength() == 1 ) {
                            try {
                                //Java
                                cm.StringMaxLength = Integer.parseInt(((Element) xnr.item(0)).getAttribute("value"));
                                //Cs cm.StringMaxLength = int.Parse(xnr[0].Attributes.GetNamedItem("value").Value);
                            }
                            catch ( Exception e ) {
                                cm.StringMaxLength = FieldMD.Default.StringMaxLength;
                            }
                        }
                        else
                            cm.StringMaxLength = FieldMD.Default.StringMaxLength;
                    }

                    if ( bCheckingAgainstPMML ) {
                        //Search for PMML DataFieldName map
                        for ( j = 0; j < nDataFieldNames; j++ ) {
                            if ( cm.Name.equals(lFieldStringNames[j]) ) {
                                cm.MapToMapKey(lFieldStringNames[j]);
                                break;
                            }
                        }
                    }
                    else if ( bFillDictionaryNames ) {
                        cm.MapToMapKey(cm.Name);
                    }

                }

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error mapping input columns:", e);
            }
        }

        public static String XSDHeader()
        {
            /* Java >>> */
            String rv = "<?xml version=\"1.0\"?>\n"
                    + "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" \n"
                    + " xmlns:xi=\"http://www.w3.org/2001/XInclude\" \n"
                    + " attributeFormDefault=\"unqualified\" \n"
                    + " elementFormDefault=\"qualified\">\n";
            /* <<< Java */
            /* C# >>> *
            String rv = @"<?xml version=""1.0""?>
                        <xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"" 
                            xmlns:xi=""http://www.w3.org/2001/XInclude"" 
                            attributeFormDefault=""unqualified"" 
                            elementFormDefault=""qualified"">";
            /* <<< C# */
            return rv;
        }

        public static String XSDRecordSet_Open(String rns, String rn)
        {
            //Java
            String rv = "\n<xs:element name=" + '"' + rns.trim() + '"' + "><xs:complexType><xs:sequence><xs:element name=" + '"' + rn.trim() + '"' + " maxOccurs=\"unbounded\"><xs:complexType><xs:sequence>";
            //C# String rv = "\n<xs:element name=" + '"' + rns.Trim() + '"' + "><xs:complexType><xs:sequence><xs:element name=" + '"' + rn.Trim() + '"' + " maxOccurs=\"unbounded\"><xs:complexType><xs:sequence>";
            return rv;
        }

        public static String XSDRecordSet_Close()
        {
            String rv = "</xs:sequence></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element>";
            return rv;
        }

        public static String XSDFooter()
        {
            String rv = "\n</xs:schema>";
            return rv;
        }

        public static String XSDColumn(String name, String dtyp)
        {
            String rv = "<xs:element name=" + '"' + name + '"' + " type=" + '"' + dtyp + '"' + "/>";
            return rv;
        }

        public static String XSDTypes()
        {
            /* Java >>> */
            String rv = "\n"
                    + "<xs:simpleType name=\"Nbr\">\n"
                    + "    <xs:union memberTypes=\"xs:decimal xs:integer xs:negativeInteger xs:nonNegativeInteger xs:positiveInteger xs:nonPositiveInteger xs:long xs:int xs:short xs:byte xs:unsignedLong xs:unsignedShort xs:unsignedInt xs:unsignedByte xs:float xs:double\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"Dbl\">\n"
                    + "    <xs:union memberTypes=\"xs:decimal xs:long xs:int xs:short xs:byte xs:unsignedLong xs:unsignedShort xs:unsignedInt xs:unsignedByte xs:float xs:double\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"Lng\">\n"
                    + "    <xs:union memberTypes=\"xs:long\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"Int\">\n"
                    + "    <xs:union memberTypes=\"xs:integer xs:negativeInteger xs:nonNegativeInteger xs:positiveInteger xs:nonPositiveInteger xs:int xs:short xs:unsignedShort xs:unsignedInt\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS\">\n"
                    + "    <xs:union memberTypes=\"xs:token xs:NMTOKEN xs:normalizedString xs:string\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str1\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"1\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str2\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"2\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str4\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str8\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"8\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str16\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"16\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str32\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"32\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str64\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"64\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str128\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"128\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str256\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"256\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str512\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"512\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str1024\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"1024\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str\">\n"
                    + "    <xs:union memberTypes=\"Str1 Str2 Str4 Str8 Str16 Str32 Str64 Str128 Str512 Str1024\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"Dbl_List\"><xs:list itemType=\"Dbl\"/></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Int_List\"><xs:list itemType=\"Int\"/></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Lng_List\"><xs:list itemType=\"Lng\"/></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS_List\"><xs:list itemType=\"VLS\"/></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str_List\"><xs:list itemType=\"Str\"/></xs:simpleType> \n";
            /* <<< Java */
            /* C# >>> *
            String rv = @"   
            <xs:simpleType name=""Nbr"">
                <xs:union memberTypes=""xs:decimal xs:integer xs:negativeInteger xs:nonNegativeInteger xs:positiveInteger xs:nonPositiveInteger xs:long xs:int xs:short xs:byte xs:unsignedLong xs:unsignedShort xs:unsignedInt xs:unsignedByte xs:float xs:double""/>
            </xs:simpleType>
            <xs:simpleType name=""Dbl"">
                <xs:union memberTypes=""xs:decimal xs:long xs:int xs:short xs:byte xs:unsignedLong xs:unsignedShort xs:unsignedInt xs:unsignedByte xs:float xs:double""/>
            </xs:simpleType>
            <xs:simpleType name=""Lng"">
                <xs:union memberTypes=""xs:long""/>
            </xs:simpleType>
            <xs:simpleType name=""Int"">
                <xs:union memberTypes=""xs:integer xs:negativeInteger xs:nonNegativeInteger xs:positiveInteger xs:nonPositiveInteger xs:int xs:short xs:unsignedShort xs:unsignedInt""/>
            </xs:simpleType>
            <xs:simpleType name=""VLS"">
                <xs:union memberTypes=""xs:token xs:NMTOKEN xs:normalizedString xs:string""/>
            </xs:simpleType>
            <xs:simpleType name=""Str1""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""1""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str2""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""2""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str4""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""4""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str8""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""8""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str16""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""16""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str32""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""32""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str64""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""64""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str128""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""128""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str256""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""256""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str512""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""512""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str1024""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""1024""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str"">
                <xs:union memberTypes=""Str1 Str2 Str4 Str8 Str16 Str32 Str64 Str128 Str512 Str1024""/>
            </xs:simpleType>
            <xs:simpleType name=""Dbl_List""><xs:list itemType=""Dbl""/></xs:simpleType>
            <xs:simpleType name=""Int_List""><xs:list itemType=""Int""/></xs:simpleType>
            <xs:simpleType name=""Lng_List""><xs:list itemType=""Lng""/></xs:simpleType>
            <xs:simpleType name=""VLS_List""><xs:list itemType=""VLS""/></xs:simpleType>
            <xs:simpleType name=""Str_List""><xs:list itemType=""Str""/></xs:simpleType> 
            ";
            /* <<< C# */
            return rv;
        }

    }

/* C# >>> *
}
/* <<< C# */
