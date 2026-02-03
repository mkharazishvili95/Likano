namespace Likano.Web.Helpers
{
    public static class SeoHelper
    {
        public static class Keywords
        {
            public const string Georgian = "სამზარეულო, ნიჟარა, ონკანი, გაზქურა, ღუმელი, გამწოვი, სამზარეულოს ტექნიკა, სანტექნიკა საბითუმო ფასად, ჩასაშენებელი ტექნიკა, ლიკანო";
            
            public const string English = "kitchen, sink, faucet, gas stove, oven, hood, kitchen appliances, wholesale plumbing, built-in appliances, likano";
            
            public const string Russian = "кухня, раковина, кран, газовая плита, духовка, вытяжка, кухонная техника, сантехника оптом, встраиваемая техника, ликано";
            
            public const string All = Georgian + ", " + English + ", " + Russian;
        }

        public static class Slogans
        {
            public const string Main = "სამზარეულო იწყება სწორი ტექნიკით";
            public const string Secondary = "სანტექნიკა საბითუმო ფასად - იპოვე შენთვის საუკეთესო ტექნიკა";
        }

        public static class Descriptions
        {
            public const string HomePage = "Likano.ge - სამზარეულოს პროფესიონალური ტექნიკა საბითუმო ფასად. ნიჟარა, ონკანი, გაზქურა, ღუმელი, გამწოვი და ჩასაშენებელი ტექნიკა. უფასო მიწოდება თბილისში.";
            
            public const string ContactPage = "დაგვიკავშირდით Likano.ge - თბილისი, აგლაძის ქ. №15 | ☎ 599 45 79 50 | სამზარეულოს ტექნიკის პროფესიონალები";
            
            public static string ProductDetails(string productName, string categoryName) =>
                $"{productName} - {categoryName} Likano.ge-ზე | საბითუმო ფასად | უფასო მიწოდება თბილისში | გარანტია | პროფესიონალური კონსულტაცია";
        }

        public static string GenerateProductSchema(dynamic product, string productUrl, string imageUrl)
        {
            var price = product.price ?? 0;
            var availability = (product.quantity ?? 0) > 0 ? "InStock" : "OutOfStock";
            
            return $@"{{
              ""@context"": ""https://schema.org/"",
              ""@type"": ""Product"",
              ""name"": ""{EscapeJson(product.name?.ToString() ?? "")}"",
              ""image"": ""{imageUrl}"",
              ""description"": ""{EscapeJson(product.description?.ToString() ?? "")}"",
              ""brand"": {{
                ""@type"": ""Brand"",
                ""name"": ""Likano""
              }},
              ""offers"": {{
                ""@type"": ""Offer"",
                ""url"": ""{productUrl}"",
                ""priceCurrency"": ""GEL"",
                ""price"": ""{price}"",
                ""availability"": ""https://schema.org/{availability}"",
                ""seller"": {{
                  ""@type"": ""Organization"",
                  ""name"": ""Likano.ge""
                }}
              }}
            }}";
        }

        public static string GenerateOrganizationSchema()
        {
            return @"{
              ""@context"": ""https://schema.org"",
              ""@type"": ""Store"",
              ""name"": ""Likano.ge"",
              ""description"": ""სამზარეულოს პროფესიონალური ტექნიკა საბითუმო ფასად"",
              ""url"": ""https://www.likano.ge"",
              ""logo"": ""https://ik.imagekit.io/rqxdk712q/products/17827bc1-7332-4122-b0aa-7e5adc3e229e_likanologo"",
              ""image"": ""https://ik.imagekit.io/rqxdk712q/products/17827bc1-7332-4122-b0aa-7e5adc3e229e_likanologo"",
              ""telephone"": ""+995599457950"",
              ""address"": {
                ""@type"": ""PostalAddress"",
                ""streetAddress"": ""აგლაძის ქ. №15"",
                ""addressLocality"": ""თბილისი"",
                ""addressCountry"": ""GE""
              },
              ""geo"": {
                ""@type"": ""GeoCoordinates"",
                ""latitude"": ""41.7151"",
                ""longitude"": ""44.8271""
              },
              ""openingHoursSpecification"": {
                ""@type"": ""OpeningHoursSpecification"",
                ""dayOfWeek"": [
                  ""Monday"",
                  ""Tuesday"",
                  ""Wednesday"",
                  ""Thursday"",
                  ""Friday"",
                  ""Saturday""
                ],
                ""opens"": ""10:00"",
                ""closes"": ""19:00""
              },
              ""sameAs"": [
                ""https://www.facebook.com/likanogeorgia""
              ]
            }";
        }

        private static string EscapeJson(string text)
        {
            return text.Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", "");
        }
    }
}