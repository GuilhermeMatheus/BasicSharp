# Welcome to BasicSharp Compiler!

BasicSharp is an simple programming language very similar to C# and Java.

<br/>

B# allows simple procedures and functions structures with basic statements like loops and conditional statements.

# "Hello World!"
<pre>
implements System.Console;

module HelloWorldModule {
  my void Main() {
	  WriteLine("Hello World!");
	}
}
</pre>

In addition, BasicSharp Compiler allows external references from any .NET assembly. External assemblies are used with the <i>implements directive</i>, that must precede the full name of an static class.

<pre>
implements System.Console;   // WriteLine(string); ReadLine(); Clear(); [...]
implements System.Convert;   // ToInt32(string);  [...]
implements System.Math;      // Sqrt(string);  [...]
</pre>

<br />

The absolute path of this .DLL files are stored in the project file, in XML format.

# IDE Features

These screen shots show B# IDE running.
 
### Tokens list
When you select an item on token list, it's highlighted on text editor!
![IDE Tokens](https://github.com/GuilhermeMatheus/BasicSharp/raw/master/img/ss-tokens.png)

### Syntax Tree visualizer
When you select an node on Syntax Tree visualizer, it's highlighted on text editor too!
![IDE SyntaxTree](https://github.com/GuilhermeMatheus/BasicSharp/raw/master/img/ss-syntaxtree.png)

### MSIL on-edit view
You can see your MSIL generated while typing!
![IDE MSIL](https://github.com/GuilhermeMatheus/BasicSharp/raw/master/img/ss-msil.png)
