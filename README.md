# General ReadMe

The primary objective of WDS-JniPMML-XLL is to provide model evaluators to Excel.  In particular, access to the standard PMML evaluator is
a starting point, both for use and/or for comparison.  Later versions will be include other model specs and implement other evaluators.

Please see documentation articles for a brief introduction on use.

Other evaluators aside, there is a technical challenge to providing access to the standard PMML evaluator, jpmml, which is cross-programing 
languages.  Under the hood, to create a fast efficient Excel interface that insulates the user from the technical details, the usual addin 
languages (C#/VB/VBA) must take data from the workbook (in multiple columns and possibly multiple rows), transform it, call the jpmml in Java, 
and then return the appropriate data (with possibly multiple columns and rows) back to the workbook.

This effort does not preclude writing a PMML evaluator (or re-writing jpmml, which may be a good idea) in another language.  However, as
other evalutaors as added, a common Excel based interface than provides a basis for comparison.



## Through this version, WDS-JniPMML-XLL provides:
<ul>
<li>A pair of Excel AddIns (XLLs) and VBA support for:</li>
    <ul>
    <li>Evaluating PMML models</li>
        <ul>
        <li>As an Excel function call</li>
        <li>Using the <i>de facto</i> standard implementation, jpmml.evaluator</li>
        <li>Using input data from an in-worksheet table</li>
            <i>Uses XmlMap'd exportable ListObjects, but provides tools to facilitate</i>
        <li>Can evaluate one or multiple observations (rows) per call</li>
        <li>Results returned as normal function outputs</li>
        <li>With cacheable models for efficiency</li>
        </ul>
    <li>Additional data wrangling tools for</li>
        <ul>
        <li>Importing/Exporting HDF5 compound datasets</li>
        <li>Importing/Exporting flat files</li>
        </ul>
    <li>Additional VBA module handling</li>
    </ul>
<li>A Java wrapper of jpmml.evaluator</li>
    <ul>
    <li>Callable from the XLL via jni</li>
    <li>Testable as a standalone from the command line</li>
            <i>But, can be called through the Excel AddIn using the JVM.</i>
    <li>Input and output data can be:</li>
        <ul>
            <li>HDF5 compound datasets</li>
            <li>Flat files</li>
            <li>In memory (as when called through jni)</li>
        </ul>
    </ul>
<li>A launch script and examples are included</li>
    <ul>
    <li>WDS-JniPMML-XLL-Launch.bat:  a script for launching a new Excel instance, running the AddIns without installing</li>
    <li>WDS-JniPMML-XLL-Test-Launch.bat:  a script for running the AddIns and the example workbook WDS-JniPMML-XLL-Test.xlsm</li>
    <li>test/data: A test set of the usual PMML cases</li>
    </ul>
</ul>

## A Few Project Organization Notes
<ul>
<li>JniPMML-[AAA]: Language specific libraries that directly related to project objectives</li>
   <ul>
   <li> Where-ever-possible and as-close-as-possible code naming conventions and structure is kept similar across languages.</li>
   </ul>
<li>WDS-[AAA]: Language specific utility libraries that can be used independently of the JniPMML-[AAA] libs</li>
<li>lib: compiled final products which could be used directly</li>
<li>scripts: make scripts, for cross-language documentation building in particular</li>
</ul>


## Prerequisites
<ul>
<li>64 bit Excel</li>
    Although, if compiling, 32 bit could possibly be added.
<li>Access to the VBA project object model (if using the VBA module handlers</li>
<li>HDF5 and HDFView</li>
    <ul>
    <li>The HDF5 and HDFView libs are required if compiling, but the functionality could be removed.</li>
    <li>The provided jars require at least HDFView be on the path or the path passed in as a command line option when starting Excel</li>
    </ul>
<li>Java jdk-12</li>
    Required when using the latest HDFView install.
<li>Compiling environment</li>
    The github configurations are for Visual Studio Community Edition and Intellij Community edition.
<li>DocFx</li>
    DocFx is used for the documentation build, including DocFxDoclet on the JavaDoc side.
</ul>
    


## License Note
All code contributions and development from Wypasek Data Science, Inc. (WDataSci) published on its public github site is released under the
MIT license.  Code from other sources is noted as such, and any assemblies, XLL's, and/or jars that may contain other software (for example,
as Apache's Maven or ExcelDna may bundle from other sources) are released along with the commonly used IDE project and/or solution files
used to generate them.


