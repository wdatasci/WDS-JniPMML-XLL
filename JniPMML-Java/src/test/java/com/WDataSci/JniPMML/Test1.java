package com.WDataSci.JniPMML;

import com.WDataSci.WDS.WDSException;
import org.junit.Test;

import static org.junit.Assert.*;

public class Test1
{
    @Test
    public void shouldAnswerWithTrue()
    {

        String[] args = {"--pmml", "..\\test\\data\\IrisMultinomReg.xml"
                , "--input", "..\\test\\data\\Iris.csv"
                //,"--input-type","delimited"
                , "--input-has-header"
                , "--input-schema", "..\\test\\data\\Iris_Input.xsd"
                //,"--input-schema-recordset-element","RecordSet"
                , "--output", "..\\test\\output\\IrisMultinomReg_output.csv"
                , "--output-header"
                , "--no-verbose"
        };

        String[] args2 = {"--pmml", "..\\test\\data\\IrisMultinomReg.xml"
                , "--input", "..\\test\\data\\Iris.h5"
                , "--input-HDF5-DataSet", "RecordSet"
                , "--input-type", "HDF5"
                , "--output", "..\\test\\output\\IrisMultinomReg_output.h5"
                //Java-HDF and hdf.object have trouble writing the variable Length strings it may have just read
                //,"--output-HDF5-strings-with-fixed-Length","0"
                , "--output-HDF5-strings-with-fixed-Length", "32"
                , "--output-type", "HDF5"
                , "--output-header"
                //,"--no-output-input-fields"
                , "--no-verbose"
        };

        String[] args3 = {"--pmml", "..\\test\\data\\IrisMultinomReg.xml"
                , "--input", "..\\test\\data\\Iris.csv"
                , "--input-has-header"
                , "--input-schema", "..\\test\\data\\Iris_Input.xsd"
                , "--input-schema-recordset-element", "RecordSet"
                , "--output", "..\\test\\output\\IrisMultinomReg_output_2.h5"
                , "--output-HDF5-strings-with-fixed-Length", "32"
                , "--output-type", "HDF5"
                //,"--no-output-input-fields"
                , "--no-verbose"
        };

        String[] args4 = {"--pmml", "..\\test\\data\\IrisMultinomReg.xml"
                , "--input", "..\\test\\data\\Iris.csv"
                , "--input-has-header"
                , "--input-schema-type", "NamingConvention"
                , "--output", "..\\test\\output\\IrisMultinomReg_output_3.h5"
                , "--output-HDF5-strings-with-fixed-Length", "32"
                , "--output-type", "HDF5"
                //,"--no-output-input-fields"
                , "--no-verbose"
        };

        String args5 = "--pmml ..\\test\\data\\IrisMultinomReg.xml"
                + " --input ..\\test\\data\\Iris.csv"
                + " --input-has-header"
                + " --input-schema-type xsd"
                + " --output ..\\test\\output\\IrisMultinomReg_output_4.h5"
                + " --output-HDF5-strings-with-fixed-Length 32"
                + " --output-type HDF5"
                + " --no-verbose"
                ;

        String args6 = "--pmml ..\\test\\data\\single_audit_kmeans.xml"
                + " --input ..\\test\\data\\Audit.csv"
                + " --input-has-header"
                + " --input-schema-type NamingConvention"
                + " --output ..\\test\\output\\single_audit_kmeans_output.h5"
                + " --output-HDF5-strings-with-fixed-Length 32"
                + " --output-type HDF5"
                + " --no-verbose"
                ;

        String[] args_for_Iris = {"--pmml", "..\\test\\data\\IrisMultinomReg.xml"
                , "--output", "..\\test\\output\\IrisMultinomReg_output.csv"
                , "--input", "..\\test\\data\\Iris.csv"
                ,"--input-type","delimited"
                , "--input-has-header"
                , "--input-schema", "..\\test\\data\\Iris_Input.xsd"
                , "--input-schema-recordset-element","RecordSet"
                , "--output-header"
                , "--no-output-input-fields"
                , "--no-verbose"
        };

        String[] models_for_Iris = {
                //"41_xform_iris",
                 "IrisGeneralRegression"
                ,"IrisHClust"
                ,"IrisKMeans"
                ,"IrisLinearReg"
                ,"IrisMultinomReg"
                ,"IrisRandomForest"
                ,"IrisTree"
                ,"ensemble_iris_dectree"
                ,"ensemble_iris_linreg"
                ,"ensemble_iris_mlp"
                ,"ensemble_iris_svm"
                ,"single_iris_dectree"
                ,"single_iris_kmeans"
                ,"single_iris_logreg"
                ,"single_iris_mlp"
                ,"single_iris_svm"
        };

        String[] args_for_Audit = {"--pmml", "..\\test\\data\\single_audit_svm.xml"
                , "--output", "..\\test\\output\\single_audit_svm_output.csv"
                , "--input", "..\\test\\data\\Audit.h5"
                , "--input-HDF5-DataSet", "RecordSet"
                , "--input-type", "HDF5"
                , "--output-header"
                , "--no-output-input-fields"
                , "--no-verbose"
        };

        String[] models_for_Audit = {
                //"41_xform_audit",
                "AuditBinaryReg",
                "single_audit_dectree",
                "AuditKMeans",
                "AuditRandomForest",
                "AuditSVM",
                "AuditTree",
                "ensemble_audit_dectree",
                "ensemble_audit_kmeans",
                "ensemble_audit_logreg",
                "ensemble_audit_mlp",
                "ensemble_audit_svm",
                "single_audit_kmeans",
                "single_audit_logreg",
                "single_audit_mlp",
                "single_audit_svm"
        };

        String[] args_for_ElNino = {"--pmml", "xxxx"
                , "--output", "xxx"
                , "--input", "..\\test\\data\\Elnino.h5"
                ,"--input-type","HDF5"
                , "--input-HDF5-DataSet", "RecordSet"
                , "--output-header"
                , "--no-output-input-fields"
                , "--no-verbose"
        };

        String[] models_for_ElNino = {
                "ElNinoLinearReg"
                ,"ElNinoPolReg"
        };


        try {

            /*
            */
            System.out.println(System.getProperty("java.class.path"));
            JniPMML.mCmdRun(args5);
            Cmd.main(args);
            Cmd.main(args2);
            Cmd.main(args3);
            Cmd.main(args4);
            JniPMML.mCmdRun(args5);
            JniPMML aJniPMML=new JniPMML();
            JniPMML.mCmdRun(args6);


            for (int i=0;i<models_for_Iris.length;i++) {
                System.out.println("Iris, i="+i+", model="+models_for_Iris[i]);
                args_for_Iris[1]="..\\test\\data\\"+models_for_Iris[i]+".xml";
                args_for_Iris[3]="..\\test\\output\\"+models_for_Iris[i]+".output.csv";
                System.out.println("args >>>");
                for (int j=0;j<4;j++) //args_for_Iris.length;j++)
                    System.out.println(args_for_Iris[j]);
                System.out.println("<<< args");
                Cmd.main(args_for_Iris);
                System.out.println("Iris, i="+i+", model="+models_for_Iris[i]);
            }
            /*
            */

            for (int i=0;i<models_for_Audit.length;i++) {
                System.out.println("Audit, i="+i+", model="+models_for_Audit[i]);
                args_for_Audit[1]="..\\test\\data\\"+models_for_Audit[i]+".xml";
                args_for_Audit[3]="..\\test\\output\\"+models_for_Audit[i]+".output.csv";
                System.out.println("args >>>");
                for (int j=0;j<4;j++) //args_for_Audit.length;j++)
                    System.out.println(args_for_Audit[j]);
                System.out.println("<<< args");
                Cmd.main(args_for_Audit);
                System.out.println("Audit, i="+i+", model="+models_for_Audit[i]);
            }

            for (int i=0;i<models_for_ElNino.length;i++) {
                System.out.println("ElNino, i="+i+", model="+models_for_ElNino[i]);
                args_for_ElNino[1]="..\\test\\data\\"+models_for_ElNino[i]+".xml";
                args_for_ElNino[3]="..\\test\\output\\"+models_for_ElNino[i]+".output.csv";
                System.out.println("args >>>");
                for (int j=0;j<4;j++) //args_for_ElNino.length;j++)
                    System.out.println(args_for_ElNino[j]);
                System.out.println("<<< args");
                Cmd.main(args_for_ElNino);
                System.out.println("ElNino, i="+i+", model="+models_for_ElNino[i]);
            }

        } catch (Exception e) {
            com.WDataSci.WDS.WDSException je = new com.WDataSci.WDS.WDSException("Test1 Error", e);
            System.out.print(je.getMessage());
            fail();

        }
        assertTrue(true);
    }
}
