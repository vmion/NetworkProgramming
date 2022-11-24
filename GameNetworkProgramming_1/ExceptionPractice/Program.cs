using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array = new int[4];
            try
            {
                array[0] = 100;
                array[4] = 200; //예외 발생
            }
            catch(IndexOutOfRangeException e)
            {
                //배열의 인덱스에 대한 예외
                Console.WriteLine(e.Message);
            }
            try
            {
                array[5] = 200;
            }
            catch(Exception e)
            {
                //모든 예외에 대한 처리
                Console.WriteLine(e.Message);
            }
            try
            {
                int k = 5;
                if (k >= array.Length)
                    throw new Exception("배열의 인덱스 범위를 벗어났습니다.");
                array[5] = 200;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
