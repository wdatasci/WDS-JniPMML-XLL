module com.WDataSci.JniPMML {
    //requires slf4j.simple;
    //requires java.xml;
    //requires com.fasterxml.jackson.core;
    requires org.apache.commons.io;
    requires com.WDataSci.WDS;
    requires jcommander;
    requires pmml.model;
    requires pmml.evaluator;
    requires hdfobject;
    requires jdk.xml.dom;
    requires commons.csv;
    requires java.sql;
    requires com.sun.istack.runtime;
    requires com.fasterxml.jackson.databind;
    opens com.WDataSci.JniPMML;
    exports com.WDataSci.JniPMML;
}