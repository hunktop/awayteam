using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileInterfaceState
{
	// we probably need two or three more states than this, I'm just practicing...
	Idle, // idle will highlight tiles passively as you mouse over
	Drag // this is when you have clicked on an object which may pathfind
}

public class TileInterface
{
	private Map map;
	public bool HighlightTiles
	{
		get;
		set;
	}
	private FSprite activeTile;
	
	public TileInterface(Map map)
	{
		this.map = map;
		this.HighlightTiles = true;
		this.activeTile = new FSprite("whitetile");
		this.activeTile.x = 0.0f;
		this.activeTile.y = 0.0f;
		this.activeTile.color = Color.yellow;
		this.activeTile.alpha = 0.5f;
	}
	
	public void Update(float dt)
	{
		for (int ii = 0; ii < this.map.Width; ii++)
        {
            for (int jj = 0; jj < this.map.Height; jj++)
            {
				var tempTile = this.map.Tiles[ii, jj];
				if (tempTile.Contains(this.map.mouseManager.mousePosition))
				{
					this.activeTile.x = tempTile.x;
					this.activeTile.y = tempTile.y;
					break;
				}
            }
        }
	}
}

