module com.WDataSci.WDS {
    requires java.base;
    requires org.apache.commons.io;
    opens com.WDataSci.WDS;
    exports com.WDataSci.WDS;
}