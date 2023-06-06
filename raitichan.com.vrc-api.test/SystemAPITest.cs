using NUnit.Framework;
using raitichan.com.vrchat_api;

namespace raitichan.com.vrc_api.test;

[TestFixture]
public class SystemAPITest {
	[Test]
	public void GetTest() {
		APIClient apiClient = new();
		SystemAPI systemApi = new(apiClient);
		ConfigResult? config = systemApi.GetConfig();
		Assert.AreEqual(config?.clientApiKey, "JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26");
	}
}