using System.Net;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace COSHH_Generator.Scrapers
{
    
    static class SigmaAldrich
    {
        private static string apiUrl = "https://www.sigmaaldrich.com/api";
        //Regex to find the statements
        

        static public async void SearchAsync(string query, Action<List<SigmaAldrich.Result>>? callback = null)
        {
            var results = await SearchAsync(query);
            if (callback is not null) callback(results);

        }

        static public async Task<List<Result>> SearchAsync(string query)
        {
            string encodedQuery = WebUtility.UrlEncode(query.Trim().ToLower().Replace(" ", "-"));
            string jsonData = "{\"operationName\":\"ProductSearch\",\"variables\":{\"searchTerm\":\"" + encodedQuery + "\",\"page\":1,\"group\":\"substance\",\"selectedFacets\":[],\"sort\":\"relevance\",\"type\":\"PRODUCT_NAME\"},\"query\":\"query ProductSearch($searchTerm: String, $page: Int\u0021, $sort: Sort, $group: ProductSearchGroup, $selectedFacets: [FacetInput\u0021], $type: ProductSearchType, $catalogType: CatalogType, $orgId: String, $region: String, $facetSet: [String], $filter: String) {\\n  getProductSearchResults(input: {searchTerm: $searchTerm, pagination: {page: $page}, sort: $sort, group: $group, facets: $selectedFacets, type: $type, catalogType: $catalogType, orgId: $orgId, region: $region, facetSet: $facetSet, filter: $filter}) {\\n    ...ProductSearchFields\\n    __typename\\n  }\\n}\\n\\nfragment ProductSearchFields on ProductSearchResults {\\n  metadata {\\n    itemCount\\n    setsCount\\n    page\\n    perPage\\n    numPages\\n    redirect\\n    __typename\\n  }\\n  items {\\n    ... on Substance {\\n      ...SubstanceFields\\n      __typename\\n    }\\n    ... on Product {\\n      ...SubstanceProductFields\\n      __typename\\n    }\\n    __typename\\n  }\\n  facets {\\n    key\\n    numToDisplay\\n    isHidden\\n    isCollapsed\\n    multiSelect\\n    prefix\\n    options {\\n      value\\n      count\\n      __typename\\n    }\\n    __typename\\n  }\\n  didYouMeanTerms {\\n    term\\n    count\\n    __typename\\n  }\\n  __typename\\n}\\n\\nfragment SubstanceFields on Substance {\\n  _id\\n  id\\n  name\\n  synonyms\\n  empiricalFormula\\n  linearFormula\\n  molecularWeight\\n  aliases {\\n    key\\n    label\\n    value\\n    __typename\\n  }\\n  images {\\n    sequence\\n    altText\\n    smallUrl\\n    mediumUrl\\n    largeUrl\\n    brandKey\\n    productKey\\n    label\\n    videoUrl\\n    __typename\\n  }\\n  casNumber\\n  products {\\n    ...SubstanceProductFields\\n    __typename\\n  }\\n  match_fields\\n  __typename\\n}\\n\\nfragment SubstanceProductFields on Product {\\n  name\\n  productNumber\\n  productKey\\n  isSial\\n  isMarketplace\\n  marketplaceSellerId\\n  marketplaceOfferId\\n  cardCategory\\n  cardAttribute {\\n    citationCount\\n    application\\n    __typename\\n  }\\n  substance {\\n    id\\n    __typename\\n  }\\n  casNumber\\n  attributes {\\n    key\\n    label\\n    values\\n    __typename\\n  }\\n  speciesReactivity\\n  brand {\\n    key\\n    erpKey\\n    name\\n    color\\n    __typename\\n  }\\n  images {\\n    altText\\n    smallUrl\\n    mediumUrl\\n    largeUrl\\n    __typename\\n  }\\n  linearFormula\\n  molecularWeight\\n  description\\n  sdsLanguages\\n  sdsPnoKey\\n  similarity\\n  paMessage\\n  features\\n  catalogId\\n  materialIds\\n  erp_type\\n  __typename\\n}\\n\"}";
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Add("authority", "www.sigmaaldrich.com");
            request.Headers.Add("accept", "*/*");
            request.Headers.Add("accept-language", "en-GB,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("origin", "https://www.sigmaaldrich.com");
            request.Headers.Add("referer", "https://www.sigmaaldrich.com/GB/en/search/sds?focus=products&page=1&perpage=30&sort=relevance&term=sds&type=product_name");
            request.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"112\", \"Google Chrome\";v=\"112\", \"Not:A-Brand\";v=\"99\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
            request.Headers.Add("x-gql-access-token", "0fc06011-4b1b-11ee-ae1b-5bc4ccb3b87e");
            request.Headers.Add("x-gql-country", "GB");
            request.Headers.Add("x-gql-language", "en");
            request.Headers.Add("x-gql-operation-name", "ProductSearch");
            request.Headers.Add("x-gql-user-erp-type", "ANONYMOUS");
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var jtoken = JsonConvert.DeserializeObject<JToken>(HttpUtility.HtmlDecode(content))!;
                    var results = jtoken.SelectToken("data.getProductSearchResults.items")!.ToObject<List<Result>>()!;
                    
                    return results;
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            return new List<Result>();
        }

        static public List<Result> Search(string query, bool print = false)
        {
            List<Result> results = SearchAsync(query).Result;
            if (print)
            {
                PrintResults(results);
            }
            return results;
        }

        static public async Task<SafetyData> SelectResult(List<Result> results, int resultIndex,int productIndex)
        {
            //Trace.WriteLine(results[resultIndex].products![productIndex].fullName);
            string url = results[resultIndex].products![productIndex].link!;
            return await SDSParser.Extract(url);
        }

        
        public static void PrintResults(in List<Result> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                Result result = results[results.Count - i - 1];
                Console.WriteLine($"{results.Count - i}.{result.name}");
                for (int j = 0; j < result.products.Count; j++)
                {
                    Result.Product product = result.products[j];
                    Console.WriteLine($"\t{j + 1}. {product.description}");
                }
                Console.WriteLine();
            }
        }

        public class Result
        {
            [JsonProperty("name")]
            public required string name { get; set; }

            [JsonProperty("empiricalFormula")]
            public string empiricalFormula { get; set; }

            [JsonProperty("molecularWeight")]
            public string molecularWeight { get; set; }

            [JsonProperty("products")]
            public List<Product> products { get; set; }

            public class Product
            {
                [JsonProperty("name")]
                public string name { get; set; }

                [JsonProperty("description")]
                public string? description { get; set; }

                public string? fullName { 
                    get {
                        return $"{name} {description}";
                    }
                }

                [JsonProperty("productNumber")]
                public string productNumber { get; set; }

                [JsonProperty("productKey")]
                public string productKey { get; set; }

                [JsonProperty("brand")]
                public Brand brand { get; set; }

                [JsonProperty("sdsLanguages")]
                public List<string>? sdsLanguages { get;set; }

                public string? link{ 
                    get {
                        if (sdsLanguages == null || sdsLanguages.Count == 0) return null;
                        return $"https://www.sigmaaldrich.com/GB/en/sds/{brand.key.ToLower()}/{productNumber}";
                    } 
                }

                public class Brand
                {
                    [JsonProperty("key")]
                    public string key { get; set; }
                       
                    [JsonProperty("erpKey")]
                    public string erpKey { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }

                    [JsonProperty("color")]
                    public string color { get; set; }

                    [JsonProperty("__typename")]
                    public string __typename { get; set; }

                }

            }
        }

    }



}
