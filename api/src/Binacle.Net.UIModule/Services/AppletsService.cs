namespace Binacle.Net.UIModule.Services;

internal class AppletsService
{
	public IReadOnlyList<Models.Applet> Applets { get; }
	
	public AppletsService()
	{
		this.Applets = new List<Models.Applet>
		{
			new Models.Applet
			{
				Title = "Packing Demo",
				Icon = "deployed_code",
				ShortDescription = "Put in your own bins and items, pick an algorithm, and watch Binacle.Net pack them.",
				Description = "Put in your own bins and items, pick an algorithm, and watch Binacle.Net pack them in 3D. Each algorithm searches differently, so the same items can land in different places.",
				Page = "/Packing"
			},
			new Models.Applet
			{
				Title = "ViPaq Decoder",
				Icon = "deployed_code_update",
				ShortDescription = "Paste a ViPaq string from a response and see the packing drawn in 3D.",
				Description = "Paste a ViPaq string from a Binacle.Net response and see the packing drawn in 3D. The string carries the whole layout, so it is decoded here in your browser and nothing is sent anywhere.",
				Page = "/Vipaq"
			},
			new Models.Applet
			{
				Title = "This Instance",
				Icon = "monitoring",
				ShortDescription = "What this container is running, what is switched on, and the presets it loaded.",
				Description = "What this container is running, what is switched on, and the presets it loaded. Everything here is read from the instance you are on, so it answers whether your own configuration arrived the way you meant it to.",
				Page = "/Instance"
			},
		};
		
	}
}
