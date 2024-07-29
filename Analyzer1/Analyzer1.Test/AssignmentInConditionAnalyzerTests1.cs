using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.Analyzer1Analyzer,
    Analyzer1.Analyzer1CodeFixProvider>;

namespace Analyzer1.Test
{
    [TestClass]
    public class AssignmentInConditionAnalyzerTests
    {
        // テストケース: 条件式内で代入がある場合、警告を発生させる。
        [TestMethod]
        public async Task AssignmentInCondition_ShouldTriggerWarning()
        {
            var test = @"
using System;

namespace SampleNamespace
{
    public class SampleClass
    {
        public void TestMethod()
        {
            int a = 10;
            int b = 5;

            // 条件式内で代入を行うため警告が発生する
            if ((a = b) > 0) 
            {
                Console.WriteLine(""This line should trigger a warning."");
            }
        }
    }
}";
            // DiagnosticDescriptor を使用して期待する診断を指定
            var expected = VerifyCS.Diagnostic(AssignmentInConditionAnalyzer.Rule)
                .WithLocation(12, 17) // "(a = b)" の場所
                .WithArguments("(a = b)");

            await VerifyCS.VerifyAnalyzerAsync( test, expected );
        }

        // テストケース: 条件式内で代入がない場合、警告を発生させない。
        [TestMethod]
        public async Task NoAssignmentInCondition_ShouldNotTriggerWarning()
        {
            var test = @"
using System;

namespace SampleNamespace
{
    public class SampleClass
    {
        public void TestMethod()
        {
            int a = 10;
            int b = 5;

            // 正しい条件式
            if (a == b)
            {
                Console.WriteLine(""This line is correct."");
            }
        }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync( test );
        }
    }
}
