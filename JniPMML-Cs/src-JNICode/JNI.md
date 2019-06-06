---
uid: JNI
summary: *content
---
The JNI namespace (and the files in the JNICode subdirectory of the project) come from an excellent 
article by Simon Agholor,
<a href="https://www.codeproject.com/Articles/245622/Using-the-Java-Native-Interface-in-Csharp">Using the Java Native Interface in C#</a>, 
with a link to source code.
<!--
<a href="https://www.codeproject.com/KB/cs/JNICode/Source_code.zip">source code</a>.
-->

Here there are some minor changes. Most notably, a slight adjustment on thrown exceptions and some additional signature handling. In
particular, the passing of a direct ByteBuffer has been added for passing large block of data from C# to Java/jni more efficiently.




