using System.Text.RegularExpressions;

namespace raitichan.com.vrchat_api; 

public class Instance {
	private static readonly Regex PATTERN = new("(?<WorldID>(?:wrld|wld)_[0-9a-fA-F]{8}(?:-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}):(?<InstanceID>.+)", RegexOptions.Compiled);
	
	public string WorldId { get; set; }
	public string InstanceId { get; set; }

	public Instance(string location) {
		Match match = PATTERN.Match(location);
		if (!match.Success) throw new InvalidOperationException(location);
		this.WorldId = match.Groups["WorldID"].Value;
		this.InstanceId = match.Groups["InstanceID"].Value;
	}

}