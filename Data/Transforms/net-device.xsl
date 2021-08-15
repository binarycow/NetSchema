<?xml version='1.0' encoding='UTF-8'?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
            xmlns:nc="urn:ietf:params:xml:ns:netconf:base:1.0"
            xmlns:en="urn:ietf:params:xml:ns:netconf:notification:1.0" xmlns:dev="uri:example:net-device">
  <output method="text"/>
  <include href="jsonxsl-templates.xsl"/>
  <strip-space elements="*"/>
  <template name="nsuri-to-module">
    <param name="uri"/>
    <choose>
      <when test="$uri='uri:example:net-device'">
        <text>net-device</text>
      </when>
    </choose>
  </template>
  <template match="//nc:*/dev:device">
    <call-template name="container">
      <with-param name="level">1</with-param>
      <with-param name="nsid">net-device:</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:hostname">
    <call-template name="leaf">
      <with-param name="level">2</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:mgmt-ip">
    <call-template name="leaf">
      <with-param name="level">2</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:spanning-tree">
    <call-template name="container">
      <with-param name="level">2</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:spanning-tree/dev:mst">
    <call-template name="container">
      <with-param name="level">3</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:spanning-tree/dev:mst/dev:region-name">
    <call-template name="leaf">
      <with-param name="level">4</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:non-keyed-list">
    <call-template name="list">
      <with-param name="level">2</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:non-keyed-list/dev:foobar">
    <call-template name="container">
      <with-param name="level">4</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:non-keyed-list/dev:foobar/dev:sub-item">
    <call-template name="leaf">
      <with-param name="level">5</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:non-keyed-list/dev:test-item">
    <call-template name="leaf">
      <with-param name="level">4</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:interfaces">
    <call-template name="container">
      <with-param name="level">2</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:interfaces/dev:interface">
    <call-template name="list">
      <with-param name="level">3</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:interfaces/dev:interface/dev:name">
    <call-template name="leaf">
      <with-param name="level">5</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:interfaces/dev:interface/dev:enabled">
    <call-template name="leaf">
      <with-param name="level">5</with-param>
      <with-param name="type">unquoted</with-param>
    </call-template>
  </template>
  <template match="//nc:*/dev:device/dev:dns-server">
    <call-template name="leaf-list">
      <with-param name="level">2</with-param>
      <with-param name="type">string</with-param>
    </call-template>
  </template>
</stylesheet>