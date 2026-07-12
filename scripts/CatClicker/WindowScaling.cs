using Godot;

namespace CatClicker;

public partial class WindowScaling : Node
{
	// Controls by how many times the base viewport size the window must be resized
	// for the UI to readjust to the new scale.
	// A value of 1.0f means the first rescale happens at 2x base size.
	// A value of 0.5f means the first rescale happens at 1.5x base size.
	// A value of 0.25f means the first rescale happens at 1.25x base size.
	// etc.
	[Export(PropertyHint.Range, "0.1,1,0.01")]
	public float ScalingStepSize = 1.0f;

	[Export] public float MinScale = 1.0f;

	private Vector2I _baseSize = new(1280, 720);
	
	public override void _Ready()
	{
		_baseSize = new Vector2I(
			ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32(),
			ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32()
		);
		var window = GetWindow();
		window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
		window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand; // Overridden by settings
		window.ContentScaleStretch = Window.ContentScaleStretchEnum.Fractional;
		
		GetViewport().SizeChanged += () => CallDeferred(MethodName.UpdateLastUsedWindowMode);
		window.SizeChanged += () => CallDeferred(MethodName.OnWindowResized);
		CallDeferred(MethodName.UpdateMajor);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		SettingsManager.Instance.PropertyUpdated += OnSettingsPropertyUpdated;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		SettingsManager.Instance.PropertyUpdated -= OnSettingsPropertyUpdated;
	}

	private void OnSettingsPropertyUpdated(string propertyName, Variant newValue)
	{
		if (propertyName is nameof(SettingsData.FullscreenMode) or nameof(SettingsData.LastUsedFullscreenMode)
		    or nameof(SettingsData.LastUsedWindowedMode) or nameof(SettingsData.Resolution))
			UpdateMajor();
	}

	public void ToggleFullscreen()
	{
		SettingsManager.Instance.Settings.FullscreenMode = SettingsManager.Instance.Settings.FullscreenMode == default
			? SettingsManager.Instance.Settings.LastUsedFullscreenMode
			: default;
		
		SettingsManager.Instance.EmitChanged(nameof(SettingsData.FullscreenMode), Variant.From(SettingsManager.Instance.Settings.FullscreenMode));
	}

	private void OnViewportSizeChanged()
	{
		CallDeferred(MethodName.UpdateLastUsedWindowMode);
	}

	private void UpdateLastUsedWindowMode()
	{
		var windowMode = DisplayServer.Singleton.WindowGetMode();
		if (windowMode is not (DisplayServer.WindowMode.Fullscreen or DisplayServer.WindowMode.ExclusiveFullscreen))
		{
			if (SettingsManager.Instance.Settings.LastUsedWindowedMode != windowMode)
			{
				// windowed/maximized mode changed
				SettingsManager.Instance.Settings.LastUsedWindowedMode = windowMode;
				SettingsManager.Instance.EmitChanged(nameof(SettingsData.LastUsedWindowedMode), Variant.From(SettingsManager.Instance.Settings.LastUsedWindowedMode));
				OnWindowedMaximizedModeChanged();
			}

			if (SettingsManager.Instance.Settings.FullscreenMode != default)
			{
				// got kicked out of full screen mode
				SettingsManager.Instance.Settings.FullscreenMode = default;
				SettingsManager.Instance.EmitChanged(nameof(SettingsData.FullscreenMode), Variant.From(SettingsManager.Instance.Settings.FullscreenMode));
				OnFullscreenModeExited();
			}
		}
		else
		{
			if (SettingsManager.Instance.Settings.LastUsedFullscreenMode != windowMode)
			{
				// fullscreen mode changed
				SettingsManager.Instance.Settings.LastUsedFullscreenMode = windowMode;
				SettingsManager.Instance.EmitChanged(nameof(SettingsData.LastUsedFullscreenMode), Variant.From(SettingsManager.Instance.Settings.LastUsedFullscreenMode));
				OnFullscreenModeChanged();
			}
		}
	}

