# Notes on Java x Cs

<i>Author: Christian Wypasek</i>

The mirroring of the C# and Java code is meant not to be slick or cute. It is simply because both implementations are reading and writing
the same formats.  When handing off data in a ByteBuffer between C# and Java, in both directions, the formats must be <i>exactly</i> the
same.  (Note, not going down the AST route. It seems like if you are going to go that route, you should be all in.)

Some syntax differences are too big to bridge, such as in how enums are more flexible in Java than C#.  With enums, just the values and
methods (extensions in C#) are in common.  The source codes will still be organized similarly, but this is also why enums are not otherwise
in the files with their naturally associated classes.

Some syntax differences are not marked but obvious:
<ul>
<li>Non-method properties, such as length/Length or boolean/Boolean, which are easy enough to fix in IDEs.</li>
<li>To break String object references in Java where C# does not require it, a simple new_String() function in C# is a pass through 
and differs only with the "_".</li>
<li>Method <i>throws</i> required in Java but not C# are on separate lines and commented out in C#.</li>
<li>In switch-case statements on enum values where Java case statements do not require qualified names, there will be two lines
one uncommented for Java, the other commented for C#, and visa-versa.</li>
</ul>

The syntax differences for many common methods amount only to the case of the leading letter, such as with
Java's String.toString() vs C#'s String.ToString().  When this leading case issue is on a class method, they can be minimized through C#'s 
static extension methods,
included in a static class, [JavaLikeExtensions](../../**/obj/WDS-Cs/com.WDataSci.WDS.JavaLikeExtensions.yml).
Why not just let one letter differences ride, like in length/Length above? One line in one file and one less thing to
highlight a difference in vimdiff.
Other differences can be eliminated through specially named classes, mimicking
names and methods used on the Java side, such as Map, PrintWriter, and ArrayList.  Even though broken out in the documentation,
on C#, they can all be included in the WDSXJava.cs, along with JavaLikeExtensions.

Syntax differences over lines or blocks are handled in two ways:  First, when a one line change is required, a comment leading
with //Java or //C# precedes the line.  On the Java side, the //C# and subsequent line are collapsed, commenting out the C# syntax.
The reverse treatment is used on the C# side. 

For example, in C# version:
```cs
//C#
if ( !base.Equals(arg) ) return false;
//Java if ( !super.Equals(arg) ) return false;
```
And in the Java version:
```cs
//C# if ( !base.Equals(arg) ) return false;
//Java 
if ( !super.Equals(arg) ) return false;
```

For larger blocks, we can exploit the behavior that an open-comment /* jumps over other open comments until the first closing */.
Therefore, in the C# version (Note that the Java >>> comment is open):
```cs
/* C# >>> */
if ( !base.Equals(arg) ) return false;
/* <<< C# */
/* Java >>> *   
if ( !super.Equals(arg) ) return false;
/* <<< Java */
```
And in the Java version (Note that the C# >>> comment is open):
```cs
/* C# >>> *
if ( !base.Equals(arg) ) return false;
/* <<< C# */
/* Java >>> */
if ( !super.Equals(arg) ) return false;
/* <<< Java */
```


