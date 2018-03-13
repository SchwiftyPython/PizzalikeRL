using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.Reflection;

public class ConditionChecker {
    
    public MethodInfo CheckCondition(string conditionCode) {

        var provider = CodeDomProvider.CreateProvider("CSharp");
        var parameters = new CompilerParameters{GenerateExecutable = false, GenerateInMemory = true};

        parameters.ReferencedAssemblies.AddRange(new[] 
        {   "UnityEngine.dll",
            "System.Collections.dll",
            "System.Collections.Generic.dll",
            "System.Linq.dll",
            "Random = UnityEngine.Random.dll"
        });

        parameters.CompilerOptions = $"/lib:{Environment.CurrentDirectory}";

        var codeTemplate = @"
            using System;       
            
                public class Check
			    {
			        dummy  
                }
            ";

        var finalCode = codeTemplate.Replace("dummy", conditionCode);

        var results = provider.CompileAssemblyFromSource(parameters, finalCode);

        if (results.Errors.HasErrors)
        {
            var sb = new StringBuilder();

            foreach (CompilerError error in results.Errors)
            {
                sb.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
            }

            throw new InvalidOperationException(sb.ToString());
        }

        var assembly = results.CompiledAssembly;
        var program = assembly.GetType("Check");
        return program.GetMethod("Execute");
    }
}
