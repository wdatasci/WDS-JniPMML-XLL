# Notes on JniPMML

<i>Author: Christian Wypasek</i>

## Simple Motivation

My daughter, a college student, asked me to explain this project in one sentence and this was as close as I could get:  Scientists build
models.  For even something as simple as linear regression, there is a formula that needs to evaluated.  It might be for my own purposes, or
it might be for a company I work for, but model implementation needs to be easily accessible. Even though data scientists might use special
tools, everyone in financial services at least has Excel.

## Slightly More Technical Motivation

Regardless of whether or not Excel might be highly regarded as a computational framework among academicians, it is ubiquitous in financial
services (even if it might not be used well).  Therefore, it makes sense that invoking an XML based evaluator from within Excel would be
worthwhile.   In particular, since Excel can enable rapid visualization, one should also be able to compare evaluator implementations and
view model response to variable changes and/or model structure in a live manner.

## XML/PMML

For someone like myself who works across the spectrum of big data projects  (project management and business interface, data science, and
data engineer) and works across multiple programming languages, consistency of treatments is a fundamental key to efficiency.  After years
of engineering databases, building complex statistical models for financial instruments, and incorporating these models into asset backed
cash flow valuations, the greatest risks in this data science process are often operational.  There is the most obvious question,  "Is the
data being used for forecasting sufficiently like the data the model was fit on?",  but one also has to ask "Is the model being calculated
correctly?". 

From personal experience, hand coding something like a scoring model requires significant quality checks and carries the persistent risk
that something was overlooked.  It does not take too many hand coding events to make one believe there has got to be a better way, both for
efficiency of process and the reduction of mistakes that come from mind numbing exercises.   Starting back in 1998/1999, I started using
markup styles to facilitate both the modeling process and facilitating the implementation for scoring and other types of regresson and
non-parametric models.  Since then, PMML (predictive modeling markup language) as become an industry standard.  

The PMML standard has evolved and early versions were not sophisticated enough for my needs.  For example, the Scorecard implementation was
not added until the end of 2011, and transformations were not added until 2014.  For all that it is, PMML is still a communication standard
for model implementation and is often generated after a model has been fit.  Continued diligence is required so the communicated model truly
represents the intended relationship between the input data and the output results.   A process oriented view of statistical model
building starts with data preparation and can be exploited at every step of the process through to final implementation.  


<i>There may be more than one way to skin a cat, but very few which leave you with anything looks like a cat.</i> My personal work has
included using mathematical and statistical model specifications in XML with implementations in SAS, C++, Python, R, in-database (Vertica,
MSSQL) UDTFs in C++/Java/R, and VBA (in Excel).  After drilling into PMML implementation details, there is still much to be desired.  An
updated XML specification used by WDataSci for model fitting and alternate implementations will be released on the its github as a later
project, but transformation (such as through XSLT) into PMML for delivery is reasonable given the industry standardization that PMML offers.
Other model implementation specifications, such as pfa, will emerge, and Excel will remain a platform for either a model delivery or easy
comparison.


## WDS-JniPMML as a multi-language project

The JniPMML project combines several APIs, each for a specific purpose:
<ul>
<li>Java</li> The <i>de facto</i> implementation of PMML is jpmml.evaluator.  JniPMML-Java wraps the implementation in a manner that
creates a standalone jar that can also be called from C# via jni.
<li>C#</li> Using the ExcelDna project to facilitate Excel functionality, the JniPMML-Cs assembly wraps the jni calls.
<li>VB</li> Some odds and ends which I have traditionally done in VBA, but using ExcelDna .Net.   In particular, wrangling of the VBA
modules is done in VB.
<li>VBA</li> Certain Excel functions created with ExcelDna through either C# or VB become <i>volatile</i> in that they recalculate
at every calculation event (which can be a bad thing).  However, good old fashioned VBA can do the same thing in a non-volatile manner.
</ul>

Working in different languages for different aspects of a larger project is not unusual.  For example, database work might be done in SQL,
with processing either in database or written-out-processed-read-back-in, and final summaries might have an entirely different framework.
When sub-projects have many parallel functions, the tendency of programmers to have a project on one side and then start from scratch on the
other side, can lead to unexpected differences which the programmer then might struggle to balance.  Complete one side, move to the other,
discover some new or useful treatment, go back to the first side, restart loop.  This project also started in that manner. 

Passing data back and forth in-memory between Java and C# involves packing memory in a particular way, which also turned out to be the
HDF5.PInvoke bulk writes a HDF5 compound dataset (such as R can export).  Development of the project included consideration of in-memory
HDF5s, which despite HDF5 docs, is not ready for prime-time.  For testing purposes, HDF5 CompoundDS and flat file functionality is included
in the JniPMML-Java project and the Excel AddIn.

Finally, the Excel AddIn also includes other tools representative of some extended functionality I have come to expect over the years, such
as VBA component wrangling and other examples.  Even if this project is not used extensively outside of WDataSci, this project also become
an in-house reference for C#/Java differences and quirks, DocFx, Excel AddIns (quirks across C#, ExcelDna, VB, VBA, COM, non-COM), PMML (and
jpmml quirks), HDF5 (and quirks across HDF.PInvoke, HDF-Java, HDF-Object), etc.


