using System.Net;
using NUnit.Framework;
using raitichan.com.vrchat_api;

namespace raitichan.com.vrc_api.test; 

[TestFixture]
public class APIClientTest {
	[TestCase("config")]
	public void GetTest(string endPoint) {
		APIClient apiClient = new();
		var response = apiClient.Get(endPoint);
		Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
	}
}