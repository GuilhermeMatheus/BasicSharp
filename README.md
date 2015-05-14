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

 <br/>
 
 # Simple Calculator

<pre> 
implements System.Console;
implements System.Convert;
implements System.Math;

module Calculator {
	
	my void Main()
	{
		while(true)
		{
			WriteLine("BSharp Calculator!");
			WriteLine("Press any key to continue...");
			ReadLine();
			Clear();
			WriteLine("Digite uma opcao:");
			WriteLine("    1. Pow");
			WriteLine("    2. Mult");
			WriteLine("    3. Sqrt");
			WriteLine("    4. Factoring");
			WriteLine("    5. Exit");
			
			string _option = ReadLine();
			int option = ToInt32(_option);
			bool isValidOption = option > 0 & option <= 4;
			
			if (isValidOption)
			{
				if(option == 1)
				{
					Pow();
				}
				if(option == 2)
				{
					Mult();
				}
				if(option == 3)
				{
					Sqrt();
				}
				if(option == 4)
				{
					double n = getDouble();
					Factoring(ToInt32(n));
					return;
				}
				if(option == 5)
				{
					return;
				}
			}
		}
	}
	
	everybody void Pow()
	{
		double n = getDouble();
		double p = getDouble();
		
		Write("Pow of ");	
		Write(n);
		Write(" at ");
		Write(p);
		Write(" is ");
		Write(Pow(n, p));
		WriteLine();
	}
	
	everybody void Mult()
	{
		double n = getDouble();
		double p = getDouble();
		
		Write(n);
		Write(" * ");
		Write(p);
		Write(" equals to ");
		Write(n * p);
		WriteLine();
	}
	
	everybody void Sqrt()
	{
		double n = getDouble();
		
		Write("Sqrt of ");	
		Write(n);
		Write(" is ");
		Write(Sqrt(n));
	}
	
	everybody void Factoring(int n)
	{
		int i;
		int result = 1;
		for(i = 0; i < n; i += 1)
		{
			result *= (n - i);
		}		
		
		Write("Factoring ");	
		Write(n);
		Write(" is ");
		Write(result);
	}
	
	my double getDouble()
	{
		WriteLine("Enter a number: ");	
		return ToDouble(ReadLine());
	}
}
</pre>
