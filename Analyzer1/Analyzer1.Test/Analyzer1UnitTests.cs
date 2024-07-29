using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.Analyzer1Analyzer,
    Analyzer1.Analyzer1CodeFixProvider>;

namespace Analyzer1.Test
{
    [TestClass]
    public class Analyzer1UnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync( test );
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            var test = @"
using System;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello World"");
    }

    void exampleMethod() // 警告が表示されるはず
    {
    }
}
@";
            await VerifyCS.VerifyAnalyzerAsync( test );
        }

        [TestMethod]
        public async Task SummaryCommentAnalyzer_Test1()
        {
            var test = @"
using System;
class Program
{
    // <summary>コメントがないため警告が発生するパブリックメソッド
    public void SampleMethod()
    {
        Console.WriteLine( ""This is a sample method."" );
    }

    // <summary>コメントがないため警告が発生するパブリックプロパティ
    public int SampleProperty { get; set; }

    // <summary>コメントがあるため警告が発生しないパブリックメソッド
    /// <summary>
    /// このメソッドはサンプルとしての目的を果たします。
    /// </summary>
    public void DocumentedMethod()
    {
        Console.WriteLine( ""This is a documented method."" );
    }
}
@";
            await VerifyCS.VerifyAnalyzerAsync( test );
        }

        //Diagnostic and CodeFix both triggered and checked for
    //    [TestMethod]
    //    public async Task TestMethod2()
    //    {
    //        var test = @"
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;
    //using System.Diagnostics;

    //namespace ConsoleApplication1
    //{
    //    class {|#0:TypeName|}
    //    {   
    //    }
    //}";

    //        var fixtest = @"
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;
    //using System.Diagnostics;

    //namespace ConsoleApplication1
    //{
    //    class TYPENAME
    //    {   
    //    }
    //}";

    //        var expected = VerifyCS.Diagnostic("Analyzer1").WithLocation(0).WithArguments("TypeName");
    //        await VerifyCS.VerifyCodeFixAsync( test, expected, fixtest );
    //    }
    }
}
