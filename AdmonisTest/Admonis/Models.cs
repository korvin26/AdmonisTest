using System.Collections.Generic;

namespace AdmonisTest.Admonis
{
    public class AdmonisProduct
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionLong { get; set; }
        public string WarrantyPeriod { get; set; }
        public string WarrantyBy { get; set; }
        public string Makat { get; set; }
        public string Model { get; set; }
        public decimal Price_Cost_Customer { get; set; }
        public decimal Price_Cost { get; set; }
        public decimal Price_Market { get; set; }
        public decimal Price_Publish { get; set; }
        public string Brand { get; set; }
        public string Volume { get; set; }
        public string ClassificationID { get; set; }
        public string SubClass { get; set; }
        public string PlatformCategoryID { get; set; }
        public string VolumeType { get; set; }
        public string VideoLink { get; set; }
        public string StorageLocation { get; set; }
        public string UPC { get; set; }
        public string StatusID { get; set; }
        public string StatusComments { get; set; }
        public string Package { get; set; }
        public bool IsOneSized { get; set; }

        public List<AdmonisProductOption> Options { get; set; } = new List<AdmonisProductOption>();
    }

    public class AdmonisProductOption
    {
        public string optionSugName1 { get; set; }
        public string optionSugName1Title { get; set; }
        public string optionSugName2 { get; set; }
        public string optionSugName2Title { get; set; }
        public string ProductMakat { get; set; }
        public string optionMakat { get; set; }
        public string optionName { get; set; }
        public string optionModel { get; set; }
        public decimal optionPrice_Cost_Customer { get; set; }
        public decimal optionPrice_Cost { get; set; }
        public decimal optionPrice_Publish { get; set; }
        public decimal optionPrice_Market { get; set; }
        public string optionstorageLocation { get; set; }
        public string optionupc { get; set; }
    }


}
