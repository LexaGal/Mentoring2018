<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:x="http://library.by/catalog"
  xmlns:ext="http://library.by/ext"
  exclude-result-prefixes="x">

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="x:catalog">
    <rss version="2.0">
      <channel>
        <title>RSS books</title>
        <xsl:apply-templates/>
      </channel>
    </rss>
  </xsl:template>

  <xsl:template match="x:book">
    <xsl:element name="item">
      <xsl:element name="title">
        <xsl:value-of select="x:title"/>
      </xsl:element>
      <xsl:element name="pubDate">
        <xsl:value-of select="ext:GetPubDate(x:registration_date)"/>
      </xsl:element>
      <xsl:if test="(x:genre = 'Computer') and (boolean(./x:isbn))">
        <xsl:element name="link">
          <xsl:value-of select="concat('http://my.safaribooksonline.com/', x:isbn)"/>
        </xsl:element>
      </xsl:if>
    </xsl:element>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
