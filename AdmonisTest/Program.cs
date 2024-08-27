using AdmonisTest.Admonis;
using System;
using System.Text.Json;
using System.IO;
using System.Text;

namespace AdmonisTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = Encoding.GetEncoding("Windows-1255");
            AdmonisClass admonis = new AdmonisClass();
            admonis.LoadProductsFromXml("Admonis\\Product.xml");
            var outputFilePath = "Admonis\\output.json";

            // I commented those outputs since in test wasn't mention output at all
            // Output to file
            //ToFile(outputFilePath, JsonSerializer.Serialize(admonis.Products));

            // Output to console
            //foreach (var product in admonis.Products)
            //{
            //    Console.WriteLine($"Product ID: {product.Makat}");
            //    Console.WriteLine($"Name: {product.Name}");
            //    Console.WriteLine($"Description: {product.Description}");
            //    Console.WriteLine($"Description Long: {product.DescriptionLong}");
            //    Console.WriteLine($"Brand: {product.Brand}");

            //    foreach (var option in product.Options)
            //    {
            //        Console.WriteLine($"  Option Name: {option.optionName}");
            //        Console.WriteLine($"  OptionSugName1: {option.optionSugName1}");
            //        Console.WriteLine($"  OptionSugName2: {option.optionSugName2}");
            //    }
            //}
            Console.ReadLine();
        }
        static void ToFile(string path, string jsonString)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("");
                }
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(jsonString);
            }
        }
    }
}
