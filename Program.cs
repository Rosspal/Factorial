using System;
using System.Threading.Tasks;
using System.IO;
using CommandLine;


namespace Factorial
{
    class Program
    {
        public class Options
        {
            [Option(shortName: 'p', longName: "path", Required = false, HelpText = "the path to the file", Default = "test.txt")]
            public string path { get; set; }

            [Option(shortName: 'm', longName: "maxDegreeOfParallelism", Required = false, HelpText = "max degree of parallelism.", Default = -1)]
            public int maxDegreeParall { get; set; }
        }


        static void Main(string[] args)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = ParseMaxDegreeParall(args);

            string fileName = ParsePath(args);
            StreamReader fileIn = new StreamReader(fileName);
            StreamWriter fileOut = new StreamWriter("result.txt");

            int[] array;
            long[] result;

            while (!fileIn.EndOfStream)
            {
                array = FileReading(fileIn, 1_000_000);
                result = new long[array.Length];

                Parallel.For(0, array.Length, parallelOptions, (i) =>
                {
                    long temp = FactorialRecursion(array[i]);
                    result[i] = temp;
                });

                FileWriting(result,fileOut);
            }

            fileIn.Close();
            fileOut.Close();
        }


        static string ParsePath(string[] args)
        {
            string path = "";
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (File.Exists(o.path))
                       {
                           path = o.path;
                       }
                       else
                       {
                           Console.WriteLine("path error, selected default path");
                           path = "test.txt";
                       }

                   });


            return path;
        }

        static int ParseMaxDegreeParall(string[] args)
        {
            int maxDegreeParall = -1;
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if ((o.maxDegreeParall >= -1) && (o.maxDegreeParall != 0))
                       {
                           maxDegreeParall = o.maxDegreeParall;
                       }
                       else
                       {
                           Console.WriteLine("value error, selected default value");
                           maxDegreeParall = -1;
                       }

                   });
    

            return maxDegreeParall;
        }

        static int[] FileReading(StreamReader fileIn, int count )
        {
            
            int[] array = new int[count];

            int index = 0;
            while ( (!fileIn.EndOfStream) && (index < count) )
            {
                try
                {
                    array[index] = int.Parse(fileIn.ReadLine());
                    index++;
                }
                catch
                {

                }
            }
            Array.Resize(ref array,index);
            return array;
        }

        static void FileWriting(long[] result, StreamWriter fileOut)
        {
            foreach (long number in result)
            {
                fileOut.WriteLine(number);
            }
        }

        static long FactorialRecursion(int number)
        {
            if (number == 0) { return 1; }
            return number * FactorialRecursion(number - 1);
        }
    }
}
