using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.ServiceDiscovery.Client;

public class RemoteDiscoveryService : IDiscoveryService
{
	private readonly string _address;

	public RemoteDiscoveryService(string address)
	{
		_address = address;
	}

	async Task<ServiceAddress[]> IDiscoveryService.ResolveService(string service, string tag)
	{
		try
		{
			return JsonConvert.DeserializeObject<ServiceAddress[]>(await HttpHelper.DownloadStringTaskAsync(_address + "Data/Resolve/" + service + "/" + tag));
		}
		catch (Exception ex)
		{
			Debug.Print("Error on ResolveService: " + ex.Message);
			return null;
		}
	}

	async Task<ServiceAddress[]> IDiscoveryService.DiscoverServices()
	{
		ServiceAddress[] result = null;
		try
		{
			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage obj = await client.GetAsync(_address + "Data/GetDiscoveredServices");
				obj.EnsureSuccessStatusCode();
				result = JsonConvert.DeserializeObject<ServiceAddress[]>(await obj.Content.ReadAsStringAsync());
			}
			return result;
		}
		catch (Exception ex)
		{
			Debug.Print("Error on DiscoverServices: " + ex.Message);
			return null;
		}
	}
}
