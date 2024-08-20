# net8readCryptedAppConfig

Read an aspnet_regiis encrypted app.config in .NET 8
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // ソースコードが格納されているディレクトリ
        string sourceCodeDirectory = @"C:\Path\To\Your\SourceCode";

        // ディレクトリ内のすべてのC#ファイルを取得
        var csFiles = Directory.GetFiles(sourceCodeDirectory, "*.cs", SearchOption.AllDirectories);

        // ストアドプロシージャ名のリストを保持するリスト
        var storedProcedures = new List<string>();

        foreach (var file in csFiles)
        {
            var code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);

            var root = tree.GetRoot();
            var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                if (memberAccess != null && memberAccess.Name.Identifier.Text == "Execute")
                {
                    var argumentList = invocation.ArgumentList.Arguments;

                    if (argumentList.Count > 0)
                    {
                        var firstArgument = argumentList[0].Expression as LiteralExpressionSyntax;

                        if (firstArgument != null && firstArgument.IsKind(SyntaxKind.StringLiteralExpression))
                        {
                            var storedProcedureName = firstArgument.Token.ValueText;
                            storedProcedures.Add(storedProcedureName);
                            Console.WriteLine($"Found stored procedure: {storedProcedureName} in {file}");
                        }
                    }
                }
            }
        }

        // SP名を一覧で表示
        Console.WriteLine("\nList of stored procedures used:");
        foreach (var sp in storedProcedures.Distinct())
        {
            Console.WriteLine(sp);
        }
    }
}
