using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace FaraBombRush.Configs;

public class PluginConfig {
	public static PluginConfig Instance { get; set; }

	public virtual bool IsDebug { get; set; } = false;
		
	public virtual bool IsBombCommandEnable { get; set; } = true;

	public virtual bool IsBombCutEnable { get; set; } = true;

	public virtual float BombSpawnDistance { get; set; } = 30.0f;

	public virtual int BombLineCount { get; set; } = 5;

	/// <summary>
	/// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
	/// </summary>
	public virtual void OnReload() {
		// Do stuff after config is read from disk.
	}

	/// <summary>
	/// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
	/// </summary>
	public virtual void Changed() {
		// Do stuff when the config is changed.
	}

	/// <summary>
	/// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
	/// </summary>
	public virtual void CopyFrom(PluginConfig other) {
		// This instance's members populated from other
	}
}