	private void OnWindowResized()
	{
		// GD.Print($"Window resized: {GetWindow().Size}");
		UpdateMinor();
	}

	private void OnWindowedMaximizedModeChanged()
	{
		// GD.Print("Windowed/Maximized mode changed");
		UpdateMajor();
	}

	private void OnFullscreenModeExited()
	{
		// GD.Print("Fullscreen mode exited");
		UpdateMajor();
	}

	private void OnFullscreenModeChanged()
	{
		// GD.Print("Fullscreen mode changed");
		UpdateMajor();
	}

	private void UpdateMinor()
	{
		// Change parameters of how the inside of the window is drawn. Called when the window is resized,
		// and after major updates.
		var window = GetWindow();
		var windowSize = window.Size;
		var resolution = SettingsManager.Instance.Settings.Resolution;
		
		if (SettingsManager.Instance.Settings.Resolution == default)
		{
			// Expand
			resolution = windowSize;
		}

		float scale = Mathf.Min(
			Mathf.Min(
				Mathf.Max(MinScale, Mathf.Floor((float)resolution.X / _baseSize.X / ScalingStepSize) * ScalingStepSize),
				Mathf.Max(MinScale, Mathf.Floor((float)resolution.Y / _baseSize.Y / ScalingStepSize) * ScalingStepSize)
			),
			4
		);
		window.ContentScaleFactor = scale;
		window.ContentScaleSize = resolution;
	}

	private void UpdateMajor()
	{
		// Change parameters of the window. Should only be called on singular detected changes such as settings changes,
		// which cannot be triggered as a side effect of this function (to avoid calling this function on loop)
		var window = GetWindow();
		
		var mode = SettingsManager.Instance.Settings.FullscreenMode;
		if (mode == default) mode = SettingsManager.Instance.Settings.LastUsedWindowedMode;

		bool expand = SettingsManager.Instance.Settings.Resolution == default;
		
		window.ContentScaleAspect = expand ? Window.ContentScaleAspectEnum.Expand : Window.ContentScaleAspectEnum.Keep;

		if (DisplayServer.Singleton.WindowGetMode() is not DisplayServer.WindowMode.Fullscreen)
			window.Unresizable = false;
		
		// GD.Print($"Set mode {mode} via UpdateMajor");
		DisplayServer.Singleton.WindowSetMode(mode);

		if (mode is DisplayServer.WindowMode.Windowed)
			window.Unresizable = !expand;
		
		if (SettingsManager.Instance.Settings.Resolution != default &&
			mode is not (DisplayServer.WindowMode.Maximized or DisplayServer.WindowMode.Fullscreen
				or DisplayServer.WindowMode.ExclusiveFullscreen))
		{
			// GD.Print($"Set size {SettingsManager.Instance.Settings.Resolution} via UpdateMajor");
			GetWindow().Size = SettingsManager.Instance.Settings.Resolution;

			if (mode is DisplayServer.WindowMode.Windowed)
			{
				var estimatedDecorationSize = new Vector2I(0, 24);
				// Note: GetSizeWithDecorations returns inaccurate values if the window size is larger than the screen it's in (seems like it's clamped to the screen size),
				// so instead using a hard-coded decoration size.
				if (IsBiggerThanScreen(GetWindow().Size + estimatedDecorationSize))
				{
					// Align with top left of screen if bigger than the screen (such that the title bar is visible and user is able to drag it)
					GetWindow().Position = DisplayServer.ScreenGetPosition(DisplayServer.WindowGetCurrentScreen()) + estimatedDecorationSize;
				}
				else
				{
					// Center if possible otherwise
					GetWindow().MoveToCenter();
				}
			}
		}

		CallDeferred(MethodName.UpdateMinor);
	}

	private static bool IsBiggerThanScreen(Vector2I size)
	{
		var currentScreenSize = DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen());
		return size.X > currentScreenSize.X || size.Y > currentScreenSize.Y;
	}
}
