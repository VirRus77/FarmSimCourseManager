<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="xsl msxsl">

  <xsl:output method="xml" encoding="utf-8" indent="yes"/>
  <xsl:strip-space elements="*"/>

  <xsl:namespace-alias stylesheet-prefix="msxsl" result-prefix="xsl"/>

  <xsl:template match="@* | node()">
    <xsl:choose>
      <xsl:when test="name() = 'course'">
        <course>
          <xsl:apply-templates select="@*[not(name() = 'numWaypoints')]"/>
          <xsl:for-each select="waypoint">
            <xsl:variable name="name">
              <xsl:value-of select="'waypoint'"/>
              <xsl:value-of select="position()"/>
            </xsl:variable>
            <xsl:element name="{$name}">
              <xsl:apply-templates select="@*|node()" />
            </xsl:element>
          </xsl:for-each>
        </course>
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy>
          <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
