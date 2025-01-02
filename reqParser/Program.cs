using System;
using System.Dynamic;
using System.Linq;
//GET /abc HTTP/1.1

namespace parser{


	public class Parser{

		internal static void Main()
		{
			string raw = "GET /submit-form HTTP/1.1";
			Dictionary<string, string> req = parse(raw);
			
			foreach (KeyValuePair<string, string> kvp in req)
			{
				Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
			}
		
		}
		
		public static Dictionary<string, string> parse(string rawREQ){
			
			char foc = '/';
			string[] rawSubset = rawREQ.Split(new[] {foc}, 3);
			string[] subset = new string[] { rawSubset[0].Trim(), rawSubset[1].Substring(0, rawSubset[1].Length - 5), "HTTP/" + rawSubset[2].Split('\n')[0] };
			string body = "";
			if (rawSubset[2].Split("\n\n").Length > 1)
			{
				body = rawSubset[2].Split("\n\n")[1];
			}
			
			Dictionary<string, string> parsedReq = new Dictionary<string, string >();
			parsedReq.Add("method", subset[0]);
			parsedReq.Add("url", subset[1]);
			parsedReq.Add("httpV", subset[2]);
			parsedReq.Add("body", body);
			List<string> headers = new List<string>();
			if(body != "")
			{
				Console.WriteLine("NOT OK");
				string? hold = rawREQ.Substring(rawREQ.IndexOf("\n") + 1, rawREQ.Length-rawREQ.IndexOf("\n") - body.Length-2);
				if (!string.IsNullOrEmpty(hold))
				{
					headers = hold.Split("\n").ToList();
				}
			}
			else
			{
				Console.WriteLine("Ok....");
				if (rawREQ.IndexOf("HTTP/" + rawSubset[2].Split('\n')[0]) + 1 == rawREQ.IndexOf("\n") || rawREQ.IndexOf("HTTP/" + rawSubset[2].Split('\n')[0]) + 1 == rawREQ.IndexOf("\n\r"))
				    
				{
						var hold = rawREQ.Substring(rawREQ.IndexOf("\n") + 1, rawREQ.Length-rawREQ.IndexOf("\n")-1);
        				if (!string.IsNullOrEmpty(hold))
        				{
        					headers = hold.Split("\n").ToList();
        				}			
				}
				
			}
			headers = headers.Where(h => !string.IsNullOrEmpty(h)).ToList();
			if (headers.Count > 0)
			{
				Console.WriteLine("Header -> "+headers[0].GetType());
				foreach (string h in headers)
				{
					Console.WriteLine(h);
					parsedReq.Add(h.Split(':')[0].Trim(), h.Split(':')[1].Trim());
				}
			}
				
			
			Console.WriteLine("PARSE COMPLETE");
			return parsedReq;
		}	
	}

}
