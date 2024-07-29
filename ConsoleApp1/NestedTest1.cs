using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class NestedTest1
    {
        // ネストが深すぎるサンプルメソッド
        public void DeeplyNestedMethod()
        {
            if ( true )
            {
                for ( int i = 0; i < 1; i++ )
                {
                    while ( true )
                    {
                        do
                        {
                            switch ( i )
                            {
                                case 0:
                                    // ここで5つ目のネスト
                                    Console.WriteLine( "Deeply nested code" );
                                    break;
                            }
                        } while ( false );
                    }
                }
            }
        }

        // ネストが許容範囲内のサンプルメソッド
        public void AcceptableMethod()
        {
            if ( true )
            {
                for ( int i = 0; i < 1; i++ )
                {
                    // ネストレベルが2つの例
                    Console.WriteLine( "This is fine" );
                }
            }
        }
    }
}
