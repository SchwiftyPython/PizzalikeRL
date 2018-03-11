using System;
using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CSharp;
using System.Reflection;

public class ConditionChecker {

    //todo: Likely make this return a bool
    public MethodInfo CheckCondition(string conditionCode) {

        var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters();

        const string codeTemplate = @"
            public class ConditionCheck
			{
			    dummy 
            }";

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
        var program = assembly.GetType("ConditionCheck");
        return program.GetMethod("Execute");
    }
}
