using System;
using System.Dynamic;
using System.Linq;

namespace parser{


	public class Parser{

		internal static void Main()
		{
			//Test
			string raw = "GET /abc HTTP/1.1\nheader:abc";
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
				string? hold = rawREQ.Substring(rawREQ.IndexOf("\n") + 1, rawREQ.Length-rawREQ.IndexOf("\n") - body.Length-2);
				if (!string.IsNullOrEmpty(hold))
				{
					headers = hold.Split("\n").ToList();
				}
			}
			else
			{
				if (rawREQ.IndexOf("HTTP/" + rawSubset[2].Split('\n')[0]) + rawSubset[2].Split('\n')[0].Length + 5 == rawREQ.IndexOf("\n") || rawREQ.IndexOf("HTTP/" + rawSubset[2].Split('\n')[0]) + rawSubset[2].Split('\n')[0].Length + 5 == rawREQ.IndexOf("\r\n"))
				    
				{
						Console.WriteLine("It does....");
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
				foreach (string h in headers)
				{
					parsedReq.Add(h.Split(':')[0].Trim(), h.Split(':')[1].Trim());
				}
			}
				
			
			Console.WriteLine("PARSE COMPLETE");
			return parsedReq;
		}	
	}

}
