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
                        //C# XmlNode wds_StringMaxLength = xn.Attributes.GetNamedItem("wds:StringMaxLength");
                        //Java 
                        String wds_StringMaxLength = xn.getAttribute("wds:StringMaxLength");
                        if ( wds_StringMaxLength != null ) {
                            try {
                                //C# cm.StringMaxLength = int.Parse(wds_StringMaxLength.Value);
                                //Java 
                                cm.StringMaxLength = Integer.parseInt(wds_StringMaxLength);
                            } catch ( Exception e) {
                                    cm.StringMaxLength = FieldMD.Default.StringMaxLength;
                            }
                        }
                        else {
                            /* Java >>> */
                            if (xn.hasAttributes()) {
                                for ( int ani = 0; ani < xn.getAttributes().getLength(); ani++ ) {
                                    if ( xn.getAttributes().item(ani).getNodeName().toLowerCase().endsWith("maxlength") ) {
                                        cm.StringMaxLength = Integer.parseInt(xn.getAttributes().item(ani).getNodeValue());
                                        typl[0] = 0;
                                        break;
                                    }
                                }
                            }
                            /* <<< Java */
                            /* C# >>> *
                            for (int ani=0 ;ani<xn.Attributes.Count ; ani++ ) {
                                if ( xn.Attributes.Item(ani).LocalName.toLowerCase().endsWith("maxlength") ) {
                                    cm.StringMaxLength = int.Parse(xn.Attributes.Item(ani).Value);
                                    typl[0] = 0;
                                    break;
                                }
                            }
                            /* <<< C# */
                            if ( typl[0] < 0 ) {
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
                        }
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
                    + " xmlns:wds=\"http://WDataSci.com\" \n"
                    + " attributeFormDefault=\"unqualified\" \n"
                    + " elementFormDefault=\"qualified\">\n";
            /* <<< Java */
            /* C# >>> *
            String rv = @"<?xml version=""1.0""?>
                        <xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"" 
                            xmlns:xi=""http://www.w3.org/2001/XInclude"" 
                            xmlns:wds=""http://WDataSci.com""
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
                    + "<xs:simpleType name=\"Str3\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"3\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str4\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str5\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"5\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str6\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"6\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str7\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"7\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str8\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"8\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str9\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"9\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str10\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"10\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str11\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"11\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str12\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"12\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str13\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"13\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str14\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"14\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str15\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"15\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str16\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"16\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str17\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"17\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str18\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"18\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str19\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"19\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str20\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"20\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str21\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"21\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str22\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"22\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str23\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"23\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str24\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"24\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str25\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"25\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str26\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"26\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str27\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"27\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str28\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"28\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str29\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"29\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str30\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"30\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str31\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"31\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str32\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"32\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str33\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"33\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str34\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"34\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str35\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"35\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str36\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"36\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str37\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"37\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str38\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"38\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str39\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"39\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str40\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"40\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str64\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"64\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str128\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"128\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str256\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"256\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str512\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"512\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str1024\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"1024\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"Str\">\n"
                    + "    <xs:union memberTypes=\"Str1 Str2 Str3 Str4 Str5 Str6 Str7 Str8 Str9 Str10 Str11 Str12 Str13 Str14 Str15 Str16 Str17 Str18 Str19 Str20 Str21 Str22 Str23 Str24 Str25 Str26 Str27 Str28 Str29 Str30 Str31 Str32 Str33 Str34 Str35 Str36 Str37 Str38 Str39 Str40 Str64 Str128 Str256 Str512 Str1024\"/>\n"
                    + "</xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS1\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"1\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS2\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"2\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS3\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"3\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS4\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS5\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"5\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS6\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"6\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS7\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"7\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS8\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"8\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS9\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"9\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS10\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"10\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS11\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"11\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS12\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"12\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS13\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"13\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS14\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"14\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS15\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"15\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS16\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"16\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS17\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"17\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS18\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"18\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS19\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"19\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS20\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"20\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS21\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"21\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS22\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"22\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS23\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"23\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS24\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"24\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS25\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"25\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS26\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"26\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS27\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"27\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS28\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"28\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS29\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"29\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS30\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"30\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS31\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"31\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS32\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"32\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS33\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"33\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS34\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"34\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS35\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"35\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS36\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"36\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS37\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"37\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS38\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"38\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS39\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"39\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS40\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"40\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS64\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"64\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS128\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"128\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS256\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"256\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS512\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"512\"/></xs:restriction></xs:simpleType>\n"
                    + "<xs:simpleType name=\"VLS1024\"><xs:restriction base=\"xs:string\"><xs:whiteSpace value=\"collapse\"/><xs:maxLength value=\"1024\"/></xs:restriction></xs:simpleType>\n"
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
            <xs:simpleType name=""Str3""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""3""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str4""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""4""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str5""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""5""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str6""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""6""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str7""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""7""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str8""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""8""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str9""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""9""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str10""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""10""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str11""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""11""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str12""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""12""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str13""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""13""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str14""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""14""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str15""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""15""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str16""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""16""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str17""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""17""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str18""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""18""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str19""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""19""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str20""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""20""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str21""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""21""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str22""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""22""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str23""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""23""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str24""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""24""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str25""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""25""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str26""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""26""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str27""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""27""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str28""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""28""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str29""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""29""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str30""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""30""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str31""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""31""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str32""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""32""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str33""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""33""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str34""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""34""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str35""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""35""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str36""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""36""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str37""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""37""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str38""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""38""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str39""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""39""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str40""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""40""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str64""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""64""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str128""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""128""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str256""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""256""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str512""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""512""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str1024""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""1024""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""Str"">
                <xs:union memberTypes=""Str1 Str2 Str3 Str4 Str5 Str6 Str7 Str8 Str9 Str10 Str11 Str12 Str13 Str14 Str15 Str16 Str17 Str18 Str19 Str20 Str21 Str22 Str23 Str24 Str25 Str26 Str27 Str28 Str29 Str30 Str31 Str32 Str33 Str34 Str35 Str36 Str37 Str38 Str39 Str40 Str64 Str128 Str256 Str512 Str1024""/>
            </xs:simpleType>
            <xs:simpleType name=""VLS1""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""1""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS2""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""2""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS3""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""3""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS4""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""4""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS5""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""5""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS6""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""6""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS7""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""7""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS8""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""8""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS9""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""9""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS10""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""10""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS11""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""11""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS12""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""12""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS13""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""13""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS14""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""14""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS15""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""15""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS16""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""16""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS17""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""17""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS18""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""18""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS19""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""19""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS20""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""20""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS21""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""21""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS22""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""22""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS23""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""23""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS24""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""24""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS25""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""25""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS26""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""26""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS27""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""27""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS28""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""28""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS29""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""29""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS30""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""30""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS31""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""31""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS32""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""32""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS33""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""33""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS34""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""34""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS35""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""35""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS36""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""36""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS37""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""37""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS38""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""38""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS39""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""39""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS40""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""40""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS64""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""64""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS128""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""128""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS256""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""256""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS512""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""512""/></xs:restriction></xs:simpleType>
            <xs:simpleType name=""VLS1024""><xs:restriction base=""xs:string""><xs:whiteSpace value=""collapse""/><xs:maxLength value=""1024""/></xs:restriction></xs:simpleType>
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
