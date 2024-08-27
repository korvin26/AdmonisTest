using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace AdmonisTest.Admonis
{
    /// <summary>
    /// Provides functionality to load and parse products from an XML file with an emphasis on speed.
    /// 
    /// This class is designed to efficiently process large XML files by prioritizing speed over memory consumption.
    /// Concurrent collections such as <see cref="ConcurrentBag{T}"/> and <see cref="ConcurrentDictionary{TKey,TValue}"/>
    /// are used to facilitate parallel processing and ensure thread safety during operations. This design choice allows
    /// the class to quickly handle extensive product data, even if it results in higher memory usage. The primary goal 
    /// of this implementation is to minimize processing time, making it suitable for scenarios where performance is 
    /// critical and memory consumption is a secondary concern.
    /// </summary>
    public class AdmonisClass
    {
        public ConcurrentBag<AdmonisProduct> Products { get; set; } = new ConcurrentBag<AdmonisProduct>();
        private ConcurrentDictionary<string, string> _unparsedProducts = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, string> _translations = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Loads products from the specified XML file and parses them into product objects in parallel.
        /// </summary>
        /// <param name="filePath">The path to the XML file containing product data.</param>
        public void LoadProductsFromXml(string filePath)
        {
            var startTime = DateTime.Now;
            var productElements = new List<string>();


            LoadUnparsedProducts(filePath);
            var elapsed = DateTime.Now;
            Console.WriteLine("Loaded unparsed in seconds {0}", (DateTime.Now - startTime).TotalSeconds);
            ExecuteParallel();
            Console.WriteLine("Finished mapping in seconds {0}", (DateTime.Now - elapsed).TotalSeconds);
            Console.WriteLine("Total execution time in seconds {0}", (DateTime.Now - startTime).TotalSeconds);
        }

        /// <summary>
        /// Processes each unparsed product XML string in parallel, 
        /// checking if it contains variants and parsing it into a product object.
        /// </summary>
        private void ExecuteParallel()
        {
            Parallel.ForEach(_unparsedProducts, product =>
            {
                try
                {
                    string productXml = product.Value;
                    if (productXml.Contains("<variants>"))
                    {
                        AdmonisProduct parsedProduct = ParseProductFromXml(productXml);
                        if (parsedProduct != null)
                            Products.Add(parsedProduct);
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing product {product.Key}: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Reads the XML file and stores each product element's XML as a string in the dictionary.
        /// </summary>
        /// <param name="filePath">The path to the XML file.</param>
        private void LoadUnparsedProducts(string filePath)
        {
            // Use XmlReader to stream through the XML file
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement("product"))
                    {
                        var productId = reader.GetAttribute("product-id");
                        string productXml = reader.ReadOuterXml();
                        _unparsedProducts.TryAdd(productId, productXml);
                    }
                }
            }
        }

        /// <summary>
        /// Parses a single product from its XML representation stored in memory.
        /// </summary>
        /// <param name="productXml">The XML string representing the product.</param>
        /// <returns>A populated <see cref="AdmonisProduct"/> object, or null if parsing fails.</returns>
        private AdmonisProduct ParseProductFromXml(string productXml)
        {
            AdmonisProduct product = new AdmonisProduct();
            // Use XmlReader to stream through the XML
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(productXml)))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        HandleProductElement(reader, ref product);
                    }
                }
            }
            return product;
        }

        /// <summary>
        /// Processes individual XML elements and assigns their values to the corresponding properties of the product.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> instance used to read the XML.</param>
        /// <param name="product">The <see cref="AdmonisProduct"/> object to populate.</param>
        private void HandleProductElement(XmlReader reader, ref AdmonisProduct product)
        {
            switch (reader.Name)
            {
                case "product":
                    var productId = reader.GetAttribute("product-id");
                    product.Makat = productId;
                    break;

                case "display-name":
                    product.Name = reader.ReadString();
                    break;

                case "short-description":
                    product.Description = reader.ReadString();
                    break;

                case "long-description":
                    product.DescriptionLong = reader.ReadString();
                    break;

                case "brand":
                    product.Brand = reader.ReadString();
                    break;

                case "f54ProductVideo":
                    product.VideoLink = reader.ReadString();
                    break;

                case "variation-attribute":
                    ProcessVariationAttribute(reader);
                    break;

                case "variant":
                    // Handle variants as product options
                    var optId = reader.GetAttribute("product-id");
                    var opt = HandleProductOptionElement(optId, product.Makat);
                    product.Options.Add(opt);
                    break;
            }
        }

        /// <summary>
        /// Processes the variation-attribute element to extract the translation mapping between 
        /// variation-attribute-value and display-value, but only if the attribute-id is "f54ProductColor".
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> instance used to read the XML.</param>
        private void ProcessVariationAttribute(XmlReader reader)
        {
            // Check if the current variation-attribute has attribute-id="f54ProductColor"
            var attributeId = reader.GetAttribute("attribute-id");

            if (attributeId == "f54ProductColor")
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement("variation-attribute-value"))
                    {
                        // Get the value attribute (key for the translation dictionary)
                        string valueKey = reader.GetAttribute("value");

                        // Move to the display-value element within variation-attribute-value
                        while (reader.Read())
                        {
                            if (reader.IsStartElement("display-value"))
                            {
                                // Read the display value (value for the translation dictionary)
                                string displayValue = reader.ReadString();

                                // Add to the translations dictionary
                                if (!string.IsNullOrEmpty(valueKey) && !_translations.ContainsKey(valueKey))
                                {
                                    _translations.TryAdd(valueKey, displayValue);
                                }

                                // Break after reading 'display-value'
                                break;
                            }

                            // Exit if we reach the end of the current 'variation-attribute-value'
                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "variation-attribute-value")
                            {
                                break;
                            }
                        }
                    }

                    // Stop if we reach the end of the current variation-attribute element
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "variation-attribute")
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses a variant option from its XML representation and associates it with the specified parent product.
        /// </summary>
        /// <param name="productId">The ID of the variant product.</param>
        /// <param name="parentId">The ID of the parent product.</param>
        /// <returns>A populated <see cref="AdmonisProductOption"/> object.</returns>
        private AdmonisProductOption HandleProductOptionElement(string productId, string parentId)
        {
            var option = new AdmonisProductOption();
            option.ProductMakat = parentId;
            option.optionMakat = productId;

            var productXml = _unparsedProducts[productId];

            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(productXml)))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "custom-attribute":
                                string attributeId = reader.GetAttribute("attribute-id");
                                if (attributeId == "f54ProductColor")
                                {
                                    option.optionSugName1 = "צבע";
                                    option.optionSugName1Title = "בחר צבע";
                                    var color = reader.ReadString();
                                    option.optionName = _translations[color];
                                }
                                if (attributeId == "f54ProductSize")
                                {
                                    var value = reader.ReadString();
                                    if (value == "UNI")
                                        break;
                                    else
                                    {
                                        // When color and size options availiable we will set color name to optionSugName2 and size to optionName
                                        option.optionSugName2 = option.optionName;
                                        option.optionSugName2Title = "בחר מידה";
                                        option.optionName = value;
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            return option;
        }
    }
}
