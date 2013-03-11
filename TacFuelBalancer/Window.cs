using System;
using UnityEngine;

internal abstract class Window
{
	private bool visible;
	private Rect windowPos = new Rect(60f, 60f, 60f, 60f);
	private string windowTitle;
	private PartModule partModule;
	private int windowId;
	
	protected Window(string windowTitle, PartModule partModule)
	{
		this.windowTitle = windowTitle;
		this.partModule = partModule;
		this.windowId = windowTitle.GetHashCode() + new System.Random().Next(65536);
	}
	
	public bool IsVisible()
	{
		return this.visible;
	}
	
	public virtual void SetVisible(bool newValue)
	{
		if (newValue)
		{
			if (!this.visible)
			{
				RenderingManager.AddToPostDrawQueue(3, new Callback(this.CreateWindow));
			}
		}
		else
		{
			if (this.visible)
			{
				RenderingManager.RemoveFromPostDrawQueue(3, new Callback(this.CreateWindow));
			}
		}
		this.visible = newValue;
	}
	
	public void SetSize(int width, int height)
	{
		this.windowPos.width = (float)width;
		this.windowPos.height = (float)height;
	}
	
	public virtual void Load(ConfigNode config, string subnode)
	{
		Debug.Log(string.Concat(new object[]
		{
			"TAC Atomic Clock [",
			Time.time,
			"]: Load ",
			subnode
		}));
		if (config.HasNode(subnode))
		{
			ConfigNode node = config.GetNode(subnode);
			float num;
			if (node.HasValue("xPos") && float.TryParse(node.GetValue("xPos"), out num))
			{
				this.windowPos.xMax = num;
			}
			if (node.HasValue("yPos") && float.TryParse(node.GetValue("yPos"), out num))
			{
				this.windowPos.yMin = num;
			}
		}
	}
	
	public virtual void Save(ConfigNode config, string subnode)
	{
		Debug.Log(string.Concat(new object[]
		{
			"TAC Atomic Clock [",
			Time.time,
			"]: Save ",
			subnode
		}));
		ConfigNode configNode;
		if (config.HasNode(subnode))
		{
			configNode = config.GetNode(subnode);
		}
		else
		{
			configNode = new ConfigNode(subnode);
			config.AddNode(configNode);
		}
		configNode.AddValue("xPos", this.windowPos.xMin);
		configNode.AddValue("yPos", this.windowPos.yMin);
	}
	
	protected virtual void CreateWindow()
	{
		try
		{
			if (this.partModule.part.State != PartStates.DEAD && this.partModule.vessel.isActiveVessel)
			{
				GUI.skin = (HighLogic.Skin);
				this.windowPos = GUILayout.Window(this.windowId, this.windowPos, new GUI.WindowFunction(this.Draw), this.windowTitle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true),
					GUILayout.ExpandHeight(true)
				});
			}
			else
			{
				this.SetVisible(false);
			}
		}
		catch
		{
			this.SetVisible(false);
		}
	}
	
	protected abstract void Draw(int windowID);
}
