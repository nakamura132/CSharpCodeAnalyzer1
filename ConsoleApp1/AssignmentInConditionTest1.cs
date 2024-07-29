using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class AssignmentInConditionTest1
    {
        // 条件式内で代入を行うサンプルメソッド
        public void TestMethod()
        {
            int a = 10;
            int b = 5;

            // 警告が発生する条件式
            if ( ( a = b ) > 0 )
            {
                Console.WriteLine( "This line should trigger a warning." );
            }

            // 正しい条件式
            if ( a == b )
            {
                Console.WriteLine( "This line is correct." );
            }

            // 警告が発生する条件式
            while ( ( b = a ) > 0 )
            {
                Console.WriteLine( "This line should trigger a warning." );
                a--;
            }

            bool bb = false;
            if( bb = true )
            {

            }
        }
    }
}
