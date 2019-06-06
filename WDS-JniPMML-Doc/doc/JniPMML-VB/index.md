---
uid: JniPMML-VB.index.md
title: JniPMML-VB API
---

## JniPMML-VB

The JniPMML-VB code is primarily for some additional Excel manipulation functionality.  In particular,
the wrangling the Excel VBE components.  The ExcelDna and Microsoft.Office.Interop.Excel libraries
are generally mirrored in both C# and VB, however, ExcelDna UDF functions which take references as 
objects so that information about the caller can be determined at run-time become automatically 
volatile.   For this reason, there are several function wrappers implemented in VBA which must be
either in an another addin, or as a VBA module in the workbook.   The JniPMML-VB (and supporting
WDS-VB code which is pulled into the assembly) addin facilitates these wrapped functions by 
providing a wrangler for a WDSJniPMML.bas module.

