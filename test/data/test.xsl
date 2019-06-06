<!--CodeRef: CJW - hint from http://vim.wikia.com/wiki/Automatically_indent_an_XML_file_using_XSLT-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 <xsl:output method="xml" indent="yes"/>
 <xsl:strip-space elements="*"/>
 <xsl:template match="/node()/child::*[local-name()='element' and @name='Rows']//*[local-name()='element' and @name='Row']//*[local-name()='element']">
  <xsl:copy-of select="."/>
 </xsl:template>
</xsl:stylesheet>
