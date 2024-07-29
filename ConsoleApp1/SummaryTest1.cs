using System;

namespace ConsoleApp1
{
    internal class SummaryTest1
    {
        // <summary>コメントがないため警告が発生するパブリックメソッド
        public void SampleMethod()
        {
            Console.WriteLine( "This is a sample method." );
        }

        // <summary>コメントがないため警告が発生するパブリックプロパティ
        public int SampleProperty { get; set; }

        // <summary>コメントがあるため警告が発生しないパブリックメソッド
        /// <summary>
        /// このメソッドはサンプルとしての目的を果たします。
        /// </summary>
        public void DocumentedMethod()
        {
            Console.WriteLine( "This is a documented method." );
        }
    }
}
