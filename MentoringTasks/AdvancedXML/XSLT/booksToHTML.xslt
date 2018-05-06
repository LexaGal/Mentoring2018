<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:x="http://library.by/catalog"
                exclude-result-prefixes="x">

  <xsl:param name="Date" select="''"/>

  <xsl:output method="html" indent="yes"/>

  <xsl:template match="x:catalog">

  </xsl:template>

  <xsl:template match="/">
    <html>
      <head>
        <style>
          table thead {
            font-weight: bold;
          }
          table td {
            border: 1px solid;
            padding: 5px;
          }
        </style>
      </head>
      <body>
        <xsl:element name="h3">
          <xsl:value-of select="$Date"/>
        </xsl:element>
        <xsl:element name="genres">
          <xsl:for-each select="//x:genre[not(text()=preceding::x:genre/text())]">
            <xsl:variable name="currentGenre" select="." />
            <h3>
              <xsl:value-of select="."/>
            </h3>
            <table>
              <thead>
                <tr>
                  <td>Author</td>
                  <td>Title</td>
                  <td>Publish Date</td>
                  <td>Registration Date</td>
                </tr>
              </thead>
              <tbody>
                <xsl:for-each select="//x:catalog/x:book[x:genre/text()=$currentGenre]">
                  <tr>
                    <td>
                      <xsl:value-of select="./x:author" />
                    </td>
                    <td>
                      <xsl:value-of select="./x:title" />
                    </td>
                    <td>
                      <xsl:value-of select="./x:publish_date" />
                    </td>
                    <td>
                      <xsl:value-of select="./x:registration_date" />
                    </td>
                  </tr>
                </xsl:for-each>
              </tbody>
            </table>
            <label>
              Total: <xsl:value-of select="count(//x:catalog/x:book[x:genre/text()=$currentGenre])" />
            </label>
          </xsl:for-each>
          <div>
            <h3>
              Books total: <xsl:value-of select="count(//x:catalog/x:book)" />
            </h3>
          </div>
        </xsl:element>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
